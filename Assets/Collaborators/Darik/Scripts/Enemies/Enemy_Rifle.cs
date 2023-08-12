using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Darik
{
    public class Enemy_Rifle : Enemy
    {
        public enum State { Appear, Idle, Move, FireAttack, BashAttack, Die }
        StateMachine<State, Enemy_Rifle> stateMachine;

        [SerializeField] private TMP_Text stateText;
        [SerializeField] private float appearTime = 2f;
        [SerializeField] private float fireAttackRange = 10f;
        [SerializeField] private float bashAttackRange = 1f;
        [SerializeField] private float bashAttackTiming = 0.1f;

        [SerializeField] Transform muzzlePosition;
        [SerializeField] private float fireCoolTime = 0.5f;
        [SerializeField] private float bashCoolTime = 3f;
        [SerializeField] private int bashAttackDamage = 1;

        private Vector3 moveDir;
        private bool isMove = false;
        private bool fireReload = true;
        private bool bashReload = true;

        protected override void Awake()
        {
            base.Awake();

            stateMachine = new StateMachine<State, Enemy_Rifle>(this);
            stateMachine.AddState(State.Appear, new AppearState(this, stateMachine));
            stateMachine.AddState(State.Idle, new IdleState(this, stateMachine));
            stateMachine.AddState(State.Move, new MoveState(this, stateMachine));
            stateMachine.AddState(State.FireAttack, new FireAttackState(this, stateMachine));
            stateMachine.AddState(State.BashAttack, new BashAttackState(this, stateMachine));
            stateMachine.AddState(State.Die, new DieState(this, stateMachine));
        }

        private void Start()
        {
            stateMachine.SetUp(State.Appear);
        }

        private void Update()
        {
            stateMachine.Update();
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            if (isDie)
                stateMachine.ChangeState(State.Appear);
            isDie = false;
        }

        [PunRPC]
        public override void Hit(int damage, Vector3 hitPoint, Vector3 normal)
        {
            base.Hit(damage, hitPoint, normal);

            if (debug)
                Debug.Log($"hitted, current Hp : {curHp} / {maxHp}");

            if (curHp <= 0)
            {
                isDie = true;
                stateMachine.ChangeState(State.Die);
            }

            if (!isMove)
                anim.SetTrigger("OnHit");
        }

        IEnumerator FireAttackCoroutine()
        {
            while (fireReload)
            {
                fireReload = false;
                photonView.RPC("Fire", RpcTarget.AllViaServer);
                yield return new WaitForSeconds(fireCoolTime);
                fireReload = true;
            }
        }

        [PunRPC]
        public void Fire()
        {
            if (debug)
                Debug.Log("Fire");

            Vector3 dir = ((target.position + Vector3.up * 1.666f) - muzzlePosition.position).normalized;
            GameManager.Resource.Instantiate<GameObject>("Prefabs/EnemyBullets/EnemyBullet", muzzlePosition.position, Quaternion.LookRotation(dir), true);
            anim.SetTrigger("OnFireAttack");
        }

        IEnumerator BashAttackCoroutine()
        {
            while (bashReload)
            {
                bashReload = false;
                photonView.RPC("SetTriggerBashAttack", RpcTarget.AllViaServer);
                yield return new WaitForSeconds(bashAttackTiming);

                if (debug)
                    Debug.Log("Bash");
                target.gameObject.GetComponent<IHittable>()?.TakeDamage(bashAttackDamage, Vector3.zero, transform.forward);
                yield return new WaitForSeconds(bashCoolTime - bashAttackTiming);
                bashReload = true;
            }
        }

        [PunRPC]
        public void SetTriggerBashAttack()
        {
            anim.SetTrigger("OnBashAttack");
        }

        [PunRPC]
        public void SetBoolMove(bool isWalk)
        {
            anim.SetBool("IsWalk", isWalk);
        }

        #region State
        private abstract class Enemy_RifleState : StateBase<State, Enemy_Rifle>
        {
            protected GameObject gameObject => owner.gameObject;
            protected Transform transform => owner.transform;
            protected Rigidbody rb => owner.rb;
            protected Animator anim => owner.anim;
            protected Collider collider => owner.collider;

            protected Enemy_RifleState(Enemy_Rifle owner, StateMachine<State, Enemy_Rifle> stateMachine) : base(owner, stateMachine)
            {
            }
        }

        private class AppearState : Enemy_RifleState
        {
            private float curTime;

            public AppearState(Enemy_Rifle owner, StateMachine<State, Enemy_Rifle> stateMachine) : base(owner, stateMachine)
            {
            }

            public override void Setup()
            {

            }

            public override void Enter()
            {
                curTime = 0f;
                owner.stateText.text = "Appear";
            }

            public override void Update()
            {
                curTime += Time.deltaTime;
            }

            public override void Transition()
            {
                if (curTime > owner.appearTime)
                    stateMachine.ChangeState(State.Idle);
            }

            public override void Exit()
            {

            }
        }

        private class IdleState : Enemy_RifleState
        {
            public IdleState(Enemy_Rifle owner, StateMachine<State, Enemy_Rifle> stateMachine) : base(owner, stateMachine)
            {
            }

            public override void Setup()
            {

            }

            public override void Enter()
            {
                owner.stateText.text = "Idle";
            }

            public override void Update()
            {
                owner.SearchTarget();
            }

            public override void Transition()
            {
                if (owner.target != null)
                    stateMachine.ChangeState(State.Move);
            }

            public override void Exit()
            {

            }
        }

        private class MoveState : Enemy_RifleState
        {
            public MoveState(Enemy_Rifle owner, StateMachine<State, Enemy_Rifle> stateMachine) : base(owner, stateMachine)
            {
            }

            public override void Setup()
            {

            }

            public override void Enter()
            {
                owner.StartCoroutine(owner.NavDestinationCoroutine());
                owner.isMove = true;
                owner.photonView.RPC("SetBoolMove", RpcTarget.AllViaServer, true);
                owner.stateText.text = "Walk";
            }

            public override void Update()
            {
                if (owner.target != null)
                {
                    owner.moveDir = owner.target.transform.position - transform.position;
                    owner.squareDistanceToTarget = owner.SquareDistanceToTarget(owner.moveDir);
                }
            }

            public override void Transition()
            {
                if (owner.target == null)
                    stateMachine.ChangeState(State.Idle);

                if (!owner.CheckIsBlocked(owner.fireAttackRange) && owner.CheckInOfRange(owner.fireAttackRange))
                    stateMachine.ChangeState(State.FireAttack);
            }

            public override void Exit()
            {
                owner.agent.isStopped = true;
                owner.StopAllCoroutines();
                owner.isMove = false;
                owner.photonView.RPC("SetBoolMove", RpcTarget.AllViaServer, false);
            }
        }

        private class FireAttackState : Enemy_RifleState
        {
            public FireAttackState(Enemy_Rifle owner, StateMachine<State, Enemy_Rifle> stateMachine) : base(owner, stateMachine)
            {
            }

            public override void Setup()
            {

            }

            public override void Enter()
            {
                owner.fireReload = true;
                owner.StartCoroutine(owner.FireAttackCoroutine());
                owner.stateText.text = "FireAttack";
            }

            public override void Update()
            {
                owner.SearchTarget();
                if (owner.target != null)
                {
                    owner.moveDir = owner.target.transform.position - transform.position;
                    owner.squareDistanceToTarget = owner.SquareDistanceToTarget(owner.moveDir);
                    
                    owner.moveDir.Normalize();
                    transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(owner.moveDir), 0.1f);
                }
            }

            public override void Transition()
            {
                if (owner.target == null)
                    stateMachine.ChangeState(State.Idle);

                if (!owner.CheckIsBlocked(Vector3.Distance(transform.position, owner.target.position)))
                {
                    if (owner.CheckInOfRange(owner.bashAttackRange))
                        stateMachine.ChangeState(State.BashAttack);

                    if (owner.CheckOutOfRange(owner.fireAttackRange))
                        stateMachine.ChangeState(State.Move);
                }
                else
                    stateMachine.ChangeState(State.Move);
            }

            public override void Exit()
            {
                owner.StopAllCoroutines();
            }
        }

        private class BashAttackState : Enemy_RifleState
        {
            public BashAttackState(Enemy_Rifle owner, StateMachine<State, Enemy_Rifle> stateMachine) : base(owner, stateMachine)
            {
            }

            public override void Setup()
            {

            }

            public override void Enter()
            {
                owner.bashReload = true;
                owner.StartCoroutine(owner.BashAttackCoroutine());
                owner.stateText.text = "BashAttack";
            }

            public override void Update()
            {
                owner.SearchTarget();
                if (owner.target != null)
                {
                    owner.moveDir = owner.target.transform.position - transform.position;
                    owner.squareDistanceToTarget = owner.SquareDistanceToTarget(owner.moveDir);

                    owner.moveDir.Normalize();
                    transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(owner.moveDir), 0.1f);
                }
            }

            public override void Transition()
            {
                if (owner.target == null)
                    stateMachine.ChangeState(State.Idle);

                if (!owner.CheckIsBlocked(Vector3.Distance(transform.position, owner.target.position)))
                {
                    if (owner.CheckOutOfRange(owner.bashAttackRange))
                        stateMachine.ChangeState(State.FireAttack);
                }
                else
                    stateMachine.ChangeState(State.Move);
            }

            public override void Exit()
            {
                owner.StopAllCoroutines();
            }
        }

        private class DieState : Enemy_RifleState
        {
            private float curTime;

            public DieState(Enemy_Rifle owner, StateMachine<State, Enemy_Rifle> stateMachine) : base(owner, stateMachine)
            {
            }

            public override void Setup()
            {

            }

            public override void Enter()
            {
                curTime = 0f;
                anim.SetBool("IsDie", true);
                owner.stateText.text = "Die";
            }

            public override void Update()
            {
                curTime += Time.deltaTime;
                if (curTime > 3f)
                    PhotonNetwork.Destroy(gameObject);
            }

            public override void Transition()
            {

            }

            public override void Exit()
            {
                anim.SetBool("IsDie", false);
            }
        }
        #endregion
    }
}
