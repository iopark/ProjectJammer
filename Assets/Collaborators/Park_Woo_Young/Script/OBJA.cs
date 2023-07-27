using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;
using Photon.Pun.Demo.PunBasics;
using Photon.Pun;

namespace Park_Woo_Young
{
    public class OBJA : MonoBehaviour
    {
        // 교란기 상호작용시 -> 교란기가 데이터 매니저에 신호를 주고
        // 교란기 피격 -> 게임매니저에 보내서 처리하게 함
        //

        [SerializeField] float turnSpeed; // 회전속도
        [SerializeField] int currentHP; // 현재 HP
        [SerializeField] int maxHP;     // 고정시킬 체력
        [SerializeField] int repair;    // 수리상태
        [SerializeField] int maxRepair; // 클리어를 위한 수리목표

        public float repairTime;          // 수리시간
        public bool hit = true;

        public enum State { A, B, C, D, E }

        State state = State.A;

        private void Awake()
        {
            currentHP = maxHP;
            repair = 0;
            repairTime = 0;
        }
        private void Start()
        {
            // 임시로 시작시 바로 실행
            // GameMamager.Data.GameStart();
        }
        public void Interacter()
        {
            // 상호작용
            // photonView.RPC("GameStart",RpcTarget.MasterClient);
            // photonView 게임 오븢젝트가 자기를 서버에 알림
        }
        [PunRPC]
        private void GameStart()
        {

        }

        public void TakeDamage(int damage)
        {
            //photonView.RPC("Hit", RpcTarget.MasterClient);
        }
        [PunRPC]
        private void Hie(int damage)
        {
            currentHP -= damage;
            //GameManager.Data.ChangeHp(currentHP);

        }


        private void Update()
        {
            switch (state)
            {
                case State.A:
                    AUpdate();
                    break;
                case State.B:
                    BUpdate();
                    break;
                case State.C:
                    CUpdate();
                    break;
                case State.D:
                    DUpdate();
                    break;
                case State.E:
                    EUpdate();
                    break;
            }
        }

        public void AUpdate()
        {
            // 활성화전
            repairTime = 0f;    // 델타타임 0으로 고정
            if (Input.GetKeyDown(KeyCode.F))
            {
                state = State.B;
            }
        }

        public void BUpdate()
        {
            // 활성화 후 & 재가동
            Rotate();
            repairTime += Time.deltaTime;
            if (currentHP >= maxHP)     // 현재체력이 최대체력보다 큰 경우
            {
                /*if (repairTime >= 10)   // 수리되는 시간이 10보다 크거나 같으면
                {
                    repair += 10;       // 수리상태에 10을 더해줌
                    repairTime = 0f;    // 시간을 0으로 초기화
                }*/
            }
            else if (currentHP < maxHP) // 현재 체력이 최대체력보다 작은 경우
            {
                repairTime = 0;         // 수리를 중단하기 위해 시간은 0으로 고정
            }


            else if (hit)               // bool인 hit가 
            {
                State state = State.C; // 데미지를 받음
            }
            else if (currentHP <= 0)
            {
                state = State.D; // 파괴
            }
            else if (repair >= maxRepair)
            {
                state = State.E; // 성공
            }
        }

        public void CUpdate()
        {
            // 데미지를 받을 때\
            turnSpeed = 0f;
            repairTime = 0f;


            // 플레이어가 다시 재가동
            if (Input.GetKeyDown(KeyCode.F))
            {
                hit = false;
                state = State.B;
            }
        }

        public void DUpdate()
        {
            // 파괴
            currentHP = 0;
            repair = 0;
            Debug.Log("Game Over");
        }

        public void EUpdate()
        {
            Debug.Log("Game Clear");
            // 수리성공
        }

        private void Rotate()
        {
            transform.Rotate(Vector3.back, turnSpeed * Time.deltaTime);
        }
        private void OnTriggerStay(Collider other)
        {
            if (other.name == "Player")
            {
                if (Input.GetKeyDown(KeyCode.F))
                {
                    if (State.A != State.B)
                    {

                    }
                }
            }
        }
    }
}
