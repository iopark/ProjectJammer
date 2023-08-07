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
    [SerializeField] float meleeDistance; 
    [SerializeField] LayerMask targetMask;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        _camera = Camera.main;
    }
    public void Strike()
    {
        photonView.RPC("MeleeCalculation", RpcTarget.MasterClient); 
    }

    Vector3 centrePoint;
    Vector3 middlePoint = new Vector3(0.5f, 0.5f, 0);
    Vector3 localEndPoint;
    Vector3 endPoint;
    [PunRPC]
    public void MeleeCalculation()
    {

        photonView.RPC("ClientResponse", RpcTarget.All); 
    }

    [PunRPC]
    public void ClientResponse()
    {

    }

    private void OnMelee(InputValue input)
    {

    }
    #region Melee Attack Under Construction 
    //Coroutine Striking;
    //IEnumerator Striking()
    //{
    //    isSwinging = true;
    //    leftArmRig.weight = 0f;
    //    rightArmRig.weight = 1f;
    //    anim.SetTrigger("Melee");
    //    yield return new WaitForSeconds(.5f);
    //    leftArmRig.weight = 1f;
    //    rightArmRig.weight = 0f;
    //    isSwinging = false;
    //}
    #endregion
}
