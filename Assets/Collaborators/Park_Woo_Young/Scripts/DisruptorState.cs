using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

namespace Park_Woo_Young
{
    public class DisruptorState : MonoBehaviourPunCallbacks, Darik.IHittable, IInteractable
    {
        [SerializeField] GameObject hologram;            // ������ ���� Ȧ�α׷��� ȸ���� �ֱ� ����
        [SerializeField] GameObject test;
        [SerializeField] float ttt;
        [SerializeField] new Renderer renderer;
        public float t;
        [SerializeField] Material hologram_Blue;         // Ȱ��ȭ�� Ȧ�α׷� ����(�Ķ�)
        [SerializeField] Material hologram_Red;          // ����� Ȧ�α׷� ����(����)
        [SerializeField] Slider hp_Gauge;                // ü�°�����
        [SerializeField] Slider progress_Gauge;          // ���൵������
        [SerializeField] int perSecond;              // ������ ���൵, ü�� ȸ���� �ʿ��� �ð� !!0���� �����ҽ� �����Ⱑ ������ ��!!

        [SerializeField] float maxHologramRotSpeed = 100;// Ȧ�α׷� �ִ� ȸ���ӵ�
        [SerializeField] int maxHP = 100;                // �ִ� ü��
        [SerializeField] int maxProgress = 100;          // Ŭ��� �ʿ��� ���൵
        [SerializeField] int progressGoesUp = 1;         // ������ ���൵�� �ʿ��� �ð��� �����Ǹ� ���൵�� �ö󰡴� �ӵ�
        [SerializeField] int hpRepair = 1;               // ü�°��ҽ� �ð��� ���� ȸ���ӵ�

        public float interaction = 4;                    // ��ȣ�ۿ� �Ÿ�
        public float time;                               // ��ŸŸ��
        public bool disruptorHit;                        // ���ݴ����� �� ���ߴ� ���·� �Ѿ�� �ϱ�
        public bool deBug;                               // Ž������ ������ Ȯ��
        private float hologramRotSpeed = 0;              // Ȧ�α׷� ���� ȸ���ӵ�
        public int currentHP;                            // ���� HP(���۽� maxHp�� ���� ������)
        public int progress = 0;                         // ���� ���൵

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
            perSecond = 1;
            SetDisruptor();
            
        }
        private void TestHit(int damage)
        {
            if(Input.GetKeyDown(KeyCode.E)) 
            {
                Hit(damage);
            }
        }
        private void Hit(int damage)
        {
            if (progress >= 0)
            {
                progress -= damage;
                if (progress < -1)
                {
                    progress = 0;
                }
                
            }
            if (progress == 0 || currentHP <= maxHP)
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
            TestHit(10);

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
            if (time > perSecond)
            {
                if (progress >= 0 && currentHP == 100)
                {
                    print("1");
                    time = 0;
                    progress += progressGoesUp;
                    currentHP = maxHP;
                }
                else if (progress <= 0 && currentHP < 99 )
                {
                    print("2");
                    time = 0;
                    progress = 0;
                    currentHP += hpRepair;
                }
            }
            if (progress >= maxProgress)
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
            if (currentHP <= 0)
            {
                //SceneManager.LoadScene(""); // ������ �ı��� ���⿡�� ���� �ҷ����ֱ�.
                state = State.Destroyed;
                print("������ �ı�");
            }
        }

        public void DestroyedUpdate()
        {
            
        }
        public void SuccessUpdate()
        {
            T1();
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
        private void T1()
        {
            t += Time.deltaTime;
            if (t > 0.2)
            {
                test.transform.localScale = new Vector3(1f + ttt, 1f + ttt, 1f + ttt);
            }
        }
        public void TT()
        {
            test.transform.localScale = new Vector3(1f +ttt, 1f +ttt, 1f +ttt);
            ttt++;
        }

        public void Interact()
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

