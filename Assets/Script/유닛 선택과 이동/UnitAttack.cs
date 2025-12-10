using UnityEngine;
using Photon.Pun;

public class UnitAttack : MonoBehaviourPun
{
    private UnitStatController statController;
    private TeamEntity myTeam;

    private float fireCountdown = 0f;
    private Transform currentTarget;

    public Transform firePoint;

    // 적을 찾는 주기 (매 프레임 찾으면 성능 저하가 올 수 있음)
    private float updateTargetCount = 0f;

    void Start()
    {
        statController = GetComponent<UnitStatController>();
        myTeam = GetComponent<TeamEntity>();
    }

    void Update()
    {
        if (!photonView.IsMine) return;
        // 데이터가 없으면 공격 불가
        if (statController == null || statController.unitData == null) return;

        // 매 프레임 실행하면 무거울 수 있으므로 0.5초마다 타겟 갱신 (선택 사항)
        updateTargetCount -= Time.deltaTime;
        if (updateTargetCount <= 0f)
        {
            FindNearestEnemy();
            updateTargetCount = 0.5f;
        }

        if (currentTarget != null)
        {
            LookAtTarget();

            if (fireCountdown <= 0f)
            {
                // [변경] 데이터에서 근접/원거리 여부 확인
                if (statController.unitData.isMelee)
                {
                    photonView.RPC("RPC_MeleeAttack", RpcTarget.All);
                }
                else
                {
                    photonView.RPC("RPC_RangedAttack", RpcTarget.All);
                }

                // [변경] 데이터에서 공격 속도 가져오기
                fireCountdown = 1f / statController.unitData.fireRate;
            }
        }

        fireCountdown -= Time.deltaTime;
    }

    // [추가됨] 가장 가까운 적을 찾는 로직
    void FindNearestEnemy()
    {
        // 데이터에서 사거리 가져오기
        float range = statController.unitData.attackRange;

        // 사거리 내의 모든 콜라이더 검출
        Collider[] colliders = Physics.OverlapSphere(transform.position, range);

        float shortestDistance = Mathf.Infinity;
        Transform nearestEnemy = null;

        foreach (Collider col in colliders)
        {
            // 자기 자신은 제외
            if (col.gameObject == gameObject) continue;

            // 상대방이 TeamEntity를 가지고 있는지 확인
            TeamEntity targetTeam = col.GetComponent<TeamEntity>();

            // TeamEntity가 있고, 나와 팀이 다를 경우에만 적으로 간주
            if (targetTeam != null && targetTeam.GetTeamID() != myTeam.GetTeamID())
            {
                // 거리를 계산해서 가장 가까운 적을 찾음
                float distanceToEnemy = Vector3.Distance(transform.position, col.transform.position);
                if (distanceToEnemy < shortestDistance)
                {
                    shortestDistance = distanceToEnemy;
                    nearestEnemy = col.transform;
                }
            }
        }

        // 가장 가까운 적을 타겟으로 설정
        currentTarget = nearestEnemy;
    }

    // [추가됨] 타겟을 바라보는 로직
    void LookAtTarget()
    {
        if (currentTarget == null) return;

        Vector3 dir = currentTarget.position - transform.position;
        // 높이 차이는 무시하고 수평 회전만 하려면 y를 0으로 설정
        dir.y = 0;

        if (dir != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(dir);
            // 부드럽게 회전 (Time.deltaTime * 속도)
            Vector3 rotation = Quaternion.Lerp(transform.rotation, lookRotation, Time.deltaTime * 10f).eulerAngles;
            transform.rotation = Quaternion.Euler(0f, rotation.y, 0f);
        }
    }

    [PunRPC]
    void RPC_RangedAttack()
    {
        GameObject prefab = statController.unitData.projectilePrefab;
        if (prefab == null) return;

        GameObject bulletGO = Instantiate(prefab, firePoint.position, firePoint.rotation);
        Projectile projectile = bulletGO.GetComponent<Projectile>();

        if (projectile != null)
        {
            projectile.damage = statController.unitData.damage;
            projectile.shooterTeamID = myTeam.GetTeamID();
            projectile.isRealBullet = photonView.IsMine;
        }
    }

    [PunRPC]
    void RPC_MeleeAttack()
    {
        if (photonView.IsMine && currentTarget != null)
        {
            float dist = Vector3.Distance(transform.position, currentTarget.position);
            // 사거리 내에 있는지 다시 확인 (약간의 오차 허용 * 1.5f)
            if (dist <= statController.unitData.attackRange * 1.5f)
            {
                UnitHealth enemyHealth = currentTarget.GetComponent<UnitHealth>();
                // 혹은 부모에 Health가 있다면: currentTarget.GetComponentInParent<UnitHealth>();

                if (enemyHealth != null)
                {
                    enemyHealth.TakeDamage(statController.unitData.damage);
                }
            }
        }
    }

    // [선택] 에디터에서 공격 사거리를 눈으로 보기 위한 기즈모
    void OnDrawGizmosSelected()
    {
        if (statController != null && statController.unitData != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, statController.unitData.attackRange);
        }
    }
}