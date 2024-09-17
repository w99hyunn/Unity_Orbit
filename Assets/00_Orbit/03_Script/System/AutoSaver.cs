using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace STARTING
{
    public class AutoSaver : MonoBehaviour
    {
        [Header("설정된 시간마다 자동저장(초 단위)")]
        public float interval = 180f;
        public Button SaveButton; // PauseMenu > Save 버튼 클릭 이벤트 발생

        void Awake()
        {
            StartCoroutine(AutoClick());
        }

        public void GameSave()
        {
            GameManager.Instance.SaveGame();
        }

        private IEnumerator AutoClick()
        {
            while (true)
            {
                yield return new WaitForSeconds(interval);
                if (SceneManager.GetActiveScene().name == "WorldScene" && PlayerStats.Instance.playerState != PlayerState.DIE)
                {
                    SaveButton.onClick.Invoke();
                }
            }
        }
    }
}