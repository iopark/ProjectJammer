using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Darik
{
    public class HitEffect : MonoBehaviour
    {
        private float curTime;

        private void Update()
        {
            curTime += Time.deltaTime;
            if (curTime > 1f)
                GameManager.Resource.Destroy(gameObject);
        }

        private void OnEnable()
        {
            curTime = 0f;
        }
    }
}
