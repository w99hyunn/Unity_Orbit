using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletManager : MonoBehaviour
{
    private Rigidbody bulletRigidbody;


    [SerializeField]
    private float moveSpeed = 10f;
    private float destroyTime = 3f;
    // Start is called before the first frame update
    void Start()
    {
        bulletRigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        destroyTime -= Time.deltaTime;
        if (destroyTime <=0)
        {
            DestroyBullet();
        }
        BulletMove();
    }

    private void BulletMove()
    {
        bulletRigidbody.velocity = transform.forward * moveSpeed;
    }

    private void DestroyBullet()
    {
        Destroy(gameObject);
        destroyTime = 3;
    }

    private void OnTriggerEnter(Collider other)
    {
        DestroyBullet();
    }
}
