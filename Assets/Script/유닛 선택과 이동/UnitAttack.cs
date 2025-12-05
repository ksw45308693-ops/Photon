using UnityEngine;

public class UnitAttack : MonoBehaviour
{
    private UnitAssembler assembler;
    public Transform firePoint;
    private float fireCountdown = 0f;
    private Transform currentTarget;

    // 공격 대상 태그 (자동 설정됨)
    private string enemyTag;

    void Start()
    {
        assembler = GetComponent<UnitAssembler>();

        // --- 내 태그를 보고 적 태그를 자동 결정 ---
        if (gameObject.CompareTag("Player"))
        {
            enemyTag = "Enemy";
        }
        else if (gameObject.CompareTag("Enemy"))
        {
            enemyTag = "Player";
        }
        else
        {
            enemyTag = "Enemy"; // 안전장치
        }
    }

    void Update()
    {
        // 부품이 없으면 공격 불가
        if (assembler == null || assembler.weaponPart == null) return;

        FindNearestEnemy();

        if (currentTarget != null)
        {
            // 적을 바라보게 회전
            Vector3 dir = currentTarget.position - transform.position;
            Quaternion lookRot = Quaternion.LookRotation(dir);
            Vector3 rotation = Quaternion.Lerp(transform.rotation, lookRot, Time.deltaTime * 10f).eulerAngles;
            transform.rotation = Quaternion.Euler(0f, rotation.y, 0f);

            // 발사 타이머 체크
            if (fireCountdown <= 0f)
            {
                Shoot();
                fireCountdown = 1f / assembler.weaponPart.fireRate;
            }
        }

        fireCountdown -= Time.deltaTime;
    }

    // --- [수정된 부분] 우선순위 타겟팅 로직 (유닛 > 기지) ---
    void FindNearestEnemy()
    {
        float range = assembler.weaponPart.attackRange;
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, range);

        GameObject bestTargetUnit = null; // 우선순위 1: 유닛
        GameObject bestTargetBase = null; // 우선순위 2: 기지(건물)

        float closestUnitDist = Mathf.Infinity;
        float closestBaseDist = Mathf.Infinity;

        foreach (var hit in hitColliders)
        {
            // 1. 나의 적 태그인지 확인
            if (hit.CompareTag(enemyTag))
            {
                float dist = Vector3.Distance(transform.position, hit.transform.position);

                // 2. 이 적이 '기지'인지 이름으로 판단
                // (주의: 기지 오브젝트 이름에 "Base"나 "Tower"가 포함되어 있어야 합니다!)
                bool isBase = hit.name.Contains("Base") || hit.name.Contains("Tower");

                if (isBase)
                {
                    // 기지라면 -> 가장 가까운 기지 기억
                    if (dist < closestBaseDist)
                    {
                        closestBaseDist = dist;
                        bestTargetBase = hit.gameObject;
                    }
                }
                else
                {
                    // 유닛(기지가 아님)이라면 -> 가장 가까운 유닛 기억
                    if (dist < closestUnitDist)
                    {
                        closestUnitDist = dist;
                        bestTargetUnit = hit.gameObject;
                    }
                }
            }
        }

        // [핵심 로직] 최종 타겟 결정
        // 사거리 안에 '유닛'이 하나라도 있으면 -> 무조건 유닛을 때림 (기지가 더 가까워도 무시)
        if (bestTargetUnit != null)
        {
            currentTarget = bestTargetUnit.transform;
        }
        // 유닛이 없으면 -> 그때서야 기지를 때림
        else if (bestTargetBase != null)
        {
            currentTarget = bestTargetBase.transform;
        }
        else
        {
            currentTarget = null;
        }
    }

    void Shoot()
    {
        if (assembler.weaponPart.projectilePrefab == null) return;

        GameObject bulletGO = Instantiate(assembler.weaponPart.projectilePrefab, firePoint.position, firePoint.rotation);

        Projectile projectile = bulletGO.GetComponent<Projectile>();
        if (projectile != null)
        {
            projectile.damage = assembler.weaponPart.damage;
            projectile.targetTag = enemyTag; // 총알에게 적 태그 전달
        }
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