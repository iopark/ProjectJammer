using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LDW
{
    public class LobbyCameraMove : MonoBehaviour
    {
        public Transform targetPosition;

        public float smoothTime = 0.3f;

        private Vector3 velocity = Vector3.zero;

        public bool isActive = false;

        public void MoveStart()
        {
            isActive = true;
        }

        private void Update()
        {
            if (isActive)
            {
                Camera.main.transform.position = Vector3.SmoothDamp(Camera.main.transform.position, targetPosition.position, ref velocity, smoothTime);

                if (Vector3.Distance(targetPosition.position, Camera.main.transform.position) < 0.1f)
                {
                    isActive = false;
                }
            }
        }
    }
}