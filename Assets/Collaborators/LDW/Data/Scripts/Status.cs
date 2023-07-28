using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;

namespace LDW
{
    public class Status : MonoBehaviour, IPunObservable
    {
        public TMP_Text idText;
        public TMP_Text hpText;
        public TMP_Text attackText;

        public int id;
        public int hp;
        public int attack;

        public Dictionary<int, Stat> statDict { get; private set; } = new Dictionary<int, Stat>();

        public void Init(int id)
        {
            statDict = GameManager.Data.MonsterStatDict;

            idText.text = $"ID : {id}";
            hpText.text = $"HP : {statDict[id].hp}";
            attackText.text = $"Attack : {statDict[id].attack}";
        }

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                stream.SendNext(hp);
                stream.SendNext(attack);
            }
            else
            {
                hp = (int)stream.ReceiveNext();
                attack = (int)stream.ReceiveNext();
            }
        }
    }
}