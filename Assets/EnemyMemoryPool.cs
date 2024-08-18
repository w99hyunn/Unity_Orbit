using System.Collections;
using UnityEngine;

public class EnemyMemoryPool : MonoBehaviour
{
    public GameObject enemySpawnPointPrefab; //적 등장 위치 알려주는 프리팹
    [SerializeField]
    private GameObject enemyPrefab; //적 프리팹
    [SerializeField]
    private float enemySpawnTime = 1; //적 생성 주기
    [SerializeField]
    private float enemySpawnLatency = 1; //타일 생성 후 적이 등장하기까지 대기 시간

    private MemoryPool spawnPointMemoryPool; //적 등장 위치를 알려주는 프리팹의 활성 비활성관리
    private MemoryPool enemyMemoryPool; //적 생성과 활성 비활성관리

    private int numberOfEnemiesSpawnedAtOnce = 1; //동시 생성되는 적 숫자
    private Vector2Int mapSize = new Vector2Int(100, 100); //맵 크기

    private void Awake()
    {
        spawnPointMemoryPool = new MemoryPool(enemySpawnPointPrefab);
        enemyMemoryPool = new MemoryPool(enemyPrefab);

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

                item.transform.position = new Vector3(Random.Range(-mapSize.x * 0.49f, mapSize.x * 0.49f), 1,
                                                      Random.Range(-mapSize.y * 0.49f, mapSize.y * 0.49f));

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

        // 타일 오브젝트를 비활성화
        spawnPointMemoryPool.DeactivatePoolItem(point);
    }
}
