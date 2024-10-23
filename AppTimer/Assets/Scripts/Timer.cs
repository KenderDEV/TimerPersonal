using UnityEngine;
using System;
using Unity.VisualScripting;
using UnityEngine.UI;
using TMPro;
[Serializable]

public class Timer : MonoBehaviour
{
    void Start()
    {
        next = second;
    }
    public int ID { get; set; }
    public int second { get; set; }
    public int next { get; set; }

}
