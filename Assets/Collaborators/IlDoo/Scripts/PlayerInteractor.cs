using ildoo;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteractor : MonoBehaviourPun
{
    [SerializeField] float distToDistruptor;
    FPSCameraController camController;

    private void Awake()
    {
        camController = GetComponent<FPSCameraController>();
    }

    [PunRPC] 
    public void AttemptToActivate()
    {

    }
}
