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
    }

    private void OnDestroy() {
        startbutton.onClick.RemoveListener(StartGame);
    }

    private void StartGame() {
        Debug.Log("Start Game");
        SceneManager.LoadScene("OutdoorsScene");
        SceneManager.SetActiveScene(SceneManager.GetSceneByName("OutdoorsScene"));
    }
}
