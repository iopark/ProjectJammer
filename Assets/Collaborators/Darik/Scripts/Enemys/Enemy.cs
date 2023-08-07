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

            ParticleSystem hitEffect = GameManager.Resource.Load<ParticleSystem>("Prefabs/Effects/HitEffect");
            GameManager.Resource.Instantiate(hitEffect, hitPoint, Quaternion.LookRotation(normal), true);
        }
    }
}
