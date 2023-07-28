using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LDW 
{
    #region Stat
    [Serializable]
    public class Stat
    {
        public int id; // ID
        public int hp;
        public int attack;
    }

    [Serializable]
    public class StatData : ILoader<int, Stat>
    {
        public List<Stat> stats = new List<Stat>();  // json ���Ͽ��� ����� ���

        public Dictionary<int, Stat> MakeDict() // �������̵�
        {
            Dictionary<int, Stat> dict = new Dictionary<int, Stat>();
            foreach (Stat stat in stats)    // ����Ʈ���� Dictionary�� �ű�� �۾�. ToDictionary�� ��õ���� ����
                dict.Add(stat.id, stat); // level�� ID(Key)�� 
            return dict;
        }
    }
    #endregion
}