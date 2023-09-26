using Photon.Pun;
using TMPro;
using UnityEngine;
using MySql.Data.MySqlClient;
using System;

namespace LDW
{
    public class LoginPanel : MonoBehaviour
    {
        [SerializeField] GameObject registerPanel;

        [SerializeField] TMP_InputField idInputField;
        [SerializeField] TMP_InputField passwordInputField;

        private MySqlConnection con;
        private MySqlDataReader reader;

        private void Start()
        {
            con = GameManager.Data.con;
            reader = GameManager.Data.reader;
        }

        private void OnDisable()
        {
            idInputField.text = "";
            passwordInputField.text = "";
        }

        // Login 버튼 실행 함수
        public void Login()
        {
            try
            {
                string id = idInputField.text;
                string pass = passwordInputField.text;

                string sqlCommand = string.Format($"SELECT ID,Password FROM user_info WHERE ID='{id}'");
                MySqlCommand cmd = new MySqlCommand(sqlCommand, con);
                reader = cmd.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read()) // 테이블 전체를 읽기. read => true : 읽을 자료가 있음 / false : 다 읽음
                    {
                        string readID = reader["ID"].ToString();
                        string readPass = reader["Password"].ToString();

                        if (pass == readPass)
                        {
                            Debug.Log($"ID : {readID}, Password : {readPass}");
                            PhotonNetwork.LocalPlayer.NickName = id;
                            PhotonNetwork.ConnectUsingSettings(); // 접속 신청
                            if (!reader.IsClosed)
                                reader.Close();
                            return;
                        }
                        else
                        {
                            Debug.Log("Wrong Password");
                            if (!reader.IsClosed)
                                reader.Close();
                            return;
                        }
                    }
                }
                else
                {
                    Debug.Log("There is no player id");
                }
                if (!reader.IsClosed)
                    reader.Close();
            }
            catch (Exception e)
            {
                Debug.Log(e.Message);
            }
        }
    }
}