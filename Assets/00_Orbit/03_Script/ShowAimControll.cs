using KINEMATION.FPSAnimationFramework.Runtime.Recoil;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowAimControll : MonoBehaviour
{
    public RecoilAnimation recoil;
    public GameObject aimImage;

    void Update()
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
}
