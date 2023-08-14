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
        [SerializeField] int fixedHealth; 

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
                if (photonView.IsMine)
                    gameSceneUI.GameSceneUIUpdate();
            }
        }
        public bool isDead;
        //EFFECTS 
        [SerializeField] private ParticleSystem afterShot;
        [SerializeField] public PlayerGameSceneUI gameSceneUI;
        Animator anim;

        Renderer[] playerRenderer;
        private void Awake()
        {
            anim = GetComponent<Animator>();
            health = fixedHealth; 
            if (photonView.IsMine)
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
            if (photonView.IsMine)
                gameSceneUI.GameSceneUIUpdate();
        }
        private void Death()
        {
            isDead = true;
            if (photonView.IsMine)
                onDeath?.Invoke(); // Preceeding disabling gameobj; 
            photonView.RPC("DeathSync", RpcTarget.AllViaServer); 
        }

        [PunRPC]
        public void DeathSync()
        {
            StartCoroutine(DeathRoutine());
        }

        public bool isFullHealth()
        {
            return health == 100; 
        }
        [PunRPC]
        public void AddHealth(int amount)
        {
            if (!photonView.IsMine || isDead)
                return; 
            Health += amount;
            Health = Mathf.Clamp(health, 0, 100);
        }

        public void Respawn()
        {
            //transform.position = Respawn Position. 
            //Turn off the Renderer ONLY 

            gameObject.SetActive(true); 
        }

        IEnumerator DeathRoutine()
        {
            anim.SetTrigger("PlayerDeath");
            yield return new WaitForSeconds(3f); 
            gameObject.SetActive(false);
        }

        IEnumerator RespawnRoutine()
        {
            yield return new WaitForSeconds(5);
            Respawn(); 
        }
        #region Debugging 
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
        #endregion
    }
}
