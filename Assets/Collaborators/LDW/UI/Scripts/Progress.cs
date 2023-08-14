using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Progress : MonoBehaviour
{
    [SerializeField] TMP_Text progress;

    private void Start()
    {
        progress.text = "진행도 : 0 %";
        progress.text = $"진행도 : {GameManager.Data.DisruptorProgress} %";
        GameManager.Data.disruptorUpdate += UpdateProgressUI; 
    }

    public void UpdateProgressUI(int value)
    {
        progress.text = $"진행도 : {value} %";
    }
}
