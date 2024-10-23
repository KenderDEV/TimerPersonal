using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Unity.VisualScripting;
using System.Collections;


public class SoundManager : MonoBehaviour
{
    [Header("Config Style")]
    [Header("Config System")]
    [SerializeField] Slider volumeslider;
    [SerializeField] AudioSource source;
    [SerializeField] GameObject parent;
    [SerializeField] List<Sound> Sounds = new List<Sound>();
    [SerializeField] Sprite play, stop, check;
    [SerializeField] GameObject Panel;
    [SerializeField] GameManager style;

    List<GameObject> SoundsGO = new List<GameObject>(), buttonsPLay = new List<GameObject>(), buttonsSelect = new List<GameObject>();
    bool playingSound = false;
    int SelectSound;

    void Start()
    {
        CreateSounds();
        volumeslider.value = 0.5f;
    }

    public void VolumeControl()
    {
        source.volume = volumeslider.value;
    }

    public void PlayAlarm()
    {
        Panel.SetActive(true);
        source.Play();
        GameManager.BringWindowToFront();
        GameManager.FlashWindow();
    }

    public void StopAlarm()
    {
        source.Stop();
    }

    void SelectAudio(int ID)
    {
        if (ID < 1 || ID > buttonsSelect.Count) return; // Validación de rango

        for (int i = 0; i < buttonsSelect.Count; i++)
        {
            buttonsSelect[i].SetActive(i == ID - 1);
        }

        SelectSound = ID - 1;
        source.clip = Sounds[SelectSound].audio;
    }

    void Reproduc(int ID)
    {
        if (ID < 1 || ID > buttonsPLay.Count) return; // Validación de rango

        if (playingSound)
        {
            buttonsPLay[ID - 1].GetComponent<Image>().sprite = play;
            source.Pause();
            playingSound = false;
        }
        else
        {
            buttonsPLay[ID - 1].GetComponent<Image>().sprite = stop;
            source.clip = Sounds[ID - 1].audio;
            source.Play();
            playingSound = true;

            // Actualizar otros botones
            for (int i = 0; i < buttonsPLay.Count; i++)
            {
                if (i != ID - 1)
                {
                    buttonsPLay[i].GetComponent<Image>().sprite = play;
                }
            }
        }
    }

    void UpdateListPosition()
    {
        for (int i = 0; i < Sounds.Count; i++)
        {
            var rectTransform = SoundsGO[i]?.GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                rectTransform.anchoredPosition = new Vector2(0, 150f - (i * 75));
                rectTransform.sizeDelta = new Vector2(350, 60);
            }
        }
    }

    void CreateSounds()
    {
        for (int i = 0; i < Sounds.Count; i++)
        {
            int ID = i + 1;
            var SoundView = new GameObject(Sounds[i].name);
            SoundView.transform.SetParent(parent.transform);
            SoundView.AddComponent<Image>().sprite = style.buttonSprite();
            SoundView.GetComponent<Image>().type = Image.Type.Sliced;

            var SoundText = new GameObject("text " + ID);
            ConfigText(SoundText.AddComponent<TextMeshProUGUI>(), ID);
            SoundText.transform.SetParent(SoundView.transform);
            SoundText.GetComponent<RectTransform>().anchoredPosition = new Vector2(-50, 0);

            CreateButtonSound(ID, SoundView);
            CreateButtonSelect(ID, SoundView);
            SoundsGO.Add(SoundView);
        }
        UpdateListPosition();
    }

    void CreateButtonSound(int ID, GameObject SoundView)
    {
        var ButtonSound = new GameObject("ButtonSound " + ID);
        ButtonSound.transform.SetParent(SoundView.transform);
        ButtonSound.AddComponent<Button>().onClick.AddListener(() => Reproduc(ID));
        ConfigButton(ButtonSound, 150, 40);

        var ImageSound = new GameObject("ImageSound " + ID);
        ImageSound.AddComponent<Image>().sprite = play;
        ImageSound.transform.SetParent(ButtonSound.transform);
        ConfigButton(ImageSound, 0, 30);

        buttonsPLay.Add(ImageSound);
    }

    void CreateButtonSelect(int ID, GameObject SoundView)
    {
        var ButtonSelect = new GameObject("ButtonSelect " + ID);
        ButtonSelect.transform.SetParent(SoundView.transform);
        ButtonSelect.AddComponent<Button>().onClick.AddListener(() => SelectAudio(ID));
        ConfigButton(ButtonSelect, 100, 40);
        ButtonSelect.AddComponent<Image>().sprite = style.buttonSprite();
        ButtonSelect.GetComponent<Image>().type = Image.Type.Sliced;
        ButtonSelect.GetComponent<Image>().pixelsPerUnitMultiplier = 2;

        var ImageSelect = new GameObject("ImageSelect " + ID);
        ImageSelect.AddComponent<Image>().sprite = check;
        ImageSelect.transform.SetParent(ButtonSelect.transform);
        ConfigButton(ImageSelect, 0, 30);
        ImageSelect.SetActive(false);

        buttonsSelect.Add(ImageSelect);
    }

    GameObject ConfigButton(GameObject button, float Xpos, float XYScale)
    {
        if (button.GetComponent<RectTransform>() == null)
        {
            button.AddComponent<RectTransform>();
        }
        button.GetComponent<RectTransform>().anchoredPosition = new Vector2(Xpos, 0);
        button.GetComponent<RectTransform>().sizeDelta = new Vector2(XYScale, XYScale);
        return button;
    }

    TextMeshProUGUI ConfigText(TextMeshProUGUI text, int ID)
    {
        text.text = Sounds[ID - 1].name;
        text.alignment = TextAlignmentOptions.Left;
        text.color = Color.white;
        text.verticalAlignment = VerticalAlignmentOptions.Middle;
        return text;
    }
}
