using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class FPSCameraController : MonoBehaviourPun
{
    [SerializeField] float mouseSensitivity;
    [SerializeField] Transform cameraRoot;
    [SerializeField] Transform aimTarget;

    Camera _camera; 
    

    [Header("Relating to the rotation")]
    private Vector2 lookDelta;
    private float xRotation; 
    private float yRotation;

    private void Awake()
    {
        if (!photonView.IsMine)
            return; 
        _camera = Camera.main;
        _camera.transform.position = cameraRoot.position;
        _camera.transform.rotation = cameraRoot.rotation;
        _camera.transform.SetParent(cameraRoot); 
        //cameraRoot = GetComponent<Transform>(); 
    }

    private void OnEnable()
    {
        Cursor.lockState = CursorLockMode.Locked; // ���콺 Ŀ�� �߾ӿ� ���� �� ������ ���� 
    }

    private void OnDisable()
    {
        Cursor.lockState = CursorLockMode.None; // ���콺 Ŀ�� �ٽ� ������, �ٽ� ����. 
    }

    private void LateUpdate()
    {
        Look(); 
    }

    Vector3 centrePoint;
    private void Look()
    {
        yRotation += lookDelta.x * mouseSensitivity * Time.deltaTime; // y�� ���������� ȸ���� x ���� �������� ȸ�� ��Ų��. 
        xRotation -= lookDelta.y * mouseSensitivity * Time.deltaTime; // x�� ���������� ȸ���� �ԷµǴ� y���� �������� �Է� 
        xRotation = Mathf.Clamp(xRotation, -80f, 80f);

        cameraRoot.localRotation = Quaternion.Euler(xRotation,0,0);
        transform.localRotation = Quaternion.Euler(0,yRotation,0);

        //centrePoint = camera.ScreenToViewportPoint(new Vector3(0.5f, 0.5f, 0)); 
        Vector3 setAimTarget = camera.transform.position + camera.transform.forward * 50;
        aimTarget.position = setAimTarget;
    }

    private void OnLook(InputValue value)
    {
        lookDelta = value.Get<Vector2>();
    }
}
