using ildoo;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Darik
{
    public class Enemy_Blade : Enemy
    {
        public enum State { Appear, Idle, Move, Attack, Die }
        StateMachine<State, Enemy_Blade> stateMachine;

        [SerializeField] private TMP_Text stateText;
        [SerializeField] private LayerMask disruptorLayer;
        [SerializeField] private float appearTime = 2f;
        [SerializeField] private float attackRange;
        [SerializeField] private float attackCoolTime = 3f;
        [SerializeField] private float attackTiming = 0.2f;
        [SerializeField] private int damage = 1;

        private Vector3 moveDir;
        private bool isMove = false;
        private bool reload = true;

        protected override void Awake()
        {
            base.Awake();

            stateMachine = new StateMachine<State, Enemy_Blade>(this);
            stateMachine.AddState(State.Appear, new AppearState(this, stateMachine));
            stateMachine.AddState(State.Idle, new IdleState(this, stateMachine));
            stateMachine.AddState(State.Move, new MoveState(this, stateMachine));
            stateMachine.AddState(State.Attack, new AttackState(this, stateMachine));
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

        IEnumerator AttackCoroutine()
        {
            while (reload)
            {
                reload = false;
                photonView.RPC("SetTriggerAttack", RpcTarget.AllViaServer);
                yield return new WaitForSeconds(attackTiming);

                target.gameObject.GetComponent<IHittable>()?.TakeDamage(damage, Vector3.zero, transform.forward);

                yield return new WaitForSeconds(attackCoolTime - attackTiming);
                reload = true;
            }
        }

        [PunRPC]
        public void SetTriggerAttack()
        {
            anim.SetTrigger("OnAttack");
        }

        [PunRPC]
        public void SetBoolMove(bool isWalk)
        {
            anim.SetBool("IsWalk", isWalk);
        }

        #region State
        private abstract class Enemy_BladeState : StateBase<State, Enemy_Blade>
        {
            protected GameObject gameObject => owner.gameObject;
            protected Transform transform => owner.transform;
            protected Rigidbody rb => owner.rb;
            protected Animator anim => owner.anim;
            protected Collider collider => owner.collider;

            protected Enemy_BladeState(Enemy_Blade owner, StateMachine<State, Enemy_Blade> stateMachine) : base(owner, stateMachine)
            {
            }
        }

        private class AppearState : Enemy_BladeState
        {
            private float curTime;

            public AppearState(Enemy_Blade owner, StateMachine<State, Enemy_Blade> stateMachine) : base(owner, stateMachine)
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

        private class IdleState : Enemy_BladeState
        {
            public IdleState(Enemy_Blade owner, StateMachine<State, Enemy_Blade> stateMachine) : base(owner, stateMachine)
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

        private class MoveState : Enemy_BladeState
        {
            public MoveState(Enemy_Blade owner, StateMachine<State, Enemy_Blade> stateMachine) : base(owner, stateMachine)
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

                if (owner.disruptorLayer.Contain(owner.target.gameObject.layer))
                {
                    if (owner.CheckInOfRange(owner.attackRange))
                        stateMachine.ChangeState(State.Attack);
                }
                else
                {
                    if (owner.CheckInOfRange(1))
                        stateMachine.ChangeState(State.Attack);
                }
                
            }

            public override void Exit()
            {
                owner.agent.isStopped = true;
                owner.StopAllCoroutines();
                owner.isMove = false;
                owner.photonView.RPC("SetBoolMove", RpcTarget.AllViaServer, false);
            }
        }

        private class AttackState : Enemy_BladeState
        {
            public AttackState(Enemy_Blade owner, StateMachine<State, Enemy_Blade> stateMachine) : base(owner, stateMachine)
            {
            }

            public override void Setup()
            {

            }

            public override void Enter()
            {
                owner.reload = true;
                owner.StartCoroutine(owner.AttackCoroutine());
                owner.stateText.text = "Attack";
            }

            public override void Update()
            {
                owner.SearchTarget();
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

                if (owner.CheckOutOfRange(owner.attackRange))
                    stateMachine.ChangeState(State.Move);
            }

            public override void Exit()
            {
                owner.StopAllCoroutines();
            }
        }

        private class DieState : Enemy_BladeState
        {
            private float curTime;

            public DieState(Enemy_Blade owner, StateMachine<State, Enemy_Blade> stateMachine) : base(owner, stateMachine)
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
                if (curTime > 3f && owner.photonView.IsMine)
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
