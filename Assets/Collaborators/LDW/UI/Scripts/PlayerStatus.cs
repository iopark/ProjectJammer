using ildoo;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LDW
{
    public class PlayerStatus : MonoBehaviour
    {
        [SerializeField] PlayerHealth playerHealth;
        [SerializeField] TMP_Text hpText;
        [SerializeField] Slider hpSlider;

        public int playerhp;

        private void Start()
        {
            playerhp = playerHealth.health;

            hpText.text = $"{playerhp}";
        }

        public void UpdatePlayerStatusUI()
        {
            playerhp = playerHealth.health;

            hpText.text = $"{playerhp}";
            hpSlider.value = playerhp;
        }
    }
}