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
            PlayerHealth playerHealth= other.gameObject.GetComponent<PlayerHealth>();

            // PlayerShooter ������Ʈ�� ������, �� ������Ʈ�� �����ϸ�
            if (playerHealth != null)
            {
                if (playerHealth.isFullHealth())
                    return;
                // ���� ���� źȯ ���� ammo ��ŭ ���ϱ�, ��� Ŭ���̾�Ʈ���� ����
                if (PhotonNetwork.IsMasterClient)
                {
                    playerHealth.photonView.RPC("AddHealth", RpcTarget.All, healAmount);
                }
                GameManager.Resource.Destroy(gameObject);
            }
            // ��� Ŭ���̾�Ʈ������ �ڽ��� �ı�
        }
    }
}

