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
    }

    public void UpdateProgressUI()
    {
        progress.text = $"진행도 : {GameManager.Data.disruptorProgress} %";
    }
}
