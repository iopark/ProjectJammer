using Darik;
using ildoo;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Park_Woo_Young
{
    public class Emp : MonoBehaviourPunCallbacks //, Darik.IHittable
    {

        [SerializeField] LayerMask targetEnemy;

        public void OnTriggerEnter(Collider other)
        {
            if (targetEnemy.Contain(other.gameObject.layer))
            {
                //other.gameObject.GetComponent<IHittable>();
                Debug.Log("sdsds");
            }      
        }

        /*public void TakeDamage(int damage, Vector3 hitPoint, Vector3 normal)
        {
            
        }*/
    }
}

