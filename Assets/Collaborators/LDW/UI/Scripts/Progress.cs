using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Progress : MonoBehaviourPunCallbacks, IPunObservable
{
    public TMP_Text progressText;

    public float progress;

    public void GameStart()
    {
        StartCoroutine(ProgressStartRoutine());
    }

    IEnumerator ProgressStartRoutine()
    {
        while (true)
        {
            progress += Time.deltaTime;
            progressText.text = $"{progress}";

            yield return null;
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(progress);
        }
        else
        {
            progress = (int)stream.ReceiveNext();
        }
    }
}
