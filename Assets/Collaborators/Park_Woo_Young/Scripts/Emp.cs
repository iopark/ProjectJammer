using Darik;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Park_Woo_Young
{
    public class Emp : MonoBehaviourPunCallbacks, Darik.IHittable
    {

        [SerializeField] LayerMask targetEnemy;

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.layer == targetEnemy)
            {
               //gameObject
            }
            else
            {
                return;
            }
        }
        public void TakeDamage(int damage, Vector3 hitPoint, Vector3 normal)
        {
        }
    }
}

