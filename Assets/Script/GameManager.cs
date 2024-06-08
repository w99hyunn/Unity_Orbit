using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
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

    private AudioSource audioSource;

    public static GameManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        currentShootDelay = 0;
        InitBullet();
        tipkey.gameObject.SetActive(false);
        audioSource = GetComponent<AudioSource>();
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

    public void Shooting(Vector3 targetPosition, Enemy enemy, AudioSource weaponSound, AudioClip shootingSound)
    {
        currentShootDelay += Time.deltaTime;

        if (currentShootDelay < maxShootDelay || currentBullet <= 0)
        {
            return;
        }
        currentBullet -= 1;
        currentShootDelay = 0;

        weaponSound.clip = shootingSound;
        weaponSound.Play();

        Vector3 aim = (targetPosition - bulletPoint.position).normalized;

        //Instantiate(weaponFlashFX, bulletPoint);
        GameObject flashFX = PoolManager.instance.ActivateObj(1);
        SetObjPosition(flashFX, bulletPoint);
        flashFX.transform.rotation = Quaternion.LookRotation(aim, Vector3.up);

        //Instantiate(bulletcaseFX, bulletCasePoint);
        GameObject caseFX = PoolManager.instance.ActivateObj(2);
        SetObjPosition(caseFX, bulletCasePoint);

        //Instantiate(bulletObj, bulletPoint.position, Quaternion.LookRotation(aim, Vector3.up));
        /* 총알을 생성하여 맞추는 것 */
        GameObject prefabToSpawn = PoolManager.instance.ActivateObj(0);
        SetObjPosition(prefabToSpawn, bulletPoint);
        prefabToSpawn.transform.rotation = Quaternion.LookRotation(aim, Vector3.up);

        //Raycast
        /* Raycast를 사용하여 맞추는 것
        if (enemy != null && enemy.enemyCurrentHP > 0)
        {
            enemy.enemyCurrentHP -= 1;
            Debug.Log("enemy HP : " + enemy.enemyCurrentHP);
        }*/
    }

    public void ReroadClip()
    {
        //Instantiate(weaponClipFX, weaponClipPoint);
        GameObject clipFX = PoolManager.instance.ActivateObj(3);
        SetObjPosition(clipFX, weaponClipPoint);
        InitBullet();
    }

    private void InitBullet()
    {
        currentBullet = maxBullet;
    }

    private void SetObjPosition(GameObject obj, Transform targetTransform)
    {
        obj.transform.position = targetTransform.position;
    }

    public void PlaySound(AudioClip clip)
    {
        if (clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }
}
