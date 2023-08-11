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

            buttons["SettingButton"].onClick.AddListener(() => { LobbyButton(); });
        }

        public void LobbyButton()
        {
            GameManager.UI.ClosePopUpUI();
            GameManager.UI.ShowPopUpUI<PopUpUI>("GameOver");
        }
    }
}