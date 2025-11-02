using UnityEngine;
using UnityEditor;

namespace STARTING
{
    public class DebugPanelEditor : EditorWindow
    {
        private PlayerStats playerStats;
        private Inventory inventory;

        [MenuItem("STARTING/Debug Panel")]
        public static void ShowWindow()
        {
            // 에디터 창 생성
            GetWindow<DebugPanelEditor>("Debug Panel");
        }

        private void OnEnable()
        {
            Texture2D icon = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/00_Orbit/04_Images/StartingSymbol.png");
            titleContent = new GUIContent("Debug Panel", icon);

            // 필요한 컴포넌트를 에디터가 시작될 때 설정
            if (PlayerStats.Instance != null)
            {
                playerStats = PlayerStats.Instance;
                inventory = playerStats.gameObject.GetComponent<Inventory>();
            }
        }

        private void OnGUI()
        {
            if (playerStats == null || inventory == null)
            {
                EditorGUILayout.HelpBox("PlayerStats 또는 Inventory가 초기화되지 않았습니다.", MessageType.Warning);
                if (GUILayout.Button("플레이어 컴포넌트 찾기"))
                {
                    OnEnable(); // 다시 컴포넌트를 찾음
                }
                return;
            }

            GUILayout.Space(20);
            GUILayout.Label("싱글플레이 전용", EditorStyles.boldLabel);
            GUILayout.Space(20);

            if (GUILayout.Button("힐링 +200"))
            {
                playerStats.Healing(200);
            }
            GUILayout.Space(10);
            if (GUILayout.Button("모든 적 삭제"))
            {
                DestroyEnemies();
            }
            GUILayout.Space(10);
            if (GUILayout.Button("데미지 -20"))
            {
                playerStats.TakeDamage(20);
            }
            GUILayout.Space(10);
            if (GUILayout.Button("마나 -20"))
            {
                playerStats.UseMana(20);
            }
            GUILayout.Space(10);
            if (GUILayout.Button("경험치 +50"))
            {
                playerStats.GainExperience(50);
            }
            GUILayout.Space(10);
            if (GUILayout.Button("게임 저장"))
            {
                GameManager.Instance.SaveGame();
            }
            GUILayout.Space(10);
            if (GUILayout.Button("칩 +200"))
            {
                inventory.GainChip(200);
            }
        }

        private void DestroyEnemies()
        {
            // "Enemy" 태그를 가진 모든 오브젝트를 배열로 가져옴
            GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

            // 각 오브젝트를 순회하며 파괴
            foreach (GameObject enemy in enemies)
            {
                DestroyImmediate(enemy);  // 에디터에서 즉시 파괴
            }

            Debug.Log(enemies.Length + "개의 Enemy 오브젝트가 파괴되었습니다.");
        }
    }
}