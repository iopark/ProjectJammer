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
            if (!photonView.IsMine)
                return;

            PlayerHealth playerHealth= other.gameObject.GetComponent<PlayerHealth>();

            // PlayerShooter ������Ʈ�� ������, �� ������Ʈ�� �����ϸ�
            if (playerHealth != null)
            {
                // ���� ���� źȯ ���� ammo ��ŭ ���ϱ�, ��� Ŭ���̾�Ʈ���� ����
                playerHealth.photonView.RPC("AddHealth", RpcTarget.All, healAmount);
                PhotonNetwork.Destroy(gameObject);
            }

            // ��� Ŭ���̾�Ʈ������ �ڽ��� �ı�
            
        }
    }
}

