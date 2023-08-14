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
        PlayerHealth playerHealth;
        PlayerDeathCam postDeathCam; 
        private void Awake()
        {
            gameSceneUI = GetComponentInChildren<PlayerGameSceneUI>();
            playerInput = GetComponent<PlayerInput>();
            playerHealth = GetComponent<PlayerHealth>();
            if (photonView.IsMine)
            {
                playerHealth.onDeath += ProceedingDeath; 
                _camera = Camera.main;
                postDeathCam = _camera.transform.GetComponent<PlayerDeathCam>();
                playerHealth.onDeath += postDeathCam.ActivateUponDeath; 
                SetPlayerColor();
                PlayerData playerthis = new PlayerData(photonView.ViewID, 100, 0, true);
                GameManager.Data.playerDict.Add(photonView.ViewID, playerthis);
                gameObject.name = PhotonNetwork.LocalPlayer.NickName;
            }
            if (!photonView.IsMine)
            {
                Destroy(playerInput);
                PlayerData playerthis = new PlayerData(photonView.ViewID, 100, 0, true);
                GameManager.Data.playerDict.Add(photonView.ViewID, playerthis);
                int nonOwnerMask = LayerMask.NameToLayer("Default");
                SetGameLayerRecursive(gameObject, nonOwnerMask);
                gameObject.name = PhotonNetwork.LocalPlayer.NickName;
                gameSceneUI.gameObject.SetActive(false);
            }
        }

        private void Start()
        {
            GameManager.Data.GameOver += UnLockCursor; 
        }
        public void ProceedingDeath()
        {
            playerInput.enabled = false;
            _camera.transform.parent = deathCamHolder.transform;
            photonView.RPC("RegisterDeath", RpcTarget.AllViaServer); 
        }

        [PunRPC]
        public void RegisterDeath()
        {
             GameManager.Data.playerDict[photonView.ViewID].isAlive = false;
             GameManager.Data.DeathCount();
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
            //GameManager.Data.playerDict[photonView.ViewID].isAlive = true; 
        }

        public void UnLockCursor()
        {
            Cursor.lockState = CursorLockMode.None;
        }
        private void OnDisable()
        {
            if (playerInput == null)
                return;
            playerInput.enabled = false;
        }
    }
}
