using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Darik
{
    public class Enemy : MonoBehaviourPun, IHittable
    {
        [SerializeField] protected bool debug;
        [SerializeField] protected int maxHp = 10;

        protected Rigidbody rb;
        protected Animator anim;
        protected new Collider collider;
        protected NavMeshAgent agent;
        protected Transform target;

        protected int squareDistanceToTarget;
        protected int curHp;
        protected bool isDie = false;

        protected virtual void Awake()
        {
            rb = GetComponent<Rigidbody>();
            anim = GetComponentInChildren<Animator>();
            collider = GetComponent<Collider>();
            agent = GetComponent<NavMeshAgent>();
        }

        protected virtual void OnEnable()
        {
            curHp = maxHp;
        }

        protected void SearchTarget()
        {
            target = GameManager.Enemy.SearchTarget();
        }

        protected int SquareDistanceToTarget(Vector3 toTarget)
        {
            return (int)(toTarget.x * toTarget.x + toTarget.y * toTarget.y + toTarget.z * toTarget.z);
        }

        protected virtual IEnumerator NavDestinationCoroutine(bool isRun = false)
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

        public void TakeDamage(int damage, Vector3 hitPoint, Vector3 normal)
        {
            if (curHp > 0)
            {
                if (debug)
                    Debug.Log("Hitted");

                photonView.RPC("Hit", RpcTarget.AllViaServer, damage, hitPoint, normal);
            }
        }

        [PunRPC]
        public virtual void Hit(int damage, Vector3 hitPoint, Vector3 normal)
        {
            curHp -= damage;

            GameManager.Resource.Instantiate<GameObject>("Prefabs/Effects/HitEffect", hitPoint, Quaternion.LookRotation(normal), transform, true);
        }
    }
}
