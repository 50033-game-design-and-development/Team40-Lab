using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public int deathCount = 0;

    public GameObject throwerPrefab;
    public Transform throwerSpawnPoint;
    public Transform[] throwerPath;
    public bool throwerSpawned = false;
    public Transform marioTransform;

    void Awake()
    {
        if (Instance == null) { Instance = this; DontDestroyOnLoad(gameObject); }
        else Destroy(gameObject);
    }

    public void RegisterDeath()
    {
        deathCount++;
    }

    public void TrySpawnThrower()
    {
        if (deathCount >= 3 && !throwerSpawned && throwerPrefab && throwerSpawnPoint)
        {
            deathCount = 0;
            // spawn thrower
            var go = Instantiate(throwerPrefab, marioTransform.position + Vector3.up * 4f, Quaternion.identity);
            // var mover = go.GetComponent<PathMover>();
            go.GetComponent<FollowShoot>().target = marioTransform;
            // if (mover != null) mover.waypoints = throwerPath;
            throwerSpawned = true;
        }


    }
}
