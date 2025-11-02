using UnityEngine;

public class MoveableLODUpdate : MonoBehaviour
{
    private LODGroup lodGroup;

    void Start()
    {
        lodGroup = GetComponent<LODGroup>();
    }

    void Update()
    {
        lodGroup.RecalculateBounds();
    }
}
