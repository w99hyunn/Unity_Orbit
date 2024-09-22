using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace STARTING
{
    public class RootBoxSpawner : MonoBehaviour
    {
        public GameObject rootBox;
        public List<Transform> spawnPoints;

        [Header("루트박스 스폰 주기")]
        public float spawnInterval = 30f;

        private Dictionary<Transform, GameObject> occupiedSpawnPoints;

        void Start()
        {
            occupiedSpawnPoints = new Dictionary<Transform, GameObject>();
            StartCoroutine(SpawnRoutine());
        }

        IEnumerator SpawnRoutine()
        {
            while (true)
            {
                if (PlayerStats.Instance.playerState == PlayerState.LOADING)
                {
                    yield return null;
                }

                SpawnObject();
                yield return new WaitForSeconds(spawnInterval);
            }
        }

        void SpawnObject()
        {
            List<Transform> availableSpawnPoints = new List<Transform>();
            foreach (Transform spawnPoint in spawnPoints)
            {
                if (!occupiedSpawnPoints.ContainsKey(spawnPoint))
                {
                    availableSpawnPoints.Add(spawnPoint);
                }
            }

            if (availableSpawnPoints.Count == 0)
            {
                return;
            }

            Transform randomSpawnPoint = availableSpawnPoints[Random.Range(0, availableSpawnPoints.Count)];
            GameObject spawnedObject = Instantiate(rootBox, randomSpawnPoint.position, randomSpawnPoint.rotation);

            occupiedSpawnPoints[randomSpawnPoint] = spawnedObject;

            spawnedObject.GetComponent<WeaponChangeNpc>().OnDestroyed += () =>
            {
                StartCoroutine(HandleSpawnPointCooldown(randomSpawnPoint));
            };
        }

        IEnumerator HandleSpawnPointCooldown(Transform spawnPoint)
        {
            // 루트 상자 파괴 후 15초 지나야 스폰 포인트 다시 사용 가능(힐템이 15초간 지속)
            yield return new WaitForSeconds(15f);
            occupiedSpawnPoints.Remove(spawnPoint);
        }
    }
}