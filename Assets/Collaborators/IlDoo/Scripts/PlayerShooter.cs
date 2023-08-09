using Photon.Pun;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions; 

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
        bool isRapidFiring; 
        bool isShooting; 
        public float fireRate { get; private set; }
        [SerializeField] float rapidFireEval;
        private void OnEnable()
        {
            var 
            nextFire = 0f;
        }

        private void Start()
        {
            anim = GetComponent<Animator>();
            fireRate = currentGun.fireRate;
            isSwinging = false;
            isRapidFiring = false; 
            isShooting = false;
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
            ContestForRapidFire(); 
        }

        private void ContestForRapidFire()
        {

        }
        float holdTimer = 0f;  
        private void OnFire(InputValue value)
        {
            Fire(); 
        }
        public void OnFire(InputAction.CallbackContext context)
        {
            //isShooting = value.isPressed;
            //Fire();
            //if (value.Get<>)
            //either player is firing 
            if (context.started)
            {
                //Register, try for fire. 
                //Run the Hold Timer for further analysis on the Rapid Firing 
                if (context.duration >= rapidFireEval && !isRapidFiring)
                {
                    isRapidFiring = true;
                    StartCoroutine(RapidFire());
                }
                else
                {
                    Fire();
                }
            }
            else if (context.canceled)
            {
                isRapidFiring = false;
                holdTimer = 0f;
                StopAllCoroutines();
                //If there has been any 
            }
        }
        public void Fire()
        {
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
            nextFire = Time.time + fireRate;
            currentGun.Fire();
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
        
        IEnumerator RapidFire()
        {
            while (true)
            {
                Fire();
            }
        }
    }
}
