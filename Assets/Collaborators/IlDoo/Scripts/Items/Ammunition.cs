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

            // PlayerShooter ������Ʈ�� ������, �� ������Ʈ�� �����ϸ�
            if (playerShooter != null && playerShooter.currentGun != null)
            {
                // ���� ���� źȯ ���� ammo ��ŭ ���ϱ�, ��� Ŭ���̾�Ʈ���� ����
                playerShooter.currentGun.photonView.RPC("AmmoChange", RpcTarget.AllViaServer, ammoFillAmount);
                photonView.RPC("ItemUseSync", RpcTarget.AllViaServer);
            }
            // ��� Ŭ���̾�Ʈ������ �ڽ��� �ı�
        }

        [PunRPC]
        public void ItemUseSync()
        {
            GameManager.Resource.Destroy(gameObject);
        }
    }
}

