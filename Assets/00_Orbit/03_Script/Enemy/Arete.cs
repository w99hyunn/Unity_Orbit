using UnityEngine;

public class Arete : MonoBehaviour
{
    public DungeonTimer dungeonTimer;
    public EnemyMemoryPool enemySpawnTrigger;
    public EnemyMemoryPool enemySpawnTrigger2;

    public void OnDestroy()
    {
        Destroy(enemySpawnTrigger);
        Destroy(enemySpawnTrigger2);
        UIManager.Instance.ScriptText_Enable($"{GameManager.Instance.currentZoneName}이 해방됐다. 더 이상 드론이 소환되지 않을거야.");
        dungeonTimer.Destroy_Arete();
        Destroy(this.gameObject);
    }
}