using UnityEditor;
using UnityEngine;

namespace STARTING
{
    [CreateAssetMenu(fileName = "SceneData", menuName = "STARTING/SceneData", order = 1)]
    public class SceneData : ScriptableObject
    {
        [System.Serializable]
        public class SceneInfo
        {
            [Header("실제 씬 이름")]
            public string sceneName;

            [Header("변수 이름")]
            public string sceneIdentifier;
        }

        public SceneInfo[] scenes;

        public string GetSceneName(string identifier)
        {
            foreach (var scene in scenes)
            {
                if (scene.sceneIdentifier == identifier)
                {
                    return scene.sceneName;
                }
            }
            return null;
        }

//에디터에서 빌드 세팅에 있는 씬을 모두 불러와서 SceneData asset에 추가하는 코드
#if UNITY_EDITOR
        public void LoadScenesFromBuildSettings()
        {
            int sceneCount = EditorBuildSettings.scenes.Length;

            foreach (var buildScene in EditorBuildSettings.scenes)
            {
                string sceneName = System.IO.Path.GetFileNameWithoutExtension(buildScene.path);
                if (!IsSceneNameExists(sceneName))
                {
                    AddScene(sceneName);
                }
            }
        }

        private bool IsSceneNameExists(string sceneName)
        {
            foreach (var scene in scenes)
            {
                if (scene.sceneName == sceneName)
                {
                    return true;
                }
            }
            return false;
        }

        private void AddScene(string sceneName)
        {
            SceneInfo newScene = new SceneInfo
            {
                sceneName = sceneName,
                sceneIdentifier = "Scene_" + scenes.Length
            };

            System.Array.Resize(ref scenes, scenes.Length + 1);
            scenes[scenes.Length - 1] = newScene;
        }
#endif

    }
}