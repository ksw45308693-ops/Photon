using UnityEngine;
using UnityEngine.AI; // 필수

public class EnemyAI : MonoBehaviour
{
    [Header("Settings")]
    public float chaseRange = 20f;   // 적이 플레이어를 인식하는 범위
    public string targetTag = "Player"; // 쫓아갈 대상의 태그

    private NavMeshAgent agent;
    private Transform currentTarget;
    private float updateRate = 0.5f; // 0.5초마다 타겟 갱신 (성능 최적화)

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        // 0.5초마다 "가장 가까운 타겟 찾기" 함수 실행
        InvokeRepeating("UpdateTarget", 0f, updateRate);
    }

    void Update()
    {
        // 타겟이 있으면 쫓아감
        if (currentTarget != null)
        {
            agent.SetDestination(currentTarget.position);
        }
        else
        {
            // 타겟이 없거나 죽어서 사라지면 제자리에 멈춤
            agent.ResetPath();
        }
    }

    // 가장 가까운 플레이어를 찾는 함수
    void UpdateTarget()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag(targetTag);
        float shortestDistance = Mathf.Infinity;
        GameObject nearestPlayer = null;

        foreach (GameObject player in players)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

            // 인식 범위(chaseRange) 안이고, 지금까지 찾은 것보다 더 가까우면 갱신
            if (distanceToPlayer < shortestDistance && distanceToPlayer <= chaseRange)
            {
                shortestDistance = distanceToPlayer;
                nearestPlayer = player;
            }
        }

        if (nearestPlayer != null)
        {
            currentTarget = nearestPlayer.transform;
        }
        else
        {
            currentTarget = null;
        }
    }

    // 에디터에서 인식 범위 눈으로 보기
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, chaseRange);
    }
}