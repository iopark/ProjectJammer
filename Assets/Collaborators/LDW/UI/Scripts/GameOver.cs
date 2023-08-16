using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace LDW
{
    public class GameOver : PopUpUI
    {
        protected override void Awake()
        {
            base.Awake();

            buttons["LobbyButton"].onClick.AddListener(() => { LobbyButton(); });
        }

        public void LobbyButton()
        {
            PhotonNetwork.AutomaticallySyncScene = false;
            // PhotonNetwork.LocalPlayer.SetLoad(false);

            PhotonNetwork.LeaveRoom();

            SceneManager.LoadScene("LobbySceneLDW-0816-1620");
            GameManager.UI.ClosePopUpUI();  // �˾� �ݱ�(GameOverPanel)
        }
    }
}