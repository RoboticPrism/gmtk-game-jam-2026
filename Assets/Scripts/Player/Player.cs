using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player singleton;

    public PlayerMovement playerMovement;
    public PlayerInventory playerInventory;
    public PlayerAnimator playerAnimator;

    public void Awake()
    {
        if(singleton)
        {
            Debug.LogError("Another player already exists!");
        }
        else
        {
            singleton = this;
        }
    }
}
