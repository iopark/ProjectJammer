using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Darik
{
    public class Enemy : MonoBehaviourPun, IHittable
    {
        protected Rigidbody rb;
        protected Animator anim;
        protected new Collider collider;

        protected Transform target = null;
        protected int curHp;
        protected bool isDie = false;

        protected virtual void Awake()
        {
            rb = GetComponent<Rigidbody>();
            anim = GetComponentInChildren<Animator>();
            collider = GetComponent<Collider>();
        }

        protected virtual void OnEnable()
        {
            isDie = false;
        }

        public void TakeDamage(int damage, Vector3 hitPoint, Vector3 normal)
        {
            photonView.RPC("Hit", RpcTarget.All, damage, hitPoint, normal);
        }

        [PunRPC]
        protected virtual void Hit(int damage, Vector3 hitPoint, Vector3 normal)
        {
            curHp -= damage;

            ParticleSystem hitEffect = GameManager.Resource.Load<ParticleSystem>("Prefabs/Effects/HitEffect");
            GameManager.Resource.Instantiate(hitEffect, hitPoint, Quaternion.LookRotation(normal), true);
        }
    }
}
