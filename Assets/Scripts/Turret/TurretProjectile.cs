using UnityEngine;

public class TurretProjectile : MonoBehaviour
{
    [SerializeField]
    private float speed;

    private Turret turret;

    private TowerDefenseGremlin target;
    private Vector3 lastTargetLocation;

    [SerializeField]
    private AudioClip shootClip;

    private AudioSource audioSource;

    public void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void Setup(Turret turret, TowerDefenseGremlin target)
    {
        this.turret = turret;
        this.target = target;
        if (shootClip)
        {
            audioSource.PlayOneShot(shootClip);
        }
    }

    private float timeElapsed = 0f;
    // Update is called once per frame
    void Update()
    {
        if(target)
        {
            lastTargetLocation = target.transform.position + new Vector3(0.5f,0.5f,0);
        }

        timeElapsed += Time.deltaTime * speed;
        transform.position = Vector3.Lerp(transform.position, lastTargetLocation, Mathf.Clamp01(timeElapsed));
        
        if(Vector3.Distance(transform.position, lastTargetLocation) < 0.1f)
        {
            if(target)
            {
                target.OnShotByTurret(turret);
            }
            Destroy(gameObject);
        }
    }
}
