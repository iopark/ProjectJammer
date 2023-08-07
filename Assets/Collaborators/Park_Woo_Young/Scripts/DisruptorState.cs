using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

namespace Park_Woo_Young
{
    public class DisruptorState : MonoBehaviourPunCallbacks, Darik.IHittable
    {

        public bool deBug;
        public float interaction = 4;         
        [SerializeField] GameObject hologram; 
        [SerializeField] Slider hpGauge;      
        [SerializeField] Slider repairGauge;
        [SerializeField] float fixTurnSpeed;  
        [SerializeField] float turnSpeed;     
        [SerializeField] float repair;
        [SerializeField] float maxRepair;
        public float repairTime;
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
        private void dam(int damage)
        {
            if (Input.GetKeyDown(KeyCode.Space)) 
            {
                Hit(damage);
            }
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
            dam(10);
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
                print("������ �ı�");
            }
            if (repair >= maxRepair)
            {
                //SceneManager.LoadScene("");
                state = State.Success;
                print("������ ��������");
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

