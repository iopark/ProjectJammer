using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ildoo
{
    public interface IConsumable
    {
        public abstract void Consume(GameObject target); 
    }
}

