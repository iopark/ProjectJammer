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
        public Vector3 camCentrePoint;
        public Vector3 camCentreForward; 
        Camera _camera;


        [Header("Relating to the rotation")]
        private Vector2 lookDelta;
        private float xRotation;
        private float yRotation;

        private void Awake()
        {
            camCentreForward = Vector3.zero; 
            camCentrePoint = Vector3.zero;
            if (!photonView.IsMine)
                return;
            SetMainCamPos(); 
        }

        private void Start()
        {

        }
        private void SetMainCamPos()
        {
            _camera = Camera.main;
            _camera.transform.position = cameraRoot.position;
            _camera.transform.rotation = cameraRoot.rotation;
            _camera.transform.SetParent(cameraRoot);
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
            Cursor.lockState = CursorLockMode.None;
        }

        private void LateUpdate()
        {
            if (!photonView.IsMine)
                return;
            Look();
        }
        Vector3 camFOVCentre = new Vector3(0.5f, 0.5f, 0); 

        private void Look()
        {
            yRotation += lookDelta.x * mouseSensitivity * Time.deltaTime;
            xRotation -= lookDelta.y * mouseSensitivity * Time.deltaTime;
            xRotation = Mathf.Clamp(xRotation, -80f, 80f);

            cameraRoot.localRotation = Quaternion.Euler(xRotation, 0, 0);
            transform.localRotation = Quaternion.Euler(0, yRotation, 0);

            Vector3 setAimTarget = _camera.transform.position + _camera.transform.forward * 50;
            aimTarget.position = setAimTarget;
            camCentrePoint = _camera.ViewportToWorldPoint(camFOVCentre);
            camCentreForward = _camera.transform.forward; 
        }

        private void OnLook(InputValue value)
        {
            lookDelta = value.Get<Vector2>();
        }
    }
}

