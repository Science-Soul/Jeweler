using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PSPooler : MonoBehaviour
{
    private static PSPooler _instance;
    public static PSPooler Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<PSPooler>();

                if (_instance == null)
                {
                    GameObject managerPrefab = Resources.Load<GameObject>("Prefabs/ParticlePoolManager");
                    if (managerPrefab != null)
                    {
                        GameObject managerObject = Instantiate(managerPrefab);
                        _instance = managerObject.GetComponent<PSPooler>();
                    }
                    else
                    {
                        Debug.LogError("ParticlePoolManager Prefab not found in Resources/Prefabs/ParticlePoolManager !");
                    }
                }
            }
            return _instance;
        }
    }

    public GameObject particlePrefab;
    const int POOL_SIZE = 100;
    public Transform poolContainerParent;

    private Queue<GameObject> availableObjects = new Queue<GameObject>();
    private bool isInitialized = false;

    void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
            return;
        }

        if (_instance == null)
        {
            _instance = this;
        }

        DontDestroyOnLoad(this.gameObject);

        if (!isInitialized)
        {
            InitializePool();
        }
    }

    private void InitializePool()
    {
        if (poolContainerParent == null)
        {
            GameObject container = new GameObject("___PARTICLE_POOL_CONTAINER___");
            container.transform.SetParent(this.transform);
            poolContainerParent = container.transform;
        }

        for (int i = 0; i < POOL_SIZE; i++)
        {
            AddObjectToPool();
        }
        isInitialized = true;
    }

    private void AddObjectToPool()
    {
        GameObject obj = Instantiate(particlePrefab, poolContainerParent);
        obj.SetActive(false);
        availableObjects.Enqueue(obj);
    }

    public GameObject SpawnParticle(Vector3 position)
    {
        GameObject objectToSpawn = availableObjects.Dequeue();

        objectToSpawn.transform.position = position;
        objectToSpawn.SetActive(true);

        return objectToSpawn;
    }

    public void ReturnToPool(GameObject objectToReturn)
    {
        objectToReturn.SetActive(false);
        availableObjects.Enqueue(objectToReturn);
    }

}
