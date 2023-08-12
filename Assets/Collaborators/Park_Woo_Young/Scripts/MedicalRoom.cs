using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MedicalRoom : MonoBehaviourPunCallbacks
{
    [SerializeField] GameObject rr;
    [SerializeField] GameObject room;

    private Vector3 vector3;

    private void SetMedicalRoom()
    {
        //GameManager.Data.MedicalRoom = this.transform;
        if (Input.GetKeyDown(KeyCode.E))
        {
            rr.transform.position = room.transform.position;
        }
        

    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        SetMedicalRoom();
    }
}
