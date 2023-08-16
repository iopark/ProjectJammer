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
            SetPlayerColor();

            if (photonView.IsMine)
            {
                playerHealth.onDeath += ProceedingDeath; 
                _camera = Camera.main;
                postDeathCam = _camera.transform.GetComponent<PlayerDeathCam>();
                playerHealth.onDeath += postDeathCam.ActivateUponDeath;

                if (!GameManager.Data.playerDict.ContainsKey(photonView.ViewID))
                {
                    PlayerData playerthis = new PlayerData(photonView.ViewID, 100, 0, true);
                    GameManager.Data.playerDict.Add(photonView.ViewID, playerthis);
                }
                else
                {
                    GameManager.Data.playerDict[photonView.ViewID].isAlive = true;
                }
                gameObject.name = PhotonNetwork.LocalPlayer.NickName;
            }
            if (!photonView.IsMine)
            {
                Destroy(playerInput);
                if (!GameManager.Data.playerDict.ContainsKey(photonView.ViewID))
                {
                    PlayerData playerthis = new PlayerData(photonView.ViewID, 100, 0, true);
                    GameManager.Data.playerDict.Add(photonView.ViewID, playerthis);
                }
                else
                {
                    GameManager.Data.playerDict[photonView.ViewID].isAlive = true;
                }
                int nonOwnerMask = LayerMask.NameToLayer("Default");
                SetGameLayerRecursive(gameObject, nonOwnerMask);
                gameObject.name = PhotonNetwork.LocalPlayer.NickName;
                gameSceneUI.gameObject.SetActive(false);
            }
        }

        private void Start()
        {
            if (photonView.IsMine)
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
            if (photonView.IsMine)
                Cursor.lockState = CursorLockMode.None;
        }
        private void OnDisable()
        {
            if (playerInput == null)
                return;
            playerInput.enabled = false;
            if (photonView.IsMine)
                GameManager.Data.GameOver -= UnLockCursor; 
        }
        public void OnPauseAttempt(InputValue value)
        {
            if (Cursor.lockState == CursorLockMode.Locked)
                Cursor.lockState = CursorLockMode.None;
            else
                Cursor.lockState = CursorLockMode.Locked;

            if (GameManager.Data.DisruptorProgress == 100)
                return;

            if (GameManager.UI.popUp_Stack.Count > 0)
                GameManager.UI.ClosePopUpUI();

            GameManager.UI.ShowPopUpUI<PopUpUI>("PausePopUpUI");
        }
    }
}
