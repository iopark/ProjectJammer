using UnityEngine;

namespace LDW
{
    public class PlayerGameSceneUI : SceneUI
    {
        [SerializeField] PlayerStatus playerStatus;
        [SerializeField] WeaponStat weaponStat;
        [SerializeField] Progress progress;

        protected override void Awake()
        {
            base.Awake();
        }

        public void GameSceneUIUpdate()
        {
            playerStatusUIUpdate();
            weaponStatUIUpdate();
        }

        // �÷��̾� Status UI ����
        private void playerStatusUIUpdate()
        {
            playerStatus.UpdatePlayerStatusUI();
        }

        // źâ UI ����
        private void weaponStatUIUpdate()
        {
            weaponStat.UpdateWeponStatUI();
        }
        
        // ������ ���൵ ����
        public void ProgressUIUpdate()
        {
            progress.UpdateProgressUI();
        }
    }
}
