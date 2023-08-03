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
        [SerializeField]Rig animRig; 

        private void Awake()
        {
            anim = GetComponent<Animator>();
        }

        private void Start()
        {
            reloadInterval = new WaitForSeconds(currentGun.reloadInterval); 
        }
        private void OnEnable()
        {
            isReloading = false;
            isShooting = false;
            nextFire = 0f;
            fireRate = currentGun.fireRate;
        }

        void Update()
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
            if (isReloading)
                return;
            if (currentGun.CurrentAmmo <= 0)
                //TODO: Out of Ammo Interaction? 
                return;
            //TODO: Make Auto Firing Rifle 
            Fire();
            //isShooting = input.isPressed;
        }
        public void Fire()
        {
            nextFire = Time.time + fireRate;
            currentGun.Fire();
            anim.SetTrigger("Fire");
        }

        Coroutine reloading; 
        private void OnReload(InputValue input)
        {
            //if (isReloading || currentGun.CurrentAmmo == currentGun.maxAmmo)
            //    return;
            reloading  = StartCoroutine(Reload());
            currentGun.Reloaded(); 
        }

        IEnumerator Reload()
        {
            //This is synced through Photon view Animator 
            animRig.weight = 0f;
            anim.SetTrigger("Reload");
            isReloading = true;
            //재장전 시작시 weight 재설정 
            yield return reloadInterval;
            isReloading = false;
            animRig.weight = 1f;
        }
    }
}
