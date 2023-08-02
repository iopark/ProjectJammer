using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

namespace Darik
{
    public class Enemy_Blade : Enemy
    {
        public enum State { Appear, Idle, Attack, Walk, Die }
        StateMachine<State, Enemy_Blade> stateMachine;

        [SerializeField] private bool debug;
        [SerializeField] private TMP_Text stateText;
        [SerializeField] LayerMask layerMask;
        [SerializeField] float moveSpeed = 5f;
        [SerializeField] private int attackRange;

        private Vector3 moveDir;
        private int squareDistanceToTarget;

        protected override void Awake()
        {
            base.Awake();

            stateMachine = new StateMachine<State, Enemy_Blade>(this);
            stateMachine.AddState(State.Appear, new AppearState(this, stateMachine));
            stateMachine.AddState(State.Idle, new IdleState(this, stateMachine));
            stateMachine.AddState(State.Attack, new AttackState(this, stateMachine));
            stateMachine.AddState(State.Walk, new WalkState(this, stateMachine));
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
        }

        private void SearchTarget()
        {
            Collider[] targets = Physics.OverlapSphere(transform.position, 5f, layerMask);
            if (targets.Length != 0)
            {
                if (debug)
                    Debug.Log("Target On");
                target = targets[0].gameObject.transform;
            }
            else
            {
                if (debug)
                    Debug.Log("Target null");
            }
        }

        private int SquareDistanceToTarget(Vector3 toTarget)
        {
            return (int)(toTarget.x * toTarget.x + toTarget.y * toTarget.y + toTarget.z * toTarget.z);
        }

        protected override void Hit(int damage, Vector3 hitPoint, Vector3 normal)
        {
            base.Hit(damage, hitPoint, normal);

            if (curHp <= 0)
            {
                isDie = true;
                stateMachine.ChangeState(State.Die);
            }
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
            private float curTIme;

            public AppearState(Enemy_Blade owner, StateMachine<State, Enemy_Blade> stateMachine) : base(owner, stateMachine)
            {
            }

            public override void Setup()
            {

            }

            public override void Enter()
            {
                curTIme = 0f;
                owner.stateText.text = "Appear";
            }

            public override void Update()
            {
                curTIme += Time.deltaTime;
            }

            public override void Transition()
            {
                if (curTIme > 5f)
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
                //owner.target = GameManager.Data.disruptor;
                owner.SearchTarget();
            }

            public override void Transition()
            {
                if (owner.target != null)
                    stateMachine.ChangeState(State.Walk);
            }

            public override void Exit()
            {

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
                owner.stateText.text = "Attack";
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
                if (owner.squareDistanceToTarget > owner.attackRange)
                    stateMachine.ChangeState(State.Walk);
            }

            public override void Exit()
            {

            }
        }

        private class WalkState : Enemy_BladeState
        {
            public WalkState(Enemy_Blade owner, StateMachine<State, Enemy_Blade> stateMachine) : base(owner, stateMachine)
            {
            }

            public override void Setup()
            {

            }

            public override void Enter()
            {
                anim.SetBool("IsWalk", true);
                owner.stateText.text = "Walk";
            }

            public override void Update()
            {
                if (owner.target != null)
                {
                    owner.moveDir = owner.target.transform.position - transform.position;
                    owner.squareDistanceToTarget = owner.SquareDistanceToTarget(owner.moveDir);

                    owner.moveDir.Normalize();
                    transform.Translate(owner.moveDir * owner.moveSpeed * Time.deltaTime, Space.World);
                    transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(owner.moveDir), 0.1f);
                }
            }

            public override void Transition()
            {
                if (owner.target == null)
                    stateMachine.ChangeState(State.Idle);
                else if (owner.squareDistanceToTarget <= owner.attackRange)
                    stateMachine.ChangeState(State.Attack);
            }

            public override void Exit()
            {
                anim.SetBool("IsWalk", false);
            }
        }

        private class DieState : Enemy_BladeState
        {
            public DieState(Enemy_Blade owner, StateMachine<State, Enemy_Blade> stateMachine) : base(owner, stateMachine)
            {
            }

            public override void Setup()
            {

            }

            public override void Enter()
            {
                anim.SetBool("IsDie", true);
                owner.stateText.text = "Die";
            }

            public override void Update()
            {

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
