using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

namespace Park_Woo_Young
{
    public class DisruptorState : MonoBehaviourPunCallbacks, Darik.IHittable
    {
        [SerializeField] GameObject hologram;            // ������ ���� Ȧ�α׷��� ȸ���� �ֱ� ����
        [SerializeField] new Renderer renderer;
        [SerializeField] Material hologram_Blue;         // Ȱ��ȭ�� Ȧ�α׷� ����(�Ķ�)
        [SerializeField] Material hologram_Red;          // ����� Ȧ�α׷� ����(����)
        [SerializeField] Slider hp_Gauge;                // ü�°�����
        [SerializeField] Slider progress_Gauge;          // ���൵������
        [SerializeField] int second = 1;                 // ������ ���൵, ü�� ȸ���� �ʿ��� �ð�

        [SerializeField] float maxHologramRotSpeed = 100;// Ȧ�α׷� �ִ� ȸ���ӵ�
        [SerializeField] int maxHP = 100;                // �ִ� ü��
        [SerializeField] int maxProgress = 100;          // Ŭ��� �ʿ��� ���൵
        [SerializeField] int progressGoesUp = 1;         // ������ ���൵�� �ʿ��� �ð��� �����Ǹ� ���൵�� �ö󰡴� �ӵ�
        [SerializeField] int hpRepair = 1;               // ü�°��ҽ� �ð��� ���� ȸ���ӵ�

        public Transform Player;                         // interaction ���� �ȿ� ������ �� �÷��̾� Ž�� Ȯ��
        public float interaction = 4;                    // ��ȣ�ۿ� �Ÿ�
        public float time;                               // ��ŸŸ��
        public bool disruptorHit;                        // ���ݴ����� �� ���ߴ� ���·� �Ѿ�� �ϱ�
        public bool deBug;                               // Ž������ ������ Ȯ��
        private float hologramRotSpeed = 0;              // Ȧ�α׷� ���� ȸ���ӵ�
        private int currentHP;                           // ���� HP(���۽� maxHp�� ���� ������)
        private int progress = 0;                        // ���� ���൵

        public enum State { Activate, Stop, Success, Destroyed }
        State state = State.Activate;

        private void Start()
        {
            // �ӽ÷� ���۽� �ٷ� ����
            GameStart();

        }
        
        private void SetDisruptor()
        {
            GameManager.Data.Disruptor = this.transform;
            GameManager.Enemy.ChangeTarget(true);
        }

        public void GameStart()
        {
            renderer.sharedMaterial = hologram_Blue;
            hologramRotSpeed = maxHologramRotSpeed;
            currentHP = maxHP;
            SetDisruptor();
            progress = 1;
            
        }

        private void Hit(int damage)
        {
            
            if (progress >= 0)
            {
                progress -= damage;
            }
            else if (progress < 0)
            {
                currentHP -= damage;
            }

            disruptorHit = true;
        }
        
        private void MaxGauge()
        {
            hp_Gauge.maxValue = maxHP;
            progress_Gauge.maxValue = maxProgress;
        }
        private void HpGauge()
        {
            
            hp_Gauge.value = currentHP;
        }
        private void ProgressGauge()
        {
            progress_Gauge.value = progress;
        }


        private void Update()
        {
            HpGauge();      
            ProgressGauge();
            MaxGauge();

            switch (state)
            {
                case State.Activate:
                    ActivateUpdate();
                    break;
                case State.Stop:
                    StopDisruptor();
                    break;
                case State.Success:
                    SuccessUpdate(); 
                    break;
                case State.Destroyed:
                    DestroyedUpdate();
                    break;
            }
        }

        public void ActivateUpdate()
        {
            Rotate();
            time += Time.deltaTime;
            if (time > second)
            {
                if (progress > 0)
                {
                    progress += progressGoesUp;
                    time = 0;
                }
                if (progress < 1)
                {
                    currentHP += hpRepair;
                    time = 0;
                }
            }

            if (currentHP == 0)
            {
                //SceneManager.LoadScene(""); // ������ �ı��� ���⿡�� ���� �ҷ����ֱ�.
                state = State.Destroyed;
                print("������ �ı�");
            }
            if (progress == maxProgress)
            {
                //SceneManager.LoadScene(""); // ������ ���� ���⿡�� ���� �ҷ����ֱ�.
                state = State.Success;
                print("������ ��������");
            }
            if (disruptorHit)
            {
                hologramRotSpeed = 0;
                state = State.Stop;
                GameManager.Enemy.ChangeTarget(false);
                renderer.sharedMaterial = hologram_Red;
                
            }
        }
        // ������ �簡��
        private void StopDisruptor()
        {
            time = 0;
            if(!disruptorHit)
            {
                GameManager.Enemy.ChangeTarget(true);
                hologramRotSpeed = maxHologramRotSpeed;
                state = State.Activate;
                renderer.sharedMaterial = hologram_Blue;
            }
        }

        public void DestroyedUpdate()
        {
            
        }
        public void SuccessUpdate()
        {

        }

        private void Rotate()
        {
            hologram.transform.Rotate(Vector3.back, hologramRotSpeed * Time.deltaTime);
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
        public void RepairInteraction()
        {
            if (Input.GetKeyDown(KeyCode.F))
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

        private void OnTriggerStay(Collider other)
        {
            if (other.tag == "Player")     
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

