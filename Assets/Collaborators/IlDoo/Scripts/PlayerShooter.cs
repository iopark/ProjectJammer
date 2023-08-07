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

        [SerializeField] private float nextFire;
        public float fireRate { get; private set; }
        
        private void OnEnable()
        {
            nextFire = 0f;
            fireRate = currentGun.fireRate;
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
                //TODO: Out of Ammo Interaction? 
                return;
            else if (Time.time < nextFire)
            {
                Debug.Log("Recharging"); 
                return;
            }

            //TODO: Make Auto Firing Rifle 
            Fire();
            //isShooting = input.isPressed;
        }
        public void Fire()
        {
            nextFire = Time.time + fireRate;
            currentGun.Fire();
        }

        Coroutine reloading; 
        private void OnReload(InputValue input)
        {
            if (currentGun.isReloading || currentGun.CurrentAmmo == currentGun.maxAmmo)
                return;
            currentGun.Reload(); 
        }
    }
}
