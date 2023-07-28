using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

namespace Park_Woo_Young
{
    public class DisruptorState : MonoBehaviourPunCallbacks
    {
        // ������ ��ȣ�ۿ�� -> �����Ⱑ ������ �Ŵ����� ��ȣ�� �ְ�
        // ������ �ǰ� -> ���ӸŴ����� ������ ó���ϰ� ��
        [SerializeField] GameObject hologram;   // ������ ���� Ȧ�α׷��� ȸ���� �ֱ� ����
        [SerializeField] Slider repairGauge;
        [SerializeField] Slider hpGauge;

        [SerializeField] float fixTurnSpeed;    // ���� ȸ���ӵ�
        [SerializeField] float turnSpeed;       // ȸ���ӵ�
        [SerializeField] int fixHP;             // ������ų ü��
        [SerializeField] int currentHP;         // ���� HP

        [SerializeField] int repair;            // ��������
        [SerializeField] int maxRepair;         // Ŭ��� ���� ������ǥ

        public Transform player;
        //Vector3 pos;

        public float repairTime;                 // �����ð�
        public bool hit;

        public bool testRepair;                  // �׽�Ʈ�ϱ����� �������̶�� �Ǵ�
        public float testRepairTime;             // �����ð�

        public bool re;
        public bool hp;
        public float ti;


        public enum State { Disabled, Activate, Stop, Destroyed, RepairCompleted }

        // State state = State.Disabled;
        State state = State.Activate;

        private void Awake()
        {
            currentHP = fixHP;
            //repair = 0;
            //repairTime = 0;
        }
        private void Start()
        {
            turnSpeed = fixTurnSpeed;
            // �ӽ÷� ���۽� �ٷ� ����
            // GameMamager.Data.GameStart();
            //pos = this.gameObject.transform.position;
            //Debug.Log(pos);
            //Debug.Log(pos.y);

        }
        public void Interacter()
        {
            // ��ȣ�ۿ�
            photonView.RPC("GameStart", RpcTarget.MasterClient);
            // photonView ���� ������Ʈ�� �ڱ⸦ ������ �˸�
        }
        [PunRPC]
        private void GameStart()
        {

        }
        [PunRPC]
        public void TakeDamage(int damage)
        {
            photonView.RPC("Hit", RpcTarget.MasterClient, damage);
            photonView.RPC("HIT", RpcTarget.All, currentHP);
            photonView.RPC("HIT", RpcTarget.All, repair);
        }
        [PunRPC]
        private void Hit(int damage)
        {

            if (repair > 0)                 // �������൵�� 0���� ũ��
            {
                repair -= damage;           // ���� ���������� ���� ���൵�� ����
            }
            else if (repair <= 0)           // �������൵�� 0���� �۰ų� ������
            {
                currentHP -= damage;        // ü���� ����
            }
            hit = true;
            //GameManager.Data.ChangeHp(currentHP);

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
        
        private void Test()
        {
            ti += Time.deltaTime;
            if (re)
            {
                if (ti > 1)
                {
                    if (repair >= 0)
                    {
                        hit = true;
                        repair -= 1;
                        ti = 0;
                    }
                }
            }
            else if (hp)
            {
                if (ti > 1)
                {
                    if (currentHP >= 0)
                    {
                        hit = true;
                        currentHP -= 1;
                        ti = 0;
                    }
                }
            }
        }

        private void Update()
        {
            RepairGauge();
            HpGauge();
            Test();

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
            // repairTime = 0f;    // ��ŸŸ�� 0���� ����
            if (Input.GetKeyDown(KeyCode.F))
            {
                turnSpeed = fixTurnSpeed;
                state = State.Activate;
            }
        }

        public void ActivateUpdate()
        {
            // Ȱ��ȭ �� & �簡��
            Rotate();
            
            Debug.Log("Ȱ��ȭ");
            repairTime += Time.deltaTime;
            if (currentHP >= fixHP)     // ����ü���� �ִ�ü�º��� ū ���
            {
                if (repairTime >= 2)   // �����Ǵ� �ð��� 10���� ũ�ų� ������
                {
                    repair += 1;       // �������¿� 10�� ������
                    repairTime = 0f;    // �ð��� 0���� �ʱ�ȭ
                }
            }

            if (currentHP < fixHP) // ���� ü���� �ִ�ü�º��� ���� ���
            {
                repairTime = 0f;         // ������ �ߴ��ϱ� ���� �ð��� 0���� ����
                if (testRepair)
                {
                    if (!hit)
                    {
                        testRepairTime += Time.deltaTime;
                        if (testRepairTime >= 1)
                        {
                            turnSpeed = fixTurnSpeed / 2;
                            Debug.Log("������");
                            currentHP += 1;
                            testRepairTime = 0f;
                        }
                    }
                    else
                    {
                        Debug.Log("�����ߴ�");
                        testRepair = false;
                    }
                }
            }
            else if (currentHP == fixHP)
            {
                Debug.Log("�����Ϸ�");
                testRepair = false;
            }

            if (hit)               // bool�� hit�� 
            {
                turnSpeed = 0f;
                state = State.Stop; // �������� ����
            }

            if (currentHP <= 0)
            {
                state = State.Destroyed; // �ı�
            }
            if (repair >= maxRepair)
            {
                state = State.RepairCompleted; // ����
            }
        }

        public void StopUpdate()
        {
            // �������� ���� ��
            // turnSpeed = 0f;     // ȸ���ӵ��� 0���� �༭ ���߰� ��
            // repairTime = 0f; // �����ð��� 0���� �������Ѽ� ������ ������ �ȵǰ� ��
            Debug.Log("����");

            // �÷��̾ �ٽ� �簡��
            //if (Input.GetKeyDown(KeyCode.F))
            //{
            //    hit = false;
            //    state = State.Activate;
            //}
            if (!hit)
            {
                turnSpeed = fixTurnSpeed;
                state = State.Activate;
            }

        }

        public void DestroyedUpdate()
        {
            // �ı�
            currentHP = 0;
            repair = 0;
            Debug.Log("������ �ı�");
        }

        public void RepairCompletedUpdate()
        {
            Debug.Log("������ Ȱ��ȭ");
            // ��������
        }

        private void Rotate()
        {
            //transform.Rotate(Vector3.back, turnSpeed * Time.deltaTime);
            hologram.transform.Rotate(Vector3.back, turnSpeed * Time.deltaTime);
        }
        private void OnTriggerStay(Collider other)
        {
            if (other.name == "Player")
            {
                if (Input.GetKeyDown(KeyCode.F))
                {

                }
            }
        }
    }
}