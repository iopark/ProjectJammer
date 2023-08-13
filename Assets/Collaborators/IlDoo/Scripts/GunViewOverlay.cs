using ildoo;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class GunViewOverlay : MonoBehaviourPun
{
    [SerializeField] Transform MeleeStrikeView;
    [SerializeField] GameObject gunToShake;
    [SerializeField] GameObject physicalScope; 
    [SerializeField] float shakeStrength;
    [SerializeField] float shakeSpeed;
    [SerializeField] Transform zoomPos;
    [SerializeField] public Transform aimTarget;
    Volume zoomEffect; 

    bool isZooming; 
    Gun playerGun;
    PlayerShooter playerShooter; 
    Camera overlayCam;
    private void Awake()
    {
        if (!photonView.IsMine)
            return;
        overlayCam = GameObject.FindGameObjectWithTag("GunCamera").GetComponent<Camera>();
        SetCamPos();
        zoomEffect = Camera.main.GetComponent<Volume>();
        playerShooter = GetComponentInParent<PlayerShooter>(); 
        playerGun = GetComponentInParent<Gun>();
        playerShooter.zoomIn += ZoomInAction;
        playerGun.shotFired += ShakeCam;
    }
    private void SetCamPos()
    {
        overlayCam.gameObject.transform.SetParent(transform);
        overlayCam.transform.position = transform.position;
        overlayCam.transform.rotation = transform.rotation;
    }

    //Gun Shake 
    private void Start()
    {

    }

    private void LateUpdate()
    {

    }
    #region Called by the User 
    public void ChangeToMeleeCamPos()
    {
        overlayCam.gameObject.transform.SetParent(MeleeStrikeView);
    }

    Coroutine zoomInRoutine;
    Coroutine zoomOutRoutine; 
    public void ZoomInAction(bool zoomState)
    {
        if (zoomState)
        {
            if (zoomOutRoutine != null)
                StopCoroutine(zoomOutRoutine);
            zoomInRoutine = StartCoroutine(ZoomInAction()); 
        }
        else
        {
            if (zoomInRoutine != null)
                StopCoroutine(zoomInRoutine);
            zoomOutRoutine = StartCoroutine(ZoomOutAction());
        }
    }

    public void ReturnToGunCam()
    {
        SetCamPos();
    }
    #endregion

    public void ShakeCam()
    {
        if (isZooming)
        {
            StopAllCoroutines();
            StartCoroutine(ZoomGunShake()); 
        }
        else
        {
            StopAllCoroutines();
            StartCoroutine(GunShake());
        }
    }
    //Transform originalPos;

    //float timer; 
    //const float shakeTiming = 0.3f; 

    Vector2 randomSource;
    Vector3 randomPos;
    //This should be implemented through the localPositioning and rotation. 

    const float returnTime = .2f;
    float shakeTimer;
    Vector3 originalPos;
    IEnumerator GunShake()
    {
        originalPos = overlayCam.transform.position;
        randomPos.x = overlayCam.transform.localPosition.x;
        randomPos.y = overlayCam.transform.localPosition.y;// + randomSource.y;
        randomPos.z = overlayCam.transform.localPosition.z + shakeStrength;// + 0.03f;
        overlayCam.transform.localPosition = randomPos;
        overlayCam.transform.rotation = transform.rotation;
        shakeTimer = 0f;
        while (shakeTimer < returnTime)
        {
            shakeTimer += Time.deltaTime;
            //overlayCam.transform.rotation = gameObject.transform.rotation; 
            overlayCam.transform.position = Vector3.Lerp(overlayCam.transform.position, gameObject.transform.position, shakeSpeed);
            yield return null;
        }
        yield return null;
        SetCamPos();
    }

    IEnumerator ZoomGunShake()
    {
        originalPos = overlayCam.transform.position;
        randomPos.x = overlayCam.transform.localPosition.x;
        randomPos.y = overlayCam.transform.localPosition.y;// + randomSource.y;
        randomPos.z = overlayCam.transform.localPosition.z + shakeStrength;// + 0.03f;
        overlayCam.transform.localPosition = randomPos;
        overlayCam.transform.rotation = transform.rotation;
        shakeTimer = 0f;
        while (shakeTimer < returnTime)
        {
            shakeTimer += Time.deltaTime;
            //overlayCam.transform.rotation = gameObject.transform.rotation; 
            overlayCam.transform.rotation = zoomPos.rotation;
            overlayCam.transform.position = Vector3.Lerp(overlayCam.transform.position, zoomPos.position, shakeSpeed);
            yield return null;
        }
        yield return null;
        SetCamPos();
    }

    float deltaDist;
    const float normalNearClipPlane = .15f;
    const float zoomNearClipPlane = .01f; 
    IEnumerator ZoomInAction()
    {
        isZooming = true;
        deltaDist = Vector3.SqrMagnitude(overlayCam.transform.position - zoomPos.position);
        physicalScope.gameObject.SetActive(false);
        overlayCam.nearClipPlane = zoomNearClipPlane; 
        while (deltaDist > 0.001)
        {
            deltaDist = Vector3.SqrMagnitude(overlayCam.transform.position - zoomPos.position);
            overlayCam.transform.position = Vector3.Lerp(overlayCam.transform.position, zoomPos.position, .35f);
            zoomEffect.weight = Mathf.Lerp(zoomEffect.weight, 1f, .1f); 
            yield return null;
        }
        overlayCam.transform.position = zoomPos.position;
        overlayCam.transform.rotation = zoomPos.rotation;
        zoomEffect.weight = 1f; 
       yield return null;
    }
    IEnumerator ZoomOutAction()
    {
        deltaDist = Vector3.SqrMagnitude(overlayCam.transform.position - gameObject.transform.position);
        overlayCam.nearClipPlane = normalNearClipPlane;
        zoomEffect.weight = 0f; 
        while (deltaDist > 0.001)
        {
            deltaDist = Vector3.SqrMagnitude(overlayCam.transform.position - gameObject.transform.position); 
            overlayCam.transform.position = Vector3.Lerp(overlayCam.transform.position, gameObject.transform.position, .45f);
            isZooming = false;
            physicalScope.gameObject.SetActive(true);
            zoomEffect.weight = Mathf.Lerp(zoomEffect.weight, 0, .45f);
            yield return null;
        }
        yield return null;
        zoomEffect.weight = 0f; 
        physicalScope.gameObject.SetActive(true);
        SetCamPos(); 
    }
    //Temporarily turn off 
}
