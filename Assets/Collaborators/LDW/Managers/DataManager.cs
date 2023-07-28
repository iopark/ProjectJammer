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

        public UnityAction<float> OnChangedProgress;

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

        Loader LoadJson<Loader, Key, Value>(string path) where Loader : ILoader<Key, Value>
        {
            TextAsset textAsset = GameManager.Resource.Load<TextAsset>($"{path}"); // Hard Coding
            return JsonUtility.FromJson<Loader>(textAsset.text);
        }

        public void ChangeProgress(float progress)
        {
            OnChangedProgress?.Invoke(progress);
        }

        public void Init()
        {
            MonsterStatDict = LoadJson<StatData, int, Stat>("StatData").MakeDict();
        }
    }
}
