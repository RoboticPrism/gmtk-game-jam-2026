using UnityEngine;
using System;

public abstract class BumpableTile : MonoBehaviour
{

    public event Action OnInteract;

    public virtual void OnBump() {
        OnInteract?.Invoke();
    }
}
