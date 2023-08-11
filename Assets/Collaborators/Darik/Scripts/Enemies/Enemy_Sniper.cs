using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Darik
{
    public class Enemy_Sniper : Enemy
    {
        public enum State { Appear, Idle, MoveForward, MoveBackward, Attack, Die }
        StateMachine<State, Enemy_Sniper> stateMachine;

        [SerializeField] private TMP_Text stateText;
        [SerializeField] private float appearTime = 2f;
        [SerializeField] private float attackRange = 20f;
        [SerializeField] private float runRange = 10f;

        [SerializeField] Transform muzzleALeftPosition;
        [SerializeField] Transform muzzleARightPosition;
        [SerializeField] Transform muzzleBLeftPosition;
        [SerializeField] Transform muzzleBRightPosition;
        [SerializeField] private float fireCoolTime = 0.5f;

        private Vector3 moveDir;
        private bool reload = true;
        private bool isFireA = true;

        protected override void Awake()
        {
            base.Awake();

            stateMachine = new StateMachine<State, Enemy_Sniper>(this);
            stateMachine.AddState(State.Appear, new AppearState(this, stateMachine));
            stateMachine.AddState(State.Idle, new IdleState(this, stateMachine));
            stateMachine.AddState(State.MoveForward, new MoveForwardState(this, stateMachine));
            stateMachine.AddState(State.MoveBackward, new MoveBackwardState(this, stateMachine));
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

        private void SearchPlayer()
        {
            target = GameManager.Enemy.SearchPlayer();
        }

        protected override IEnumerator NavDestinationCoroutine(bool isRun = false)
        {
            agent.isStopped = false;
            while (true)
            {
                SearchPlayer();
                if (target != null)
                {
                    if (isRun)
                        agent.destination = transform.position + (transform.position - target.position);
                    else
                        agent.destination = target.position;
                }

                yield return new WaitForSeconds(0.2f);
            }
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
        }

        IEnumerator FireCoroutine()
        {
            while (reload)
            {
                reload = false;
                photonView.RPC("Fire", RpcTarget.AllViaServer);
                yield return new WaitForSeconds(fireCoolTime);
                reload = true;
            }
        }

        [PunRPC]
        public void Fire()
        {
            if (debug)
                Debug.Log("Fire");

            if (isFireA)
            {
                Vector3 dirL = ((target.position + Vector3.up * 1.666f) - muzzleALeftPosition.position).normalized;
                Vector3 dirR = ((target.position + Vector3.up * 1.666f) - muzzleARightPosition.position).normalized;
                GameManager.Resource.Instantiate<GameObject>("Prefabs/EnemyBullets/EnemyLaser", muzzleALeftPosition.position, Quaternion.LookRotation(dirL), true);
                GameManager.Resource.Instantiate<GameObject>("Prefabs/EnemyBullets/EnemyLaser", muzzleARightPosition.position, Quaternion.LookRotation(dirR), true);
                anim.SetTrigger("OnFireA");
                isFireA = false;
            }
            else
            {
                Vector3 dirL = ((target.position + Vector3.up * 1.666f) - muzzleBLeftPosition.position).normalized;
                Vector3 dirR = ((target.position + Vector3.up * 1.666f) - muzzleBRightPosition.position).normalized;
                GameManager.Resource.Instantiate<GameObject>("Prefabs/EnemyBullets/EnemyLaser", muzzleBLeftPosition.position, Quaternion.LookRotation(dirL), true);
                GameManager.Resource.Instantiate<GameObject>("Prefabs/EnemyBullets/EnemyLaser", muzzleBRightPosition.position, Quaternion.LookRotation(dirR), true);
                anim.SetTrigger("OnFireB");
                isFireA = true;
            }
        }

        [PunRPC]
        public void SetFloatAnimSpeed(int animSpeed)
        {
            anim.SetFloat("AnimSpeed", animSpeed);
        }

        [PunRPC]
        public void SetBoolMove(bool isWalk)
        {
            anim.SetBool("IsWalk", isWalk);
        }

        #region State
        private abstract class Enemy_SniperState : StateBase<State, Enemy_Sniper>
        {
            protected GameObject gameObject => owner.gameObject;
            protected Transform transform => owner.transform;
            protected Rigidbody rb => owner.rb;
            protected Animator anim => owner.anim;
            protected Collider collider => owner.collider;

            protected Enemy_SniperState(Enemy_Sniper owner, StateMachine<State, Enemy_Sniper> stateMachine) : base(owner, stateMachine)
            {
            }
        }

        private class AppearState : Enemy_SniperState
        {
            private float curTime;

            public AppearState(Enemy_Sniper owner, StateMachine<State, Enemy_Sniper> stateMachine) : base(owner, stateMachine)
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

        private class IdleState : Enemy_SniperState
        {
            public IdleState(Enemy_Sniper owner, StateMachine<State, Enemy_Sniper> stateMachine) : base(owner, stateMachine)
            {
            }

            public override void Setup()
            {

            }

            public override void Enter()
            {
                owner.photonView.RPC("SetFloatAnimSpeed", RpcTarget.AllViaServer, 0);
                owner.stateText.text = "Idle";
            }

            public override void Update()
            {
                owner.SearchPlayer();
                if (owner.target != null)
                {
                    owner.moveDir = owner.target.transform.position - transform.position;
                    owner.squareDistanceToTarget = owner.SquareDistanceToTarget(owner.moveDir);
                }
            }

            public override void Transition()
            {
                if (owner.target != null)
                {
                    if (owner.squareDistanceToTarget <= (owner.runRange * owner.runRange))
                        stateMachine.ChangeState(State.MoveBackward);
                    else
                        stateMachine.ChangeState(State.MoveForward);
                }
            }

            public override void Exit()
            {
                owner.photonView.RPC("SetFloatAnimSpeed", RpcTarget.AllViaServer, 1);
            }
        }

        private class MoveForwardState : Enemy_SniperState
        {
            public MoveForwardState(Enemy_Sniper owner, StateMachine<State, Enemy_Sniper> stateMachine) : base(owner, stateMachine)
            {
            }

            public override void Setup()
            {

            }

            public override void Enter()
            {
                owner.StartCoroutine(owner.NavDestinationCoroutine());
                owner.photonView.RPC("SetBoolMove", RpcTarget.AllViaServer, true);
                owner.stateText.text = "Forward";
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

                if (owner.squareDistanceToTarget <= (owner.attackRange * owner.attackRange))
                    stateMachine.ChangeState(State.Attack);
            }

            public override void Exit()
            {
                owner.agent.isStopped = true;
                owner.StopAllCoroutines();
                owner.photonView.RPC("SetBoolMove", RpcTarget.AllViaServer, false);
            }
        }

        private class MoveBackwardState : Enemy_SniperState
        {
            public MoveBackwardState(Enemy_Sniper owner, StateMachine<State, Enemy_Sniper> stateMachine) : base(owner, stateMachine)
            {
            }

            public override void Setup()
            {

            }

            public override void Enter()
            {
                owner.StartCoroutine(owner.NavDestinationCoroutine(true));
                owner.photonView.RPC("SetFloatAnimSpeed", RpcTarget.AllViaServer, -1);
                owner.photonView.RPC("SetBoolMove", RpcTarget.AllViaServer, true);
                owner.stateText.text = "Backward";
            }

            public override void Update()
            {
                owner.SearchPlayer();
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

                if (owner.squareDistanceToTarget > (owner.runRange * owner.runRange))
                    stateMachine.ChangeState(State.Attack);
            }

            public override void Exit()
            {
                owner.agent.isStopped = true;
                owner.StopAllCoroutines();
                owner.photonView.RPC("SetBoolMove", RpcTarget.AllViaServer, false);
                owner.photonView.RPC("SetFloatAnimSpeed", RpcTarget.AllViaServer, 1);
            }
        }

        private class AttackState : Enemy_SniperState
        {
            public AttackState(Enemy_Sniper owner, StateMachine<State, Enemy_Sniper> stateMachine) : base(owner, stateMachine)
            {
            }

            public override void Setup()
            {

            }

            public override void Enter()
            {
                owner.reload = true;
                owner.StartCoroutine(owner.FireCoroutine());
                owner.stateText.text = "Attack";
            }

            public override void Update()
            {
                owner.SearchPlayer();
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

                if (owner.squareDistanceToTarget <= (owner.runRange * owner.runRange))
                    stateMachine.ChangeState(State.MoveBackward);

                if (owner.squareDistanceToTarget > (owner.attackRange * owner.attackRange))
                    stateMachine.ChangeState(State.MoveForward);
            }

            public override void Exit()
            {
                owner.StopAllCoroutines();
            }
        }

        private class DieState : Enemy_SniperState
        {
            private float curTime;

            public DieState(Enemy_Sniper owner, StateMachine<State, Enemy_Sniper> stateMachine) : base(owner, stateMachine)
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