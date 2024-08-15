using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerList : MonoBehaviour
{
    public List<GameObject> gameObjects;

    void Start()
    {
        StartCoroutine(ActivateObjectsAfterDelay(0.1f));
    }

    IEnumerator ActivateObjectsAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        foreach (GameObject obj in gameObjects)
        {
            if (obj != null)
            {
                obj.SetActive(true);
            }
        }
    }
}
