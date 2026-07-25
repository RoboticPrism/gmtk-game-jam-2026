using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Bootstrap : MonoBehaviour
{
    public static Bootstrap Instance { get; private set; }
    public AudioManager Audio { get; private set; }

    public Button settingMenuButton;
    
    public GameObject settingsMenu;
    
    public Button muteButton;
    public Sprite muteOn;
    public Sprite muteOff;
    private bool isMuted = false;
    
    public Slider volumeSlider;
    private float currentVolume;
    
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        Audio = GetComponent<AudioManager>();
    }

    void Start() {
        settingMenuButton.onClick.AddListener(ToggleMenu);
        muteButton.onClick.AddListener(ToggleMute);
        volumeSlider.onValueChanged.AddListener(SetVolume);
        settingsMenu.SetActive(false);
        volumeSlider.value = AudioListener.volume;
    }
    public void ToggleMenu()
    {
        settingsMenu.SetActive(!settingsMenu.activeSelf);
    }
    public void ToggleMute()
    {
        isMuted = !isMuted;
        muteButton.image.sprite = isMuted ? muteOn : muteOff;
        AudioListener.volume = isMuted ? 0 : currentVolume;
    }

    public void SetVolume(float volume) {
        currentVolume = volume;
        if (!isMuted) {
            AudioListener.volume = currentVolume;
        }
    }
}