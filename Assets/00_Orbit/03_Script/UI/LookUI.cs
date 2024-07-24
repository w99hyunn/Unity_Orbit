using UnityEngine;

public class LookUI : MonoBehaviour
{
    private Camera camera;

    void Start()
    {
        if (camera == null)
        {
            camera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        }
    }

    void Update()
    {
        if (camera != null )
        {
            transform.LookAt(transform.position + camera.transform.rotation * Vector3.forward,
                camera.transform.rotation * Vector3.up); 
        }
    }
}
