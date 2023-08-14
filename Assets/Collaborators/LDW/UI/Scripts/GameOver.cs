using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LDW
{
    public class GameOver : PopUpUI
    {
        private Animator anim;

        protected override void Awake()
        {
            base.Awake();

            anim = GetComponent<Animator>();

            buttons["LobbyButton"].onClick.AddListener(() => { LobbyButton(); });
        }

        public void LobbyButton()
        {
            GameManager.UI.ClosePopUpUI();  // 팝업 닫기(GameOverPanel)
            PhotonNetwork.LeaveRoom();      // 로비로 돌아가기 (방에서 나가기)
        }
    }
}