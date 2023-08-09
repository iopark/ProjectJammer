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
        [SerializeField] float SpawnCoolTime ;

        private List<Enemy> enemyList;
        private bool isDisruptorActivated;
        private int targetPlayerID;

        private void Awake()
        {
            enemyList = new List<Enemy>();
        }

        public void ChangeTarget(bool isDisruptorActivated)
        {
            this.isDisruptorActivated = isDisruptorActivated;
            if (!isDisruptorActivated)
            {
                GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
                foreach (GameObject player in players)
                {
                    targetPlayerID = player.GetComponent<ildoo.Player>().UniquePlayerNumber();
                }
            }
        }

        public Transform FindTarget()
        {
            if (isDisruptorActivated)
                return GameManager.Data.Disruptor;
            else
            {
                PhotonView pv = PhotonView.Find(targetPlayerID);
                return pv.transform;
            }
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

                GameObject enemy_blade = PhotonNetwork.InstantiateRoomObject("Enemy_Blade", new Vector3(-40, 0, 30), Quaternion.identity, 0);
                enemyList.Add(enemy_blade.GetComponent<Enemy>());
                //GameObject enemy_Rifle = PhotonNetwork.InstantiateRoomObject("Enemy_Rifle", new Vector3(-35, 0, 35), Quaternion.identity, 0);
                //enemyList.Add(enemy_Rifle.GetComponent<Enemy>());
                yield return new WaitForSeconds(SpawnCoolTime);
            }
        }
    }
}
