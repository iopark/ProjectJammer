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
        [SerializeField] TMP_Text progress_Text;         // �����̵带 ��ü�� �ؽ�Ʈ ���൵
        [SerializeField] new Renderer renderer;
        [SerializeField] Material hologram_Blue;         // Ȱ��ȭ�� Ȧ�α׷� ����(�Ķ�)
        [SerializeField] Material hologram_Red;          // ����� Ȧ�α׷� ����(����)
        
        [SerializeField] int perSecond = 1;              // ������ ���൵, ü�� ȸ���� �ʿ��� �ð� !!0���� �����ҽ� �����Ⱑ ������ ��!!
        [SerializeField] float maxHologramRotSpeed = 100;// Ȧ�α׷� �ִ� ȸ���ӵ�
        [SerializeField] int maxHP = 100;                // �ִ� ü��
        [SerializeField] int maxProgress = 100;          // Ŭ��� �ʿ��� ���൵
        [SerializeField] int progressGoesUp = 1;         // ������ ���൵�� �ʿ��� �ð��� �����Ǹ� ���൵�� �ö󰡴� �ӵ�
        [SerializeField] int hpRepair = 1;               // ü�°��ҽ� �ð��� ���� ȸ���ӵ�

        public float interaction = 4;                    // ��ȣ�ۿ� �Ÿ�
        public float time;                               // ��ŸŸ��
        public bool disruptorHit;                        // ���ݴ����� �� ���ߴ� ���·� �Ѿ�� �ϱ�
        private float hologramRotSpeed = 0;              // Ȧ�α׷� ���� ȸ���ӵ�
        public int currentHP;                            // ���� HP(���۽� maxHp�� ���� ������)
        public int progress = 0;                         // ���� ���൵
        private float smallSwellingTime;                 // ó�� ���� ������ Ȯ���ϴ� �ð� 
        private float shrinkageTime;                     // ó�� Ȯ���Ѱ� �ٽ� 0���� ����µ� �ɸ��� �ð�
        private float empExplosionTime;                  // 0���� ����� ��� Ȯ��
        private float smallSwellingRange;                // ������Ʈ �ѹ��� Ȯ�����
        private float Range;                             // ������Ʈ �ѹ��� ���� ����
        private float EmpRange;                          // ����ȵ� Ȯ���ϴ� ����

        public enum State { Activate, Stop, Success, Destroyed }
        State state = State.Activate;
        private void Start()
        {
            // �ӽ÷� ���۽� �ٷ� ����
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

        private void Hit(int damage)
        {
            if (PhotonNetwork.IsMasterClient)
                photonView.RPC("DisruptorOff", RpcTarget.AllViaServer, damage);
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
                case State.Destroyed:
                    DestroyedUpdate();
                    break;
            }
        }

        public void ActivateUpdate()
        {
            Rotate();
            time += Time.deltaTime;
            progress_Text.text = $"current Progress : {progress}/{maxProgress}";
            if (time > perSecond)
            {
                if (progress >= 0 && currentHP == 100)
                {
                    time = 0;
                    progress += progressGoesUp;
                    currentHP = maxHP;
                }
                else if (progress <= 0 && currentHP < 99)
                {
                    time = 0;
                    progress = 0;
                    currentHP += hpRepair;
                }
            }
            if (progress >= maxProgress)
            {
                //SceneManager.LoadScene(""); // ������ ���� ���⿡�� ���� �ҷ����ֱ�.
                state = State.Success;
                print("������ ��������");
                progress_Text.color = Color.blue;
                progress_Text.text = "Success";
            }
            if (disruptorHit)
            {
                hologramRotSpeed = 0;
                state = State.Stop;
                GameManager.Enemy.ChangeTarget(false);
                renderer.sharedMaterial = hologram_Red;
                progress_Text.color = Color.red;

            }
        }
        // ������ �簡��
        private void StopDisruptor()
        {
            time = 0;
            progress_Text.text = $"current Progress : {progress}/{maxProgress}";
            
            if (!disruptorHit)
            {
                GameManager.Enemy.ChangeTarget(true);
                hologramRotSpeed = maxHologramRotSpeed;
                state = State.Activate;
                renderer.sharedMaterial = hologram_Blue;
                progress_Text.color = Color.white;
            }
            if (progress <= -1)
            {
                //SceneManager.LoadScene(""); // ������ �ı��� ���⿡�� ���� �ҷ����ֱ�.
                state = State.Destroyed;
                progress_Text.color = Color.red;
                progress_Text.text = "Destruction";
                print("������ �ı�");
            }
        }

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


        public void DestroyedUpdate()
        {

        }
        public void SuccessUpdate()
        {
            SuccessEffect();
        }

        private void Rotate()
        {
            hologram.transform.Rotate(Vector3.back, hologramRotSpeed * Time.deltaTime);
        }

        public void TakeDamage(int damage, Vector3 hitPoint, Vector3 normal)
        {
            if(!disruptorHit)
                Hit(damage);
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
        private void Hologram()
        {
            if (!disruptorHit)
            {
                renderer.sharedMaterial = hologram_Blue;
                progress_Text.color = Color.white;
            }
            else
            {
                renderer.sharedMaterial = hologram_Red;
                progress_Text.color = Color.red;
            }
        }

        [PunRPC]
        private void DisruptorOn()
        {
            disruptorHit = false;
            renderer.sharedMaterial = hologram_Blue;
            progress_Text.color = Color.white;
        }

        [PunRPC]
        private void DisruptorOff(int damage)
        {
            //progress -= damage;
            disruptorHit = true;
            renderer.sharedMaterial = hologram_Red;
            progress_Text.color = Color.red;
        }

    }
}

