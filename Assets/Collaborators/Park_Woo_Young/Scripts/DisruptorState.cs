using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Park_Woo_Young
{
    public class DisruptorState : MonoBehaviourPunCallbacks, Darik.IHittable, IInteractable
    {
        [SerializeField] GameObject hologram;            // 교란기 위의 홀로그램의 회전을 주기 위함
        [SerializeField] GameObject emp;                 // 클리어시 생기는 이펙트
        [SerializeField] new Renderer renderer;
        [SerializeField] Material hologram_Blue;         // 활성화시 홀로그램 색상(파랑)
        [SerializeField] Material hologram_Red;          // 멈출시 홀로그램 색상(빨강)

        [SerializeField] float perSecond = 1;              // 교란기 진행도, 체력 회복에 필요한 시간 !!0으로 설정할시 교란기가 완충이 됨!!
        [SerializeField] float maxHologramRotSpeed = 100;// 홀로그램 최대 회전속도
        [SerializeField] int maxProgress = 100;          // 클리어에 필요한 진행도

        public float interaction = 4;                    // 상호작용 거리
        public float time;                               // 델타타임
        public bool disruptorHit;                        // 공격당했을 때 멈추는 상태로 넘어가게 하기
        private float hologramRotSpeed = 0;              // 홀로그램 현재 회전속도
        public int progress = 0;                         // 현재 진행도
        private float smallSwellingTime;                 // 처음 작은 범위로 확장하는 시간 
        private float shrinkageTime;                     // 처음 확장한걸 다시 0으로 만드는데 걸리는 시간
        private float empExplosionTime;                  // 0으로 만든뒤 계속 확장
        private float smallSwellingRange;                // 업데이트 한번당 확장범위
        private float Range;                             // 업데이트 한번당 수축 범위
        private float EmpRange;                          // 수축된뒤 확장하는 범위

        public enum State { Activate, Stop, Success }
        State state = State.Stop;

        public void GameStart()
        {
            renderer.sharedMaterial = hologram_Blue;
            hologramRotSpeed = maxHologramRotSpeed;

            state = State.Activate;

            SetDisruptor();
        }
        
        private void SetDisruptor()
        {
            GameManager.Data.Disruptor = this.transform;
            GameManager.Enemy.ChangeTarget(true);
        }

        private void Update()
        {
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
            }
        }


        public void ActivateUpdate()
        {
            Rotate();
            time += Time.deltaTime;
            
            if (time > perSecond)
            {
                if (PhotonNetwork.IsMasterClient)
                {
                    photonView.RPC("ProgressGoesUp", RpcTarget.AllViaServer);
                    progress = GameManager.Data.DisruptorProgress;

                }
                time = 0;
            }

            if (progress >= maxProgress)
            {
                //SceneManager.LoadScene(""); // 교란기 성공 여기에서 신을 불러와주기.
                photonView.RPC("GameClear", RpcTarget.AllViaServer);

                state = State.Success;

                hologramRotSpeed = 500;
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

        private void StopDisruptor()
        {
            time = 0;
            
            if (!disruptorHit)
            {
                GameManager.Enemy.ChangeTarget(true);
                hologramRotSpeed = maxHologramRotSpeed;
                state = State.Activate;
                renderer.sharedMaterial = hologram_Blue;
            }
        }

        public void SuccessUpdate()
        {
            Rotate();
            GameManager.Enemy.StopSpawnEnemy();
            photonView.RPC("SuccessEffect", RpcTarget.AllViaServer);
        }

        private void Hologram()
        {
            if (!disruptorHit)
            {
                renderer.sharedMaterial = hologram_Blue;
            }
            else
            {
                renderer.sharedMaterial = hologram_Red;
            }
        }
        
        private void Rotate()
        {
            hologram.transform.Rotate(Vector3.back, hologramRotSpeed * Time.deltaTime);
        }
        
        public void Interact()
        {
            if (disruptorHit)
            {
                if (PhotonNetwork.IsMasterClient)
                    photonView.RPC("DisruptorOn", RpcTarget.AllViaServer);
            }
            else
            {
                return;
            }
        }

        private void Hit(int damage)
        {
            if (PhotonNetwork.IsMasterClient)
                photonView.RPC("DisruptorOff", RpcTarget.AllViaServer, damage);
        }

        public void TakeDamage(int damage, Vector3 hitPoint, Vector3 normal)
        {
            if(!disruptorHit)
                Hit(damage);
        }

        [PunRPC]
        private void SuccessEffect()
        {
            smallSwellingTime += Time.deltaTime;

            if (smallSwellingTime > 0.01f)
            {
                smallSwellingRange += 0.1f;
                Range += 0.1f;
                emp.transform.localScale = new Vector3(0f + smallSwellingRange, 0f + smallSwellingRange, 0f + smallSwellingRange);
                smallSwellingTime = 0;

            }
            else if (smallSwellingRange > 10)
            {
                smallSwellingTime = -1;

            }

            if (smallSwellingTime == -1)
            {
                
                shrinkageTime += Time.deltaTime;
                if (shrinkageTime > 0.01f)
                {
                    Range -= 0.4f;
                    emp.transform.localScale = new Vector3(0f - Range, 0f - Range, 0f - Range);
                }
                if (Range < 0)
                {
                    shrinkageTime = -1;
                }

            }
            if (smallSwellingTime == -1 && shrinkageTime == -1)
            {
                empExplosionTime += Time.deltaTime;
                EmpRange += 0.8f;
                emp.transform.localScale = new Vector3(0f + EmpRange, 0f + EmpRange, 0f + EmpRange);
                
            }
        }

        [PunRPC]
        public void ProgressGoesUp()
        {
            // time = 0;                   // 시간을 0으로 초기화
            GameManager.Data.DisruptorProgress++;
        }

        [PunRPC]
        private void DisruptorOn()
        {
            disruptorHit = false;
            renderer.sharedMaterial = hologram_Blue;
        }

        [PunRPC]
        private void DisruptorOff(int damage)
        {
            disruptorHit = true;
            renderer.sharedMaterial = hologram_Red;
        }

        [PunRPC]
        public void GameClear()
        {
            if (!PhotonNetwork.IsMasterClient)
                SuccessEffect();


            GameManager.Data.GameClear();
        }
    }
}

