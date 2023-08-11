using ildoo;
using TMPro;
using UnityEngine;

namespace LDW
{
    public class WeaponStat : MonoBehaviour
    {
        [SerializeField] Gun gun;

        [SerializeField] TMP_Text currentAmmoText;
        [SerializeField] TMP_Text maxAmmoText;

        public void UpdateWeponStatUI()
        {
            currentAmmoText.text = $"{gun.CurrentAmmo}";
            maxAmmoText.text = $"{gun.totalAmmo}";
        }
    }
}