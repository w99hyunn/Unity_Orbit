using System.Collections;
using UnityEngine;

public class IntroCutSceneCamera : MonoBehaviour
{
    public Camera cameraA;
    public Camera cameraB;
    public float switchDelay = 7.0f; // 7초 대기 시간

    private void Start()
    {
        // 초기 설정: A카메라는 활성화, B카메라는 비활성화
        cameraA.enabled = true;
        cameraB.enabled = true;

        // 7초 뒤에 카메라 전환 시작
        StartCoroutine(SwitchCameraAfterDelay());
    }

    private IEnumerator SwitchCameraAfterDelay()
    {
        // 지정된 시간만큼 대기
        yield return new WaitForSeconds(switchDelay);

        // 카메라 전환
        StartCoroutine(SmoothSwitch());
    }

    private IEnumerator SmoothSwitch()
    {
        float smoothTime = 1.0f; // 전환 시간
        float elapsedTime = 0.0f;

        Vector3 startingPos = cameraA.transform.position;
        Quaternion startingRot = cameraA.transform.rotation;

        while (elapsedTime < smoothTime)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / smoothTime;

            // A카메라를 B카메라의 위치와 회전으로 부드럽게 전환
            cameraA.transform.position = Vector3.Lerp(startingPos, cameraB.transform.position, t);
            cameraA.transform.rotation = Quaternion.Lerp(startingRot, cameraB.transform.rotation, t);

            yield return null;
        }

        // 전환 완료 후 B카메라 활성화, A카메라 비활성화
        cameraA.enabled = false;
        cameraB.enabled = true;
    }
}