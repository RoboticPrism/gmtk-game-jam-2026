using UnityEngine;
using System;

public abstract class BumpableTile : MonoBehaviour
{

    public event Action OnInteract;

    [SerializeField]
    private AudioClip bumpClip;

    private AudioSource audioSource;

    public void Start()
    {
        audioSource = GetComponent<AudioSource>();    
    }

    public virtual void OnBump() {
        OnInteract?.Invoke();
        audioSource.PlayOneShot(bumpClip);
    }
}
