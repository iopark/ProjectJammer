using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using UnityEngine.Events;

namespace LDW
{
    public class DataManager : MonoBehaviour
    {
        public MySqlConnection con;
        public MySqlDataReader reader;

        private void Start()
        {
            ConnectDataBase();
            Init();
        }

        private void ConnectDataBase()
        {
            try
            {
                string serverInfo = "Server=127.0.0.1; DataBase=userdata; Uid=root; Pwd=1234; Port=3306; CharSet=utf8; ";
                con = new MySqlConnection(serverInfo);
                con.Open();

                // 성공했을 때
                Debug.Log("DataBase connect Success");
            }
            catch (Exception e)
            {
                Debug.Log(e.Message);
            }
        }

        public Dictionary<int, Stat> MonsterStatDict { get; private set; } = new Dictionary<int, Stat>();
        public Dictionary<int, JammerStat> JammerStatDict { get; private set; } = new Dictionary<int, JammerStat>();

        Loader LoadJson<Loader, Key, Value>(string path) where Loader : ILoader<Key, Value>
        {
            TextAsset textAsset = GameManager.Resource.Load<TextAsset>($"{path}"); // Hard Coding
            return JsonUtility.FromJson<Loader>(textAsset.text);
        }

        public void Init()
        {
            MonsterStatDict = LoadJson<StatData, int, Stat>("StatData").MakeDict();
            JammerStatDict = LoadJson<JammerStatData, int, JammerStat>("JammerData").MakeDict();

            for(int id = 0; id < JammerStatDict.Count; id++)
            {
                Debug.Log(JammerStatDict[id].id);
                Debug.Log(JammerStatDict[id].progress);
                Debug.Log(JammerStatDict[id].InitProgress);
            }
        }
    }
}
