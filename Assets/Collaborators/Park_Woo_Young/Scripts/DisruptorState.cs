using Photon.Pun;
using UnityEditor;
using UnityEditor.ShaderGraph;
using UnityEngine;
using UnityEngine.Rendering;
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
        public float interaction = 4;           // ��ȣ�ۿ� �Ÿ�(�⺻ 4)
        [SerializeField] GameObject hologram;   // ������ ���� Ȧ�α׷��� ȸ���� �ֱ� ����
        [SerializeField] Slider hpGauge;        // ü�°�����
        [SerializeField] float fixTurnSpeed;    // ���� ȸ���ӵ�
        [SerializeField] float turnSpeed;       // ȸ���ӵ�
        [SerializeField] int fixHP;             // ������ų ü��
        [SerializeField] int currentHP;         // ���� HP
        public bool disruptorHit;               // �ǰݴ����� �� ���ߴ� ���·� �Ѿ�� �ϱ�
        [SerializeField] Material mat1;         // Ȱ��ȭ�� Ȧ�α׷� ����(�Ķ�)
        [SerializeField] Material mat2;         // ����� Ȧ�α׷� ����9����)
        [SerializeField] Renderer renderer;
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
            GameManager.Data.ChangeTarget(true);
        }

        public void GameStart()
        {
            renderer.sharedMaterial = mat1;
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
                //SceneManager.LoadScene(""); // ������ �ı��� ���⿡�� ���� �ҷ����ֱ�.
                state = State.Destroyed;
                print("������ �ı�");
            }
            if (disruptorHit)
            {
                turnSpeed = 0;
                state = State.Stop;
                GameManager.Data.ChangeTarget(false);
                renderer.sharedMaterial = mat2;
                
            }
        }
        // ������ �簡��
        private void StopDisruptor()
        {
            if(!disruptorHit)
            {
                GameManager.Data.ChangeTarget(true);
                turnSpeed = fixTurnSpeed;
                state = State.Activate;
                renderer.sharedMaterial = mat1;
            }
        }

        public void DestroyedUpdate()
        {
            
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
            if (other.tag == "Player")     
            {
                Player = other.transform;   
                float ToPlayer = Vector3.Distance(transform.position, other.transform.position); // �÷��̾�� ������ ������ ��ġ ���ϱ�
                if (Player != null && ToPlayer < interaction)        // �÷��̾�鼭 ���������� �����ȿ� ������ ��
                {
                    if (Input.GetKeyDown(KeyCode.F) && disruptorHit) // �����Ⱑ �ǰݴ��� ���¿����� FŰ�� ���� ��ĥ �� ����
                        disruptorHit = false;
                }
                else if (Player != null && ToPlayer > interaction)
                {
                    Player = null;
                }
            }
        }

        public void DisruptorActivate()
        {
            if (disruptorHit)
            {
                disruptorHit = false;
            }
            else
            {
                return;
            }
        }
    }
}

