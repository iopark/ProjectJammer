using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ildoo
{
    //Build under assumption such would be generated using RoomObject. 
    public class Item : MonoBehaviourPun, IConsumable
    {
        public virtual void Consume(GameObject target)
        {

        }
    }
}

