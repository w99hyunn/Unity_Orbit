using KINEMATION.FPSAnimationFramework.Runtime.Recoil;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ShowGunControll : MonoBehaviour
{
    public RecoilAnimation recoil;
    public GameObject aimImage;
    public TMP_Text fireMode;

    void Update()
    {
        ShowDefaultAim();
        ShowFireMode();
    }

    private void ShowDefaultAim()
    {
        if (false == recoil.isAiming)
        {
            aimImage.SetActive(true);
        }
        else if (true == recoil.isAiming)
        {
            aimImage.SetActive(false);
        }
    }

    private void ShowFireMode()
    {
        fireMode.text = recoil.fireMode.ToString().ToUpper();
    }
}
