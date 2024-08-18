using UnityEngine;

public class Status : MonoBehaviour
{
    [Header("Walk, Run Speed")]
    public float walkSpeed;
    public float runSpeed;

    public float WalkSpeed => walkSpeed;
    public float RunSpeed => runSpeed;
}
