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

            // PlayerShooter ������Ʈ�� ������, �� ������Ʈ�� �����ϸ�
            if (playerHealth != null)
            {
                if (playerHealth.isFullHealth())
                    return; 
                // ���� ���� źȯ ���� ammo ��ŭ ���ϱ�, ��� Ŭ���̾�Ʈ���� ����
                playerHealth.photonView.RPC("AddHealth", RpcTarget.All, healAmount);
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

