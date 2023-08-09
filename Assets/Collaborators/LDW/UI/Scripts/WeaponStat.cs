using ildoo;
using TMPro;
using UnityEngine;

namespace LDW
{
    public class WeaponStat : MonoBehaviour
    {
        [SerializeField] Gun gun;
        [SerializeField] TMP_Text ammoText;

        public void UpdateWeponStatUI()
        {
            ammoText.text = $"ammo / ammo";
        }
    }
}