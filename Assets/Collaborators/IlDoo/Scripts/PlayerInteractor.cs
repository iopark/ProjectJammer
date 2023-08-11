using Darik;
using ildoo;
using Park_Woo_Young;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ildoo
{
    public class PlayerInteractor : MonoBehaviourPun
    {
        [SerializeField] float distToDistruptor;
        [SerializeField] LayerMask disruptorMask;
        FPSCameraController camController;
        Camera _cameraMain;
        LineRenderer lineRenderer;

        private void Awake()
        {
            camController = GetComponent<FPSCameraController>();
            _cameraMain = Camera.main;
            lineRenderer = _cameraMain.GetComponent<LineRenderer>();
            lineRenderer.enabled = false;
        }
        Vector3 originPoint;
        Vector3 middlePoint = new Vector3(0f, 0.5f, 0);
        private void OnInteract(InputValue value)
        {
            TryInteract();
        }
        private void OnDisable()
        {
            lineRenderer.enabled = false;
        }

        RaycastHit localHit;
        private void Update()
        {
            if (!photonView.IsMine)
                return; 
            originPoint = _cameraMain.ViewportToWorldPoint(middlePoint);

            lineRenderer.SetPosition(0, gameObject.transform.position);
            if (Physics.Raycast(originPoint, _cameraMain.transform.forward, out localHit, distToDistruptor, disruptorMask))
            {
                //피격이 발생했다면, 
                lineRenderer.SetPosition(1, localHit.point);
                lineRenderer.enabled = true;
            }
            else
            {
                lineRenderer.enabled = false;
            }
        }

        private void TryInteract()
        {
            photonView.RPC("AttemptToActivate", RpcTarget.MasterClient, camController.camCentrePoint, camController.camCentreForward);
        }

        RaycastHit masterHit;
        [PunRPC]
        public void AttemptToActivate(Vector3 startPos, Vector3 startPosForward)
        {
            if (Physics.Raycast(startPos, startPosForward, out masterHit, distToDistruptor, disruptorMask))
            {
                //이펙트에 대해서 오브젝트 풀링으로 구현 
                IInteractable interactable = masterHit.transform.GetComponent<IInteractable>();
                interactable?.Interact();
            }
        }
    }
}
