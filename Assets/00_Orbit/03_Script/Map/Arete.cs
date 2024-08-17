using UnityEngine;

public class Arete : MonoBehaviour
{
    public DungeonTimer dungeonTimer;

    private void OnDestroy()
    {
        dungeonTimer.Destroy_Arete();
    }
}
