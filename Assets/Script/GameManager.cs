using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("Bullet")]
    [SerializeField]
    private Transform bulletPoint;
    [SerializeField]
    private GameObject bulletObj;
    [SerializeField]
    private float maxShootDelay = 0.1f;
    [SerializeField]
    private float currentShootDelay = 0.1f;
    [SerializeField]
    private TMP_Text currentbulletText;
    [SerializeField]
    private TMP_Text maxbulletText;
    private int maxBullet = 50;
    private int currentBullet = 0;

    [Header("Weapon FX")]
    [SerializeField]
    private GameObject weaponFlashFX;
    [SerializeField]
    private Transform bulletCasePoint;
    [SerializeField]
    private GameObject bulletcaseFX;
    [SerializeField]
    private Transform weaponClipPoint;
    [SerializeField]
    private GameObject weaponClipFX;

    [SerializeField]
    private TipKey tipkey;


    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        currentShootDelay = 0;
        InitBullet();
        tipkey.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        currentbulletText.text = "0" + currentBullet.ToString();
        maxbulletText.text = maxBullet.ToString();
        if (currentBullet <= 15)
        {
            tipkey.Reload();
        }
        else
        {
            tipkey.gameObject.SetActive(false);
        }
    }

    public void Shooting(Vector3 targetPosition)
    {
        currentShootDelay += Time.deltaTime;

        if (currentShootDelay < maxShootDelay || currentBullet <= 0)
        {
            return;
        }
        currentBullet -= 1;
        currentShootDelay = 0;

        Instantiate(weaponFlashFX, bulletPoint);
        Instantiate(bulletcaseFX, bulletCasePoint);

        Vector3 aim = (targetPosition - bulletPoint.position).normalized;
        Instantiate(bulletObj, bulletPoint.position, Quaternion.LookRotation(aim, Vector3.up));
    }

    public void ReroadClip()
    {
        Instantiate(weaponClipFX, weaponClipPoint);
        InitBullet();
    }

    private void InitBullet()
    {
        currentBullet = maxBullet;
    }
}
