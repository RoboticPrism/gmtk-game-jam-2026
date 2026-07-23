using UnityEngine;
using System.Collections;

public class DestroyEffect : MonoBehaviour
{
    [SerializeField]
    private AudioClip destroyClip;

    private AudioSource audioSource;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.PlayOneShot(destroyClip);
        StartCoroutine(DestroyAnimation());
    }

    IEnumerator DestroyAnimation()
    {
        yield return new WaitForSeconds(1f);
        Destroy(gameObject);
    }
}
