using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;

namespace Darik
{
    public class Enemy_Rifle : Enemy
    {
        public enum State { Appear, Idle, Move, Attack, Die }
        StateMachine<State, Enemy_Rifle> stateMachine;

        [SerializeField] private TMP_Text stateText;
        [SerializeField] private float appearTime = 3f;
        [SerializeField] private int attackRange;

        private Vector3 moveDir;
        private bool isMove = false;
        private int squareDistanceToTarget;
        private bool reload = true;

        protected override void Awake()
        {
            base.Awake();

            stateMachine = new StateMachine<State, Enemy_Rifle>(this);
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

        private int SquareDistanceToTarget(Vector3 toTarget)
        {
            return (int)(toTarget.x * toTarget.x + toTarget.y * toTarget.y + toTarget.z * toTarget.z);
        }

        IEnumerator NavDestinationCoroutine()
        {
            agent.isStopped = false;
            while (true)
            {
                SearchTarget();
                if (target != null)
                    agent.destination = target.position;

                yield return new WaitForSeconds(0.2f);
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

                if (owner.squareDistanceToTarget <= owner.attackRange)
                    stateMachine.ChangeState(State.Attack);
            }

            public override void Exit()
            {
                owner.agent.isStopped = true;
                owner.StopAllCoroutines();
                owner.isMove = false;
                owner.photonView.RPC("SetBoolMove", RpcTarget.AllViaServer, false);
            }
        }

        private class AttackState : Enemy_RifleState
        {
            public AttackState(Enemy_Rifle owner, StateMachine<State, Enemy_Rifle> stateMachine) : base(owner, stateMachine)
            {
            }

            public override void Setup()
            {

            }

            public override void Enter()
            {
                owner.reload = true;
                //owner.StartCoroutine(owner.AttackCoroutine());
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

                if (owner.squareDistanceToTarget > owner.attackRange)
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

            }
        }
        #endregion
    }
}
