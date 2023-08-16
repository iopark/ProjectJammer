using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Park_Woo_Young
{
    public class DisruptorState : MonoBehaviourPunCallbacks, Darik.IHittable, IInteractable
    {
        [SerializeField] GameObject hologram;            // ������ ���� Ȧ�α׷��� ȸ���� �ֱ� ����
        [SerializeField] GameObject emp;                 // Ŭ����� ����� ����Ʈ
        [SerializeField] new Renderer renderer;
        [SerializeField] Material hologram_Blue;         // Ȱ��ȭ�� Ȧ�α׷� ����(�Ķ�)
        [SerializeField] Material hologram_Red;          // ����� Ȧ�α׷� ����(����)

        [SerializeField] float perSecond = 1;              // ������ ���൵, ü�� ȸ���� �ʿ��� �ð� !!0���� �����ҽ� �����Ⱑ ������ ��!!
        [SerializeField] float maxHologramRotSpeed = 100;// Ȧ�α׷� �ִ� ȸ���ӵ�
        [SerializeField] int maxProgress = 100;          // Ŭ��� �ʿ��� ���൵

        public float interaction = 4;                    // ��ȣ�ۿ� �Ÿ�
        public float time;                               // ��ŸŸ��
        public bool disruptorHit;                        // ���ݴ����� �� ���ߴ� ���·� �Ѿ�� �ϱ�
        private float hologramRotSpeed = 0;              // Ȧ�α׷� ���� ȸ���ӵ�
        public int progress = 0;                         // ���� ���൵
        private float smallSwellingTime;                 // ó�� ���� ������ Ȯ���ϴ� �ð� 
        private float shrinkageTime;                     // ó�� Ȯ���Ѱ� �ٽ� 0���� ����µ� �ɸ��� �ð�
        private float empExplosionTime;                  // 0���� ����� ��� Ȯ��
        private float smallSwellingRange;                // ������Ʈ �ѹ��� Ȯ�����
        private float Range;                             // ������Ʈ �ѹ��� ���� ����
        private float EmpRange;                          // ����ȵ� Ȯ���ϴ� ����

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
                //SceneManager.LoadScene(""); // ������ ���� ���⿡�� ���� �ҷ����ֱ�.
                photonView.RPC("GameClear", RpcTarget.AllViaServer);

                state = State.Success;

                hologramRotSpeed = 500;
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
            // time = 0;                   // �ð��� 0���� �ʱ�ȭ
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

