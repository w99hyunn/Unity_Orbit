using Demo.Scripts.Runtime.Character;
using KINEMATION.FPSAnimationFramework.Runtime.Recoil;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShowGunControll : MonoBehaviour
{
    public GameObject Player;
    private RecoilAnimation recoil;
    private FPSController fpsController;
    public GameObject aimImage;
    public TMP_Text fireMode;

    public Image weaponBase;
    public Sprite weaponMK18;
    public Sprite weaponAK12;
    public Sprite weaponAK74;
    public Sprite weaponPistol;
    public Sprite weaponFAL;

    private void Start()
    {
        recoil = Player.GetComponent<RecoilAnimation>();
        fpsController = Player.GetComponent<FPSController>();

        // 무기 변경 상태와 에임 변경 상태 이벤트 구독
        fpsController.OnActiveWeaponIndexChanged += ChangeWeapon;
        fpsController.OnActiveAiming += ChangeAimState;
    }

    void Update()
    {
        ShowFireMode();
    }

    private void ShowFireMode()
    {
        fireMode.text = recoil.fireMode.ToString().ToUpper();
    }

    private void ChangeAimState(FPSAimState aimState)
    {
        if (aimState == FPSAimState.Aiming)
        {
            aimImage.SetActive(true);
        }
        else if (aimState == FPSAimState.None)
        {
            aimImage.SetActive(false);
        }
    }

    private void ChangeWeapon(int index)
    {
        switch (index)
        {
            case 0:
                weaponBase.sprite = weaponMK18;
                break;
            case 1:
                weaponBase.sprite = weaponAK12;
                break;
            case 2:
                weaponBase.sprite = weaponAK74;
                break;
            case 3:
                weaponBase.sprite = weaponPistol;
                break;
            case 4:
                weaponBase.sprite = weaponFAL;
                break;
        }
    }

    // 이벤트 구독 해제 (메모리 누수 방지)
    void OnDestroy()
    {
        fpsController.OnActiveWeaponIndexChanged -= ChangeWeapon;
        fpsController.OnActiveAiming -= ChangeAimState;
    }
}
