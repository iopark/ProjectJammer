using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace LDW
{
    public class Progress : MonoBehaviourPunCallbacks, IPunObservable
    {
        public TMP_Text progressText;
        public float progress;

        public Dictionary<int, JammerStat> JammerStatDict;

        public void StartProgress(int id)
        {
            JammerStatDict = GameManager.Data.JammerStatDict;

            progress = JammerStatDict[id].progress;
            progressText.text = $"{progress} / 100";
        }

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                stream.SendNext(progress);
            }
            else
            {
                progress = (float)stream.ReceiveNext();
            }
        }
    }
}