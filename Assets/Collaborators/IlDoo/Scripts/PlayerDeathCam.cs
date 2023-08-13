using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ildoo
{
    public class PlayerDeathCam : MonoBehaviour
    {
        PlayerInput playerInput;
        ildoo.Player player; 

        private void Awake()
        {
            playerInput = GetComponent<PlayerInput>();
            playerInput.enabled = false;
        }

    }
}

