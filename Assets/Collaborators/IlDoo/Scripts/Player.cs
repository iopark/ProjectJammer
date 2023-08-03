using Photon.Pun;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ildoo 
{ 
public class Player : MonoBehaviourPun 
{
   private PlayerInput playerInput;
   private void Awake() 
   {
        playerInput = GetComponent<PlayerInput>(); 
        if (!photonView.IsMine)
            Destroy(playerInput); 
   } 

   //In case of Death and Revive
    private void OnEnable() 
   {
        if (playerInput == null)
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
