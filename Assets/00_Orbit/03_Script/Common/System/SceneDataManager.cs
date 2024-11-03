using UnityEngine;

namespace STARTING
{
    public class SceneDataManager : MonoBehaviour
    {
        public SceneData sceneData;

        private static SceneDataManager _instance;

        public static SceneDataManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    GameObject obj = new GameObject("SceneDataManager");
                    _instance = obj.AddComponent<SceneDataManager>();
                    DontDestroyOnLoad(obj);

                    _instance.LoadSceneData();
                }
                return _instance;
            }
        }

        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
                DontDestroyOnLoad(gameObject);

                LoadSceneData();
            }
            else if (_instance != this)
            {
                Destroy(gameObject);
            }
        }

        private void LoadSceneData()
        {
            if (sceneData == null)
            {
                sceneData = Resources.Load<SceneData>("SceneData");

                if (sceneData == null)
                {
                    Debug.LogError("SceneData.asset를 찾을 수 없음. Resources 폴더 안에 존재해야함.");
                }
            }
        }

        public static SceneData GetSceneData()
        {
            return Instance.sceneData;
        }

        public static string GetSceneName(string identifier)
        {
            if (Instance.sceneData != null)
            {
                return Instance.sceneData.GetSceneName(identifier);
            }
            return null;
        }
    }
}
