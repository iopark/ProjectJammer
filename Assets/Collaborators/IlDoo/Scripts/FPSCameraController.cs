using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ildoo
{
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
            Cursor.lockState = CursorLockMode.Locked;
            //lock back to the player's sight location; 
        }

        private void OnDisable()
        {
            //freeroaming camera 
            //temporailty disable camera stacking 
        }

        private void LateUpdate()
        {
            Look();
        }

        private void Look()
        {
            yRotation += lookDelta.x * mouseSensitivity * Time.deltaTime;
            xRotation -= lookDelta.y * mouseSensitivity * Time.deltaTime;
            xRotation = Mathf.Clamp(xRotation, -80f, 80f);

            cameraRoot.localRotation = Quaternion.Euler(xRotation, 0, 0);
            transform.localRotation = Quaternion.Euler(0, yRotation, 0);

            Vector3 setAimTarget = _camera.transform.position + _camera.transform.forward * 50;
            aimTarget.position = setAimTarget;
        }

        private void OnLook(InputValue value)
        {
            lookDelta = value.Get<Vector2>();
        }
    }
}

