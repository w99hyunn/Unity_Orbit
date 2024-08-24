using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Pool;

public class EnemyMemoryPool : MonoBehaviour
{
    public Transform target;
    public GameObject enemyPrefab;
    public float enemySpawnTime = 1;
    public float enemySpawnLatency = 1;

    private ObjectPool<GameObject> enemyMemoryPool;
    private int numberOfEnemiesSpawnedAtOnce = 1;

    [Header("처음 시작 시 생성할 몬스터 수")]
    public int initSpawnMonster = 3;

    [Header("최대 몬스터 수 제한")]
    public int maxTotalEnemies = 20; // 최대 생성 몬스터 수

    private int currentEnemyCount = 0; // 현재 활성화 몬스터 수

    private void Awake()
    {
        if (target == null)
        {
            target = GameObject.FindGameObjectWithTag("Player").transform;
        }

        enemyMemoryPool = new ObjectPool<GameObject>(
            createFunc: () =>
            {
                GameObject enemy = Instantiate(enemyPrefab);

                // 몬스터가 죽을 때 풀로 반환
                enemy.GetComponent<EnemyFSM>().OnDeath += () =>
                {
                    enemyMemoryPool.Release(enemy);
                    currentEnemyCount--; // 현재 활성화 몬스터 수 감소
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
        // 1초 대기
        yield return new WaitForSeconds(0.5f);

        // 지정된 횟수만큼 적을 생성
        for (int i = 0; i < initSpawnMonster; ++i)
        {
            SpawnEnemy();
        }
    }

    private IEnumerator SpawnEnemyRoutine()
    {
        yield return new WaitForSeconds(0.5f);

        int currentNumber = 0;
        int maximumNumber = 5;

        while (true)
        {
            // 총 몬스터 수가 최대치에 도달하지 않았을 때만 생성
            if (currentEnemyCount < maxTotalEnemies)
            {
                for (int i = 0; i < numberOfEnemiesSpawnedAtOnce; ++i)
                {
                    if (currentEnemyCount < maxTotalEnemies)
                    {
                        SpawnEnemy();
                    }
                }
            }

            currentNumber++;

            if (currentNumber >= maximumNumber)
            {
                currentNumber = 0;
                numberOfEnemiesSpawnedAtOnce = Mathf.Min(numberOfEnemiesSpawnedAtOnce + 1, 10);
            }

            yield return new WaitForSeconds(enemySpawnTime);
        }
    }

    private void SpawnEnemy()
    {
        if (currentEnemyCount >= maxTotalEnemies)
        {
            return; // 최대 몬스터 수에 도달하면 더 이상 생성 X
        }

        GameObject item = enemyMemoryPool.Get();
        currentEnemyCount++; // 활성화된 몬스터 수 증가

        Vector3 scale = transform.localScale;
        Vector3 spawnPosition = new Vector3(
            Random.Range(-scale.x * 0.5f, scale.x * 0.5f),
            1,
            Random.Range(-scale.z * 0.5f, scale.z * 0.5f)
        );

        var navMeshAgent = item.GetComponent<NavMeshAgent>();
        if (navMeshAgent != null)
        {
            navMeshAgent.Warp(transform.position + spawnPosition);
            if (!navMeshAgent.isOnNavMesh)
            {
                enemyMemoryPool.Release(item);
                currentEnemyCount--; // 실패 시 활성화된 몬스터 수 감소 (Object Pool에만 존재)
                return;
            }
        }

        // 몬스터를 생성한 Pool 정보 설정 후 추적
        item.GetComponent<EnemyFSM>().Setup(target, this);
    }
}
