using UnityEngine.SceneManagement;
using UnityEngine;
using System.Collections;

namespace STARTING
{
    /// <summary>
    /// 던전 퇴장 Trigger
    /// </summary>
    public class DungeonExit : MonoBehaviour
    {
        public AudioClip exitSound;

        private bool _isPlayerInTrigger = false;

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                _isPlayerInTrigger = true;
                UIManager.Instance.interactionKeyEnable("던전 퇴장", "F");
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                _isPlayerInTrigger = false;
                UIManager.Instance.interactionKeyDisable();
            }
        }
        private void Update()
        {
            if (_isPlayerInTrigger && Input.GetKeyDown(KeyCode.F) && PlayerStats.Instance.playerState != PlayerState.LOADING)
            {
                HandleDungeonExit();
            }
        }

        private void HandleDungeonExit()
        {
            if (GameObject.FindWithTag("Arete") != null)
            {
                UIManager.Instance.ScriptText_Enable("아레테를 파괴하지 않으면 돌아갈 수 없어.");
            }
            else if (GameObject.FindWithTag("Enemy") != null)
            {
                UIManager.Instance.ScriptText_Enable("들킬 위험이 있겠어... 드론을 모두 제거하자.");
            }
            else
            {
                PlayerStats.Instance.playerState = PlayerState.LOADING;
                GameManager.Instance.PlaySound(exitSound);
                UIManager.Instance.DungeonLoading("아레테가 파괴되었습니다.", GameManager.Instance.currentZoneName + " 구역이 해방됩니다!", "원래 있던 곳으로 돌아갑니다.");
                UIManager.Instance.interactionKeyDisable();
                StartCoroutine(LoadWorldSceneAfterDelay(3f));
            }
        }

        private IEnumerator LoadWorldSceneAfterDelay(float delay)
        {
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("WorldScene");
            asyncLoad.allowSceneActivation = false;

            yield return new WaitUntil(() => asyncLoad.progress >= 0.9f);

            asyncLoad.allowSceneActivation = true;

            yield return new WaitUntil(() => asyncLoad.isDone);

            SceneManager.SetActiveScene(SceneManager.GetSceneByName("WorldScene"));

            GameManager.Instance.SetPos(GameManager.Instance.LoadPlayerPosition());

            yield return new WaitForSeconds(delay);
            asyncLoad.completed += OnSceneLoaded;
        }

        private void OnSceneLoaded(AsyncOperation asyncOperation)
        {
            UIManager.Instance.DungeonLoadingComplete();
            PlayerStats.Instance.ChangeState(1f, PlayerState.IDLE);
        }
    }
}