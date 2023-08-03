using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace Park_Woo_Young
{
    public class DisruptorState : MonoBehaviourPunCallbacks, Darik.IHittable
    {
        // 교란기 상호작용시 -> 교란기가 데이터 매니저에 신호를 주고
        // 교란기 피격 -> 게임매니저에 보내서 처리하게 함
        // 교란기가 생길 위치를 저장하고 그걸 데이터매니저에 보낸다음 위치를 가져오기
        [SerializeField] GameObject hologram;   // 교란기 위의 홀로그램의 회전을 주기 위함
        [SerializeField] Slider repairGauge;    // 수리진행도 게이지
        [SerializeField] Slider hpGauge;        // 체력게이지

        [SerializeField] float fixTurnSpeed;    // 고정 회전속도
        [SerializeField] float turnSpeed;       // 회전속도
        [SerializeField] int fixHP;             // 고정시킬 체력
        [SerializeField] int currentHP;         // 현재 HP

        [SerializeField] int repair;            // 수리상태
        [SerializeField] int maxRepair;         // 클리어를 위한 수리목표

        public Transform player;

        public float repairTime;                 // 수리시간

        // 테스트용 변수
        public bool testRepair;                  // 테스트하기위해 수리중이라고 판단 (hp가 100아래인경우)
        public bool interacter;

        public enum State { Disabled, Activate, Stop, Destroyed, RepairCompleted }

        // State state = State.Disabled;    // 활성화 전
        State state = State.Activate;       // 활성화

        private void Start()
        {
            // 임시로 시작시 바로 실행
            GameStart();
        }

        private void SetDisruptor()
        {
            GameManager.Data.Disruptor = this.transform;
        }

        [PunRPC]
        public void GameStart()
        {
            turnSpeed = fixTurnSpeed;
            currentHP = fixHP;
            repair = 0;
            SetDisruptor();
        }

        [PunRPC]
        private void Hit(int damage)
        {
            repair -= damage;
            if (repair <= 0)
            {
                currentHP -= damage;
            }
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

        private void Update()
        {
            RepairGauge(); // 수리게이지
            HpGauge();     // 체력게이지
            
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
        }

        public void ActivateUpdate()
        {
            // 활성화 후 & 재가동
            //Debug.Log("활성화");
            Rotate();
            repairTime += Time.deltaTime;
            if (repairTime > 1)
            {
                repair -= 1;
                repairTime = 0;
                if (repair <= 0)
                {
                    repair = 0;
                    currentHP -= 10;
                }
            }
            if (currentHP <=0)
            {
                state = State.Destroyed;
            }
            
        }

        public void StopUpdate()
        {
            // 데미지를 받을 때
            //Debug.Log("멈춤");
        }

        public void DestroyedUpdate()
        {
            // 파괴
            Debug.Log("교란기 파괴");
        }

        public void RepairCompletedUpdate()
        {
            // 수리성공
            //Debug.Log("교란기 활성화");
        }

        private void Rotate()
        {
            hologram.transform.Rotate(Vector3.back, turnSpeed * Time.deltaTime);
        }

        public void TakeDamage(int damage, Vector3 hitPoint, Vector3 normal)
        {
            photonView.RPC("Hit", RpcTarget.MasterClient, damage); //
        }
    }
    
}
