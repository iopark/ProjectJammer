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
            if (!photonView.IsMine)
                return;
            PlayerShooter playerShooter = other.gameObject.GetComponent<PlayerShooter>();

            // PlayerShooter ������Ʈ�� ������, �� ������Ʈ�� �����ϸ�
            if (playerShooter != null && playerShooter.currentGun != null)
            {
                // ���� ���� źȯ ���� ammo ��ŭ ���ϱ�, ��� Ŭ���̾�Ʈ���� ����
                playerShooter.currentGun.photonView.RPC("AmmoChange", RpcTarget.All, ammoFillAmount);
                PhotonNetwork.Destroy(gameObject);
            }
            // ��� Ŭ���̾�Ʈ������ �ڽ��� �ı�
        }
    }
}

