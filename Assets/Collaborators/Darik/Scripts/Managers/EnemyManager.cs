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
        [SerializeField] float spawnBladeCoolTime = 20f;
        [SerializeField] float spawnRifleCoolTime = 20f;
        [SerializeField] float spawnSniperCoolTime = 20f;

        public List<Transform> enemySpawnPoints;
        public List<int> playerIds;
        private bool isDisruptorActivated;
        private int curTargetId;

        private void Awake()
        {
            enemySpawnPoints = new List<Transform>();
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

        public Transform SearchTarget()
        {
            if (isDisruptorActivated)
                return GameManager.Data.Disruptor;
            else
                return PhotonView.Find(curTargetId).transform;
        }

        public Transform SearchPlayer()
        {
            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
            int targetId = -1;

            if (players.Length > 0)
            {
                float shortestDistance = Mathf.Infinity;
                foreach (GameObject player in players)
                {
                    if (player != null)
                    {
                        Vector3 toTarget = transform.position - player.transform.position;
                        float squrDistance = (toTarget.x * toTarget.x + toTarget.y * toTarget.y + toTarget.z * toTarget.z);
                        if (shortestDistance > squrDistance)
                        {
                            shortestDistance = squrDistance;
                            targetId = player.GetComponent<ildoo.Player>().UniquePlayerNumber(); ;
                        }
                    }
                }
            }
            
            return PhotonView.Find(targetId).transform;
        }

        public void GenerateEnemy()
        {
            StartCoroutine(GenerateEnemyBladeCoroutine());
            StartCoroutine(GenerateEnemyRifleCoroutine());
            StartCoroutine(GenerateEnemySniperCoroutine());
        }

        IEnumerator GenerateEnemyBladeCoroutine()
        {
            while (true)
            {
                if (enemySpawnPoints.Count > 0)
                {
                    int randomIndex = Random.Range(0, enemySpawnPoints.Count);
                    Vector3 randomRange = new Vector3(Random.Range(-5, 6), 0, Random.Range(-5, 6));

                    PhotonNetwork.InstantiateRoomObject("Enemy_Blade", enemySpawnPoints[randomIndex].position + randomRange, Quaternion.identity, 0);

                    if (debug)
                        Debug.Log("GenerateBlade");
                }

                yield return new WaitForSeconds(spawnBladeCoolTime);
            }
        }

        IEnumerator GenerateEnemyRifleCoroutine()
        {
            while (true)
            {
                if (enemySpawnPoints.Count > 0)
                {
                    int randomIndex = Random.Range(0, enemySpawnPoints.Count);
                    Vector3 randomRange = new Vector3(Random.Range(-5, 6), 0, Random.Range(-5, 6));

                    PhotonNetwork.InstantiateRoomObject("Enemy_Rifle", enemySpawnPoints[randomIndex].position + randomRange, Quaternion.identity, 0);

                    if (debug)
                        Debug.Log("GenerateRifle");
                }

                yield return new WaitForSeconds(spawnRifleCoolTime);
            }
        }

        IEnumerator GenerateEnemySniperCoroutine()
        {
            while (true)
            {
                if (enemySpawnPoints.Count > 0)
                {
                    int randomIndex = Random.Range(0, enemySpawnPoints.Count);
                    Vector3 randomRange = new Vector3(Random.Range(-5, 6), 0, Random.Range(-5, 6));

                    PhotonNetwork.InstantiateRoomObject("Enemy_Sniper", enemySpawnPoints[randomIndex].position + randomRange, Quaternion.identity, 0);

                    if (debug)
                        Debug.Log("GenerateSniper");
                }

                yield return new WaitForSeconds(spawnSniperCoolTime);
            }
        }
    }
}
