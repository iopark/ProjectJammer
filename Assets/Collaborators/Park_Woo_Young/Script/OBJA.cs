using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;
using Photon.Pun.Demo.PunBasics;
using Photon.Pun;

namespace Park_Woo_Young
{
    public class OBJA : MonoBehaviour
    {
        // ������ ��ȣ�ۿ�� -> �����Ⱑ ������ �Ŵ����� ��ȣ�� �ְ�
        // ������ �ǰ� -> ���ӸŴ����� ������ ó���ϰ� ��
        //

        [SerializeField] float turnSpeed; // ȸ���ӵ�
        [SerializeField] int currentHP; // ���� HP
        [SerializeField] int maxHP;     // ������ų ü��
        [SerializeField] int repair;    // ��������
        [SerializeField] int maxRepair; // Ŭ��� ���� ������ǥ

        public float repairTime;          // �����ð�
        public bool hit = true;

        public enum State { A, B, C, D, E }

        State state = State.A;

        private void Awake()
        {
            currentHP = maxHP;
            repair = 0;
            repairTime = 0;
        }
        private void Start()
        {
            // �ӽ÷� ���۽� �ٷ� ����
            // GameMamager.Data.GameStart();
        }
        public void Interacter()
        {
            // ��ȣ�ۿ�
            // photonView.RPC("GameStart",RpcTarget.MasterClient);
            // photonView ���� ������Ʈ�� �ڱ⸦ ������ �˸�
        }
        [PunRPC]
        private void GameStart()
        {

        }

        public void TakeDamage(int damage)
        {
            //photonView.RPC("Hit", RpcTarget.MasterClient);
        }
        [PunRPC]
        private void Hie(int damage)
        {
            currentHP -= damage;
            //GameManager.Data.ChangeHp(currentHP);

        }


        private void Update()
        {
            switch (state)
            {
                case State.A:
                    AUpdate();
                    break;
                case State.B:
                    BUpdate();
                    break;
                case State.C:
                    CUpdate();
                    break;
                case State.D:
                    DUpdate();
                    break;
                case State.E:
                    EUpdate();
                    break;
            }
        }

        public void AUpdate()
        {
            // Ȱ��ȭ��
            repairTime = 0f;    // ��ŸŸ�� 0���� ����
            if (Input.GetKeyDown(KeyCode.F))
            {
                state = State.B;
            }
        }

        public void BUpdate()
        {
            // Ȱ��ȭ �� & �簡��
            Rotate();
            repairTime += Time.deltaTime;
            if (currentHP >= maxHP)     // ����ü���� �ִ�ü�º��� ū ���
            {
                /*if (repairTime >= 10)   // �����Ǵ� �ð��� 10���� ũ�ų� ������
                {
                    repair += 10;       // �������¿� 10�� ������
                    repairTime = 0f;    // �ð��� 0���� �ʱ�ȭ
                }*/
            }
            else if (currentHP < maxHP) // ���� ü���� �ִ�ü�º��� ���� ���
            {
                repairTime = 0;         // ������ �ߴ��ϱ� ���� �ð��� 0���� ����
            }


            else if (hit)               // bool�� hit�� 
            {
                State state = State.C; // �������� ����
            }
            else if (currentHP <= 0)
            {
                state = State.D; // �ı�
            }
            else if (repair >= maxRepair)
            {
                state = State.E; // ����
            }
        }

        public void CUpdate()
        {
            // �������� ���� ��\
            turnSpeed = 0f;
            repairTime = 0f;


            // �÷��̾ �ٽ� �簡��
            if (Input.GetKeyDown(KeyCode.F))
            {
                hit = false;
                state = State.B;
            }
        }

        public void DUpdate()
        {
            // �ı�
            currentHP = 0;
            repair = 0;
            Debug.Log("Game Over");
        }

        public void EUpdate()
        {
            Debug.Log("Game Clear");
            // ��������
        }

        private void Rotate()
        {
            transform.Rotate(Vector3.back, turnSpeed * Time.deltaTime);
        }
        private void OnTriggerStay(Collider other)
        {
            if (other.name == "Player")
            {
                if (Input.GetKeyDown(KeyCode.F))
                {
                    if (State.A != State.B)
                    {

                    }
                }
            }
        }
    }
}
