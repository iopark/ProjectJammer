using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ildoo
{
    public class ItemSpawner : MonoBehaviourPun
    {
        [Header("Spawning Settings")]
        [SerializeField] int itemLimit;
        [SerializeField] float spawnInterval;
        [SerializeField] GameObject spawningObj; // 
        int? itemCounter;
        WaitForSeconds spawnRoutineInterval;
        private void Awake()
        {

        }

        public void StartSpawning()
        {
            if (!PhotonNetwork.IsMasterClient)
                return;
            if (itemCounter == null)
                itemCounter = 0;
            spawnRoutineInterval = new WaitForSeconds(spawnInterval);
            StartCoroutine(SpawnRoutine());
        }

        IEnumerator SpawnRoutine()
        {
            while (true)
            {
                AttemptForRespawn();
                yield return spawnRoutineInterval;
            }
        }
        Vector2 randomSeed;
        Vector3 randomSpawnPos;
        Vector3 newSpawningPos;
        public void AttemptForRespawn()
        {
            itemCounter = transform.childCount;
            if (itemCounter >= itemLimit)
                return;

            newSpawningPos = transform.position;
            newSpawningPos.y = 0.5f;
            GameObject newItem = PhotonNetwork.Instantiate(spawningObj.name, newSpawningPos, Quaternion.identity);
            newItem.transform.parent = transform;
            //photonView.RPC("SyncRespawn", RpcTarget.AllViaServer, newSpawningPos);
        }
        //이게 생성
        //[PunRPC]
        //public void SyncRespawn(Vector3 spawnPos)
        //{
        //    GameObject newItem = GameManager.Resource.Instantiate(spawningObj, spawnPos, Quaternion.identity, true);
        //    newItem.transform.parent = transform;
        //}
    }
}

