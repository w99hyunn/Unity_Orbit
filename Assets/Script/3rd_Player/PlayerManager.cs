using Cinemachine;
using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class PlayerManager : MonoBehaviour
{
    private StarterAssetsInputs input;
    private ThirdPersonController controller;
    private Animator anim;

    [Header("Aim")]
    [SerializeField]
    private CinemachineVirtualCamera aimCam;
    [SerializeField]
    private GameObject aimImage;
    [SerializeField]
    private GameObject aimObj;
    [SerializeField]
    private float aimObjDis = 10f;
    [SerializeField]
    private LayerMask targetLayer;

    [Header("IK")]
    [SerializeField]
    private Rig aimRig;

    private Enemy enemy;

    [Header("Weapon Sound Effect")]
    [SerializeField]
    private AudioClip shootingSound;
    [SerializeField]
    private AudioClip[] reloadSound;
    private AudioSource weaponSound;

    public static PlayerManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 씬 전환 시에도 파괴되지 않도록 설정
        }
        else
        {
            Destroy(gameObject);
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        input = GetComponent<StarterAssetsInputs>();
        controller = GetComponent<ThirdPersonController>();
        anim = GetComponent<Animator>();
        weaponSound = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        AimCheck();
    }

    private void AimCheck()
    {
        if (input.reroad)
        {
            input.reroad = false;

            if (controller.isReroad)
            {
                return;
            }
            AimControll(false);
            SetRigWeight(0);
            anim.SetLayerWeight(1, 1);
            anim.SetTrigger("Reroad");
            controller.isReroad = true;
        }

        if (controller.isReroad)
        {
            return;
        }

        if (input.aim)
        { 
            AimControll(true);
            anim.SetLayerWeight(1, 1);

            Vector3 targetPosition = Vector3.zero;
            Transform camTransform = Camera.main.transform;
            RaycastHit hit;


            if (Physics.Raycast(camTransform.position, camTransform.forward, out hit, Mathf.Infinity, targetLayer))
            {
                //Debug.Log("Name : " + hit.transform.gameObject.name);
                targetPosition = hit.point;
                aimObj.transform.position = hit.point;

                enemy = hit.collider.gameObject.GetComponent<Enemy>();
            }
            else
            {
                targetPosition = camTransform.position + camTransform.forward * aimObjDis;
                aimObj.transform.position = camTransform.position + camTransform.forward * aimObjDis;
            }

            Vector3 targetAim = targetPosition;
            targetAim.y = transform.position.y;
            Vector3 aimDir = (targetAim - transform.position).normalized;

            transform.forward = Vector3.Lerp(transform.forward, aimDir, Time.deltaTime * 30f);

            SetRigWeight(1);

            if (input.shoot)
            {
                anim.SetBool("Shoot", true);
                GameManager.Instance.Shooting(targetPosition, enemy, weaponSound, shootingSound);
            }
            else
            {
                anim.SetBool("Shoot", false);
            }
        }
        else
        {
            AimControll(false);
            SetRigWeight(0);
            anim.SetLayerWeight(1, 0);
            anim.SetBool("Shoot", false);
        }
    }

    private void AimControll(bool isCheck)
    {
        controller.isAimMove = isCheck;
        aimCam.gameObject.SetActive(isCheck);
        aimImage.gameObject.SetActive(isCheck);
    }

    public void Reroad()
    {
        //Debug.Log("ReRoad");
        controller.isReroad = false;
        SetRigWeight(1);
        anim.SetLayerWeight(1, 0);
        PlayWeaponSound(reloadSound[2]);
    }

    private void SetRigWeight(float weight)
    {
        aimRig.weight = weight;
    }

    public void ReroadWeaponClip()
    {
        GameManager.Instance.ReroadClip();
        PlayWeaponSound(reloadSound[0]);
    }

    public void ReloadInsertClip()
    {
        PlayWeaponSound(reloadSound[1]);
    }

    private void PlayWeaponSound(AudioClip sound)
    {
        weaponSound.clip = sound;
        weaponSound.Play();
    }

}
