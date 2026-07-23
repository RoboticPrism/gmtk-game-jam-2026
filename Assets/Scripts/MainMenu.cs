using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public Button startbutton;
    void Start()
    {
        startbutton.onClick.AddListener(StartGame);
        Bootstrap.Instance.Audio.PlayMusic();
    }

    private void OnDestroy() {
        startbutton.onClick.RemoveListener(StartGame);
    }

    private void StartGame() {
        Debug.Log("Start Game");
        Bootstrap.Instance.Audio.PlayButtonSFX();
        Bootstrap.Instance.Audio.StopMusic();
        SceneManager.LoadScene("OutdoorsScene");
    }
}
