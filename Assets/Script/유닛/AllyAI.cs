using UnityEngine;
using UnityEngine.AI;

public class AllyAI : MonoBehaviour
{
    [Header("AI Settings")]
    public float detectionRange = 15f; // 적 감지 범위
    public float stopDistance = 8f;    // 적과 유지할 거리 (무기 사거리보다 약간 짧게)
    public float updateRate = 0.5f;    // 탐색 주기 (최적화)

    private NavMeshAgent agent;
    private Transform currentTarget;
    private UnitAssembler assembler;   // 무기 사거리 가져오기용

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        assembler = GetComponent<UnitAssembler>();

        // 내 무기 사거리에 맞춰서 멈추는 거리 자동 조절 (약간의 여유 둠)
        if (assembler != null && assembler.weaponPart != null)
        {
            stopDistance = assembler.weaponPart.attackRange * 0.8f;
        }

        InvokeRepeating("ScanForEnemy", 0f, updateRate);
    }

    void Update()
    {
        // 타겟이 존재하면
        if (currentTarget != null)
        {
            float dist = Vector3.Distance(transform.position, currentTarget.position);

            // 1. 적이 너무 멀면 -> 추격
            if (dist > stopDistance)
            {
                agent.SetDestination(currentTarget.position);
            }
            // 2. 적이 사거리 안(공격 가능 거리)에 들어왔으면 -> 멈춤 (UnitAttack이 쏠 수 있게)
            else
            {
                agent.ResetPath();
                // (이때 회전은 UnitAttack 스크립트가 알아서 처리함)
            }
        }
    }

    // 가장 가까운 적을 찾는 함수
    void ScanForEnemy()
    {
        // 플레이어가 강제로 이동 명령을 내린 상태라면 AI 잠시 중지 (마우스 우클릭 우선)
        // (경로가 있고, 도착하지 않았으며, 움직이는 중이라면)
        if (agent.hasPath && agent.remainingDistance > agent.stoppingDistance)
        {
            currentTarget = null;
            return;
        }

        // 주변의 적 탐색
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, detectionRange);
        float shortestDistance = Mathf.Infinity;
        GameObject nearestEnemy = null;

        foreach (var hitCollider in hitColliders)
        {
            // 'Enemy' 태그를 가진 녀석만 찾음
            if (hitCollider.CompareTag("Enemy"))
            {
                float distanceToEnemy = Vector3.Distance(transform.position, hitCollider.transform.position);
                if (distanceToEnemy < shortestDistance)
                {
                    shortestDistance = distanceToEnemy;
                    nearestEnemy = hitCollider.gameObject;
                }
            }
        }

        if (nearestEnemy != null)
        {
            currentTarget = nearestEnemy.transform;
        }
        else
        {
            currentTarget = null;
        }
    }

    // 에디터에서 범위 확인용
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}