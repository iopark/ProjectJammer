using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LDW
{
    public class Test : MonoBehaviour
    {
        public int id;
        public int hp;
        public int attack;

        void Start()
        {
            DataTest();
        }

        private void DataTest()
        {
            Dictionary<int, Stat> monsterStatDict = GameManager.Data.MonsterStatDict;

            id = monsterStatDict[id].id;
            hp = monsterStatDict[id].hp;
            attack = monsterStatDict[id].attack;
        }
    }
}