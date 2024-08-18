using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

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

    //private Status status;
    private NavMeshAgent navMeshAgent;
    private Transform target; //적 공격 대상

    //private void Awake()
    public void Setup(Transform target)
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        //status = GetComponent<Status>();
        navMeshAgent.updateRotation = false;
        this.target = target;
    }

    private void OnEnable()
    {
        ChangeState(EnemyState.Idle);
    }

    private void OnDisable()
    {
        StopCoroutine(enemyState.ToString());
        enemyState = EnemyState.None;
    }

    public void ChangeState(EnemyState newState)
    {
        if (enemyState == newState) return;

        StopCoroutine(enemyState.ToString());
        enemyState = newState;

        StartCoroutine(enemyState.ToString());
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
        // 1~4초 시간 대기
        int changeTime = Random.Range(1, 5);

        yield return new WaitForSeconds(changeTime);

        ChangeState(EnemyState.Wander);
    }

    IEnumerator Wander()
    {
        float currentTime = 0;
        float maxTime = 10;

        // 이동 속도 설정
        //navMeshAgent.speed = status.WalkSpeed;
        navMeshAgent.speed = 1.5f;

        // 목표 위치 설정
        navMeshAgent.SetDestination(CalculateWanderPosition());

        // 목표 위치로 회전
        Vector3 to = new Vector3(navMeshAgent.destination.x, 0, navMeshAgent.destination.z);
        Vector3 from = new Vector3(transform.position.x, 0, transform.position.z);
        transform.rotation = Quaternion.LookRotation(to - from);

        while (true)
        {
            currentTime += Time.deltaTime;

            // 목표위치에 근접하게 도달하거나 너무 오랜시간동안 배회하기 상태에 머물러 있으면
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

        float wanderRadius = 10;// 현재 위치를 원점으로 하는 원의 반지름
        int wanderJitter = 0;// 선택된 각도 (wanderJitterMin ~ wanderJitterMax)
        int wanderJitterMin = 0;// 최소 각도
        int wanderJitterMax = 360;// 최대 각도

        // 현재 적 캐릭터가 있는 월드의 중심 위치와 크기 (구역을 벗어난 행동을 하지 않도록)
        Vector3 rangePosition = Vector3.zero;
        Vector3 rangeScale = Vector3.one * 100.0f;

        // 자신의 위치를 중심으로 반지름(wanderRadius) 거리, 선택된 각도(wanderJitter)에 위치한 좌표를 목표지점으로 설정
        wanderJitter = Random.Range(wanderJitterMin, wanderJitterMax);
        Vector3 targetPosition = transform.position + SetAngle(wanderRadius, wanderJitter);

        // 생성된 목표위치가 자신의 이동구역을 벗어나지 않게 조절
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

    private IEnumerator Pursuit()
    {
        while (true)
        {
            navMeshAgent.speed = 3f;

            navMeshAgent.SetDestination(target.position);

            LookRotationToTarget(); //타겟방향 주시

            CalculateDistanceToTargetAndSelectState(); //타겟과의 거리에 따라 행동 선택

            yield return null;
        }
    }

    IEnumerator Attack()
    {
        navMeshAgent.ResetPath();

        while(true)
        {
            LookRotationToTarget();
            CalculateDistanceToTargetAndSelectState();

            if (Time.time - lastAttackTime > attackRate)
            {
                lastAttackTime = Time.time;

                GameObject clone = Instantiate(projectilePrefab, projectileSpawnPoint.position, projectileSpawnPoint.rotation);
                clone.GetComponent<EnemyProjectile>().Setup(target.position);
            }
            yield return null;
        }
    }

    private void LookRotationToTarget()
    {
        // Target의 위치를 향한 벡터 계산 (x, y, z 모두 포함)
        Vector3 directionToTarget = target.position - transform.position;

        // directionToTarget 벡터의 길이를 1로 설정 (정규화)
        directionToTarget.Normalize();

        // 방향 벡터로부터의 회전 계산
        Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);

        // 회전을 적용하되, x축 회전을 포함하여 적용
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);

    }

    private void CalculateDistanceToTargetAndSelectState()
    {
        if (target == null) return;

        //플레이어와 적 사이 거리
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
        Gizmos.color = Color.black;
        Gizmos.DrawRay(new Vector3(transform.position.x, transform.position.y+4f, transform.position.z), navMeshAgent.destination - transform.position);

        //목표인식 범위
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, targetRecognitionRange);

        //추적범위
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, pursuitLimitRange);

        //공격범위
        Gizmos.color = new Color(0.39f, 0.04f, 0.04f);
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }

}
