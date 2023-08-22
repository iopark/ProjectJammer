using LDW;
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
        private List<int> alivePlayerIds;
        private bool isDisruptorActivated;
        private int curTargetId = -1;

        private void Awake()
        {
            enemySpawnPoints = new List<Transform>();
            alivePlayerIds = new List<int>();
        }

        private void OnEnable()
        {
            GameManager.Data.OnPlayerDied += RenewalTargetPlayer;
        }

        private void OnDisable()
        {
            GameManager.Data.OnPlayerDied -= RenewalTargetPlayer;
        }

        public void RenewalTargetPlayer()
        {
            alivePlayerIds.Clear();

            foreach (KeyValuePair<int, PlayerData> entry in GameManager.Data.playerDict)
            {
                if (entry.Value.isAlive)
                    alivePlayerIds.Add(entry.Value.viewId);
            }
            Debug.Log(alivePlayerIds.Count);
            SetTargetId();
        }

        public void ChangeTarget(bool isDisruptorActivated)
        {
            this.isDisruptorActivated = isDisruptorActivated;

            if (!isDisruptorActivated)
                SetTargetId();
        }

        private void SetTargetId()
        {
            if (alivePlayerIds.Count > 0)
                curTargetId = alivePlayerIds[Random.Range(0, alivePlayerIds.Count)];
            else
                curTargetId = -1;
        }

        public Transform SearchTarget()
        {
            if (isDisruptorActivated)
                return GameManager.Data.Disruptor;
            else
            {
                if (curTargetId != -1)
                    return PhotonView.Find(curTargetId).transform;
                else
                    return null;
            }
        }

        public Transform SearchPlayer()
        {
            Transform target = null;
            if (alivePlayerIds.Count > 0)
            {
                float shortestDistance = Mathf.Infinity;
                foreach (int playerId in alivePlayerIds)
                {
                    Transform player = PhotonView.Find(playerId).transform;
                    Vector3 toTarget = transform.position - player.transform.position;
                    float squrDistance = (toTarget.x * toTarget.x + toTarget.y * toTarget.y + toTarget.z * toTarget.z);
                    if (shortestDistance > squrDistance)
                    {
                        shortestDistance = squrDistance;
                        target = player;
                    }
                }
            }

            return target != null ? target : null;
        }

        public void GenerateEnemy()
        {
            StartCoroutine(GenerateEnemyBladeCoroutine());
            StartCoroutine(GenerateEnemyRifleCoroutine());
            StartCoroutine(GenerateEnemySniperCoroutine());
            //TestGenerate();
        }

        private void TestGenerate()
        {
            int randomIndex = Random.Range(0, enemySpawnPoints.Count);
            Vector3 randomRange = new Vector3(Random.Range(-5, 6), 0, Random.Range(-5, 6));
            PhotonNetwork.InstantiateRoomObject("Enemy_Blade", enemySpawnPoints[randomIndex].position + randomRange, Quaternion.identity, 0);

            randomIndex = Random.Range(0, enemySpawnPoints.Count);
            randomRange = new Vector3(Random.Range(-5, 6), 0, Random.Range(-5, 6));
            PhotonNetwork.InstantiateRoomObject("Enemy_Rifle", enemySpawnPoints[randomIndex].position + randomRange, Quaternion.identity, 0);

            randomIndex = Random.Range(0, enemySpawnPoints.Count);
            randomRange = new Vector3(Random.Range(-5, 6), 0, Random.Range(-5, 6));
            PhotonNetwork.InstantiateRoomObject("Enemy_Sniper", enemySpawnPoints[randomIndex].position + randomRange, Quaternion.identity, 0);
        }

        IEnumerator GenerateEnemyBladeCoroutine()
        {
            while (true)
            {
                if (enemySpawnPoints.Count > 0)
                {
                    int randomIndex = Random.Range(0, enemySpawnPoints.Count);
                    Vector3 randomRange = new Vector3(Random.Range(-5, 6), 0, Random.Range(-5, 6));

                    GameObject blade = PhotonNetwork.InstantiateRoomObject("Enemy_Blade", enemySpawnPoints[randomIndex].position + randomRange, Quaternion.identity, 0);
                    //blade.GetComponent<PhotonView>().ViewID = 900;

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

                    GameObject rifle = PhotonNetwork.InstantiateRoomObject("Enemy_Rifle", enemySpawnPoints[randomIndex].position + randomRange, Quaternion.identity, 0);
                    //rifle.GetComponent<PhotonView>().ViewID = 910;

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

                    GameObject sniper = PhotonNetwork.InstantiateRoomObject("Enemy_Sniper", enemySpawnPoints[randomIndex].position + randomRange, Quaternion.identity, 0);
                    //sniper.GetComponent<PhotonView>().ViewID = 920;

                    if (debug)
                        Debug.Log("GenerateSniper");
                }

                yield return new WaitForSeconds(spawnSniperCoolTime);
            }
        }

        public void StopSpawnEnemy()
        {
            StopAllCoroutines();
        }
    }
}
