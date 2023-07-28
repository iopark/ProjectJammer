using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LDW
{
    public class ProgressButtons : MonoBehaviourPunCallbacks
    {
        public Progress[] progressList;

        public void Init(int id)
        {
            progressList[id].StartProgress(id);
        }

        public void IncreaseProgress(int id)
        {
            photonView.RPC("PunIncreaseProgress", RpcTarget.All, id);
        }

        [PunRPC]
        public void PunIncreaseProgress(int id)
        {
            GameManager.Data.JammerStatDict[id].progress++;

            progressList[id].StartProgress(id);
        }
    }
}