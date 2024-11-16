using UnityEngine;
using System;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using Unity.Mathematics;
using UnityEditor;
using Unity.VisualScripting;

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
    [Header("Prefab")]
    [SerializeField] GameObject prefabAlarm;
    [SerializeField] Transform parent;
    float tiempoActual = 0f;
    bool cronometroActivo = false;
    List<GameObject> pool = new List<GameObject>();


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
                CronoCopy.text = formatTimeExit(tiempoActual);
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
    string formatTimeExit(float tiempo)
    {
        int horas = Mathf.FloorToInt(tiempo / 3600);
        int minutos = Mathf.FloorToInt((tiempo % 3600) / 60);
        string time = string.Format("{0}.{1}", horas, minutos);
        return time;
    }

    void copyText()
    {
        TextEditor textEditor = new TextEditor();
        textEditor.text = CronoCopy.text;
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
        GameObject newItem = Instantiate(prefabAlarm, parent);
        Timer obj = newItem.GetComponent<Timer>();  // Ya no usa MonoBehaviour, así que new está permitido.
        obj.ID = ID;
        obj.second = Second;
        obj.getBtn();
        obj.getData();
        if (obj.data != null)
        {
            // Configura el tiempo formateado en el texto
            obj.data.text = formatTime(Second);
        }
        if (obj.eliminate != null)
        {
            // Configura el tiempo formateado en el texto
            obj.eliminate.onClick.AddListener(() => eliminar(obj.ID));
        }
        else
        {
            Debug.LogError("No se pudo encontrar TextMeshProUGUI en prefabAlarm.");
        }

        // Añadir el nuevo Timer a la lista de timers
        timers.Add(obj);

        // Actualizar la lista de ítems en la UI
        //print(timer.GetComponent<RectTransform>().anchoredPosition);
    }
    public void ActiveList(bool active)
    {
        UpdateItemList(timers);
    }
    private void UpdateItemList(List<Timer> listToDisplay = null)
    {
        Debug.Log("Iniciando UpdateItemList");

        // Usar la lista de timers actual si no se especifica otra
        if (listToDisplay == null)
        {
            listToDisplay = timers;
        }

        // Si no hay elementos en listToDisplay, vaciar y ocultar la lista visualmente
        if (listToDisplay.Count == 0)
        {
            foreach (Transform child in parent)
            {
                child.gameObject.SetActive(false);
                pool.Add(child.gameObject);  // Agregar a pool para reutilizar
            }
            return;
        }

        // Asegurarse de que el prefab esté asignado
        if (prefabAlarm == null)
        {
            Debug.LogError("El prefabAlarm no está asignado.");
            return;
        }

        int index = 0;

        // Recorrer los elementos de la lista de timers
        foreach (var timer in listToDisplay)
        {
            GameObject item;

            // Revisar si hay un objeto inactivo disponible en el pool
            if (index < parent.childCount && parent.GetChild(index).gameObject.activeSelf == false)
            {
                item = parent.GetChild(index).gameObject;
                item.SetActive(true);  // Reactivar el objeto
            }
            else if (pool.Count > 0) // Usar un objeto del pool si está disponible
            {
                item = pool[0];
                pool.RemoveAt(0);
                item.transform.SetParent(parent, false);
                item.SetActive(true);
            }
            else  // Crear un nuevo objeto si no hay en el pool
            {
                item = Instantiate(prefabAlarm, parent);
            }

            // Configurar el componente Timer de cada objeto con los datos actuales
            Timer setting = item.GetComponentInChildren<Timer>();
            if (setting != null)
            {
                setting.ID = timers.Count + 1;
                print(setting.ID);
                setting.second = timer.second;
                setting.getData();
                setting.getBtn();
                setting.data.text = timer.data.text;
                if (setting.eliminate != null)
                {
                    // Configura el tiempo formateado en el texto
                    setting.eliminate.onClick.AddListener(() => eliminar(setting.ID));
                }
                setting.Set();  // Llamar a Set si tiene lógica adicional de configuración
            }
            else
            {
                Debug.LogError("No se encontró el componente Timer en el prefabAlarm.");
            }

            index++;
        }

        // Desactivar los objetos sobrantes en la jerarquía
        for (int i = index; i < parent.childCount; i++)
        {
            GameObject child = parent.GetChild(i).gameObject;
            child.SetActive(false);
            pool.Add(child);  // Agregar al pool para su reutilización
        }

        Debug.Log("Finalizando UpdateItemList");
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
            Timer timerComponent = timers[i];

            // Verificar si el componente o el objeto no es nulo
            if (timerComponent != null && timerComponent.ID == ID)
            {
                // Destruir el objeto en la escena
                Destroy(timerComponent.gameObject);

                print($"eliminado {timerComponent.ID}");
                timers.RemoveAt(i);

                // Actualizar la lista de ítems en la UI después de eliminar
                UpdateItemList(timers);
                break;
            }
        }
    }
}




