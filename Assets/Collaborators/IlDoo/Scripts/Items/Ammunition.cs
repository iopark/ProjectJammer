using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

namespace ildoo
{
    public class Ammunition : Item
    {
        [SerializeField] int ammoFillAmount; 
        private void OnTriggerEnter(Collider other)
        {
            if (!PhotonNetwork.IsMasterClient)
                return;
            PlayerShooter playerShooter = other.gameObject.GetComponent<PlayerShooter>();

            // PlayerShooter 컴포넌트가 있으며, 총 오브젝트가 존재하면
            if (playerShooter != null && playerShooter.currentGun != null)
            {
                // 총의 남은 탄환 수를 ammo 만큼 더하기, 모든 클라이언트에서 실행
                playerShooter.currentGun.photonView.RPC("AmmoChange", RpcTarget.AllViaServer, ammoFillAmount);
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

