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

        //=============Non Player Settings 
        [SerializeField] GunData nonPlayerGun; 
        [SerializeField] List<Color> playerColorList;
        [SerializeField] Renderer playerRender;

        //Temporary Variable 
        [SerializeField] Canvas crossHair; 
        
       
        private void Awake() 
        {
            playerInput = GetComponent<PlayerInput>();
            SetPlayerColor();
            if (!photonView.IsMine)
            {
                crossHair.gameObject.SetActive(false);
                int nonOwnerMask = LayerMask.NameToLayer("Default"); 
                Destroy(playerInput);
                SetGameLayerRecursive(gameObject, nonOwnerMask);
            }
        }

        private void SetGameLayerRecursive(GameObject gameObject, int layer)
        {
            gameObject.layer = layer;
            foreach (Transform child in gameObject.transform)
            {
                SetGameLayerRecursive(child.gameObject, layer);
            }
        }

        private void SetPlayerColor()
        {
            int playerNumber = photonView.OwnerActorNr;
            if (playerColorList == null || playerColorList.Count <= playerNumber)
                return; 
            playerRender.material.color = playerColorList[playerNumber];
        }

        public int UniquePlayerNumber()
        {
            return photonView.ViewID; 
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
