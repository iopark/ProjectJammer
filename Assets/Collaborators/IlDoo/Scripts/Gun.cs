using Photon.Pun; 
using UnityEngine;
using System;
using System.Collections;

public class Gun : MonoBehaviourPun, IPunObservable
{
    
    // Computes and Executes all the gun functioning related activities
    private Transform muzzlePoint; 

    //AMMO
    public int maxAmmo {get; private set;}
    public int maxDistance {get; private set;}
    public int currentAmmo { get; private set;}
    public float fireRate { get; private set;} 
    //EFFECTS 
    private ParticleSystem muzzleEffect; 
    private TrailRenderer bulletTrail;
    [SerializeField] float trailLastingTime;
    WaitForSeconds bulletTrailTime; 

    //RELOAD
    public bool isReloading { get; private set;} = false;
    WaitForSeconds reloadInterval; 

    [SerializeField] GunData defaultGunInfo;
    private void Awake()
    {
        reloadInterval = new WaitForSeconds(defaultGunInfo.ReloadRate);
        bulletTrailTime = new WaitForSeconds(trailLastingTime); 
    }
    private void OnEnable() 
    {
        maxAmmo = defaultGunInfo.MaxAmmo; 
        maxDistance = defaultGunInfo.MaxDistance;
        fireRate = defaultGunInfo.FireRate;
        currentAmmo = maxAmmo; 
    }
    
    #region Shooting
    public void Fire(Action fireAnim) 
    {
        //animation? 
        photonView.RPC("PlayerShotCalculation", RpcTarget.MasterClient, fireAnim); 
    }

    [PunRPC]
    public void PlayerShotCalculation() 
    { 
        //MasterClient calculation for shots 

        photonView.RPC("PostShotWork", RpcTarget.AllViaServer); 
    }

    public void PostShotWork(Vector3 hitLoc, Vector3 hitNormal) 
    { 

    }
    IEnumerator ShotEffect() 
    { 

    }
    #endregion

    #region Reloading 

    IEnumerator Reload() 
    { 

    }
    #endregion
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) 
    {
        if (stream.IsWriting)
        {
            stream.SendNext(currentAmmo);
            stream.SendNext(isReloading);
        }
        else
        {
            currentAmmo = (int) stream.ReceiveNext();
            isReloading = (bool) stream.ReceiveNext();
        }
    }
}