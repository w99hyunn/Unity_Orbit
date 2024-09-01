using UnityEngine;

public class SunLight : MonoBehaviour
{
    private Light sun;

    private void Start()
    {
        sun = GetComponent<Light>();
    }

    void Update()
    {
        UpdateSunRotation();
    }

    private void UpdateSunRotation()
    {
        float hours = GameManager.Instance.gameTime / 3600f;

        //(05:00) = 0도, (12:00) = 90도, (18:00) = 180도
        //0도에서 180도로 선형 보간
        float rotationAngle = 0f;

        if (hours >= 5f && hours <= 18f)
        {
            rotationAngle = ((hours - 5f) / 13f) * 180f;
        }
        else if (hours < 5f)
        {
            rotationAngle = ((hours + 19f) / 13f) * 180f; // 오후 6시 이후 ~ 오전 5시 전
        }
        else if (hours > 18f)
        {
            rotationAngle = ((hours - 19f) / 13f) * 180f; // 오후 6시 이후 ~ 오전 5시 전
        }

        sun.transform.rotation = Quaternion.Euler(rotationAngle, 0, 0);
    }
}
