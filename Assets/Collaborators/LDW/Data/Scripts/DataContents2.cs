using System;
using System.Collections.Generic;

namespace LDW
{
    #region JammerStat
    [Serializable]
    public class JammerStat
    {
        public int id; // ID
        public float progress;
        public float InitProgress;
    }

    [Serializable]
    public class JammerStatData : ILoader<int, JammerStat>
    {
        public List<JammerStat> j_stats = new List<JammerStat>();  // json 파일에서 여기로 담김

        public Dictionary<int, JammerStat> MakeDict() // 오버라이딩
        {
            Dictionary<int, JammerStat> dict = new Dictionary<int, JammerStat>();
            foreach (JammerStat j_stat in j_stats)    // 리스트에서 Dictionary로 옮기는 작업. ToDictionary는 추천하지 않음
                dict.Add(j_stat.id, j_stat); // level을 ID(Key)로 
            return dict;
        }
    }
    #endregion

}