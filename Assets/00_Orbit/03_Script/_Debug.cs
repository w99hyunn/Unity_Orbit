using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace STARTING
{
    public class _Debug : MonoBehaviour
    {
        public PlayerStats playerStats;

        // Start is called before the first frame update
        void Start()
        {
            playerStats = PlayerStats.Instance;
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.M))
            {
                DestroyEnemies();
            }
            if (Input.GetKeyDown(KeyCode.V))
            {
                playerStats.TakeDamage(20);
            }
            if (Input.GetKeyDown(KeyCode.B))
            {
                playerStats.UseMana(20);
            }
            if (Input.GetKeyDown(KeyCode.N)) // N키를 눌러 경험치 얻기 테스트
            {
                playerStats.GainExperience(50);
            }
            if (Input.GetKeyDown(KeyCode.L))
            {
                GameManager.Instance.SaveGame();
            }
        }
        void DestroyEnemies()
        {
            // "Enemy" 태그를 가진 모든 오브젝트를 배열로 가져옴
            GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

            // 각 오브젝트를 순회하며 파괴
            foreach (GameObject enemy in enemies)
            {
                Destroy(enemy);
            }

            Debug.Log(enemies.Length + "개의 Enemy 오브젝트가 파괴되었습니다.");
        }

        void OnGUI()
        {
            // 화면 너비와 높이를 가져옴
            float screenWidth = Screen.width;
            float screenHeight = Screen.height;

            // 글자를 표시할 위치 및 크기를 설정
            float labelWidth = 200f;
            float labelHeight = 70f;
            float xPosition = screenWidth - labelWidth - 10f; // 오른쪽에서 10px 떨어진 위치
            float yPosition = (screenHeight / 2f) - (labelHeight / 2f); // 세로 가운데 정렬

            // GUI 영역 시작
            GUILayout.BeginArea(new Rect(xPosition, yPosition, labelWidth, labelHeight));

            // 글자 출력
            GUILayout.Label("M 몬스터 전부 제거, V 피 20 감소, B 마나 20감소, N 경험치 50증가, L 게임저장");

            // GUI 영역 종료
            GUILayout.EndArea();
        }
    }
}