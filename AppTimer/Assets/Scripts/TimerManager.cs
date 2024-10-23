using UnityEngine;
using System;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using Unity.Mathematics;

public class TimerManager : MonoBehaviour
{
    [Header("Reloj")]
    [SerializeField] TextMeshProUGUI tiempoActualText;
    [Header("Cronometer")]
    [SerializeField] TextMeshProUGUI CronoText;
    [SerializeField] TextMeshProUGUI CronoCopy;

    [Header("Alarms")]
    [SerializeField] TMP_InputField hourTimer;
    [SerializeField] TMP_InputField minuteTimer;
    [SerializeField] TMP_InputField secondTimer;
    [SerializeField] GameObject parentTimers;
    [SerializeField] GameObject parentHideTimers;
    [SerializeField] Sprite trash;
    List<Timer> timers = new List<Timer>();
    [SerializeField] ScrollRect scroll;
    [Header("Sound")]
    [SerializeField] SoundManager soundManager;
    [SerializeField] GameManager style;
    float tiempoActual = 0f;
    bool cronometroActivo = false;

    void Start()
    {
        ActualizarTextoTiempo();
    }

    // Update is called once per frame
    void Update()
    {
        RelojActual();
        if (cronometroActivo)
        {
            tiempoActual += Time.deltaTime;
            ComprobarAvisos();
            ActualizarTextoTiempo();
        }

    }
    void RelojActual()
    {
        int horasActuales, minutosActuales, segundosActuales;
        horasActuales = DateTime.Now.Hour;
        minutosActuales = DateTime.Now.Minute;
        segundosActuales = DateTime.Now.Second;
        tiempoActualText.text = $"{horasActuales.ToString("D2")}:{minutosActuales.ToString("D2")}:{segundosActuales.ToString("D2")}";
    }
    public void CronoControl(int value)
    {
        switch (value)
        {
            case 0:
                cronometroActivo = true;
                break;
            case 1:
                cronometroActivo = false;
                break;
            case 2:
                cronometroActivo = false;  // El cronómetro se detiene
                tiempoActual = 0f;
                ActualizarTextoTiempo();
                break;
            case 3:
                CronoCopy.text = CronoText.text;
                cronometroActivo = false;
                break;
            case 4:
                copyText();
                break;
        }
    }

    void ComprobarAvisos()
    {
        if (timers.Count != 0)
        {
            for (int i = 0; i < timers.Count; i++)
            {
                if (timers[i].next == ((int)tiempoActual))
                {
                    soundManager.PlayAlarm();
                    print("alarm");
                    timers[i].next += timers[i].second;
                    CronoControl(1);
                }
            }
        }
    }
    void ActualizarTextoTiempo()
    {
        CronoText.text = formatTime(tiempoActual);
    }

    string formatTime(float tiempo)
    {
        int horas = Mathf.FloorToInt(tiempo / 3600);
        int minutos = Mathf.FloorToInt((tiempo % 3600) / 60);
        int segundos = Mathf.FloorToInt(tiempo % 60);
        string time = string.Format("{0:00}:{1:00}:{2:00}", horas, minutos, segundos);
        return time;
    }

    void copyText()
    {
        TextEditor textEditor = new TextEditor();
        textEditor.text = CronoText.text;
        textEditor.SelectAll();
        textEditor.Copy();
    }
    public void SetAlarm()
    {
        if (hourTimer.text == "")
        {
            hourTimer.text = "00";
        }
        if (minuteTimer.text == "")
        {
            minuteTimer.text = "00";
        }
        if (secondTimer.text == "")
        {
            secondTimer.text = "00";
        }
        int TimerComplete = ((int.Parse(hourTimer.text)) * 3600) + ((int.Parse(minuteTimer.text)) * 60) + (int.Parse(secondTimer.text));
        int ID = timers.Count + 1;
        CreateAlarm(ID, TimerComplete);


        hourTimer.text = "";
        minuteTimer.text = "";
        secondTimer.text = "";
    }
    void CreateAlarm(int ID, int Second)
    {
        GameObject timer = new GameObject("timer " + ID);
        timer.transform.SetParent(parentHideTimers.transform);
        timer.AddComponent<Timer>().ID = ID;
        timer.GetComponent<Timer>().second = Second;
        ConfigureViewAlarm(timer, Second, ID);
        timer.AddComponent<RectTransform>();
        timer.AddComponent<Image>().sprite = style.buttonSprite();
        timer.GetComponent<Image>().type = Image.Type.Sliced;
        timers.Add(timer.GetComponent<Timer>());
        UpdateListPosition();

        //print(timer.GetComponent<RectTransform>().anchoredPosition);
    }
    public void ActiveList(bool active)
    {
        if (timers.Count != 0)
        {
            if (active)
            {
                for (int i = 0; i < timers.Count; i++)
                {
                    timers[i].transform.SetParent(parentTimers.transform);
                }
            }
            else
            {
                for (int i = 0; i < timers.Count; i++)
                {
                    timers[i].transform.SetParent(parentHideTimers.transform);
                }
            }
        }
        UpdateListPosition();
    }
    void UpdateListPosition()
    {
        for (int i = 0; i < timers.Count; i++)
        {
            if (timers[i] != null && timers[i].GetComponent<RectTransform>() != null)
            {
                if (i != 0)
                {
                    timers[i].GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 150f - (i * 75));
                }
                else
                {
                    timers[i].GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 150f);
                }
                timers[i].GetComponent<RectTransform>().sizeDelta = new Vector2(400, 50);
            }
            else
            {
                Debug.LogError("RectTransform is missing or timer is null at index: " + i);
            }
        }
    }

    TextMeshProUGUI configText(TextMeshProUGUI text, int Second)
    {
        text.text = formatTime(Second);
        text.alignment = TextAlignmentOptions.Left;
        text.color = Color.white;
        text.verticalAlignment = VerticalAlignmentOptions.Middle;
        return text;
    }
    void ConfigureViewAlarm(GameObject alarm, int Second, int ID)
    {
        GameObject text = new GameObject(alarm.name + " Text");
        text.AddComponent<TextMeshProUGUI>();
        configText(text.GetComponent<TextMeshProUGUI>(), Second);
        text.GetComponent<RectTransform>().sizeDelta = new Vector2(350, 50);
        text.transform.SetParent(alarm.transform);


        GameObject button = new GameObject(alarm.name + " Button");
        button.AddComponent<Button>().onClick.AddListener(() => eliminar(ID));
        button.AddComponent<RectTransform>().sizeDelta = new Vector2(40, 40);
        button.GetComponent<RectTransform>().anchoredPosition = new Vector2(175, 0);
        button.AddComponent<Image>().sprite = trash;
        button.transform.SetParent(alarm.transform);



    }

    void eliminar(int ID)
    {
        StartCoroutine(CoolDown(ID));
    }
    IEnumerator CoolDown(int ID)
    {
        yield return new WaitForSeconds(0.5f);
        for (int i = 0; i < timers.Count; i++)
        {
            if (timers[i] != null && timers[i].GetComponent<Timer>().ID == ID)
            {
                Destroy(timers[i].gameObject);  // Destruye el objeto
                timers.RemoveAt(i);  // Elimina el objeto de la lista para que no sea accesible después
                UpdateListPosition();  // Actualiza la lista después de eliminar
                break;  // Sal del bucle una vez que encuentres y elimines el objeto
            }
            else
            {
                print("none in list");
            }
        }
    }
}




