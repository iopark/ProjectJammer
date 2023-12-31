using UnityEngine;
using Photon.Pun; 
using Photon.Realtime; 

namespace ildoo
{
    public class GunData : MonoBehaviour
    {
        [SerializeField] int maxAmmo;
        public int MaxAmmo => maxAmmo;

        [SerializeField] int totalAmmo; 
        public int TotalAmmo => totalAmmo;
        //This later requires changes, (ammo into mag, total ammo into ammo). 
        
        [SerializeField] int damage;
        public int Damage => damage;

        [SerializeField] int maxDistance;
        public int MaxDistance => maxDistance;

        [SerializeField] float fireRate;
        public float FireRate => fireRate;

        [SerializeField] float reloadRate;
        public float ReloadRate => reloadRate;
    }
}

