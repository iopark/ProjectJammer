using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using ildoo;

namespace Darik
{
    public class Enemy : MonoBehaviourPun, IHittable
    {
        [SerializeField] protected bool debug;
        [SerializeField] protected int maxHp = 10;
        [SerializeField] protected LayerMask blockLayer;

        protected Rigidbody rb;
        protected Animator anim;
        protected new Collider collider;
        protected NavMeshAgent agent;
        protected Transform target;
        private RaycastHit hit;

        protected float squareDistanceToTarget;
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

        protected float SquareDistanceToTarget(Vector3 toTarget)
        {
            return (toTarget.x * toTarget.x + toTarget.y * toTarget.y + toTarget.z * toTarget.z);
        }

        protected bool CheckInOfRange(float range)
        {
            if (squareDistanceToTarget <= (range * range))
                return true;
            else
                return false;
        }

        protected bool CheckOutOfRange(float range)
        {
            if (squareDistanceToTarget > (range * range))
                return true;
            else
                return false;
        }

        protected virtual IEnumerator NavDestinationCoroutine(bool isRun = false)
        {
            agent.isStopped = false;
            while (true)
            {
                SearchTarget();
                if (target != null)
                    agent.destination = target.position;
                
                if (SquareDistanceToTarget(target.position - transform.position) < 200f)
                    yield return new WaitForSeconds(0.2f);
                else
                    yield return new WaitForSeconds(3f);
            }
        }

        protected void SearchTarget(bool isTargetPlayer = false)
        {
            if (isTargetPlayer)
                target = GameManager.Enemy.SearchPlayer();
            else
                target = GameManager.Enemy.SearchTarget();
        }

        protected bool CheckIsBlocked(float range)
        {
            if (Physics.Raycast(transform.position + Vector3.up * 1f, (target.position - transform.position) + Vector3.up * 1f, out hit, range))
            {
                if (blockLayer.Contain(hit.transform.gameObject.layer))
                    return true;
                else
                    return false;
            }
            else
                return false;
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
