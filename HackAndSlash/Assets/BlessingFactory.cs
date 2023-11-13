using UnityEngine;

public class BlessingFactory : MonoBehaviour
{
    public static BlessingFactory Instance { get; private set; }

    public GameObject[] playerBlessings;
    //public BoxCollider damageCollider;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        //damageCollider = GetComponent<BoxCollider>();
    }

    public void SpawnLightAttackBlessing(Vector3 spawnPosition, Quaternion spawnQuaternion)
    {
        GameObject lightAttackBlessing = Instantiate(playerBlessings[0], spawnPosition, spawnQuaternion);
    }

    public void SpawnHeavyAttackBlessing(Vector3 spawnPosition, Quaternion spawnQuaternion)
    {
        GameObject lightAttackBlessing = Instantiate(playerBlessings[1], spawnPosition, spawnQuaternion);
    }
}
