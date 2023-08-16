using LDW;
using Photon.Pun;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions; 

namespace ildoo
{
    public class PlayerShooter : MonoBehaviourPun
    {
        public Gun currentGun;

        public UnityAction<bool> zoomIn; 
        [SerializeField] GunViewOverlay gunCam;
        Crosshair crossHairUI; 
        [SerializeField] private float nextFire;
        Animator anim;
        [SerializeField] Rig leftArmRig;
        [SerializeField] Rig rightArmRig;
        bool isSwinging;
        bool isRapidFiring; 
        bool isShooting; 
        public float fireRate { get; private set; }
        [SerializeField] float rapidFireEval;

        private void Start()
        {
            if (!photonView.IsMine)
                return; 
            crossHairUI = GetComponentInChildren<Crosshair>();
            zoomIn += crossHairUI.ChangeCrosshair; 
            anim = GetComponent<Animator>();
            fireRate = currentGun.fireRate;
            isSwinging = false;
            isRapidFiring = false; 
            isShooting = false;
        }
        private void Update()
        {
            if (photonView.IsMine)
                ContestForRapidFire(); 
        }

        private void ContestForRapidFire()
        {
            if (!isShooting)
            {
                holdTimer = 0f;
                if (isRapidFiring)
                {
                    StopAllCoroutines();
                    isRapidFiring= false;
                }
            }
            else
            {
                holdTimer += Time.deltaTime;
                if (holdTimer > rapidFireEval)
                {
                    StartCoroutine(RapidFire());
                }
                else
                {
                    Fire(); 
                }
            }
        }
        float holdTimer = 0f;  
        private void OnFire(InputValue value)
        {
            isShooting = value.isPressed;
            if (isShooting)
                Fire(); 
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
        private void OnReload(InputValue input)
        {
            if (currentGun.isReloading 
                || currentGun.CurrentAmmo == currentGun.magCap
                || isSwinging)
                return;
            currentGun.Reload(); 
        }
        IEnumerator RapidFire()
        {
            while (true)
            {
                isRapidFiring = true; 
                Fire();
                yield return null; 
            }
        }

        private void OnZoom(InputValue input)
        {
            zoomIn?.Invoke(input.isPressed); 
        }
        #region Deprecated 
        //public void OnFire(InputAction.CallbackContext context)
        //{
        //    //isShooting = value.isPressed;
        //    //Fire();
        //    //if (value.Get<>)
        //    //either player is firing 
        //    if (context.started)
        //    {
        //        //Register, try for fire. 
        //        //Run the Hold Timer for further analysis on the Rapid Firing 
        //        if (context.duration >= rapidFireEval && !isRapidFiring)
        //        {
        //            isRapidFiring = true;
        //            StartCoroutine(RapidFire());
        //        }
        //        else
        //        {
        //            Fire();
        //        }
        //    }
        //    else if (context.canceled)
        //    {
        //        isRapidFiring = false;
        //        holdTimer = 0f;
        //        StopAllCoroutines();
        //        //If there has been any 
        //    }
        //}
        #endregion
    }
}
