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
        // 0단계 파괴되는 조건만
        [SerializeField] GameObject hologram;   // 교란기 위의 홀로그램의 회전을 주기 위함
        [SerializeField] Slider hpGauge;        // 체력게이지
        [SerializeField] float fixTurnSpeed;    // 고정 회전속도
        [SerializeField] float turnSpeed;       // 회전속도
        [SerializeField] int fixHP;             // 고정시킬 체력
        [SerializeField] int currentHP;         // 현재 HP

        public enum State { Activate, Destroyed }
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
            SetDisruptor();
        }

        [PunRPC]
        private void Hit(int damage)
        {
        }

        [PunRPC]
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
                state = State.Destroyed;
            }    
        }

        public void DestroyedUpdate()
        {
            Debug.Log("교란기 파괴");
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
