using Photon.Pun;
using Photon.Pun.UtilityScripts;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ildoo 
{ 
    public class Player : MonoBehaviourPun 
    {
       //Manages 1.Player Color      3.PlayerInput
        private PlayerInput playerInput;
        [SerializeField] List<Color> playerColorList;
        [SerializeField] Renderer playerRender; 
       
        private void Awake() 
        {
            playerInput = GetComponent<PlayerInput>();
            SetPlayerColor();
            if (!photonView.IsMine)
                Destroy(playerInput);
        } 
    
        private void SetPlayerColor()
        {
            int playerNumber = photonView.Owner.GetPlayerNumber();
            if (playerColorList == null || playerColorList.Count <= playerNumber)
                return; 
            playerRender.material.color = playerColorList[playerNumber];
        }

       //In case of Death and Revive
        
        private void OnEnable() 
        {
            if (playerInput == null || playerInput.enabled)
                return; 
            playerInput.enabled = true; 
        }
        
        private void OnDisable() 
        {
            if (playerInput == null)
                return; 
            playerInput.enabled = false; 
        }
    }
}
