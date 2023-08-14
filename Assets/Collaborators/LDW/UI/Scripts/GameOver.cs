using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LDW
{
    public class GameOver : PopUpUI
    {
        private Animation anim;

        protected override void Awake()
        {
            base.Awake();

            anim = GetComponent<Animation>();

            buttons["LobbyButton"].onClick.AddListener(() => { LobbyButton(); });
        }

        private void OnEnable()
        {
            anim.Play("GameOver");
        }

        public void LobbyButton()
        {
            GameManager.UI.ClosePopUpUI();  // �˾� �ݱ�(GameOverPanel)
            PhotonNetwork.LeaveRoom();      // �κ�� ���ư��� (�濡�� ������)
        }
    }
}