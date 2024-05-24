using UnityEngine;
using Cinemachine;

public class PlayerController : MonoBehaviour
{
    public Animator animator;
    public CinemachineVirtualCamera shoulderCamera;
    public float normalSpeed = 5f;
    public float aimingSpeed = 2f;
    private bool isAiming = false;

    private void Update()
    {
        HandleAiming();

    }

    private void HandleAiming()
    {
        if (Input.GetMouseButtonDown(1)) // Right mouse button
        {
            isAiming = true;
            shoulderCamera.Priority = 11; // Set higher priority to activate this camera
            animator.SetBool("isAiming", true);
        }
        else if (Input.GetMouseButtonUp(1))
        {
            isAiming = false;
            shoulderCamera.Priority = 9; // Set lower priority to deactivate this camera
            animator.SetBool("isAiming", false);
        }
    }

}
