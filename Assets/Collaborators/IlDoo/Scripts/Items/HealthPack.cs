using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ildoo
{
    public class HealthPack : Item
    {
        [SerializeField] int healAmount;
        private void OnTriggerEnter(Collider other)
        {
            if (!PhotonNetwork.IsMasterClient)
                return;

            PlayerHealth playerHealth= other.gameObject.GetComponent<PlayerHealth>();

            // PlayerShooter 컴포넌트가 있으며, 총 오브젝트가 존재하면
            if (playerHealth != null)
            {
                if (playerHealth.isFullHealth())
                    return; 
                // 총의 남은 탄환 수를 ammo 만큼 더하기, 모든 클라이언트에서 실행
                playerHealth.photonView.RPC("AddHealth", RpcTarget.All, healAmount);
                photonView.RPC("ItemUseSync", RpcTarget.AllViaServer); 
            }
            // 모든 클라이언트에서의 자신을 파괴
        }

        [PunRPC]
        public void ItemUseSync()
        {
            GameManager.Resource.Destroy(gameObject);
        }
    }
}

