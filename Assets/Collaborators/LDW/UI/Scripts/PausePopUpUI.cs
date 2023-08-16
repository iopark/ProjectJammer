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

            buttons["ResumeButton"].onClick.AddListener(() => { GameManager.UI.ClosePopUpUI(); });
            buttons["LeaveButton"].onClick.AddListener(() => { LeaveButton(); });
        }

        public void LeaveButton()
        {
            PhotonNetwork.AutomaticallySyncScene = false;
            // PhotonNetwork.LocalPlayer.SetLoad(false);

            PhotonNetwork.LeaveRoom();

            SceneManager.LoadScene("LobbySceneLDW-0816-1620");
            GameManager.UI.ClosePopUpUI();  // ÆË¾÷ ´Ý±â(GameOverPanel)
        }

        public void Test()
        {
            if (GameManager.Data.DisruptorProgress == 100)
                return;

            if (GameManager.UI.popUp_Stack.Count > 0)
                GameManager.UI.ClosePopUpUI();

            GameManager.UI.ShowPopUpUI<PopUpUI>("PausePopUpUI");
        }
    }
}