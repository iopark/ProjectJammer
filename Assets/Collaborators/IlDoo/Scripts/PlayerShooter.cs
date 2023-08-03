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

        private float nextFire;
        public float fireRate { get; private set; }
        public bool isShooting;
        public bool isReloading;
        WaitForSeconds reloadInterval;
        Animator anim;
        Rig animRig; 

        private void Awake()
        {
            anim = GetComponent<Animator>();
            animRig = GetComponent<Rig>(); 
        }

        private void Start()
        {
            reloadInterval = new WaitForSeconds(currentGun.reloadInterval); 
        }
        private void OnEnable()
        {
            nextFire = 0f;
            isShooting = false;
            fireRate = currentGun.fireRate;
        }

        void Update()
        {
            if (!photonView.IsMine)
                return;
            //Shot timing is controlled by LocalClient themselves 
            //Shot Contest is done by the masterClient, 
            //Shot effect is done alltogether on sync. 
            if (isShooting && Time.time > nextFire)
            {
                Fire(); 
                
            }
        }
        private void OnFire(InputValue input)
        {
            if (!photonView.IsMine)
                return;
            //either player is firing 
            if (isReloading)
                return;
            if (currentGun.currentAmmo <= 0)
                //TODO: Out of Ammo Interaction? 
                return;
            //TODO: 
            Fire();
            isShooting = input.isPressed;
        }
        public void Fire()
        {
            nextFire = Time.time + fireRate;
            currentGun.Fire();
            anim.SetTrigger("Fire");
        }

        private void OnReload(InputValue input)
        {
            if (isReloading || currentGun.currentAmmo == currentGun.maxAmmo)
                return;
            StartCoroutine(Reload());
            currentGun.Reloaded(); 
        }

        IEnumerator Reload()
        {
            //This is synced through Photon view Animator 
            anim.SetTrigger("Reload");
            isReloading = true;
            //������ ���۽� weight �缳�� 
            animRig.weight = 0f; 
            yield return reloadInterval;
            isReloading = false;
            animRig.weight = 1f;
        }
    }
}
