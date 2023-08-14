using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Progress : MonoBehaviour
{
    [SerializeField] TMP_Text progress;

    private void Start()
    {
        progress.text = "���൵ : 0 %";
    }

    public void UpdateProgressUI()
    {
        progress.text = $"���൵ : {GameManager.Data.disruptorProgress} %";
    }
}
