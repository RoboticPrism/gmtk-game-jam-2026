using System.Collections;
using UnityEngine;

public class EnemyDefeatEffect : MonoBehaviour
{

    [SerializeField]
    private float lifetime;

    private AudioSource audioSource;

    public void Setup(AudioClip defeatClip)
    {
        audioSource = GetComponent<AudioSource>();
        if (defeatClip)
        {
            audioSource.PlayOneShot(defeatClip);
        }
        StartCoroutine(Animate());
    }

    IEnumerator Animate()
    {
        yield return new WaitForSeconds(lifetime);
        Destroy(gameObject);
    }
}
