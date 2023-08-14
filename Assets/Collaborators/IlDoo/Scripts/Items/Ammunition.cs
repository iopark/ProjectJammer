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
            PlayerShooter playerShooter = other.gameObject.GetComponent<PlayerShooter>();

            // PlayerShooter ������Ʈ�� ������, �� ������Ʈ�� �����ϸ�
            if (playerShooter != null && playerShooter.currentGun != null)
            {
                if (playerShooter.currentGun.hasMaxCarry())
                    return; 
                // ���� ���� źȯ ���� ammo ��ŭ ���ϱ�, ��� Ŭ���̾�Ʈ���� ����
                if (PhotonNetwork.IsMasterClient)
                {
                    playerShooter.currentGun.photonView.RPC("AmmoChange", RpcTarget.AllViaServer, ammoFillAmount);
                }
                GameManager.Resource.Destroy(gameObject); 
            }
            // ��� Ŭ���̾�Ʈ������ �ڽ��� �ı�
        }
    }
}

