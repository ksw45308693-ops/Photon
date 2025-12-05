using UnityEngine;
using UnityEngine.AI;

public class LaneAI : MonoBehaviour
{
    [Header("Target Info")]
    public string enemyBaseTag; // 자동 설정됨
    public Transform targetWaypoint;
    public Transform targetBase;

    [Header("Combat")]
    public float aggroRange = 5f;

    private NavMeshAgent agent;
    private UnitAssembler assembler;
    private Transform currentEnemy;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        assembler = GetComponent<UnitAssembler>();

        // --- [수정] 태그 자동 설정 기능 추가 ---
        if (gameObject.CompareTag("Player"))
        {
            enemyBaseTag = "Enemy"; // 나는 플레이어니까 적은 Enemy
        }
        else if (gameObject.CompareTag("Enemy"))
        {
            enemyBaseTag = "Player"; // 나는 적이니까 적(상대)은 Player
        }
        else
        {
            enemyBaseTag = "Enemy"; // 기본값
        }
        // ------------------------------------

        // 1. 적 기지 찾기 (태그로 찾음)
        GameObject baseObj = GameObject.FindGameObjectWithTag(enemyBaseTag);
        if (baseObj != null)
        {
            targetBase = baseObj.transform;
        }
        else
        {
            // 적 기지를 못 찾으면 로그 띄우기 (디버깅용)
            Debug.LogWarning($"{name}이(가) {enemyBaseTag} 태그를 가진 기지를 못 찾았습니다!");
        }

        // 2. 가장 가까운 다리 찾기
        FindNearestBridge();
    }

    void Update()
    {
        // ... (나머지 코드는 기존과 동일, 그대로 두셔도 됩니다) ...
        // 아래 내용은 기존 코드를 그대로 유지하세요.

        CheckNearbyEnemies();

        if (currentEnemy != null)
        {
            float dist = Vector3.Distance(transform.position, currentEnemy.position);
            float attackRange = (assembler.weaponPart != null) ? assembler.weaponPart.attackRange : 2f;

            if (dist > attackRange * 0.6f)
            {
                agent.isStopped = false;
                agent.SetDestination(currentEnemy.position);
            }
            else
            {
                agent.isStopped = true;
                Vector3 dir = currentEnemy.position - transform.position;
                dir.y = 0;
                if (dir != Vector3.zero)
                {
                    Quaternion lookRot = Quaternion.LookRotation(dir);
                    transform.rotation = Quaternion.Lerp(transform.rotation, lookRot, Time.deltaTime * 10f);
                }
            }
            return;
        }

        agent.isStopped = false;

        if (targetWaypoint != null)
        {
            agent.SetDestination(targetWaypoint.position);
            if (Vector3.Distance(transform.position, targetWaypoint.position) < 3f)
            {
                targetWaypoint = null;
            }
        }
        else if (targetBase != null)
        {
            agent.SetDestination(targetBase.position);
        }
    }

    // ... (FindNearestBridge, CheckNearbyEnemies 함수들은 기존 코드 유지) ...
    // 아래 함수들은 기존 파일 내용 그대로 두시면 됩니다.

    void FindNearestBridge()
    {
        GameObject leftBridge = GameObject.Find("Waypoint_Left");
        GameObject rightBridge = GameObject.Find("Waypoint_Right");

        if (leftBridge == null || rightBridge == null) return;

        float distLeft = Vector3.Distance(transform.position, leftBridge.transform.position);
        float distRight = Vector3.Distance(transform.position, rightBridge.transform.position);

        if (distLeft < distRight) targetWaypoint = leftBridge.transform;
        else targetWaypoint = rightBridge.transform;
    }

    void CheckNearbyEnemies()
    {
        if (currentEnemy != null) return;

        Collider[] hits = Physics.OverlapSphere(transform.position, aggroRange);
        foreach (var hit in hits)
        {
            // 수정된 로직 반영
            if (hit.CompareTag(enemyBaseTag))
            {
                if (targetWaypoint != null && (hit.name.Contains("Base") || hit.name.Contains("Tower")))
                {
                    continue;
                }
                currentEnemy = hit.transform;
                break;
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, aggroRange);
    }
}