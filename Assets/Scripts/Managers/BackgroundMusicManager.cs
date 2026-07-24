using UnityEngine;
using System.Collections;

public class BackgroundMusicManager : MonoBehaviour
{
    public static BackgroundMusicManager singleton;

    [SerializeField]
    private AudioSource exploreMusic;

    [SerializeField]
    private AudioSource defendMusic;

    [SerializeField]
    private float fadeSpeed;

    public void Awake()
    {
        singleton = this;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        defendMusic.volume = 0; 
    }

    public void StartDefendMusic()
    {
        StartCoroutine(DefendMusicTransition());
    }

    public void StartExploreMusic()
    {
        StartCoroutine(ExploreMusicTransition());
    }

    IEnumerator DefendMusicTransition()
    {
        while(exploreMusic.volume > 0)
        {
            exploreMusic.volume -= Time.deltaTime * fadeSpeed;
            yield return null;
        }
        while(defendMusic.volume < 1)
        {
            defendMusic.volume += Time.deltaTime * fadeSpeed;
            yield return null;
        }
    }
    IEnumerator ExploreMusicTransition()
    {
        while(defendMusic.volume > 0)
        {
            defendMusic.volume -= Time.deltaTime * fadeSpeed;
            yield return null;
        }
        while(exploreMusic.volume < 1)
        {
            exploreMusic.volume += Time.deltaTime * fadeSpeed;
            yield return null;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
