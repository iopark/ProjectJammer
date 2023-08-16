using Darik;
using ildoo;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Park_Woo_Young
{
    public class Emp : MonoBehaviourPunCallbacks
    {
        [SerializeField] LayerMask targetEnemy;

        public void OnTriggerEnter(Collider other)
        {
            if (targetEnemy.Contain(other.gameObject.layer))
            {
                Darik.IHittable obj = other.gameObject.GetComponent<Darik.IHittable>();
                obj?.TakeDamage(9999, Vector3.zero, Vector3.zero);
            }      
        }
    }
}

