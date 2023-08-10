using LDW;
using Photon.Pun;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ildoo
{
    public class TeamHealth : MonoBehaviourPun
    {
        PlayerHealth playerHealth;
        [SerializeField] TMP_Text teamName;
        [SerializeField] Slider teamHPvalue;

        private void Awake()
        {
            if (photonView.IsMine)
            {
                teamName.color = Color.yellow;
                //
            }
        }

        private void Start()
        {
            foreach (KeyValuePair<int, PlayerData> entry in GameManager.Data.playerDict)
            {
                PhotonView pv = PhotonView.Find(entry.Value.viewId);
                if (pv.IsMine)
                    playerHealth = pv.gameObject.GetComponent<PlayerHealth>();
            }
            if (playerHealth == null)
            {
                Debug.Log("Something went wrong, Player info failed to be registered"); 
            }
            playerHealth.onHealthChange += LocalUpdate;
            teamName.text = playerHealth.gameObject.name;
            photonView.RPC("SyncName", RpcTarget.Others, playerHealth.gameObject.name); 
        }

        public void LocalUpdate(int value)
        {
            photonView.RPC("SyncUpdate", RpcTarget.Others, value); 
        }

        [PunRPC]
        public void SyncUpdate(int value)
        {
            teamHPvalue.value = value;
        }
        [PunRPC]
        public void SyncName(string value)
        {
            teamName.text = value;
        }

    }
}

