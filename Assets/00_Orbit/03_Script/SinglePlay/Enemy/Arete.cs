using System.Collections;
using UnityEngine;

namespace STARTING
{
    public class Arete : MonoBehaviour
    {
        public DungeonTimer dungeonTimer;
        public GameObject enemySpawner;

        public float moveRange = 0.5f; // 위아래로 이동하는 범위
        public float moveSpeed = 2f; // Lerp로 이동하는 속도
        public float pauseDuration = 0.05f; // 고점과 저점에서 멈추는 시간

        [Header("파괴 FX")]
        private AudioSource audioSource;
        public AudioClip explosionSound;
        public GameObject explosionVFX;

        private Vector3 _startPosition;
        private Vector3 _targetPosition;
        private bool _movingUp = true;

        private void Start()
        {
            audioSource = GetComponent<AudioSource>();
            _startPosition = transform.position;
            StartCoroutine(MoveObject());
        }

        IEnumerator MoveObject()
        {
            while (true)
            {
                if (_movingUp)
                {
                    _targetPosition = _startPosition + new Vector3(0, moveRange, 0);
                }
                else
                {
                    _targetPosition = _startPosition + new Vector3(0, -moveRange, 0);
                }

                float elapsedTime = 0f;
                Vector3 initialPosition = transform.position;

                while (elapsedTime < moveSpeed)
                {
                    transform.position = Vector3.Lerp(initialPosition, _targetPosition, elapsedTime / moveSpeed);
                    elapsedTime += Time.deltaTime;
                    yield return null;
                }

                transform.position = _targetPosition;

                yield return new WaitForSeconds(pauseDuration);

                _movingUp = !_movingUp;
            }
        }

        public void AreteDestroy()
        {
            GameObject explosionInstance = Instantiate(explosionVFX, this.transform.position, this.transform.rotation);
            AudioSource audioSource = explosionInstance.AddComponent<AudioSource>();

            audioSource.outputAudioMixerGroup = this.audioSource.outputAudioMixerGroup;
            audioSource.clip = explosionSound;
            audioSource.Play();
            Destroy(explosionInstance, 5f);

            // enemySpawner.SetActive(false);로 설정하면 아레테 파괴 후 몬스터가 남아있음.
            // 아레테 파괴 후 굳이 몬스터를 남겨 잡몹을 모두 잡게 하는건 불쾌한 게임성일 수 있음.
            Destroy(enemySpawner);

            UIManager.Instance.ScriptText_Enable($"{GameManager.Instance.currentZoneName} 구역이 해방됐다. 더 이상 드론이 소환되지 않을거야.");
            dungeonTimer.Destroy_Arete();
            Destroy(this.gameObject);
        }
    }
}