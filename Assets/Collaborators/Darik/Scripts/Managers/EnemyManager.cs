using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Darik
{
    public class EnemyManager : MonoBehaviour
    {
        [SerializeField] private bool debug;
        [SerializeField] float SpawnCoolTime;

        private List<int> playerIds;
        private bool isDisruptorActivated;
        private int curTargetId;

        private void Awake()
        {
            playerIds = new List<int>();
        }

        public void ChangeTarget(bool isDisruptorActivated)
        {
            this.isDisruptorActivated = isDisruptorActivated;

            if (!isDisruptorActivated)
                curTargetId = playerIds[Random.Range(0, playerIds.Count)];
        }

        public void RegistPlayers()
        {
            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
            foreach (GameObject player in players)
            {
                int playerId = player.GetComponent<ildoo.Player>().UniquePlayerNumber();
                playerIds.Add(playerId);
            }
        }

        public Transform FindTarget()
        {
            if (isDisruptorActivated)
                return GameManager.Data.Disruptor;
            else
                return PhotonView.Find(curTargetId).transform;
        }

        public Transform FindPlayer()
        {
            int targetId = playerIds[Random.Range(0, playerIds.Count)];
            return PhotonView.Find(targetId).transform;
        }

        public void GenerateEnemy()
        {
            StartCoroutine(GenerateEnemyCoroutine());
        }

        IEnumerator GenerateEnemyCoroutine()
        {
            while (true)
            {
                if (debug)
                    Debug.Log("Generate");

                PhotonNetwork.InstantiateRoomObject("Enemy_Blade", new Vector3(-40, 0, 30), Quaternion.identity, 0);
                PhotonNetwork.InstantiateRoomObject("Enemy_Rifle", new Vector3(-35, 0, 35), Quaternion.identity, 0);

                yield return new WaitForSeconds(SpawnCoolTime);
            }
        }
    }
}
