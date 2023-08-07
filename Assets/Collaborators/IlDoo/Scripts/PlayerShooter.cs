using Photon.Pun;
using System.Collections;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.InputSystem;

namespace ildoo
{
    public class PlayerShooter : MonoBehaviourPun
    {
        public Gun currentGun;
        [SerializeField] GunViewOverlay gunCam;
        [SerializeField] private float nextFire;
        Animator anim;
        [SerializeField] Rig leftArmRig;
        [SerializeField] Rig rightArmRig;
        bool isSwinging; 
        public float fireRate { get; private set; }
        
        private void OnEnable()
        {
            nextFire = 0f;
        }

        private void Start()
        {
            anim = GetComponent<Animator>();
            fireRate = currentGun.fireRate;
            isSwinging = false;
        }
        private void Update()
        {
            //if (!photonView.IsMine)
            //    return;
            //Shot timing is controlled by LocalClient themselves 
            //Shot Contest is done by the masterClient, 
            //Shot effect is done alltogether on sync. 
            //if (isShooting && Time.time > nextFire)
            //{
            //    Fire(); 
            //}
        }

        private void OnFire(InputValue input)
        {
            //either player is firing 
            if (currentGun.isReloading)
                return;
            else if (currentGun.CurrentAmmo <= 0)
                return;
            else if (Time.time < nextFire)
            {
                return;
            }
            else if (isSwinging)
                return; 

            //TODO: Make Auto Firing Rifle 
            Fire();
        }
        public void Fire()
        {
            nextFire = Time.time + fireRate;
            currentGun.Fire();
        }

        Coroutine Striking; 
        void OnMelee(InputValue input)
        {
            Striking = StartCoroutine(Strike()); 
        }

        Coroutine reloading; 
        private void OnReload(InputValue input)
        {
            if (currentGun.isReloading 
                || currentGun.CurrentAmmo == currentGun.maxAmmo
                || isSwinging)
                return;
            currentGun.Reload(); 
        }
        //Testing 
        #region Debugging for the anim expansion 
        IEnumerator Strike()
        {
            isSwinging = true; 
            leftArmRig.weight = 0f;
            rightArmRig.weight = 1f; 
            anim.SetTrigger("Melee");
            yield return new WaitForSeconds(.5f);
            leftArmRig.weight = 1f;
            rightArmRig.weight = 0f;
            isSwinging = false;
        }
        #endregion
    }
}
