using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace LDW
{
    public class DataManager : MonoBehaviour
    {
        public Transform Disruptor;

        public MySqlConnection con;
        public MySqlDataReader reader;

        public int disruptorHP;

        private void Start()
        {
            ConnectDataBase();
            DataInit();
        }

        private void ConnectDataBase()
        {
            try
            {
                string serverInfo = "Server=15.164.251.21; DataBase=userdata; Uid=root; Pwd=1234; Port=3306; CharSet=utf8; ";
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

        public void DataInit()
        {
            MonsterStatDict = LoadJson<StatData, int, Stat>("StatData").MakeDict();
            JammerStatDict = LoadJson<JammerStatData, int, JammerStat>("JammerData").MakeDict();

            disruptorHP = 100;
        }

        public void SetDisruptor()
        {
            
        }

        public void DataSync()
        {

        }

        public void SetDisruptorHP(int hp)
        {
            disruptorHP = hp;
        }
    }
}
