using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace LDW
{
    public class RepairProgress : MonoBehaviour
    {
        [SerializeField] TMP_Text repairProgress;

        public int progress;

        public void UpdateProgressUI()
        {
            progress = GameManager.Data.disruptorProgress;

            repairProgress.text = $"Progress : {progress}  /  100";
        }
    }
}