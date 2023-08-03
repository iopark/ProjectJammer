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
        // 0�ܰ� �ı��Ǵ� ���Ǹ�
        [SerializeField] GameObject hologram;   // ������ ���� Ȧ�α׷��� ȸ���� �ֱ� ����
        [SerializeField] Slider hpGauge;        // ü�°�����
        [SerializeField] float fixTurnSpeed;    // ���� ȸ���ӵ�
        [SerializeField] float turnSpeed;       // ȸ���ӵ�
        [SerializeField] int fixHP;             // ������ų ü��
        [SerializeField] int currentHP;         // ���� HP

        public enum State { Activate, Destroyed }
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
            SetDisruptor();
        }

        [PunRPC]
        private void Hit(int damage)
        {
        }

        [PunRPC]
        private void HpGauge()
        {
            hpGauge.value = currentHP;
        }

        private void Update()
        {
            HpGauge();     // ü�°�����
            
            switch (state)
            {
                case State.Activate:
                    ActivateUpdate();
                    break;
                case State.Destroyed:
                    DestroyedUpdate();
                    break;
            }
        }

        public void ActivateUpdate()
        {
            Rotate();
            if (currentHP <= 0)
            {
                state = State.Destroyed;
            }    
        }

        public void DestroyedUpdate()
        {
            Debug.Log("������ �ı�");
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
