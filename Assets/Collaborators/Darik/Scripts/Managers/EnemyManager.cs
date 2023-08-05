using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Darik
{
    public class EnemyManager : MonoBehaviour
    {
        private List<Enemy> enemyList;
        private bool isDisruptorActivated;
        private int targetPlayerID;

        private void Awake()
        {
            enemyList = new List<Enemy>();
        }

        private void OnEnable()
        {
            GameManager.Data.OnChangedTarget += ChangeTarget;
        }

        private void OnDisable()
        {
            GameManager.Data.OnChangedTarget -= ChangeTarget;
        }

        private void ChangeTarget(bool isDisruptorActivated)
        {
            this.isDisruptorActivated = isDisruptorActivated;
            if (!isDisruptorActivated)
            {/*
                foreach (Player player in PhotonNetwork.PlayerList)
                {
                    if (player.ActorNumber == PhotonNetwork.MasterClient.ActorNumber)
                    {
                        targetPlayerID = player.ActorNumber;
                        Debug.Log(PhotonView.Find(targetPlayerID).gameObject.name);
                        break;
                    }
                }*/
            }
        }

        public Transform FindTarget()
        {
            if (isDisruptorActivated)
                return GameManager.Data.Disruptor;
            else
            {/*
                PhotonView pv = PhotonView.Find(targetPlayerID);
                return pv.transform;*/
                return GameObject.Find("PlayerHolder(Clone)").transform;
            }
        }

        public void GenerateEnemy()
        {
            GameObject enemy = PhotonNetwork.InstantiateRoomObject("Enemy_Blade", new Vector3(5, 0, 5), Quaternion.identity, 0);
            enemyList.Add(enemy.GetComponent<Enemy>());
        }
    }
}
