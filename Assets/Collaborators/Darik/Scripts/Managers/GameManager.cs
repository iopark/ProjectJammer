using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;
    private static ResourceManager resourceManager;
    private static PoolManager poolManager;
    private static Darik.SpawnManager spawnManager;
    private static LDW.DataManager dataManager;
    private static LDW.UIManager UIManager;

    public static GameManager Instance { get { return instance; } }
    public static ResourceManager Resource { get { return resourceManager; } }
    public static PoolManager Pool { get { return poolManager; } }
    public static Darik.SpawnManager Spawn { get { return spawnManager; } }
    public static LDW.DataManager Data { get { return dataManager; } }
    public static LDW.UIManager UI { get { return UIManager; } }

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(this);
            return;
        }

        DontDestroyOnLoad(this);
        instance = this;
        InitManagers();
    }

    private void OnDestroy()
    {
        if (instance == this)
            instance = null;
    }

    private void InitManagers()
    {
        GameObject resourceObj = new GameObject() { name = "ResourceManager" };
        resourceObj.transform.SetParent(transform);
        resourceManager = resourceObj.AddComponent<ResourceManager>();

        GameObject poolObj = new GameObject() { name = "PoolManager" };
        poolObj.transform.SetParent(transform);
        poolManager = poolObj.AddComponent<PoolManager>();

        GameObject spawnObj = new GameObject() { name = "SpawnManager" };
        spawnObj.transform.SetParent(transform);
        spawnManager = spawnObj.AddComponent<Darik.SpawnManager>();

        GameObject dataObj = new GameObject() { name = "DataManager" };
        dataObj.transform.SetParent(transform);
        dataManager = dataObj.AddComponent<LDW.DataManager>();

        GameObject UIObj = new GameObject() { name = "UIManager" };
        UIObj.transform.SetParent(transform);
        UIManager = dataObj.AddComponent<LDW.UIManager>();
    }
}
