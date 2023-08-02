using Photon.Pun;
using UnityEngine;

public class PlayerShooter: MonoBehaviourPun 
{ 
    public GunFunction currentGun; 

    private float nextFire; 
    public float fireRate {get;private set;}
    public bool isShooting; 
    Animator anim; 

    private void Awake() 
    {
        anim = GetComponent<Animator>(); 
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
            currentGun.Fire(); 
            nextFire = Time.time + fireRate; 
        }
    }
    private void OnFire(InputValue input)
    {
        if (!photonView.IsMine)
            return;
        //either player is firing 
        else if (currentGun.isReloading)
            return;
        else if (currentGun.currentAmmo <= 0) 
            return;
        
        isShooting = input.isPressed();
        Fire(FireAnimation); 
    }
    public void Fire()
    {
        currentGun.Fire();
        anim.SetTrigger("Fire");
    }

    public void FireAnimation() 
    { 
        
    }

    private void OnReload(InputValue input)
    {
        if (reloading)
            return;
        
        else if (currentGun.MaxAmmo == currentGun.CurrentAmmo)
            return; 
        StartCoroutine(ReloadRoutine()); 
    }

    IEnumerator Reload() 
    {
        //This is synced through Photon view Animator 

    }
}