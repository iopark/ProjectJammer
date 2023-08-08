using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

namespace Park_Woo_Young
{
    public class DisruptorState : MonoBehaviourPunCallbacks, Darik.IHittable, IInteractable
    {
        [SerializeField] GameObject hologram;            // 교란기 위의 홀로그램의 회전을 주기 위함
        [SerializeField] GameObject test;
        [SerializeField] float ttt;
        [SerializeField] new Renderer renderer;
        public float t;
        [SerializeField] Material hologram_Blue;         // 활성화시 홀로그램 색상(파랑)
        [SerializeField] Material hologram_Red;          // 멈출시 홀로그램 색상(빨강)
        [SerializeField] Slider hp_Gauge;                // 체력게이지
        [SerializeField] Slider progress_Gauge;          // 진행도게이지
        [SerializeField] int perSecond;              // 교란기 진행도, 체력 회복에 필요한 시간 !!0으로 설정할시 교란기가 완충이 됨!!

        [SerializeField] float maxHologramRotSpeed = 100;// 홀로그램 최대 회전속도
        [SerializeField] int maxHP = 100;                // 최대 체력
        [SerializeField] int maxProgress = 100;          // 클리어에 필요한 진행도
        [SerializeField] int progressGoesUp = 1;         // 교란기 진행도에 필요한 시간이 충족되면 진행도가 올라가는 속도
        [SerializeField] int hpRepair = 1;               // 체력감소시 시간에 따른 회복속도

        public float interaction = 4;                    // 상호작용 거리
        public float time;                               // 델타타임
        public bool disruptorHit;                        // 공격당했을 때 멈추는 상태로 넘어가게 하기
        public bool deBug;                               // 탐지범위 기즈모로 확인
        private float hologramRotSpeed = 0;              // 홀로그램 현재 회전속도
        public int currentHP;                            // 현재 HP(시작시 maxHp랑 같게 설정함)
        public int progress = 0;                         // 현재 진행도

        public enum State { Activate, Stop, Success, Destroyed }
        State state = State.Activate;
        private void Start()
        {
            // 임시로 시작시 바로 실행
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
                //SceneManager.LoadScene(""); // 교란기 성공 여기에서 신을 불러와주기.
                state = State.Success;
                print("교란기 완전가동");
            }
            if (disruptorHit)
            {
                hologramRotSpeed = 0;
                state = State.Stop;
                GameManager.Enemy.ChangeTarget(false);
                renderer.sharedMaterial = hologram_Red;
                
            }
        }
        // 교란기 재가동
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
                //SceneManager.LoadScene(""); // 교란기 파괴시 여기에서 신을 불러와주기.
                state = State.Destroyed;
                print("교란기 파괴");
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

