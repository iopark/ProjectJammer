using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LDW
{
    public class Test : MonoBehaviour
    {
        public int level;
        public int hp;
        public int attack;

        void Start()
        {
            DataTest();
        }

        private void DataTest()
        {
            Dictionary<int, Stat> statDict = LDW.GameManager.Data.StatDict;

            level = 2;
            hp = statDict[level].hp;
            attack = statDict[level].attack;
        }
    }
}