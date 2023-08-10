using Darik;
using ildoo;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.InputSystem;

public class PlayerMelee : MonoBehaviourPun
{
    Camera _camera;
    Animator anim;
    [SerializeField] Rig leftArmRig;
    [SerializeField] Rig rightArmRig;
    [SerializeField] int meleeDamage; 
    [SerializeField] float meleeDistance; 
    [SerializeField] LayerMask targetMask;
    FPSCameraController camController;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        _camera = Camera.main;
        camController = GetComponent<FPSCameraController>();
    }
    public void Strike()
    {
        photonView.RPC("MeleeCalculation", RpcTarget.MasterClient, camController.camCentrePoint, camController.camCentreForward); 
    }

    Vector3 centrePoint;
    Vector3 middlePoint = new Vector3(0.5f, 0.5f, 0);
    Vector3 localEndPoint;
    Vector3 endPoint;
    [PunRPC]
    public void MeleeCalculation(Vector3 shotPoint, Vector3 shotPointForward)
    {
        RaycastHit hit;
        if (Physics.Raycast(shotPoint, shotPointForward, out hit, meleeDistance, targetMask))
        {
            //이펙트에 대해서 오브젝트 풀링으로 구현 
            IHittable hittableObj = hit.transform.GetComponent<IHittable>();
            hittableObj?.TakeDamage(meleeDamage, hit.point, hit.normal);
        }
        photonView.RPC("ClientResponse", RpcTarget.All); 
    }
    Coroutine StrikeAnim;
    [PunRPC]
    public void ClientResponse()
    {
        if (photonView.IsMine)
            return; 
        StrikeAnim = StartCoroutine(Striking()); 
    }

    private void OnMelee(InputValue input)
    {
        StrikeAnim = StartCoroutine(Striking()); 
    }
    #region Melee Attack Under Construction 

    IEnumerator Striking()
    {
        leftArmRig.weight = 0f;
        rightArmRig.weight = 1f;
        anim.SetTrigger("Melee");
        yield return new WaitForSeconds(.25f);

        leftArmRig.weight = 1f;
        rightArmRig.weight = 0f;
    }
    #endregion
}
