using UnityEngine;

namespace LDW
{
    public class PlayerGameSceneUI : MonoBehaviour
    {
        [SerializeField] PlayerStatus playerStatus;
        [SerializeField] WeaponStat weaponStat;

        public void GameSceneUIUpdate()
        {
            playerStatusUIUpdate();
            weaponStatUIUpdate();
        }

        private void playerStatusUIUpdate()
        {
            playerStatus.UpdatePlayerStatusUI();
        }

        private void weaponStatUIUpdate()
        {
            weaponStat.UpdateWeponStatUI();
        }
    }
}
