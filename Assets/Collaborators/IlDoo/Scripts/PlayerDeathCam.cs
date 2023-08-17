using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ildoo
{
    public class PlayerDeathCam : MonoBehaviour
    {
        Camera _camera; 
        PlayerInput playerInput;
        ildoo.Player player;
        [SerializeField] float moveSpeed; 
        [SerializeField] float mouseSensitivity;
        private float xRotation;
        private float yRotation;
        Vector3 heightValue; 
        Vector3 moveDir; 
        Vector2 lookDelta;

        Vector3 elevateVector;
        Vector3 dropVector; 
        private void Awake()
        {
            playerInput = GetComponent<PlayerInput>();
            playerInput.enabled = false;
            elevateVector = new Vector3(0, 1f, 0);
            dropVector = new Vector3(0, -1f, 0); 
        }

        private void Start()
        {
            GameManager.Data.GameOver += UnLockCursor;
        }

        private void OnDisable()
        {
            GameManager.Data.GameOver -= UnLockCursor;
        }

        private void LateUpdate()
        {
            if (!playerInput.enabled)
                return;
            Look();
            Move();
            HeightControl();
        }

        private void OnLook(InputValue value)
        {
            lookDelta = value.Get<Vector2>();
        }

        private void OnMove(InputValue value)
        {

            Vector2 input = value.Get<Vector2>();

            moveDir = new Vector3(input.x, 0, input.y);
        }

        private void OnElevate(InputValue value)
        {
            if (value.isPressed)
                heightValue = moveDir + elevateVector;
            else
                heightValue = Vector3.zero; 
        }

        private void OnDrop(InputValue value)
        {
            if (value.isPressed)
                heightValue = moveDir + dropVector;
            else
                heightValue = Vector3.zero;
        }

        private void HeightControl()
        {
            transform.Translate(heightValue * moveSpeed * Time.deltaTime, Space.Self);
        }

        public void ActivateUponDeath()
        {
            Cursor.lockState = CursorLockMode.Locked;
            playerInput.enabled = true;
        }

        public void UnLockCursor()
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        private void Look()
        {
            yRotation += lookDelta.x * mouseSensitivity * Time.deltaTime;
            xRotation -= lookDelta.y * mouseSensitivity * Time.deltaTime;
            xRotation = Mathf.Clamp(xRotation, -90f, 90f);
            transform.localRotation = Quaternion.Euler(xRotation, yRotation, 0); 
        }

        private void Move()
        {
            transform.Translate(moveDir * moveSpeed * Time.deltaTime, Space.Self); 
        }
    }
}

