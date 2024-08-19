using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;
using static UnityEngine.UI.Image;

public enum EnemyState { None = -1, Idle = 0, Wander, Pursuit, Attack, }

public class EnemyFSM : MonoBehaviour
{
    [Header("Pursuit")]
    public float targetRecognitionRange = 8; //pursuit으로 변경될 범위
    public float pursuitLimitRange = 10; //추적 범위 > 이 범위 나가면 Wander

    [Header("Attack")]
    public GameObject projectilePrefab;
    public Transform projectileSpawnPoint;
    public float attackRange = 5; // 공격 범위
    public float attackRate = 1; // 공속

    private EnemyState enemyState = EnemyState.None;
    private float lastAttackTime = 0; //공격주기

    private NavMeshAgent navMeshAgent;
    private Transform target; //적 공격 대상

    private AudioSource audioSource;
    public AudioClip shotSound;

    [Header("하위 모델링 회전 관련")]
    public Transform eyeTransform;

    // 현재 실행 중인 코루틴을 추적
    private IEnumerator currentCoroutine;

    public void Setup(Transform target)
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.updateRotation = false;
        this.target = target;
    }

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void OnEnable()
    {
        ChangeState(EnemyState.Idle);
    }

    private void OnDisable()
    {
        // 활성화된 모든 코루틴을 안전하게 종료
        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
            currentCoroutine = null;
        }
        enemyState = EnemyState.None;
    }

    public void ChangeState(EnemyState newState)
    {
        if (enemyState == newState) return;

        // 이전 코루틴을 안전하게 종료
        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
            currentCoroutine = null;
        }

        enemyState = newState;

        // 새로운 상태에 맞는 코루틴을 시작
        currentCoroutine = HandleState(newState);
        StartCoroutine(currentCoroutine);
    }

    private IEnumerator HandleState(EnemyState state)
    {
        switch (state)
        {
            case EnemyState.Idle:
                return Idle();
            case EnemyState.Wander:
                return Wander();
            case EnemyState.Pursuit:
                return Pursuit();
            case EnemyState.Attack:
                return Attack();
            default:
                return null;
        }
    }

    IEnumerator Idle()
    {
        StartCoroutine("AutoChangeFromIdleToWander");

        while (true)
        {
            CalculateDistanceToTargetAndSelectState();
            yield return null;
        }
    }

    IEnumerator AutoChangeFromIdleToWander()
    {
        int changeTime = Random.Range(1, 5);

        yield return new WaitForSeconds(changeTime);
        RestoreRotationToTarget();
        ChangeState(EnemyState.Wander);
    }

    IEnumerator Wander()
    {
        float currentTime = 0;
        float maxTime = 10;

        navMeshAgent.speed = 1.5f;
        navMeshAgent.SetDestination(CalculateWanderPosition());

        Vector3 to = new Vector3(navMeshAgent.destination.x, 0, navMeshAgent.destination.z);
        Vector3 from = new Vector3(transform.position.x, 0, transform.position.z);
        transform.rotation = Quaternion.LookRotation(to - from);

        while (true)
        {
            currentTime += Time.deltaTime;

            to = new Vector3(navMeshAgent.destination.x, 0, navMeshAgent.destination.z);
            from = new Vector3(transform.position.x, 0, transform.position.z);
            if ((to - from).sqrMagnitude < 0.01f || currentTime >= maxTime)
            {
                ChangeState(EnemyState.Idle);
            }
            CalculateDistanceToTargetAndSelectState();
            yield return null;
        }
    }

    private Vector3 CalculateWanderPosition()
    {
        float wanderRadius = 10;
        int wanderJitter = Random.Range(0, 360);

        Vector3 rangePosition = transform.position;
        Vector3 rangeScale = Vector3.one * 100.0f;

        Vector3 targetPosition = transform.position + SetAngle(wanderRadius, wanderJitter);

        targetPosition.x = Mathf.Clamp(targetPosition.x, rangePosition.x - rangeScale.x * 0.5f, rangePosition.x + rangeScale.x * 0.5f);
        targetPosition.y = 0.0f;
        targetPosition.z = Mathf.Clamp(targetPosition.z, rangePosition.z - rangeScale.z * 0.5f, rangePosition.z + rangeScale.z * 0.5f);

        return targetPosition;
    }

    Vector3 SetAngle(float radius, float angle)
    {
        Vector3 position = transform.position;

        position.x = Mathf.Cos(angle) * radius;
        position.z = Mathf.Sin(angle) * radius;

        return position;
    }

    private IEnumerator Pursuit()
    {
        navMeshAgent.speed = 3f;

        while (true)
        {
            if (navMeshAgent.destination != target.position)
            {
                navMeshAgent.SetDestination(target.position);
            }

            LookRotationToTarget();
            CalculateDistanceToTargetAndSelectState();

            yield return null;
        }
    }

    IEnumerator Attack()
    {
        navMeshAgent.ResetPath();

        while (true)
        {
            LookRotationToTarget();
            CalculateDistanceToTargetAndSelectState();

            if (Time.time - lastAttackTime > attackRate)
            {
                lastAttackTime = Time.time;

                GameObject clone = Instantiate(projectilePrefab, projectileSpawnPoint.position, projectileSpawnPoint.rotation);
                clone.GetComponent<EnemyProjectile>().Setup(target.position);
                EfxManager.Instance.PlayBullet(projectileSpawnPoint.position, projectileSpawnPoint.forward, 40f / 100f);
                PlaySound(shotSound);

                Destroy(clone, 5f); // 5초 후 발사체 제거
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

    private void CalculateDistanceToTargetAndSelectState()
    {
        if (target == null) return;

        float distance = Vector3.Distance(target.position, transform.position);

        if (distance <= attackRange)
        {
            ChangeState(EnemyState.Attack);
        }
        else if (distance <= targetRecognitionRange)
        {
            ChangeState(EnemyState.Pursuit);
        }
        else if (distance >= pursuitLimitRange)
        {
            ChangeState(EnemyState.Wander);
        }
    }

    private void OnDrawGizmos()
    {
#if UNITY_EDITOR
        Gizmos.color = Color.black;
        Gizmos.DrawRay(new Vector3(transform.position.x, transform.position.y + 4f, transform.position.z), navMeshAgent.destination - transform.position);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, targetRecognitionRange);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, pursuitLimitRange);

        Gizmos.color = new Color(0.39f, 0.04f, 0.04f);
        Gizmos.DrawWireSphere(transform.position, attackRange);
#endif
    }

    public void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }
}
