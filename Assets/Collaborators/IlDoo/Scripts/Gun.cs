using Photon.Pun; 
using UnityEngine;
using System;
using System.Collections;

namespace ildoo
{
    public class Gun : MonoBehaviourPun, IPunObservable
    {
        // Computes and Executes all the gun functioning related activities
        private Transform muzzlePoint;
        Camera _camera; 

        //AMMO
        public int maxAmmo { get; private set; }
        public int maxDistance { get; private set; }
        public int currentAmmo { get; private set; }
        public float fireRate { get; private set; }
        public float reloadInterval { get; private set; }
        public int gunDamage { get; private set; }

        //EFFECTS 
        private ParticleSystem muzzleEffect;
        private TrailRenderer bulletTrail;
        [SerializeField] float trailLastingTime;
        WaitForSeconds bulletTrailTime;


        [SerializeField] GunData defaultGunInfo;
        private void Awake()
        {
            _camera = Camera.main;
            defaultGunInfo = GetComponent<GunData>();
        }
        private void Start()
        {
            reloadInterval = defaultGunInfo.ReloadRate;
            bulletTrailTime = new WaitForSeconds(trailLastingTime);
            maxDistance = defaultGunInfo.MaxDistance;
            maxAmmo = defaultGunInfo.MaxAmmo;
            fireRate = defaultGunInfo.FireRate;
            gunDamage = defaultGunInfo.Damage;
        }
        private void OnEnable()
        {
            currentAmmo = maxAmmo;
        }

        #region Shooting
        public void Fire()
        {
            //animation? 
            photonView.RPC("PlayerShotCalculation", RpcTarget.MasterClient);
        }

        Vector3 centrePoint;
        Vector3 middlePoint = new Vector3(0.5f, 0.5f, 0);
        Vector3 endPoint; 
        [PunRPC]
        public void PlayerShotCalculation()
        {
            //MasterClient calculation for shots 
            centrePoint = _camera.ViewportToWorldPoint(middlePoint); 
            RaycastHit hit;
            if (Physics.Raycast(centrePoint, _camera.transform.forward, out hit, maxDistance))
            {
                //����Ʈ�� ���ؼ� ������Ʈ Ǯ������ ���� 
                IHittable hittableObj = hit.transform.GetComponent<IHittable>(); // Interface�� Componenentó�� ����� �����ϴ�: how crazy is that;
                hittableObj?.TakeDamage(gunDamage, hit.point, hit.normal); // if ain't null, proceed with Hit, else, return; 
                photonView.RPC("PostShotWork", RpcTarget.AllViaServer, muzzlePoint.position, hit.point);
            }
            else
            {
                //Where Quaternion.identity means no rotation value at all 
                endPoint = centrePoint +(_camera.transform.forward * maxDistance);
                photonView.RPC("PostShotWork", RpcTarget.AllViaServer, muzzlePoint.position, endPoint);
            }
            
        }

        Coroutine shotEffect; 
        public void PostShotWork(Vector3 startPos, Vector3 endPos)
        {
            shotEffect = StartCoroutine(ShotEffect(startPos, endPos)); 
        }

        IEnumerator ShotEffect(Vector3 startPos, Vector3 endPos)
        {
            //TODO: Adding Sound
            //TODO: Adding Trail 
            yield return null; 
        }
        #endregion

        public void Reloaded()
        {
            //called on by the shooter 
            currentAmmo = maxAmmo; 
        }

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                stream.SendNext(currentAmmo);
            }
            else
            {
                currentAmmo = (int)stream.ReceiveNext();
            }
        }
    }
}