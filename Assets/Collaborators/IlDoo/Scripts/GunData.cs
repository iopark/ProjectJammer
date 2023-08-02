using UnityEngine;
using Photon.Pun; 
using Photon.Realtime; 

public class GunData : MonoBehaviour 
{
    [SerializeField] int maxAmmo; 
    public int MaxAmmo => maxAmmo; 
    [SerializeField] int damage;
    public int Damage => damage; 
    [SerializeField] int maxDistance; 
    public int MaxDistance => maxDistance; 
    [SerializeField] float fireRate; 
    public float FireRate => fireRate; 
    [SerializeField] float reloadRate; 
    public float ReloadRate => reloadRate; 
}
