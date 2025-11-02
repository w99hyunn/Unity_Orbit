using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;

namespace STARTING
{
    public class SceneDataEditor : EditorWindow
    {
        private SceneData sceneData;
        private const string sceneFolderPath = "Assets/00_Orbit/01_Scenes/";

        [MenuItem("STARTING/Scene Data Wizard")]
        public static void ShowWindow()
        {
            GetWindow<SceneDataEditor>("Scene Data Wizard");
        }

        private void OnEnable()
        {
            sceneData = Resources.Load<SceneData>("SceneData");
            Texture2D icon = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/00_Orbit/04_Images/GameLevel Icon.png");
            titleContent = new GUIContent("Scene Data Wizard", icon);
        }

        private void OnGUI()
        {
            sceneData = (SceneData)EditorGUILayout.ObjectField("Scene Data", sceneData, typeof(SceneData), false);

            if (sceneData == null)
            {
                EditorGUILayout.HelpBox("Scene Data not found at specified path!", MessageType.Error);
                return;
            }

            GUILayout.Label("\n씬 전환 :", EditorStyles.boldLabel);

            foreach (var sceneInfo in sceneData.scenes)
            {
                if (GUILayout.Button(sceneInfo.sceneIdentifier))
                {
                    string scenePath = sceneFolderPath + sceneInfo.sceneName + ".unity";

                    if (Application.isPlaying)
                    {
                        SceneManager.LoadScene(sceneInfo.sceneName);
                    }
                    else
                    {
                        if (System.IO.File.Exists(scenePath))
                        {
                            EditorSceneManager.OpenScene(scenePath);
                        }
                        else
                        {
                            Debug.LogError($"Scene file not found: {scenePath}");
                        }
                    }
                }
                GUILayout.Space(10);
            }

            GUILayout.Space(20);
            GUILayout.Label("\n빌드 세팅에 세팅된 씬을 모두 불러옵니다.", EditorStyles.boldLabel);

            if (GUILayout.Button("빌드 세팅의 씬 불러오기"))
            {
                if (sceneData != null)
                {
                    sceneData.LoadScenesFromBuildSettings();
                    EditorUtility.SetDirty(sceneData);
                    Debug.Log("Scenes loaded from build settings.");
                }
                else
                {
                    Debug.LogError("Scene Data is not assigned!");
                }
            }
        }
    }
}
