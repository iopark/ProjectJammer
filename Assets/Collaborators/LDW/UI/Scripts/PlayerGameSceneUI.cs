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

        // 플레이어 Status UI 갱신
        private void playerStatusUIUpdate()
        {
            playerStatus.UpdatePlayerStatusUI();
        }

        // 탄창 UI 갱신
        private void weaponStatUIUpdate()
        {
            weaponStat.UpdateWeponStatUI();
        }
        
        // 발전기 진행도 갱신
        public void ProgressUIUpdate()
        {
            progress.UpdateProgressUI();
        }
    }
}
