using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace Park_Woo_Young
{
    public class DisruptorState : MonoBehaviourPunCallbacks, Darik.IHittable
    {
        // ������ ��ȣ�ۿ�� -> �����Ⱑ ������ �Ŵ����� ��ȣ�� �ְ�
        // ������ �ǰ� -> ���ӸŴ����� ������ ó���ϰ� ��
        // �����Ⱑ ���� ��ġ�� �����ϰ� �װ� �����͸Ŵ����� �������� ��ġ�� ��������
        [SerializeField] GameObject hologram;   // ������ ���� Ȧ�α׷��� ȸ���� �ֱ� ����
        [SerializeField] Slider repairGauge;    // �������൵ ������
        [SerializeField] Slider hpGauge;        // ü�°�����

        [SerializeField] float fixTurnSpeed;    // ���� ȸ���ӵ�
        [SerializeField] float turnSpeed;       // ȸ���ӵ�
        [SerializeField] int fixHP;             // ������ų ü��
        [SerializeField] int currentHP;         // ���� HP

        [SerializeField] int repair;            // ��������
        [SerializeField] int maxRepair;         // Ŭ��� ���� ������ǥ

        public Transform player;

        public float repairTime;                 // �����ð�

        // �׽�Ʈ�� ����
        public bool testRepair;                  // �׽�Ʈ�ϱ����� �������̶�� �Ǵ� (hp�� 100�Ʒ��ΰ��)
        public bool interacter;

        public enum State { Disabled, Activate, Stop, Destroyed, RepairCompleted }

        // State state = State.Disabled;    // Ȱ��ȭ ��
        State state = State.Activate;       // Ȱ��ȭ

        private void Start()
        {
            // �ӽ÷� ���۽� �ٷ� ����
            GameStart();
        }

        private void SetDisruptor()
        {
            GameManager.Data.Disruptor = this.transform;
        }

        [PunRPC]
        public void GameStart()
        {
            turnSpeed = fixTurnSpeed;
            currentHP = fixHP;
            repair = 0;
            SetDisruptor();
        }

        [PunRPC]
        private void Hit(int damage)
        {
            repair -= damage;
            if (repair <= 0)
            {
                currentHP -= damage;
            }
        }

        [PunRPC]
        private void RepairGauge()
        {
            repairGauge.value = repair;
        }

        [PunRPC]
        private void HpGauge()
        {
            hpGauge.value = currentHP;
        }

        private void Update()
        {
            RepairGauge(); // ����������
            HpGauge();     // ü�°�����
            
            switch (state)
            {
                case State.Disabled:
                    DisabledUpdate();
                    break;
                case State.Activate:
                    ActivateUpdate();
                    break;
                case State.Stop:
                    StopUpdate();
                    break;
                case State.Destroyed:
                    DestroyedUpdate();
                    break;
                case State.RepairCompleted:
                    RepairCompletedUpdate();
                    break;
            }
        }

        public void DisabledUpdate()
        {
            // Ȱ��ȭ��
        }

        public void ActivateUpdate()
        {
            // Ȱ��ȭ �� & �簡��
            //Debug.Log("Ȱ��ȭ");
            Rotate();
            repairTime += Time.deltaTime;
            if (repairTime > 1)
            {
                repair -= 1;
                repairTime = 0;
                if (repair <= 0)
                {
                    repair = 0;
                    currentHP -= 10;
                }
            }
            if (currentHP <=0)
            {
                state = State.Destroyed;
            }
            
        }

        public void StopUpdate()
        {
            // �������� ���� ��
            //Debug.Log("����");
        }

        public void DestroyedUpdate()
        {
            // �ı�
            Debug.Log("������ �ı�");
        }

        public void RepairCompletedUpdate()
        {
            // ��������
            //Debug.Log("������ Ȱ��ȭ");
        }

        private void Rotate()
        {
            hologram.transform.Rotate(Vector3.back, turnSpeed * Time.deltaTime);
        }

        public void TakeDamage(int damage, Vector3 hitPoint, Vector3 normal)
        {
            photonView.RPC("Hit", RpcTarget.MasterClient, damage); //
        }
    }
    
}
