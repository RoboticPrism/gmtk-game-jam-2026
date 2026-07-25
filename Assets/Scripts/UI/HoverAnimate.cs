using UnityEngine;

public class HoverAnimate : MonoBehaviour
{
    [SerializeField]
    private Transform hoverArea;

    [SerializeField]
    private float hoverOffset;

    [SerializeField]
    private float hoverSpeed;

    [SerializeField]
    private bool hoverDown = false;

    public void Update()
    {
        if(hoverDown)
        {
            hoverArea.transform.localPosition += Vector3.down * hoverSpeed * Time.deltaTime;
            if(hoverArea.transform.localPosition.y < 0)
            {
                hoverDown = false;
            }
        }   
        else
        {
            hoverArea.transform.localPosition += Vector3.up * hoverSpeed * Time.deltaTime;
            if(hoverArea.transform.localPosition.y > hoverOffset)
            {
                hoverDown = true;
            }
        }
    }
}
