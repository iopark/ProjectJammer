using UnityEngine;

namespace LDW
{
    public class GameManager : MonoBehaviour
    {
        private static GameManager instance;
        private static DataManager dataManager;
        private static PoolManager poolManager;
        private static ResourceManager resourceManager;
        private static UIManager uiManager;

        public static GameManager Instance { get { return instance; } }
        public static DataManager Data { get { return dataManager; } }
        public static PoolManager Pool { get { return poolManager; } }
        public static ResourceManager Resource { get { return resourceManager; } }
        public static UIManager UI { get { return uiManager; } }

        private void Awake()
        {
            if (instance != null)
            {
                Destroy(this);
                return;
            }

            instance = this;
            DontDestroyOnLoad(this);
            InitManagers();
        }

        private void OnDestroy()
        {
            if (instance == this)
                instance = null;
        }

        private void InitManagers()
        {
            // ResourceManager 를 우선순위로
            GameObject resourceObj = new GameObject();
            resourceObj.name = "ResourceManager";
            resourceObj.transform.parent = transform;
            resourceManager = resourceObj.AddComponent<ResourceManager>();

            GameObject poolObj = new GameObject();
            poolObj.name = "PoolManager";
            poolObj.transform.parent = transform;
            poolManager = poolObj.AddComponent<PoolManager>();

            GameObject dataObj = new GameObject();
            dataObj.name = "DataManager";
            dataObj.transform.parent = transform;
            dataManager = dataObj.AddComponent<DataManager>();
            dataManager.Init();

            GameObject uiObj = new GameObject();
            uiObj.name = "UIManager";
            uiObj.transform.parent = transform;
            uiManager = uiObj.AddComponent<UIManager>();
        }
    }
}