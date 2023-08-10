using ildoo;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunViewOverlay : MonoBehaviourPun
{
    [SerializeField] Transform MeleeStrikeView;
    [SerializeField] GameObject gunToShake;
    [SerializeField] Transform shakePos;
    Gun playerGun; 
    Camera overlayCam;
    private void Awake()
    {
        if (!photonView.IsMine)
            return;
        overlayCam = GameObject.FindGameObjectWithTag("GunCamera").GetComponent<Camera>();
        SetCamPos(); 
        playerGun = GetComponentInParent<Gun>();
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

    #region Called by the User 
    public void ChangeToMeleeCamPos()
    {
        overlayCam.gameObject.transform.SetParent(MeleeStrikeView); 
    }

    public void ReturnToGunCam()
    {
        SetCamPos(); 
    }
    #endregion

    private void Strike()
    {

    }

    public void ShakeCam()
    {
        StopAllCoroutines();
        StartCoroutine(GunShake()); 
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
        randomSource = Random.insideUnitCircle.normalized * 0.07f;
        randomPos.x = gameObject.transform.position.x + randomSource.x; //overlayCam.transform.position.x;
        randomPos.y = overlayCam.transform.position.y;// + randomSource.y;
        randomPos.z = overlayCam.transform.position.z + 0.2f;
        overlayCam.transform.position = randomPos;
        shakeTimer = 0f;
        while (shakeTimer < returnTime)
        {
            shakeTimer += Time.deltaTime;
            //overlayCam.transform.rotation = gameObject.transform.rotation; 
            overlayCam.transform.position = Vector3.Lerp(overlayCam.transform.position, gameObject.transform.position, .1f);
            yield return null;
        }
        yield return null;
        SetCamPos(); 
    }
    //Temporarily turn off 
}
