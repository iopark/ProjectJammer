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
        progress.text = $"���൵ : {GameManager.Data.DisruptorProgress} %";
        GameManager.Data.disruptorUpdate += UpdateProgressUI; 
    }

    public void UpdateProgressUI(int value)
    {
        if (value > 100)
            value = 100;

        progress.text = $"���൵ : {value} %";
    }
}
