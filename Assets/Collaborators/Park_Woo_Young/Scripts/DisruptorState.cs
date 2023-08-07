using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

namespace Park_Woo_Young
{
    public class DisruptorState : MonoBehaviourPunCallbacks, Darik.IHittable
    {

        public bool deBug;
        public float interaction = 4;         
        [SerializeField] GameObject hologram; // 홀로그램을 돌리기 위함
        [SerializeField] Slider hpGauge;      // 체력게이지
        [SerializeField] Slider repairGauge;  // 수리진행도
        [SerializeField] float fixTurnSpeed;  // 고정스피드(100)
        [SerializeField] float turnSpeed;     // 멈췄을때와 돌아갈때 스피드(0)
        [SerializeField] float repair;        // 수리진행도(0)
        [SerializeField] float maxRepair;     // 클리어하기위한 수리진행도(100)
        public float repairTime;              // 수리진행도가 올라가기 위한 시간(1)
        [SerializeField] int testRepair;      
        [SerializeField] int testCur;
        [SerializeField] int fixHP;           
        [SerializeField] int currentHP;       
        public bool disruptorHit;             
        [SerializeField] Material mat1;       
        [SerializeField] Material mat2;       
        [SerializeField] new Renderer renderer;

        public Transform Player;

        public enum State { Activate, Stop, Success, Destroyed }
        State state = State.Activate;

        private void Start()
        {
            GameStart();
        }
        
        private void SetDisruptor()
        {
            GameManager.Data.Disruptor = this.transform;
            GameManager.Enemy.ChangeTarget(true);

        }

        public void GameStart()
        {
            renderer.sharedMaterial = mat1;
            turnSpeed = fixTurnSpeed;
            currentHP = fixHP;
            repair = 0;
            SetDisruptor();
        }

        private void Hit(int damage)
        {
            disruptorHit = true;
            if (repair > 0)
            {
                repair -= damage;
            }
            else
            {
                currentHP -= damage;
            }
        }
        
        private void HpGauge()
        {
            hpGauge.value = currentHP;
        }
        private void RepairGauge()
        {
            repairGauge.value = repair;
        }

        private void Update()
        {
            HpGauge();     
            RepairGauge();
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
            repairTime += Time.deltaTime;
            if (repairTime > 1)
            {
                if (currentHP < fixHP)
                {
                    repairTime = 0;
                    currentHP += testCur;
                }
                else if (currentHP == fixHP) 
                {
                    repairTime = 0;
                    repair += testRepair;
                }
            }
            if (currentHP <= 0)
            {
                //SceneManager.LoadScene("");
                state = State.Destroyed;
                print("교란기 파괴됨");
            }
            if (repair >= maxRepair)
            {
                //SceneManager.LoadScene("");
                state = State.Success;
                print("교란기가 완벽하게 수리됨");
            }
            if (disruptorHit)
            {
                turnSpeed = 0;
                state = State.Stop;
                GameManager.Enemy.ChangeTarget(false);
                renderer.sharedMaterial = mat2;
                
            }
        }
 
        private void StopDisruptor()
        {
            print("1");
            if(!disruptorHit)
            {
                GameManager.Enemy.ChangeTarget(true);
                turnSpeed = fixTurnSpeed;
                state = State.Activate;
                renderer.sharedMaterial = mat1;
            }
        }

        private void SuccessUpdate()
        {

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
                float ToPlayer = Vector3.Distance(transform.position, other.transform.position);
                if (Player != null && ToPlayer < interaction)        
                {
                    if (Input.GetKeyDown(KeyCode.F) && disruptorHit) 
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

