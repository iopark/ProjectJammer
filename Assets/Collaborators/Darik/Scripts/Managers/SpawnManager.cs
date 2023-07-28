using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Darik
{
    public class SpawnManager : MonoBehaviour
    {
        private void Start()
        {
            StartSpawnEnemy();
        }

        public void StartSpawnEnemy()
        {
            Enemy_Blade enemy_blade = GameManager.Resource.Load<Enemy_Blade>("Prefabs/Enemys/Enemy_Blade");
            GameManager.Resource.Instantiate(enemy_blade, new Vector3(0, 0, 0), Quaternion.identity);
        }
    }
}
