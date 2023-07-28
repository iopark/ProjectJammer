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
        // 교란기 상호작용시 -> 교란기가 데이터 매니저에 신호를 주고
        // 교란기 피격 -> 게임매니저에 보내서 처리하게 함
        [SerializeField] GameObject hologram;   // 교란기 위의 홀로그램의 회전을 주기 위함
        [SerializeField] Slider repairGauge;
        [SerializeField] Slider hpGauge;

        [SerializeField] float fixTurnSpeed;    // 고정 회전속도
        [SerializeField] float turnSpeed;       // 회전속도
        [SerializeField] int fixHP;             // 고정시킬 체력
        [SerializeField] int currentHP;         // 현재 HP

        [SerializeField] int repair;            // 수리상태
        [SerializeField] int maxRepair;         // 클리어를 위한 수리목표

        public Transform player;
        //Vector3 pos;

        public float repairTime;                 // 수리시간
        public bool hit;

        public bool testRepair;                  // 테스트하기위해 수리중이라고 판단
        public float testRepairTime;             // 수리시간

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
            // 임시로 시작시 바로 실행
            // GameMamager.Data.GameStart();
            //pos = this.gameObject.transform.position;
            //Debug.Log(pos);
            //Debug.Log(pos.y);

        }
        public void Interacter()
        {
            // 상호작용
            photonView.RPC("GameStart", RpcTarget.MasterClient);
            // photonView 게임 오브젝트가 자기를 서버에 알림
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

            if (repair > 0)                 // 수리진행도가 0보다 크면
            {
                repair -= damage;           // 받은 데미지에서 수리 진행도가 깍임
            }
            else if (repair <= 0)           // 수리진행도가 0보다 작거나 같으면
            {
                currentHP -= damage;        // 체력이 깍임
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
            // 활성화전
            // repairTime = 0f;    // 델타타임 0으로 고정
            if (Input.GetKeyDown(KeyCode.F))
            {
                turnSpeed = fixTurnSpeed;
                state = State.Activate;
            }
        }

        public void ActivateUpdate()
        {
            // 활성화 후 & 재가동
            Rotate();
            
            Debug.Log("활성화");
            repairTime += Time.deltaTime;
            if (currentHP >= fixHP)     // 현재체력이 최대체력보다 큰 경우
            {
                if (repairTime >= 2)   // 수리되는 시간이 10보다 크거나 같으면
                {
                    repair += 1;       // 수리상태에 10을 더해줌
                    repairTime = 0f;    // 시간을 0으로 초기화
                }
            }

            if (currentHP < fixHP) // 현재 체력이 최대체력보다 작은 경우
            {
                repairTime = 0f;         // 수리를 중단하기 위해 시간은 0으로 고정
                if (testRepair)
                {
                    if (!hit)
                    {
                        testRepairTime += Time.deltaTime;
                        if (testRepairTime >= 1)
                        {
                            turnSpeed = fixTurnSpeed / 2;
                            Debug.Log("수리중");
                            currentHP += 1;
                            testRepairTime = 0f;
                        }
                    }
                    else
                    {
                        Debug.Log("수리중단");
                        testRepair = false;
                    }
                }
            }
            else if (currentHP == fixHP)
            {
                Debug.Log("수리완료");
                testRepair = false;
            }

            if (hit)               // bool인 hit가 
            {
                turnSpeed = 0f;
                state = State.Stop; // 데미지를 받음
            }

            if (currentHP <= 0)
            {
                state = State.Destroyed; // 파괴
            }
            if (repair >= maxRepair)
            {
                state = State.RepairCompleted; // 성공
            }
        }

        public void StopUpdate()
        {
            // 데미지를 받을 때
            // turnSpeed = 0f;     // 회전속도를 0으로 줘서 멈추게 함
            // repairTime = 0f; // 수리시간을 0으로 고정시켜서 수리가 진행이 안되게 함
            Debug.Log("멈춤");

            // 플레이어가 다시 재가동
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
            // 파괴
            currentHP = 0;
            repair = 0;
            Debug.Log("교란기 파괴");
        }

        public void RepairCompletedUpdate()
        {
            Debug.Log("교란기 활성화");
            // 수리성공
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
