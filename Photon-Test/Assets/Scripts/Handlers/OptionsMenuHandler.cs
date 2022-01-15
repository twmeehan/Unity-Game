using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionsMenuHandler : MonoBehaviour
{
    // Used to open and close various menus
    [SerializeField] private GameObject CreateRoomCanvas;
    [SerializeField] private GameObject MainMenuCanvas;
    [SerializeField] private GameObject JoinRoomCanvas;
    [SerializeField] private GameObject OptionsCanvas;

    [SerializeField] Slider slider;

    public void Start()
    {
        if (!PlayerPrefs.HasKey("musicVolume"))
        {
            PlayerPrefs.SetFloat("musicVolume", 0.0f);
        }

        slider.value = PlayerPrefs.GetFloat("musicVolume");
    }
    public void onUpdate()
    {
        PlayerPrefs.SetFloat("musicVolume", slider.value);
    }
    public void back()
    {

        CreateRoomCanvas.SetActive(false);
        JoinRoomCanvas.SetActive(false);
        MainMenuCanvas.SetActive(true);
        OptionsCanvas.SetActive(false);

    }
}
