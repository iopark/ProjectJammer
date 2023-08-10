using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ildoo;

namespace Darik
{
    public class EnemyBullet : MonoBehaviour
    {
        [SerializeField] private bool debug;
        [SerializeField] private TrailRenderer trailRenderer;
        [SerializeField] LayerMask ignoreLayerMask;
        [SerializeField] private int damage;
        [SerializeField] private float moveSpeed = 1f;

        private bool isDestroyed = false;
        private float curTime;

        private void OnEnable()
        {
            isDestroyed = false;
            curTime = 0f;
        }

        private void Update()
        {
            transform.Translate(transform.forward * moveSpeed * Time.deltaTime, Space.World);
            curTime += Time.deltaTime;
            if (curTime > 3f)
                DestroySelf();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (ignoreLayerMask.Contain(other.gameObject.layer))
                return;

            if (debug)
                Debug.Log("Collided");

            other.gameObject.GetComponent<IHittable>()?.TakeDamage(damage, transform.position, transform.forward);
            DestroySelf();
        }

        private void DestroySelf()
        {
            if (!isDestroyed)
            {
                isDestroyed = true;
                trailRenderer.Clear();
                GameManager.Resource.Destroy(gameObject);
            }
        }
    }
}
