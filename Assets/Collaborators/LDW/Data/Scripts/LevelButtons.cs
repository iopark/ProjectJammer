using Photon.Pun;
using System;

namespace LDW
{
    public class LevelButtons : MonoBehaviourPunCallbacks
    {
        public Status[] statusList;

        private void Start()
        {
            for (int id = 0; id < statusList.Length; id++)
                statusList[id].Init(id);
        }

        public void LevelButton(int id)
        {
            statusList[id].Init(id);
        }

        public void DecreaseHP(int id)
        {
            photonView.RPC("PunDecreaseHP", RpcTarget.All, id);
        }

        [PunRPC]
        public void PunDecreaseHP(int id)
        {
            //if (photonView.IsMine)
                GameManager.Data.MonsterStatDict[id].hp--;

            statusList[id].Init(id);
        }
    }
}
