using Photon.Pun;
using Photon.Realtime;
using Photon.Pun.UtilityScripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using PhotonHashtable = ExitGames.Client.Photon.Hashtable;
using TMPro;
using Unity.VisualScripting;

namespace Darik
{
    public class GameSceneManager : MonoBehaviourPunCallbacks
    {
        [SerializeField] TMP_Text infoText;
        [SerializeField] float countDownTimer = 5;

        private void Start()
        {
            // normal game mode
            if (PhotonNetwork.InRoom)
                PhotonNetwork.LocalPlayer.SetLoad(true);
            else
            {
                infoText.text = "Debug Mode";
                PhotonNetwork.LocalPlayer.NickName = $"DebugPlayer {Random.Range(1000, 10000)}";
                PhotonNetwork.ConnectUsingSettings();
            }
        }

        public override void OnConnectedToMaster()
        {
            RoomOptions options = new RoomOptions() { };
            PhotonNetwork.JoinOrCreateRoom("DebugRoom", options, TypedLobby.Default);
        }

        public override void OnJoinedRoom()
        {
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

        private void DebugGameStart()
        {
            Debug.Log("Debug Mode Game Started");
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

        private void GameStart()
        {
            Debug.Log("Normal Game Mode Started");
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
    }
}
