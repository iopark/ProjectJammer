using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace LDW
{
    public class Status : MonoBehaviour
    {
        public TMP_Text levelText;
        public TMP_Text hpText;
        public TMP_Text attackText;

        public int level;
        public int hp;
        public int attack;

        public void Init(int level)
        {
            Dictionary<int, Stat> statDict = GameManager.Data.StatDict;

            levelText.text = $"Level : {level}";
            hpText.text = $"HP : {statDict[level].hp}";
            attackText.text = $"Attack : {statDict[level].attack}";
        }
    }
}