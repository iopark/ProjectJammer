using System;
using System.Collections;
using Darik;
using LDW;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace ildoo
{

    public class PlayerHealth : MonoBehaviourPun, IHittable
    {
        public UnityAction onDeath;
        public UnityAction<int> onHealthChange; 
        public int health;
        public int Health
        {
            get
            {
                return health;
            }
            set
            {
                health = value;
                onHealthChange?.Invoke(health);
            }
        }
        public bool isDead;
        public const int fixedHealth = 100;
        //EFFECTS 
        [SerializeField] private ParticleSystem afterShot;
        [SerializeField] public PlayerGameSceneUI gameSceneUI;

        private void Awake()
        {
            gameSceneUI = GetComponentInChildren<PlayerGameSceneUI>();
        }
        private void OnEnable()
        {
            health = fixedHealth;
            isDead = false;
        }

        //Debugging Purposes

        public void TakeDamage(int damage, Vector3 hitPoint, Vector3 normal)
        {
            //For now, masterclient computes any damage taken first, and then others. 
            if (!PhotonNetwork.IsMasterClient)
                return;

            // Other client reacts the same after Masterclient 
            Health -= damage;
            photonView.RPC("HealthUpdate", RpcTarget.Others, health);
            health = Mathf.Clamp(health, 0, 100);
            //Sound? 

            //ParticleSystem effect = GameManager.Resource.Instantiate(afterShot, hitPoint, Quaternion.LookRotation(normal), true); 
            //effect.transform.SetParent(transform); 

            if (health <= 0)
            {
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
            onDeath?.Invoke();
            gameObject.SetActive(false);
        }

        //Debugging purposes
        public void OnGetHit(InputValue value)
        {
            GetDamage(20);
            gameSceneUI.GameSceneUIUpdate();
            //TeamHealthManager Update Health 
        }
        public void GetDamage(int damage)
        {
            TakeDamage(damage, Vector3.zero, Vector3.zero);
        }
        [PunRPC]
        public void AddHealth(int amount)
        {
            health += amount;
            health = Mathf.Clamp(health, 0, 100);
        }

        public void Respawn()
        {
            //transform.position = Respawn Position. 
            gameObject.SetActive(true); 
        }

        IEnumerator RespawnRoutine()
        {
            yield return new WaitForSeconds(5);
            Respawn(); 
        }
    }
}
