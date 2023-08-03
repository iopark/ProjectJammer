using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Darik
{
    public class SpawnManager : MonoBehaviour
    {
        public void StartSpawnEnemy()
        {
            //GameObject enemy_blade = GameManager.Resource.Load<GameObject>("Prefabs/Enemys/Enemy_Blade");
            PhotonNetwork.InstantiateRoomObject("Enemy_Blade", new Vector3(5, 0, 5), Quaternion.identity, 0);
        }
    }
}
