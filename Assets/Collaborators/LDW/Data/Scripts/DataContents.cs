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
        public int level; // ID
        public int hp;
        public int attack;
    }

    [Serializable]
    public class StatData : ILoader<int, Stat>
    {
        public List<Stat> stats = new List<Stat>();  // json 파일에서 여기로 담김

        public Dictionary<int, Stat> MakeDict() // 오버라이딩
        {
            Dictionary<int, Stat> dict = new Dictionary<int, Stat>();
            foreach (Stat stat in stats)    // 리스트에서 Dictionary로 옮기는 작업. ToDictionary는 추천하지 않음
                dict.Add(stat.level, stat); // level을 ID(Key)로 
            return dict;
        }
    }
    #endregion
}