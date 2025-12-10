using UnityEngine;
using Photon.Pun;

public class UnitAttack : MonoBehaviourPun
{
    private UnitStat unitStat; // [변경] Assembler 대신 Stat 참조
    private TeamEntity myTeam;

    private float fireCountdown = 0f;
    private Transform currentTarget;

    void Start()
    {
        unitStat = GetComponent<UnitStat>(); // 내 능력치 가져오기
        myTeam = GetComponent<TeamEntity>();
    }

    void Update()
    {
        if (!photonView.IsMine) return;
        if (unitStat == null) return;

        FindNearestEnemy();

        if (currentTarget != null)
        {
            LookAtTarget();

            if (fireCountdown <= 0f)
            {
                // 사거리 체크 (안전장치)
                float dist = Vector3.Distance(transform.position, currentTarget.position);
                if (dist <= unitStat.attackRange)
                {
                    if (unitStat.projectilePrefab == null)
                    {
                        photonView.RPC("RPC_MeleeAttack", RpcTarget.All);
                    }
                    else
                    {
                        photonView.RPC("RPC_RangedAttack", RpcTarget.All);
                    }
                    // 쿨타임 계산 (1초 / 공격속도)
                    fireCountdown = 1f / unitStat.attackSpeed;
                }
            }
        }
        fireCountdown -= Time.deltaTime;
    }

    void LookAtTarget()
    {
        Vector3 dir = currentTarget.position - transform.position;
        dir.y = 0;
        if (dir != Vector3.zero)
        {
            Quaternion lookRot = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Lerp(transform.rotation, lookRot, Time.deltaTime * 10f);
        }
    }

    [PunRPC]
    void RPC_MeleeAttack()
    {
        // 근접 공격 사운드/이펙트 추가 가능
        if (photonView.IsMine && currentTarget != null)
        {
            // 타겟이 아직 살아있는지 확인
            UnitHealth enemyHealth = currentTarget.GetComponentInParent<UnitHealth>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(unitStat.attackDamage);
            }
        }
    }

    [PunRPC]
    void RPC_RangedAttack()
    {
        if (unitStat.projectilePrefab == null) return;

        // 발사 위치가 없으면 내 위치에서 발사
        Vector3 spawnPos = (unitStat.firePoint != null) ? unitStat.firePoint.position : transform.position;

        GameObject bulletGO = Instantiate(unitStat.projectilePrefab, spawnPos, transform.rotation);
        Projectile projectile = bulletGO.GetComponent<Projectile>();

        if (projectile != null)
        {
            projectile.damage = unitStat.attackDamage;
            projectile.shooterTeamID = myTeam.GetTeamID();
            projectile.isRealBullet = photonView.IsMine;
        }
    }

    void FindNearestEnemy()
    {
        // 내 사거리를 Stat에서 가져옴
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, unitStat.attackRange);

        float shortestDistance = Mathf.Infinity;
        GameObject nearestEnemy = null;

        foreach (var hitCollider in hitColliders)
        {
            TeamEntity targetTeam = hitCollider.GetComponentInParent<TeamEntity>();
            // 적이고 살아있는지 확인
            if (targetTeam != null && targetTeam.GetTeamID() != myTeam.GetTeamID())
            {
                float distanceToEnemy = Vector3.Distance(transform.position, hitCollider.transform.position);
                if (distanceToEnemy < shortestDistance)
                {
                    shortestDistance = distanceToEnemy;
                    nearestEnemy = hitCollider.gameObject;
                }
            }
        }
        currentTarget = (nearestEnemy != null) ? nearestEnemy.transform : null;
    }
}