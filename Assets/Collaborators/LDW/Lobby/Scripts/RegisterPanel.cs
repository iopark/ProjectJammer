using MySql.Data.MySqlClient;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace LDW
{
    public class RegisterPanel : MonoBehaviour
    {
        [SerializeField] GameObject registerPanel;

        [SerializeField] TMP_InputField idInputField;
        [SerializeField] TMP_InputField passwordInputField;

        private MySqlConnection con;

        private void Start()
        {
            con = GameManager.Data.con;
        }

        public void Register()
        {
            try
            {
                string id = idInputField.text;
                string pass = passwordInputField.text;

                string sqlCommand = string.Format($"INSERT IGNORE INTO user_info VALUES('{id}','{pass}','{id}')");
                MySqlCommand cmd = new MySqlCommand(sqlCommand, con);

                if (cmd.ExecuteNonQuery() == 1)
                {
                    Console.WriteLine("Insert Success");
                }
                else
                {
                    Console.WriteLine("Insert Failed");
                }
            }
            catch (Exception e)
            {
                Debug.Log(e.Message);
            }
        }
    }
}