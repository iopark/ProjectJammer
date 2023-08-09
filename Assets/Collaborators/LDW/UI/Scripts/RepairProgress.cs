using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace LDW
{
    public class RepairProgress : MonoBehaviour
    {
        [SerializeField] TMP_Text repairProgress;

        public void UpdateProgressUI(int progress)
        {
            progress = GameManager.Data.disruptorProgress;

            repairProgress.text = $"Progress : {progress}  /  100";
        }
    }
}