using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Pool;

namespace STARTING
{
    public class EnemySpawner : MonoBehaviour
    {
        public Transform target;
        public GameObject enemyPrefab;
        public float enemySpawnTime = 1;

        [Header("처음 시작 시 생성할 몬스터 수")]
        public int initSpawnMonster = 3;

        [Header("최대 몬스터 수 제한")]
        public int maxTotalEnemies = 20; // 최대 생성 몬스터 수

        private ObjectPool<GameObject> _enemyMemoryPool;
        private int _numberOfEnemiesSpawnedAtOnce = 1;
        private int _currentEnemyCount = 0; // 현재 활성화 몬스터 수

        private void Start()
        {
            if (target == null)
            {
                target = GameObject.FindGameObjectWithTag("Player").transform;
            }

            _enemyMemoryPool = new ObjectPool<GameObject>(
                createFunc: () =>
                {
                    GameObject enemy = Instantiate(enemyPrefab);

                    // 몬스터가 죽을 때 풀로 반환
                    enemy.GetComponent<EnemyFSM>().OnDeath += () =>
                    {
                        _enemyMemoryPool.Release(enemy);
                        _currentEnemyCount--; // 현재 활성화 몬스터 수 감소
                    };

                    return enemy;
                },
                actionOnGet: item =>
                {
                    item.SetActive(true);
                    item.GetComponent<EnemyFSM>().ResetState(); // 상태 초기화
                    item.name = enemyPrefab.name; // 이름 변경
                },
                actionOnRelease: item =>
                {
                    item.SetActive(false); // 비활성화 처리
                },
                actionOnDestroy: Destroy,
                collectionCheck: false,
                defaultCapacity: 10,
                maxSize: 30
            );


            StartCoroutine(StartSpawnCoroutine());
            StartCoroutine(SpawnEnemyRoutine());
        }

        IEnumerator StartSpawnCoroutine()
        {
            if (PlayerStats.Instance.playerState == PlayerState.LOADING)
            {
                yield return null;
            }

            yield return new WaitForSeconds(0.5f);

            for (int i = 0; i < initSpawnMonster; ++i)
            {
                SpawnEnemy();
            }
        }

        private IEnumerator SpawnEnemyRoutine()
        {
            if (PlayerStats.Instance.playerState == PlayerState.LOADING)
            {
                yield return null;
            }

            yield return new WaitForSeconds(0.5f);

            int currentNumber = 0;
            int maximumNumber = 5;

            while (true)
            {
                // 총 몬스터 수가 최대치에 도달하지 않았을 때만 생성
                if (_currentEnemyCount < maxTotalEnemies)
                {
                    for (int i = 0; i < _numberOfEnemiesSpawnedAtOnce; ++i)
                    {
                        if (_currentEnemyCount < maxTotalEnemies)
                        {
                            SpawnEnemy();
                        }
                    }
                }

                currentNumber++;

                if (currentNumber >= maximumNumber)
                {
                    currentNumber = 0;
                    _numberOfEnemiesSpawnedAtOnce = Mathf.Min(_numberOfEnemiesSpawnedAtOnce + 1, 10);
                }

                yield return new WaitForSeconds(enemySpawnTime);
            }
        }

        private void SpawnEnemy()
        {
            if (_currentEnemyCount >= maxTotalEnemies)
            {
                return; // 최대 몬스터 수에 도달하면 더 이상 생성하지 않음
            }

            GameObject item = _enemyMemoryPool.Get();
            _currentEnemyCount++;

            Vector3 scale = transform.localScale;
            Vector3 randomOffset = new Vector3(
                Random.Range(-scale.x * 0.5f, scale.x * 0.5f),
                0,
                Random.Range(-scale.z * 0.5f, scale.z * 0.5f)
            );

            Vector3 spawnPosition = transform.position + randomOffset;

            NavMeshHit hit;
            if (NavMesh.SamplePosition(spawnPosition, out hit, 2.0f, NavMesh.AllAreas))
            {
                var navMeshAgent = item.GetComponent<NavMeshAgent>();
                if (navMeshAgent != null)
                {
                    navMeshAgent.Warp(hit.position);

                    if (!navMeshAgent.isOnNavMesh)
                    {
                        //정상적으로 안놓아졌다면 다시 스폰
                        _enemyMemoryPool.Release(item);
                        _currentEnemyCount--;
                        SpawnEnemy();
                        return;
                    }
                }
                item.GetComponent<EnemyFSM>().Setup(target, this);
            }
            else
            {
                //navMesh를 벗어난 곳에서는 다시스폰
                _enemyMemoryPool.Release(item);
                _currentEnemyCount--;
                SpawnEnemy();
            }
        }

    }
}