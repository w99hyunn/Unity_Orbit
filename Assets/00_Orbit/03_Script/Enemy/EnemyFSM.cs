using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

namespace STARTING
{
    public enum EnemyState
    { 
        None = -1, 
        Idle = 0, 
        Wander, 
        Pursuit, 
        Attack 
    }

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

        public System.Action OnDeath;
        private EnemyMemoryPool spawnPool; // 자신을 생성한 Pool을 기록(구역 트리거)

        public UnityEvent OnSetup;

        [Header("떠다니는 높이(y)")]
        public float floatAmplitude = 0.5f; // 떠다니는 높이

        [Header("떠다니는 속도(y)")]
        public float floatFrequency = 1f; // 떠다니는 속도
        private float timeOffset; // 각 오브젝트의 시간 오프셋

        [Header("플레이어 인식시 몬스터 머티리얼 변경")]
        public Material newMaterial;           // 새로 교체할 Material
        public MeshRenderer meshRenderer;     // MeshRenderer 참조를 저장할 변수
        private Material[] originalMaterials;  // 원래의 모든 Material을 저장할 배열


        private void Start()
        {
            audioSource = GetComponent<AudioSource>();
            ChangeState(EnemyState.Idle);

            if (meshRenderer != null)
            {
                originalMaterials = meshRenderer.materials;
            }
        }

        private void Update()
        {
            EnemyState newState = CalculateDistanceToTargetAndSelectState();
            if (newState != currentState)
            {
                ChangeState(newState);
            }
            // y축 움직임 (딜레이 적용)
            float newY = transform.position.y + Mathf.Sin((Time.time + timeOffset) * floatFrequency) * floatAmplitude;
            transform.position = new Vector3(transform.position.x, newY, transform.position.z);
        }

        public void Setup(Transform target, EnemyMemoryPool pool)
        {
            navMeshAgent = GetComponent<NavMeshAgent>();
            navMeshAgent.updateRotation = false;
            this.target = target;
            spawnPool = pool; // 자신을 생성한 Pool 저장
            ResetState();

            timeOffset = Random.Range(1f, 8f);
        }

        // 몬스터의 상태를 초기화하는 메서드
        public void ResetState()
        {
            transform.rotation = Quaternion.identity; // 회전값 초기화
            eyeTransform.localRotation = Quaternion.identity; // 시선 방향 초기화
            OnSetup.Invoke();
        }

        // 몬스터 죽었을 때
        public void Die()
        {
            OnDeath?.Invoke(); // 사망 시 풀로 반환
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

        IEnumerator Wander()
        {
            float currentTime = 0;
            float maxTime = 10;

            navMeshAgent.speed = 1.5f;
            navMeshAgent.SetDestination(CalculateWanderPosition());

            while (currentTime < maxTime)
            {
                currentTime += Time.deltaTime;

                Vector3 direction = navMeshAgent.velocity;

                if (direction.sqrMagnitude < 0.01f)
                {
                    // 목표 위치에 도착하면 Idle 상태로 전환
                    ChangeState(EnemyState.Idle);
                    yield break;
                }

                if (CalculateDistanceToTargetAndSelectState() != EnemyState.Wander)
                {
                    // 다른 상태로 전환해야 할 경우
                    ChangeState(CalculateDistanceToTargetAndSelectState());
                    yield break;
                }

                if (direction.sqrMagnitude > 0.01f)
                {
                    Quaternion targetRotation = Quaternion.LookRotation(direction);
                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
                }

                yield return null;
            }

            // 일정 시간이 지나면 Idle 상태로 전환
            ChangeState(EnemyState.Idle);
        }


        IEnumerator Idle()
        {
            RestoreOriginalMaterial();
            while (true)
            {
                yield return new WaitForSeconds(Random.Range(1, 5));

                // Idle 상태 중에도 상태를 계산해 전환
                EnemyState newState = CalculateDistanceToTargetAndSelectState();
                if (newState != EnemyState.Idle)
                {
                    ChangeState(newState);
                    yield break;
                }

                RestoreRotationToTarget();
                yield return StartCoroutine(Wander());
            }
        }

        IEnumerator Pursuit()
        {
            ChangeMaterial();
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
                if (PlayerStats.Instance.playerState == PlayerState.IDLE || PlayerStats.Instance.playerState == PlayerState.PAUSE)
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
                else
                {
                    yield return null;
                }
            }
        }

        public void ChangeMaterial()
        {
            if (meshRenderer != null && newMaterial != null)
            {
                Material[] materials = meshRenderer.materials;

                if (2 >= 0 && 2 < materials.Length)
                {
                    materials[2] = newMaterial;
                    meshRenderer.materials = materials;
                }

            }
        }

        public void RestoreOriginalMaterial()
        {
            if (meshRenderer != null && originalMaterials != null)
            {
                meshRenderer.materials = originalMaterials;
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
#if UNITY_EDITOR
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
#endif
    }
}