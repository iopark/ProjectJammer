using Photon.Pun;
using System;

namespace LDW
{
    public class LevelButtons : MonoBehaviourPunCallbacks
    {
        public Status status;

        public void LevelButton(int level)
        {
            status.Init(level);
        }

        public void DecreaseHP(int level)
        {
            photonView.RPC("PunDecreaseHP", RpcTarget.All, level);
        }

        [PunRPC]
        public void PunDecreaseHP(int level)
        {
            if (photonView.IsMine)
                GameManager.Data.StatDict[level].hp--;

            status.Init(level);
        }
    }
}
