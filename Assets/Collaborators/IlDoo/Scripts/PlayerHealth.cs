using System;
using Darik; 
using Photon.Pun;
using UnityEngine;

namespace ildoo { 

    public class PlayerHealth : MonoBehaviourPun, IHittable
{ 
    
        public int health; 
        public bool isDead; 
        public const int fixedHealth = 100; 
    //EFFECTS 
        [SerializeField] private ParticleSystem afterShot;

        private void OnEnable() 
        {
            health = fixedHealth; 
            isDead = false; 
        } 
        public void TakeDamage(int damage, Vector3 hitPoint, Vector3 normal) 
        { 
            //For now, masterclient computes any damage taken first, and then others. 
            if (!PhotonNetwork.IsMasterClient)
                return;

                // Other client reacts the same after Masterclient 
            
            photonView.RPC("HealthUpdate", RpcTarget.Others, health);
            health -= damage;
            health = Mathf.Clamp(health, 0, 100); 
            //UI activity? 
            //Sound? 

            //ParticleSystem effect = GameManager.Resource.Instantiate(afterShot, hitPoint, Quaternion.LookRotation(normal), true); 
            //effect.transform.SetParent(transform); 

            if (health <= 0) { 
                Death(); 
            }
        }
        [PunRPC]
        private void HealthUpdate(int health)
        {
            this.health = health; 
        }

        [PunRPC]
        private void Death() 
        { 
            isDead = true; 
            gameObject.SetActive(false); 
        }
    }
}
