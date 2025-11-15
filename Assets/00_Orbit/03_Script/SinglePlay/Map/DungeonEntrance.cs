using UnityEngine.SceneManagement;
using UnityEngine;
using System.Collections;

namespace NOLDA
{
    /// <summary>
    /// 던전 입장 Trigger 스크립트
    /// 최대한 던전 매니저에서 중앙 처리
    /// </summary>
    public class DungeonEntrance : MonoBehaviour
    {
        public Vector3 spawnPos;
        public AudioClip entranceSound;
        public HintTrigger hintTrigger;
        private Collider playerCollider;
        private string currentZoneName;

        private bool _isPlayerInTrigger = false;

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                _isPlayerInTrigger = true;
                playerCollider = other;
                currentZoneName = GameManager.Instance.currentZoneName;
                UIManager.Instance.interactionKeyEnable("던전 입장", "F");
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                _isPlayerInTrigger = false;
                playerCollider = null;
                UIManager.Instance.interactionKeyDisable();
            }
        }

        private void Update()
        {
            if (_isPlayerInTrigger && Input.GetKeyDown(KeyCode.F) && PlayerStats.Instance.playerState != PlayerState.LOADING)
            {
                HandleDungeonEntrance(playerCollider);
            }
        }
        private void HandleDungeonEntrance(Collider other)
        {
            bool isLiberated = GameManager.Instance.IsZoneLiberated(currentZoneName);

            if (isLiberated)
            {
                UIManager.Instance.ScriptText_Enable("이미 해방된 구역이다. 굳이 들어갈 필요 없겠어.");
            }
            else if (true == hintTrigger.IsEnemyInTrigger())
            {
                UIManager.Instance.ScriptText_Enable("근처에 감시 드론이 있어 들어가면 들킬 위험이 있겠어.");
            }
            else
            {
                UIManager.Instance.interactionKeyDisable();
                UIManager.Instance.DungeonLoading("주어진 시간 내에", "아레테를 파괴해야 합니다!", "아레테를 파괴하여 지역을 해방시키세요.");
                PlayerStats.Instance.playerState = PlayerState.LOADING;
                GameManager.Instance.PlaySound(entranceSound);
                GameManager.Instance.SavePlayerPosition(other.transform.position);
                StartCoroutine(LoadDungeonSceneAfterDelay(3f));
            }
        }

        private IEnumerator LoadDungeonSceneAfterDelay(float delay)
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
            yield return new WaitForSeconds(delay);
            SceneManager.LoadScene(SceneDataManager.GetSceneName("SingleDungeon"));
            GameManager.Instance.SetPos(spawnPos);
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            UIManager.Instance.DungeonLoadingComplete();
            PlayerStats.Instance.ChangeState(1f, PlayerState.IDLE);
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }
}