using Photon.Pun;
using UnityEngine;
using System.Collections;
using Darik;
using UnityEngine.Animations.Rigging;
using UnityEngine.Events;
using LDW;

namespace ildoo
{
    public class Gun : MonoBehaviourPun
    {
        #region Initializing Gun Settings 
        // Computes and Executes all the gun functioning related activities
        [SerializeField] private Transform muzzlePoint;
        Camera _camera;
        Camera _gunCamera;
        FPSCameraController camController;
        [SerializeField] LayerMask targetMask;
        PlayerGameSceneUI gameSceneUI;
        //AMMO
        public int totalAmmo { get;private set; }
        public int TotalAmmo
        {
            get
            {
                return totalAmmo;
            }
            set
            {
                totalAmmo = value;  
            }
        }
        public int maxCarryRounds { get;private set; }
        public int magCap { get; private set; }
        public int maxDistance { get; private set; }
        private int currentAmmo;
        public int CurrentAmmo
        {
            get { return currentAmmo; }
            private set
            {
                currentAmmo = value;
                //Event Trigger for the UI 
            }
        }
        public float fireRate { get; private set; }
        public float reloadInterval { get; private set; }
        public int gunDamage { get; private set; }

        //EFFECTS 
        [SerializeField] AudioSource gunShot;
        [SerializeField] AudioSource reloadSound;
        AudioClip gunShotClip;
        AudioClip reloadSoundClip; 
        [SerializeField] private ParticleSystem muzzleEffect;
        [SerializeField] private ParticleSystem shellEject;
        [SerializeField] float trailLastingTime;
        WaitForSeconds bulletTrailTime;
        //ANIMATIONS
        Animator anim;
        [SerializeField] Rig animRig;
        public bool isReloading;
        WaitForSeconds reloadYieldInterval;
        [SerializeField] GunData defaultGunInfo;
        public UnityAction shotFired;
        private void Awake()
        {
            gunShotClip = gunShot.clip; 
            reloadSoundClip = reloadSound.clip;
            _camera = Camera.main;
            _gunCamera = GameObject.FindGameObjectWithTag("GunCamera").GetComponent<Camera>();
            camController = GetComponent<FPSCameraController>();
            anim = GetComponent<Animator>();
            reloadInterval = defaultGunInfo.ReloadRate;
            reloadYieldInterval = new WaitForSeconds(reloadInterval);
            bulletTrailTime = new WaitForSeconds(trailLastingTime);
            maxDistance = defaultGunInfo.MaxDistance;
            magCap = defaultGunInfo.MaxAmmo;
            fireRate = defaultGunInfo.FireRate;
            gunDamage = defaultGunInfo.Damage;
            currentAmmo = magCap;
            totalAmmo = defaultGunInfo.TotalAmmo;
            maxCarryRounds = defaultGunInfo.TotalAmmo; 
            gameSceneUI = GetComponentInChildren<PlayerGameSceneUI>(); 
        }
        private void Start()
        {
            if (!photonView.IsMine)
                return; 
            gameSceneUI.GameSceneUIUpdate(); 
        }

        #endregion
        private void OnEnable()
        {
            currentAmmo = magCap;
            totalAmmo = defaultGunInfo.TotalAmmo; 
            if (_gunCamera.gameObject.activeSelf)
                return; 
            _gunCamera.gameObject.SetActive(true);
        }

        private void OnDisable()
        {
            anim.Rebind(); 
            _gunCamera.gameObject.SetActive(false);
        }
        #region Shooting
        public void Fire()
        {
            //animation?
            muzzleEffect.Play();
            shellEject.Play();
            shotFired?.Invoke();
            gunShot.PlayOneShot(gunShotClip); 
            centrePoint = _gunCamera.ViewportToWorldPoint(middlePoint);
            localEndPoint = centrePoint + (_gunCamera.transform.forward * maxDistance);
            PostShotWorkLocal(muzzlePoint.position, localEndPoint);
            photonView.RPC("PlayerShotCalculation", RpcTarget.MasterClient, camController.camCentrePoint, camController.camCentreForward);
        }

        Vector3 centrePoint;
        Vector3 middlePoint = new Vector3(0.5f, 0.5f, 0);
        Vector3 localEndPoint;
        Vector3 endPoint;

        [PunRPC]
        public void PlayerShotCalculation(Vector3 shotPoint, Vector3 shotPointForward)
        {
            RaycastHit hit;
            if (Physics.Raycast(shotPoint, shotPointForward, out hit, maxDistance))
            {
                if (!targetMask.Contain(hit.collider.gameObject.layer))
                    return;
                //이펙트에 대해서 오브젝트 풀링으로 구현 
                IHittable hittableObj = hit.transform.GetComponent<IHittable>();
                hittableObj?.TakeDamage(gunDamage, hit.point, hit.normal);
                photonView.RPC("PostShotWorkSync", RpcTarget.All, hit.point);
            }
            else
            {
                //Where Quaternion.identity means no rotation value at all 
                endPoint = shotPoint + (muzzlePoint.transform.forward * maxDistance);
                photonView.RPC("PostShotWorkSync", RpcTarget.All, endPoint);

                //Problem with this => in other's clients, bullet trail should be released from the muzzlepoint => *maxDistance. 
            }
        }

        const float localEffectDestructionTime = 1.0f;
        Coroutine localRoutine;
        public void PostShotWorkLocal(Vector3 startPos, Vector3 endPos)
        {
            anim.SetTrigger("Fire");
            //GameManager.AudioManager.PlaySound(gunShot); 
            currentAmmo--;
            gameSceneUI.GameSceneUIUpdate();
            TrailRenderer trail = GameManager.Resource.Instantiate<TrailRenderer>("GunRelated/BulletTrailSmoke", muzzlePoint.position, Quaternion.identity, true);
            GameManager.Resource.Destroy(trail.gameObject, localEffectDestructionTime);
            if (localRoutine != null)
                StopCoroutine(localRoutine);
            localRoutine = StartCoroutine(ShotEffectLocal(trail, startPos, endPos));
        }
        Coroutine shotEffectSync;
        [PunRPC]
        public void PostShotWorkSync(Vector3 endPos)
        {
            if (photonView.IsMine)
            {
                return;
            }
            anim.SetTrigger("Fire");
            currentAmmo--;
            TrailRenderer trail = GameManager.Resource.Instantiate<TrailRenderer>("GunRelated/BulletTrailSync", muzzlePoint.position, Quaternion.identity, true);
            GameManager.Resource.Destroy(trail.gameObject, 1.2f);
            shotEffectSync = StartCoroutine(ShotEffectSync(trail, muzzlePoint.position, endPos));
        }


        IEnumerator ShotEffectLocal(TrailRenderer trail, Vector3 startPos, Vector3 endPos)
        {
            float deltaDist = Vector3.SqrMagnitude(endPos - startPos);
            trail.Clear();
            while (deltaDist > 0.1)
            {
                trail.transform.position = Vector3.Lerp(trail.transform.position, endPos, .075f);
                deltaDist = Vector3.SqrMagnitude(endPos - startPos);
                yield return null;
            }
            trail.Clear();
        }

        IEnumerator ShotEffectSync(TrailRenderer trail, Vector3 startPos, Vector3 endPos)
        {
            float totalTime = Vector2.Distance(startPos, endPos) / maxDistance;
            float time = 0;
            trail.Clear();
            while (time < 1)
            {
                trail.transform.position = Vector3.Lerp(startPos, endPos, time);
                time += Time.deltaTime / totalTime;
                yield return null;
            }
            trail.Clear(); 
            yield return null;
        }
        #endregion
        #region Reloading 
        Coroutine reloadEffect;
        public void Reload()
        {
            photonView.RPC("ReloadEffect", RpcTarget.All);
        }

        [PunRPC]
        public void ReloadEffect()
        {
            reloadEffect = StartCoroutine(Reloading());
        }
        IEnumerator Reloading()
        {
            //재장전 시작시 weight 재설정 
            isReloading = true; 
            animRig.weight = 0f;
            //GameManager.AudioManager.PlaySound(reloadSound); 
            anim.SetTrigger("Reload");
            yield return reloadYieldInterval;
            animRig.weight = 1f;
            //currentAmmo = magCap;
            ReloadCalculation(); 
            isReloading = false;
            if (photonView.IsMine)
                gameSceneUI.GameSceneUIUpdate();
        }
        int reloadAmount; 
        private void ReloadCalculation()
        {
            reloadAmount = magCap - CurrentAmmo;
            if (reloadAmount == 0)
                return; 
            else if (reloadAmount > totalAmmo)
            {
                reloadAmount = totalAmmo; 
            }
            TotalAmmo -= reloadAmount; 
            CurrentAmmo += reloadAmount; 
        }

        public bool hasMaxCarry()
        {
            return totalAmmo == maxCarryRounds; 
        }
        int newAmount;
        [PunRPC]
        public void AmmoChange(int addingAmount)
        {
            if (!photonView.IsMine)
                return;
            
            newAmount = totalAmmo + addingAmount;
            totalAmmo = Mathf.Clamp(newAmount, 0, maxCarryRounds);
            gameSceneUI.GameSceneUIUpdate();
        }
        #endregion
    }
}
