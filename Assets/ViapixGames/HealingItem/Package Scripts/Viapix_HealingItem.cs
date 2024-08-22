using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Viapix_HealingItem : MonoBehaviour
{
    [SerializeField]
    float rotationSpeedX, rotationSpeedY, rotationSpeedZ;

    [SerializeField]
    int healingAmount;

    GameObject playerObj;

    private void Start()
    {
        playerObj = GameObject.FindGameObjectWithTag("Player");
    }

    void Update()
    {
        transform.Rotate(rotationSpeedX, rotationSpeedY, rotationSpeedZ);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            //playerObj.GetComponent<Viapix_PlayerHP>().playerHP += healingAmount;

            Destroy(gameObject);

            //print("Player HP: " + playerObj.GetComponent<Viapix_PlayerHP>().playerHP);
        }
    }
}


