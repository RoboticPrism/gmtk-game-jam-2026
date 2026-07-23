using UnityEngine;

public class AudioManager : MonoBehaviour
{

    [Header("Audio Sources")]
    [SerializeField] private AudioSource musicSource;
    
    [SerializeField] private AudioSource buttonSFXSource;
    

    public void PlayMusic()
    {
        musicSource.loop = true;
        musicSource.Play();
    }
    public void PlayButtonSFX()
    {
        buttonSFXSource.Play();
    }
    public void StopMusic()
    {
        musicSource.Stop();
    }
    public void StopButtonSFX()
    {
        buttonSFXSource.Stop();
    }
    
}