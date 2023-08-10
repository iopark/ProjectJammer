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
        public List<JammerStat> j_stats = new List<JammerStat>();  // json ���Ͽ��� ����� ���

        public Dictionary<int, JammerStat> MakeDict() // �������̵�
        {
            Dictionary<int, JammerStat> dict = new Dictionary<int, JammerStat>();
            foreach (JammerStat j_stat in j_stats)    // ����Ʈ���� Dictionary�� �ű�� �۾�. ToDictionary�� ��õ���� ����
                dict.Add(j_stat.id, j_stat); // level�� ID(Key)�� 
            return dict;
        }
    }
    #endregion

}