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
        // 교란기 상호작용시 -> 교란기가 데이터 매니저에 신호를 주고
        // 교란기 피격 -> 게임매니저에 보내서 처리하게 함
        // 교란기가 생길 위치를 저장하고 그걸 데이터매니저에 보낸다음 위치를 가져오기
        // 0단계 파괴되는 조건만1
        public bool deBug;
        public float interaction = 4;           // 상호작용 거리(기본 4)
        [SerializeField] GameObject hologram;   // 교란기 위의 홀로그램의 회전을 주기 위함
        [SerializeField] Slider hpGauge;        // 체력게이지
        [SerializeField] float fixTurnSpeed;    // 고정 회전속도
        [SerializeField] float turnSpeed;       // 회전속도
        [SerializeField] int fixHP;             // 고정시킬 체력
        [SerializeField] int currentHP;         // 현재 HP
        public bool disruptorHit;               // 피격당했을 때 멈추는 상태로 넘어가게 하기
        [SerializeField] Material mat1;         // 활성화시 홀로그램 색상(파랑)
        [SerializeField] Material mat2;         // 멈출시 홀로그램 색상9빨강)
        [SerializeField] Renderer renderer;
        public Transform Player;

        public enum State { Activate, Destroyed, Stop }
        State state = State.Activate;

        private void Start()
        {
            // 임시로 시작시 바로 실행
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
            HpGauge();     // 체력게이지
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
                //SceneManager.LoadScene(""); // 교란기 파괴시 여기에서 신을 불러와주기.
                state = State.Destroyed;
                print("교란기 파괴");
            }
            if (disruptorHit)
            {
                turnSpeed = 0;
                state = State.Stop;
                GameManager.Data.ChangeTarget(false);
                renderer.sharedMaterial = mat2;
                
            }
        }
        // 교란기 재가동
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
                float ToPlayer = Vector3.Distance(transform.position, other.transform.position); // 플레이어와 교란기 사이의 위치 구하기
                if (Player != null && ToPlayer < interaction)        // 플레이어면서 수리가능한 범위안에 들어왔을 때
                {
                    if (Input.GetKeyDown(KeyCode.F) && disruptorHit) // 교란기가 피격당한 상태에서만 F키를 눌러 고칠 수 있음
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

