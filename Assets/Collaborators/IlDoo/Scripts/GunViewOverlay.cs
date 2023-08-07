using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunViewOverlay : MonoBehaviourPun
{
    [SerializeField] Transform MeleeStrikeView;
    [SerializeField] GameObject gunToShake;
    [SerializeField] Transform shakePos; 
    Camera overlayCam;
    private void Awake()
    {
        if (!photonView.IsMine)
            return;
        overlayCam = GameObject.FindGameObjectWithTag("GunCamera").GetComponent<Camera>();
        SetCamPos(); 
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

    private void ShakeCam()
    {

    }
    //Transform originalPos;

    //float timer; 
    //const float shakeTiming = 0.3f; 

    //IEnumerator GunShake()
    //{
    //    overlayCam.gameObject.transform.SetParent(MeleeStrikeView); 
    //    while (timer < shakeTiming)
    //    {

    //    }
    //    yield return null; 
    //    gunToShake.transform.position = originalPos.position;
    //    gunToShake.transform.rotation = originalPos.rotation;
    //    SetCamPos(); 
    //}
    //Temporarily turn off 
}
