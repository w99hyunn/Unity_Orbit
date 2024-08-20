using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public enum EnemyState { None = -1, Idle = 0, Wander, Pursuit, Attack }

public class EnemyFSM : MonoBehaviour
{
    [Header("Pursuit")]
    public float targetRecognitionRange = 8;
    public float pursuitLimitRange = 10;

    [Header("Attack")]
    public GameObject projectilePrefab;
    public Transform projectileSpawnPoint;
    public float attackRange = 5;
    public float attackRate = 1;

    private EnemyState currentState = EnemyState.None;
    private float lastAttackTime = 0;

    private NavMeshAgent navMeshAgent;
    private Transform target;

    private AudioSource audioSource;
    public AudioClip shotSound;

    [Header("하위 모델링 회전 관련")]
    public Transform eyeTransform;

    private Coroutine currentStateCoroutine;

    public void Setup(Transform target)
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.updateRotation = false;
        this.target = target;
    }

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        ChangeState(EnemyState.Idle);
    }

    private void Update()
    {
        EnemyState newState = CalculateDistanceToTargetAndSelectState();
        if (newState != currentState)
        {
            ChangeState(newState);
        }
    }

    private void ChangeState(EnemyState newState)
    {
        if (currentState == newState) return;

        if (currentStateCoroutine != null)
        {
            StopCoroutine(currentStateCoroutine);
        }

        currentState = newState;
        currentStateCoroutine = StartCoroutine(currentState.ToString());
    }

    IEnumerator Idle()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(1, 5));
            RestoreRotationToTarget();
            yield return StartCoroutine(Wander());
        }
    }

    IEnumerator Wander()
    {
        float currentTime = 0;
        float maxTime = 10;

        navMeshAgent.speed = 1.5f;
        navMeshAgent.SetDestination(CalculateWanderPosition());

        while (currentTime < maxTime)
        {
            currentTime += Time.deltaTime;
            Vector3 to = new Vector3(navMeshAgent.destination.x, 0, navMeshAgent.destination.z);
            Vector3 from = new Vector3(transform.position.x, 0, transform.position.z);
            Vector3 direction = to - from;

            if (direction != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
            }

            if (direction.sqrMagnitude < 0.01f)
            {
                break;
            }

            yield return null;
        }
    }

    IEnumerator Pursuit()
    {
        while (true)
        {
            navMeshAgent.speed = 3f;
            navMeshAgent.SetDestination(target.position);
            LookRotationToTarget();
            yield return null;
        }
    }

    IEnumerator Attack()
    {
        while (true)
        {
            navMeshAgent.ResetPath();
            LookRotationToTarget();

            if (Time.time - lastAttackTime > attackRate)
            {
                lastAttackTime = Time.time;

                GameObject clone = Instantiate(projectilePrefab, projectileSpawnPoint.position, projectileSpawnPoint.rotation);
                clone.GetComponent<EnemyProjectile>().Setup(target.position);
                EfxManager.Instance.PlayBullet(projectileSpawnPoint.position, projectileSpawnPoint.forward, 40f / 100f);
                PlaySound(shotSound);
            }
            yield return null;
        }
    }

    private void LookRotationToTarget()
    {
        if (target == null || eyeTransform == null) return;

        Vector3 directionToTarget = target.position - eyeTransform.position;
        directionToTarget.Normalize();
        Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
        eyeTransform.rotation = Quaternion.Slerp(eyeTransform.rotation, targetRotation, Time.deltaTime * 5f);
    }

    private void RestoreRotationToTarget()
    {
        eyeTransform.rotation = Quaternion.Slerp(eyeTransform.rotation, Quaternion.Euler(0, 0, 0), Time.deltaTime * 5f);
    }

    private EnemyState CalculateDistanceToTargetAndSelectState()
    {
        if (target == null) return currentState;

        float distance = Vector3.Distance(target.position, transform.position);

        if (distance <= attackRange)
        {
            return EnemyState.Attack;
        }
        else if (distance <= targetRecognitionRange)
        {
            return EnemyState.Pursuit;
        }
        else if (distance >= pursuitLimitRange)
        {
            return EnemyState.Wander;
        }

        return currentState;
    }

    private Vector3 CalculateWanderPosition()
    {
        float wanderRadius = 10;
        int wanderJitter = Random.Range(0, 360);
        Vector3 rangePosition = Vector3.zero;
        Vector3 rangeScale = Vector3.one * 100.0f;

        Vector3 targetPosition = transform.position + SetAngle(wanderRadius, wanderJitter);

        targetPosition.x = Mathf.Clamp(targetPosition.x, rangePosition.x - rangeScale.x * 0.5f, rangePosition.x + rangeScale.x * 0.5f);
        targetPosition.y = 0.0f;
        targetPosition.z = Mathf.Clamp(targetPosition.z, rangePosition.z - rangeScale.z * 0.5f, rangePosition.z + rangeScale.z * 0.5f);

        return targetPosition;
    }

    Vector3 SetAngle(float radius, float angle)
    {
        Vector3 position = Vector3.zero;
        position.x = Mathf.Cos(angle) * radius;
        position.z = Mathf.Sin(angle) * radius;
        return position;
    }

    public void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.black;
        Gizmos.DrawRay(new Vector3(transform.position.x, transform.position.y + 4f, transform.position.z), navMeshAgent.destination - transform.position);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, targetRecognitionRange);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, pursuitLimitRange);

        Gizmos.color = new Color(0.39f, 0.04f, 0.04f);
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}