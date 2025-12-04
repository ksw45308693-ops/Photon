using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    [Header("Settings")]
    public float chaseRange = 20f;   // 주변 탐색 범위
    public string targetTag = "Player";

    private NavMeshAgent agent;
    private Transform currentTarget;
    private Transform mainBaseTarget; // [추가] 멀리 있는 적 기지 위치

    private float updateRate = 0.5f;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        // [추가] 맵 전체에서 'Player' 태그를 가진 기지(MainBase)를 미리 찾아둠
        GameObject baseObj = GameObject.Find("MainBase"); // 이름으로 찾기
        if (baseObj != null) mainBaseTarget = baseObj.transform;

        InvokeRepeating("UpdateTarget", 0f, updateRate);
    }

    void Update()
    {
        // 1. 주변에 적 유닛이 있으면 걔를 쫓아감 (우선순위 1)
        if (currentTarget != null)
        {
            agent.SetDestination(currentTarget.position);
        }
        // 2. 주변에 아무도 없으면? -> 적 기지로 돌격! (우선순위 2)
        else if (mainBaseTarget != null)
        {
            agent.SetDestination(mainBaseTarget.position);
        }
    }

    void UpdateTarget()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag(targetTag);
        float shortestDistance = Mathf.Infinity;
        GameObject nearestPlayer = null;

        foreach (GameObject player in players)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

            // chaseRange 안에서 가장 가까운 적 찾기
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
            currentTarget = null; // 주변에 없으면 null -> Update에서 기지로 이동함
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, chaseRange);
    }
}