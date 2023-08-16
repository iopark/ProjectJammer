using Photon.Pun;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.InputSystem;

namespace ildoo
{
    public class PlayerMover : MonoBehaviourPun, IPunObservable
    {
        //Distinguishing each client 
        private Rigidbody rigid;
        private Animator anim;

        [Header("Movement Status")]
        private Vector3 moveDir;
        [SerializeField] private float moveSpeed;
        [Range(0f, 5f)]
        [SerializeField] private float ySpeed;
        [SerializeField] private bool isGround;
        [SerializeField] private bool isRunning;

        [Header("Movement Settings")]
        [SerializeField] float walkSpeed;
        [SerializeField] float runSpeed;
        [SerializeField] private float jumpSpeed;

        //Movement Animation Sync 
        private void Awake()
        {
            //PlayerInput managed through Player Class 
            moveDir = Vector3.zero; 
            anim = GetComponent<Animator>();
            rigid = GetComponent<Rigidbody>();
        }
        private void OnEnable()
        {
            
        }
        private void OnDisable()
        {
            anim.Rebind();
        }

        private void Update()
        {
            Move();
        }
        private void FixedUpdate()
        {
            GroundCheck();
            if (!photonView.IsMine)
                SyncAnim(); 
        }
        private void SyncAnim()
        {
            anim.SetFloat("ZSpeed", moveDir.z, 0.1f, Time.deltaTime);
            anim.SetFloat("XSpeed", moveDir.x, 0.1f, Time.deltaTime);
            anim.SetFloat("Speed", moveSpeed);
        }
        //=========================Movement data===============================
        const float deltaThreshhold = 0.05f;
        float delta;
        private void Move()
        {
            //로컬 기준 움직임 
            if (moveDir.magnitude <= 0) // 안움직임 
            {
                if (moveSpeed <= 0.1)
                    moveSpeed = 0;
                moveSpeed = Mathf.Lerp(moveSpeed, 0, 0.5f);
            }
            else if (isRunning)
            {
                delta = Mathf.Abs(moveSpeed - runSpeed);
                if (delta <= deltaThreshhold)
                    moveSpeed = runSpeed;
                moveSpeed = Mathf.Lerp(moveSpeed, runSpeed, 0.5f);
            }
            else
            {
                delta = Mathf.Abs(moveSpeed - walkSpeed);
                if (delta <= deltaThreshhold)
                    moveSpeed = runSpeed;
                //default movement
                moveSpeed = Mathf.Lerp(moveSpeed, walkSpeed, 0.5f);
            }
            rigid.MovePosition(rigid.position + transform.TransformDirection(moveDir) * moveSpeed * Time.deltaTime);
            anim.SetFloat("ZSpeed", moveDir.z, 0.1f, Time.deltaTime);
            anim.SetFloat("XSpeed", moveDir.x, 0.1f, Time.deltaTime);
            anim.SetFloat("Speed", moveSpeed);
        }
        private void OnMove(InputValue value)
        {

            Vector2 input = value.Get<Vector2>();

            moveDir = new Vector3(input.x, 0, input.y);
        }
        private void RigidJump()
        {
            rigid.AddForce(Vector3.up * jumpSpeed, ForceMode.Impulse);
        }

        private void OnJump(InputValue value)
        {
            if (GroundCheck())
            {
                //ySpeed = jumpSpeed;
                RigidJump(); 
            }
        }
        RaycastHit groundHit;
        private bool GroundCheck()
        {
            isGround = Physics.SphereCast(transform.position + Vector3.up * 1, 0.5f, Vector3.down, out groundHit, 0.5f);
            return isGround;
        }
        private void OnRun(InputValue value)
        {
            isRunning = value.isPressed;
        }
        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                stream.SendNext(moveSpeed);
                stream.SendNext(moveDir);
            }
            else
            {
                moveSpeed = (float)stream.ReceiveNext();
                moveDir = (Vector3)stream.ReceiveNext();
            }
        }
    }
}
