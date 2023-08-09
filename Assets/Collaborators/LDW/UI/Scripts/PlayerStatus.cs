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
        [SerializeField] TMP_Text mpText;
        [SerializeField] Slider hpSlider;

        public int playerhp;

        private void Start()
        {
            playerhp = playerHealth.health;

            hpText.text = $"{playerhp} / 500";
        }

        public void UpdatePlayerStatusUI()
        {
            hpText.text = $"{playerhp} / 500";
            hpSlider.value = playerhp;

            mpText.text = $"0 / 500";
        }
    }
}