using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace LDW
{
    public class PausePopUpUI : PopUpUI
    {
        protected override void Awake()
        {
            base.Awake();

            buttons["ResumeButton"].onClick.AddListener(() => { Continue(); });
            buttons["LeaveButton"].onClick.AddListener(() => { LeaveButton(); });
        }

        public void Continue()
        {
            Cursor.lockState = CursorLockMode.Locked;
            GameManager.UI.ClosePopUpUI(); 
        }
        
        public void LeaveButton()
        {
            PhotonNetwork.AutomaticallySyncScene = false;
            // PhotonNetwork.LocalPlayer.SetLoad(false);

            PhotonNetwork.LeaveRoom();

            SceneManager.LoadScene("LobbySceneLDW-0816-1620");
            GameManager.UI.ClosePopUpUI();  // ÆË¾÷ ´Ý±â(GameOverPanel)
        }
    }
}