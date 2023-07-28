using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisruptorRotate : MonoBehaviour
{
    [SerializeField] float turnSpeed;
    private void Update()
    {
        transform.Rotate(Vector3.back, turnSpeed * Time.deltaTime);
    }
}
