using System.Collections;
using System.Collections.Generic;
using UnityEditor.EditorTools;
using UnityEngine;

namespace LDW
{
    public class PlayerGameSceneUI : MonoBehaviour
    {
        [SerializeField] PlayerStatus playerStatus;
        [SerializeField] WeaponStat weaponStat;
        [SerializeField] GameObject teamStats;
        [SerializeField] RepairProgress repairProgress;

        public void GameSceneUIUpdate()
        {
            progressUIUpdate();
            playerStatusUIUpdate();
            weaponStatUIUpdate();
        }

        private void progressUIUpdate()
        {
            repairProgress.UpdateProgressUI();
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
