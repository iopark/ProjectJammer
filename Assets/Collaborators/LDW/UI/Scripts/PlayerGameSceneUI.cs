using System.Collections;
using System.Collections.Generic;
using UnityEditor.EditorTools;
using UnityEngine;

namespace LDW
{
    public class PlayerGameSceneUI : MonoBehaviour
    {
        [SerializeField] PlayerStatus playerStatus;
        [SerializeField] GameObject weaponStat;
        [SerializeField] GameObject teamStats;
        [SerializeField] RepairProgress repairProgress;

        public void GameSceneUIUpdate()
        {
            repairProgress.UpdateProgressUI(GameManager.Data.disruptorProgress);
            playerStatus.UpdatePlayerStatusUI();
        }

        private void progressUIUpdate()
        {
            repairProgress.UpdateProgressUI(GameManager.Data.disruptorProgress);
        }

        private void playerStatusUIUpdate()
        {
            playerStatus.UpdatePlayerStatusUI();
        }
    }
}
