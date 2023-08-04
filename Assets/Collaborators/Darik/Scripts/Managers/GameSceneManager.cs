using Photon.Pun;
using Photon.Realtime;
using Photon.Pun.UtilityScripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using PhotonHashtable = ExitGames.Client.Photon.Hashtable;
using TMPro;

namespace Darik
{
    public class GameSceneManager : MonoBehaviourPunCallbacks
    {
        [SerializeField] TMP_Text infoText;
        [SerializeField] float countDownTimer = 5;

        private List<GameObject> enemyList;
        private Transform target;

        private void Awake()
        {
            enemyList = new List<GameObject>();
        }

        private void Start()
        {
            // normal game mode
            if (PhotonNetwork.InRoom)
                PhotonNetwork.LocalPlayer.SetLoad(true);
            // debug game mode
            else
            {
                infoText.text = "Debug Mode";
                PhotonNetwork.LocalPlayer.NickName = $"DebugPlayer {Random.Range(1000, 10000)}";
                PhotonNetwork.ConnectUsingSettings();
            }
        }

        public override void OnEnable()
        {
            base.OnEnable();
            GameManager.Data.OnChangedTarget += ChangeTarget;
        }

        public override void OnDisable()
        {
            base.OnDisable();
            GameManager.Data.OnChangedTarget -= ChangeTarget;
        }

        private void ChangeTarget(bool isDisruptor)
        {
            if (isDisruptor)
            {
                target = GameManager.Data.Disruptor;

                foreach (GameObject enemy in enemyList)
                {
                    enemy.GetComponent<Enemy>().Target = GameManager.Data.Disruptor;
                }
            }   
            /*
            else
            {
                
                foreach (Player player in PhotonNetwork.PlayerList)
                {
                    if (player.GetPlayerID() == PhotonNetwork.MasterClient.ActorNumber)
                    {
                        PhotonView pv = PhotonView.Find(player.GetPlayerID());
                        target = pv.transform;
                        break;
                    }
                }

                target = GameObject.Find("PlayerHolder(Clone)").transform;

                foreach (GameObject enemy in enemyList)
                {
                    enemy.GetComponent<Enemy>().Target = target;
                }
            }*/
        }

        public override void OnConnectedToMaster()
        {
            RoomOptions options = new RoomOptions() { };
            PhotonNetwork.JoinOrCreateRoom("DebugRoom", options, TypedLobby.Default);
        }

        public override void OnJoinedRoom()
        {
            int actorNumber = PhotonNetwork.LocalPlayer.ActorNumber;
            Player[] sortedPlayers = PhotonNetwork.PlayerList;

            for (int i = 0; i < sortedPlayers.Length; i += 1)
            {
                if (sortedPlayers[i].ActorNumber == actorNumber)
                {
                    sortedPlayers[i].SetPlayerID(sortedPlayers[i].ActorNumber);
                    break;
                }
            }

            StartCoroutine(DebugGameSetupDelay());
        }

        public override void OnDisconnected(DisconnectCause cause)
        {
            Debug.Log($"Disconnected : {cause}");
            SceneManager.LoadScene("LobbyScene");
        }

        public override void OnLeftRoom()
        {
            Debug.Log("LeftRoom");
            PhotonNetwork.LoadLevel("LobbyScene");
        }

        public override void OnMasterClientSwitched(Player newMasterClient)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                // TODO : 방장이 이어서 해야 할 일
                GenerateEnemy();
            }
        }

        public override void OnPlayerPropertiesUpdate(Player targetPlayer, PhotonHashtable changedProps)
        {
            if (changedProps.ContainsKey("Load"))
            {
                // All players were loaded 
                if (PlayerLoadCount() == PhotonNetwork.PlayerList.Length)
                {
                    if (PhotonNetwork.IsMasterClient)
                        PhotonNetwork.CurrentRoom.SetLoadTime(PhotonNetwork.ServerTimestamp);
                }
                // Some players were loaded
                else
                {
                    // Wait for other players whose not loaded
                    Debug.Log($"Wait Players {PlayerLoadCount()} / {PhotonNetwork.PlayerList.Length}");
                    infoText.text = $"Wait Players {PlayerLoadCount()} / {PhotonNetwork.PlayerList.Length}";
                }
            }
        }

        public override void OnRoomPropertiesUpdate(PhotonHashtable propertiesThatChanged)
        {
            if (propertiesThatChanged.ContainsKey("LoadTime"))
                StartCoroutine(GameStartCoroutine());
        }

        IEnumerator DebugGameSetupDelay()
        {
            // Wait 1Sec for Server
            yield return new WaitForSeconds(1f);
            DebugGameStart();
        }

        IEnumerator GameStartCoroutine()
        {
            int loadTime = PhotonNetwork.CurrentRoom.GetLoadTime();
            while (countDownTimer > (PhotonNetwork.ServerTimestamp - loadTime) / 1000f)
            {
                int remainTime = (int)(countDownTimer - (PhotonNetwork.ServerTimestamp - loadTime) / 1000f);
                infoText.text = $"All Player Loaded, Start count down : {remainTime + 1}";
                yield return new WaitForEndOfFrame();
            }
            infoText.text = "Game Start";
            GameStart();

            yield return new WaitForSeconds(1f);
            infoText.text = "";
        }

        private void DebugGameStart()
        {
            Debug.Log("Debug Mode Game Started");

            float angularStart = (360.0f / 8f) * PhotonNetwork.LocalPlayer.GetPlayerNumber();
            float x = 5.0f * Mathf.Sin(angularStart * Mathf.Deg2Rad);
            float z = 5.0f * Mathf.Cos(angularStart * Mathf.Deg2Rad);
            Vector3 position = GameManager.Data.Disruptor.position + new Vector3(x, 0.0f, z);
            Quaternion rotation = Quaternion.Euler(0.0f, angularStart, 0.0f);

            PhotonNetwork.Instantiate("PlayerHolder", position, rotation);
            
            if (PhotonNetwork.IsMasterClient)
                GenerateEnemy();
        }

        private void GameStart()
        {
            Debug.Log("Normal Game Mode Started");

            float angularStart = (360.0f / 8f) * PhotonNetwork.LocalPlayer.GetPlayerNumber();
            float x = 5.0f * Mathf.Sin(angularStart * Mathf.Deg2Rad);
            float z = 5.0f * Mathf.Cos(angularStart * Mathf.Deg2Rad);
            Vector3 position = GameManager.Data.Disruptor.position + new Vector3(x, 0.0f, z);
            Quaternion rotation = Quaternion.Euler(0.0f, angularStart, 0.0f);

            PhotonNetwork.Instantiate("PlayerHolder", position, rotation);

            if (PhotonNetwork.IsMasterClient)
                GenerateEnemy();
        }

        private int PlayerLoadCount()
        {
            int loadCount = 0;
            foreach (Player player in PhotonNetwork.PlayerList)
            {
                if (player.GetLoad())
                    loadCount++;
            }
            return loadCount;
        }

        private void GenerateEnemy()
        {
            GameObject enemy = PhotonNetwork.InstantiateRoomObject("Enemy_Blade", new Vector3(5, 0, 5), Quaternion.identity, 0);
            enemyList.Add(enemy);
            enemy.GetComponent<Enemy>().Target = target;
        }
    }
}
