using UnityEngine;

public class LookUI : MonoBehaviour
{
    private Camera mainCamera;

    void Start()
    {
        if (mainCamera == null)
        {
            mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        }
    }

    void Update()
    {
        if (mainCamera != null )
        {
            transform.LookAt(transform.position + mainCamera.transform.rotation * Vector3.forward,
                mainCamera.transform.rotation * Vector3.up); 
        }
    }
}
