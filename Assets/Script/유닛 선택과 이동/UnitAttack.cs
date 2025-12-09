using UnityEngine;
using Photon.Pun;

public class UnitAttack : MonoBehaviourPun
{
    private UnitAssembler assembler;
    private TeamEntity myTeam;

    // 공격 관련 변수
    private float fireCountdown = 0f;
    private Transform currentTarget;

    [Header("Effects")]
    public Transform firePoint; // 원거리: 발사 위치, 근접: 타격 이펙트 위치

    void Start()
    {
        assembler = GetComponent<UnitAssembler>();
        myTeam = GetComponent<TeamEntity>();
    }

    void Update()
    {
        if (!photonView.IsMine) return; // 내 유닛만 연산
        if (assembler.weaponPart == null) return;

        FindNearestEnemy();

        if (currentTarget != null)
        {
            // 1. 회전 (적을 바라봄)
            LookAtTarget();

            // 2. 공격 쿨타임 체크
            if (fireCountdown <= 0f)
            {
                // [핵심] 공격 타입에 따라 다른 행동을 함
                if (assembler.weaponPart.attackType == AttackType.Melee)
                {
                    // 근접 공격 (직접 타격)
                    photonView.RPC("RPC_MeleeAttack", RpcTarget.All);
                }
                else
                {
                    // 원거리 공격 (총알 발사)
                    photonView.RPC("RPC_RangedAttack", RpcTarget.All);
                }

                fireCountdown = 1f / assembler.weaponPart.fireRate;
            }
        }
        fireCountdown -= Time.deltaTime;
    }

    void LookAtTarget()
    {
        Vector3 dir = currentTarget.position - transform.position;
        dir.y = 0; // 높이 차이 무시 (평지 기준)
        if (dir != Vector3.zero)
        {
            Quaternion lookRot = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Lerp(transform.rotation, lookRot, Time.deltaTime * 10f);
        }
    }

    // --- [근접 공격 로직] ---
    [PunRPC]
    void RPC_MeleeAttack()
    {
        // 1. 시각 효과 (칼 휘두르는 소리나 이펙트가 있다면 여기서 재생)
        // Debug.Log($"{name}의 근접 공격! (슉!)");

        // 2. 데미지 적용 (오직 공격자 컴퓨터에서만 계산 - 중복 데미지 방지)
        if (photonView.IsMine && currentTarget != null)
        {
            // 거리가 여전히 가까운지 한 번 더 확인 (안전장치)
            float dist = Vector3.Distance(transform.position, currentTarget.position);
            // 사거리보다 약간 더 여유 있게(1.2배) 체크
            if (dist <= assembler.weaponPart.attackRange * 1.2f)
            {
                // 적의 체력 스크립트를 찾아서 직접 데미지를 줌
                UnitHealth enemyHealth = currentTarget.GetComponentInParent<UnitHealth>();
                if (enemyHealth != null)
                {
                    enemyHealth.TakeDamage(assembler.weaponPart.damage);
                }
            }
        }
    }

    // --- [원거리 공격 로직 (기존 Shoot)] ---
    [PunRPC]
    void RPC_RangedAttack()
    {
        if (assembler.weaponPart.projectilePrefab == null) return;

        GameObject bulletGO = Instantiate(assembler.weaponPart.projectilePrefab, firePoint.position, firePoint.rotation);

        Projectile projectile = bulletGO.GetComponent<Projectile>();
        if (projectile != null)
        {
            projectile.damage = assembler.weaponPart.damage;
            projectile.shooterTeamID = myTeam.GetTeamID();
            projectile.isRealBullet = photonView.IsMine;
        }
    }

    void FindNearestEnemy()
    {
        float range = assembler.weaponPart.attackRange;
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, range);

        float shortestDistance = Mathf.Infinity;
        GameObject nearestEnemy = null;

        foreach (var hitCollider in hitColliders)
        {
            TeamEntity targetTeam = hitCollider.GetComponentInParent<TeamEntity>();

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

    void OnDrawGizmosSelected()
    {
        if (assembler != null && assembler.weaponPart != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, assembler.weaponPart.attackRange);
        }
    }
}