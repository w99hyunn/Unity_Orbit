using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyMemoryPool : MonoBehaviour
{
    public Transform target;

    public GameObject enemySpawnPointPrefab; //적 등장 위치 알려주는 프리팹
    public GameObject enemyPrefab; //적 프리팹
    public float enemySpawnTime = 1; //적 생성 주기
    public float enemySpawnLatency = 1; //타일 생성 후 적이 등장하기까지 대기 시간

    private MemoryPool spawnPointMemoryPool; //적 등장 위치를 알려주는 프리팹의 활성 비활성관리
    private MemoryPool enemyMemoryPool; //적 생성과 활성 비활성관리

    private int numberOfEnemiesSpawnedAtOnce = 1; //동시 생성되는 적 숫자

    [Header("처음 시작시 생성할 몬스터 수")]
    public int initSpawnMonster = 3;

    private void Awake()
    {
        spawnPointMemoryPool = new MemoryPool(enemySpawnPointPrefab);
        enemyMemoryPool = new MemoryPool(enemyPrefab);

        // 시작 시 3마리의 적을 소환하는 로직
        for (int i = 0; i < initSpawnMonster; ++i)
        {
            GameObject item = spawnPointMemoryPool.ActivatePoolItem();

            // 현재 오브젝트의 스케일 값을 사용하여 맵의 범위를 자동 설정
            Vector3 scale = transform.localScale;
            Vector3 spawnPosition = new Vector3(
                Random.Range(-scale.x * 0.5f, scale.x * 0.5f),
                1,
                Random.Range(-scale.z * 0.5f, scale.z * 0.5f)
            );

            item.transform.position = transform.position + spawnPosition;

            StartCoroutine(SpawnEnemy(item));
        }

        StartCoroutine("SpawnTile");
    }

    private IEnumerator SpawnTile()
    {
        int currentNumber = 0;
        int maximumNumber = 50;

        while (true)
        {
            // 동시에 numberOfEnemiesSpawnedAtOnce 숫자만큼 적이 생성되도록 반복문 사용
            for (int i = 0; i < numberOfEnemiesSpawnedAtOnce; ++i)
            {
                GameObject item = spawnPointMemoryPool.ActivatePoolItem();

                // 현재 오브젝트의 스케일 값을 사용하여 맵의 범위를 자동 설정
                Vector3 scale = transform.localScale;
                Vector3 spawnPosition = new Vector3(
                    Random.Range(-scale.x * 0.5f, scale.x * 0.5f),
                    1,
                    Random.Range(-scale.z * 0.5f, scale.z * 0.5f)
                );

                item.transform.position = transform.position + spawnPosition;

                StartCoroutine("SpawnEnemy", item);
            }

            currentNumber++;

            if (currentNumber >= maximumNumber)
            {
                currentNumber = 0;
                numberOfEnemiesSpawnedAtOnce++;
            }

            yield return new WaitForSeconds(enemySpawnTime);
        }
    }

private IEnumerator SpawnEnemy(GameObject point)
{
    yield return new WaitForSeconds(enemySpawnLatency);

    // 적 오브젝트를 생성하고, 적의 위치를 point의 위치로 설정
    GameObject item = enemyMemoryPool.ActivatePoolItem();

    item.transform.position = point.transform.position;

    // NavMeshAgent를 비활성화 후 위치 설정 후 재활성화
    var navMeshAgent = item.GetComponent<NavMeshAgent>();
    if (navMeshAgent != null)
    {
        navMeshAgent.enabled = false;
    }

    item.transform.position = point.transform.position;

    if (navMeshAgent != null)
    {
        navMeshAgent.enabled = true;

        // NavMesh 위에 있는지 확인
        if (!navMeshAgent.isOnNavMesh)
        {
            // NavMesh 위에 있지 않다면 적을 파괴
            enemyMemoryPool.DeactivatePoolItem(item);
            yield break;
        }
    }

    item.GetComponent<EnemyFSM>().Setup(target);

    // 타일 오브젝트를 비활성화
    spawnPointMemoryPool.DeactivatePoolItem(point);
}

}
