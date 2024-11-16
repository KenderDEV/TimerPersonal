using UnityEngine;
using System;
using Unity.VisualScripting;
using UnityEngine.UI;
using TMPro;
[Serializable]

public class Timer : MonoBehaviour
{
    public void getData()
    {
        data = GetComponentInChildren<TextMeshProUGUI>();
    }
    public void getBtn()
    {
        eliminate = GetComponentInChildren<Button>();
    }
    public TextMeshProUGUI data { get; set; }
    public Button eliminate { get; set; }
    public void Set()
    {
        next = second;
    }
    public int ID { get; set; }
    public int second { get; set; }
    public int next { get; set; }

}
