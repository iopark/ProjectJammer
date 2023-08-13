using Photon.Pun;
using LDW;
using Photon.Pun.UtilityScripts;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Unity.VisualScripting;

namespace ildoo
{
    public class Player : MonoBehaviourPun

    {
        //Manages 1.Player Color      3.PlayerInput
        private PlayerInput playerInput;
        //=============Non Player Settings 
        [SerializeField] Transform deathCamHolder;
        Camera _camera;
        PlayerGameSceneUI gameSceneUI;
        [SerializeField] GunData nonPlayerGun;
        [SerializeField] List<Color> playerColorList;
        [SerializeField] Renderer playerRender;

        //OnDeathSettings 
        private void Awake()
        {
            playerInput = GetComponent<PlayerInput>();
            gameSceneUI = GetComponentInChildren<PlayerGameSceneUI>();
            _camera = Camera.main;
            SetPlayerColor();
            PlayerData playerthis = new PlayerData(photonView.ViewID, 100, 0, true);
            GameManager.Data.playerDict.Add(photonView.ViewID, playerthis);
            gameObject.name = PhotonNetwork.LocalPlayer.NickName; 
            if (!photonView.IsMine)
            {
                int nonOwnerMask = LayerMask.NameToLayer("Default");
                Destroy(playerInput);
                SetGameLayerRecursive(gameObject, nonOwnerMask);
                gameSceneUI.gameObject.SetActive(false);
            }
        }
        public void MoveCamera()
        {
            if (photonView.IsMine)
            {
                _camera.transform.parent = transform.parent;

            }
        }

        [PunRPC]
        public void RegisterDeath()
        {
            if (PhotonNetwork.IsMasterClient)
            {
                GameManager.Data.playerDict[photonView.ViewID].isAlive = false;
                GameManager.Data.DeathCount(); 
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
            GameManager.Data.playerDict[photonView.ViewID].isAlive = true; 
        }

        private void OnDisable()
        {
            if (playerInput == null)
                return;
            playerInput.enabled = false;
        }
    }
}
