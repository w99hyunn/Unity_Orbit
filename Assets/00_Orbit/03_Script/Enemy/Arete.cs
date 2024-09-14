using System.Collections;
using UnityEngine;

public class Arete : MonoBehaviour
{
    public DungeonTimer dungeonTimer;
    public GameObject enemySpawner;

    public float moveRange = 0.5f; // 위아래로 이동하는 범위
    public float moveSpeed = 2f; // Lerp로 이동하는 속도
    public float pauseDuration = 0.05f; // 고점과 저점에서 멈추는 시간

    private Vector3 startPosition;
    private Vector3 targetPosition;
    private bool movingUp = true;
    private void Start()
    {
        startPosition = transform.position;
        StartCoroutine(MoveObject());
    }

    IEnumerator MoveObject()
    {
        while (true)
        {
            if (movingUp)
            {
                targetPosition = startPosition + new Vector3(0, moveRange, 0);
            }
            else
            {
                targetPosition = startPosition + new Vector3(0, -moveRange, 0);
            }

            float elapsedTime = 0f;
            Vector3 initialPosition = transform.position;

            while (elapsedTime < moveSpeed)
            {
                transform.position = Vector3.Lerp(initialPosition, targetPosition, elapsedTime / moveSpeed);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            transform.position = targetPosition;

            yield return new WaitForSeconds(pauseDuration);

            movingUp = !movingUp;
        }
    }

    public void AreteDestroy()
    {
        Destroy(enemySpawner);
        UIManager.Instance.ScriptText_Enable($"{GameManager.Instance.currentZoneName} 구역이 해방됐다. 더 이상 드론이 소환되지 않을거야.");
        dungeonTimer.Destroy_Arete();
        Destroy(this.gameObject);
    }
}