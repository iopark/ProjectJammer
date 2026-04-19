using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LDW
{
    public class LobbyTest : MonoBehaviour
    {
        private void Start()
        {
            GameManager.UI.ShowPopUpUI<PopUpUI>("GameOver");
        }
    }
}