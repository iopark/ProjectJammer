using Photon.Pun;
using System.Diagnostics;
using UnityEditor.ShaderKeywordFilter;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Park_Woo_Young
{
    public class DisruptorState : MonoBehaviourPunCallbacks, Darik.IHittable
    {
        // ������ ��ȣ�ۿ�� -> �����Ⱑ ������ �Ŵ����� ��ȣ�� �ְ�
        // ������ �ǰ� -> ���ӸŴ����� ������ ó���ϰ� ��
        // �����Ⱑ ���� ��ġ�� �����ϰ� �װ� �����͸Ŵ����� �������� ��ġ�� ��������
        // 0�ܰ� �ı��Ǵ� ���Ǹ�1
        public bool deBug;
        public float interaction;               // ��ȣ�ۿ� �Ÿ�
        [SerializeField] GameObject hologram;   // ������ ���� Ȧ�α׷��� ȸ���� �ֱ� ����
        [SerializeField] Slider hpGauge;        // ü�°�����
        [SerializeField] float fixTurnSpeed;    // ���� ȸ���ӵ�
        [SerializeField] float turnSpeed;       // ȸ���ӵ�
        [SerializeField] int fixHP;             // ������ų ü��
        [SerializeField] int currentHP;         // ���� HP
        public bool disruptorHit;
        public float TT;

        public Transform Player;

        public enum State { Activate, Destroyed, Stop }
        State state = State.Activate;

        private void Start()
        {
            // �ӽ÷� ���۽� �ٷ� ����
            GameStart();
        }

        private void SetDisruptor()
        {
            GameManager.Data.Disruptor = this.transform;
        }

        public void GameStart()
        {
            turnSpeed = fixTurnSpeed;
            currentHP = fixHP;
            SetDisruptor();
        }

        private void Hit(int damage)
        {
            currentHP -= damage;
            disruptorHit = true;
        }
        
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
                case State.Stop:
                    StopDisruptor();
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
                // SceneManager.LoadScene(""); // ������ �ı��� ���⿡�� ���� �ҷ����ֱ�
                state = State.Destroyed;
            }
            if (disruptorHit)
            {
                turnSpeed = 0;
                state = State.Stop;
            }
        }
        // ������ �簡��
        private void StopDisruptor()
        {
            if(!disruptorHit)
            {
                turnSpeed = fixTurnSpeed;
                state = State.Activate;
            }
        }

        public void DestroyedUpdate()
        {
            print("������ �ı�");
        }

        private void Rotate()
        {
            hologram.transform.Rotate(Vector3.back, turnSpeed * Time.deltaTime);
        }

        public void TakeDamage(int damage, Vector3 hitPoint, Vector3 normal)
        {
            Hit(damage);
        }


        private void OnDrawGizmosSelected()
        {
            if (!deBug)
                return;

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, interaction);
        }

        private void OnTriggerStay(Collider other)
        {
            // GameManager.Dat
            if (other.name == "Player")     
            {
                Player = other.transform;   
                float ToPlayer = Vector3.Distance(transform.position, other.transform.position); // �÷��̾�� ������ ������ ��ġ ���ϱ�
                if (Player != null && ToPlayer < interaction)        // �÷��̾�鼭 ���������� �����ȿ� ������ ��
                {
                    if (Input.GetKeyDown(KeyCode.F) && disruptorHit) // �����Ⱑ �ǰݴ��� ���¿����� FŰ�� ���� ��ĥ �� ����
                    {
                        disruptorHit = false;               
                    }
                }
                else if (Player != null && ToPlayer > interaction)
                {
                    Player = null;
                }
            }
        }
    }
}

