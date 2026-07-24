using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class TowerDefenseManager : MonoBehaviour
{
    public static TowerDefenseManager singleton;

    [SerializeField]
    public bool isTowerDefenseMode = false;

    [SerializeField]
    [Tooltip("The enemy that spawns in tower defense mode")]
    private TowerDefenseGremlin gremlinPrefab;

    private List<TowerDefenseGremlin> instantiatedGremlins = new List<TowerDefenseGremlin>();

    private List<SpawnPoint> allSpawnPoints;

    private List<SpawnPoint> recentlyUsedSpawnPoints = new List<SpawnPoint>();

    [SerializeField]
    private GameObject gameoverText;

    private int playerLightRadius;

    [System.Serializable]
    public class Wave
    {
        public int gremlinCount;
        public int gremlinSpawnDelay;
    }

    [SerializeField]
    private List<Wave> waves;

    [SerializeField]
    private Wave currentWave;

    [SerializeField]
    private int currentSpawnDelay = 0;

    private int previousPyreLightRadius = 0;

    [SerializeField]
    private int defenseModePyreLightRadius;

    public void Awake()
    {
        if (singleton)
        {
            Debug.LogError("A tower defense manager already exists!");

        }
        else {
            singleton = this;
        }
        allSpawnPoints = new List<SpawnPoint>(FindObjectsByType<SpawnPoint>());
    }

    public void BeginTowerDefenseMode()
    {
        isTowerDefenseMode = true;

        // Save and update the pyre's old light radius
        previousPyreLightRadius = Pyre.singleton.GetComponent<FogOfWarLight>().lightRadius;
        Pyre.singleton.GetComponent<FogOfWarLight>().lightRadius = defenseModePyreLightRadius;

        // Save and disable the players light radius
        playerLightRadius = Player.singleton.GetComponent<FogOfWarLight>().lightRadius;
        Player.singleton.GetComponent<FogOfWarLight>().lightRadius = 0;
        FogOfWarManager.TriggerLightingUpdate();

        // Get the wave for the night
        currentWave = waves[0];
        waves.RemoveAt(0);
    }

    public void EndTowerDefenseMode()
    {
        isTowerDefenseMode = false;

        // Restore pyre light radius
        Pyre.singleton.GetComponent<FogOfWarLight>().lightRadius = previousPyreLightRadius;

        // Reenable player's light radius 
        Player.singleton.GetComponent<FogOfWarLight>().lightRadius = playerLightRadius;
        FogOfWarManager.TriggerLightingUpdate();
    }

    public void DoTurn()
    {
        if(isTowerDefenseMode && currentWave != null)
        {
            if (currentWave.gremlinCount > 0)
            {
                if (currentSpawnDelay == 0)
                {
                    SpawnGremlin();
                    currentSpawnDelay = currentWave.gremlinSpawnDelay;
                    currentWave.gremlinCount--;
                }
                else
                {
                    currentSpawnDelay--;
                }
            }
        }
    }

    public void SpawnGremlin()
    {
        // Chose a spawn point that hasn't been used recently
        List<SpawnPoint> availableSpawnPoints = allSpawnPoints;
        availableSpawnPoints.RemoveAll(point => recentlyUsedSpawnPoints.Contains(point));
        SpawnPoint spawnPoint = availableSpawnPoints[Random.Range(0, availableSpawnPoints.Count)];
        
        // Add this point to the recently used list and remove the least recent entry
        recentlyUsedSpawnPoints.Add(spawnPoint);
        if(recentlyUsedSpawnPoints.Count > 3)
        {
            recentlyUsedSpawnPoints.RemoveAt(0);
        }

        TowerDefenseGremlin gremlin = Instantiate(gremlinPrefab, spawnPoint.transform.position, Quaternion.identity);
        instantiatedGremlins.Add(gremlin);
    }

    public void OnGremlinDefeat(TowerDefenseGremlin gremlin)
    {
        instantiatedGremlins.Remove(gremlin);

        if(instantiatedGremlins.Count == 0)
        {
            EndTowerDefenseMode();
        }
    }

    public void DamagePyre()
    {
        Pyre.singleton.health--;
        if(Pyre.singleton.health <= 0)
        {
            if(gameoverCoroutine == null)
            {
                gameoverCoroutine = StartCoroutine(GameoverAnimation());
            }
        }
    }

    private Coroutine gameoverCoroutine;
    IEnumerator GameoverAnimation()
    {
        FogOfWarLight pyreLight = Pyre.singleton.GetComponent<FogOfWarLight>();
        while (pyreLight.lightRadius > 0)
        {
            yield return new WaitForSeconds(1f);
            pyreLight.lightRadius--;
            FogOfWarManager.TriggerLightingUpdate();
        }

        yield return new WaitForSeconds(1f);

        gameoverText.SetActive(true);

        yield return new WaitForSeconds(5f);

        // warp to title screen
    }
}
