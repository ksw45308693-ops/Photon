using UnityEngine;

public class UnitAttack : MonoBehaviour
{
    private UnitAssembler assembler; // 조립된 스탯 가져오기용
    public Transform firePoint;      // 총알 나가는 위치
    private float fireCountdown = 0f;
    private Transform currentTarget;

    void Start()
    {
        assembler = GetComponent<UnitAssembler>();
    }

    void Update()
    {
        // 무기가 없으면 공격 불가
        if (assembler.weaponPart == null) return;

        // 1. 적 탐색
        FindNearestEnemy();

        // 2. 적이 있고 사거리 내에 있다면
        if (currentTarget != null)
        {
            // 적을 바라봄 (Smooth 하게 회전)
            Vector3 dir = currentTarget.position - transform.position;
            Quaternion lookRot = Quaternion.LookRotation(dir);
            // Y축 회전만 적용 (땅을 보거나 하늘을 보지 않게)
            Vector3 rotation = Quaternion.Lerp(transform.rotation, lookRot, Time.deltaTime * 10f).eulerAngles;
            transform.rotation = Quaternion.Euler(0f, rotation.y, 0f);

            // 공격 쿨타임 체크
            if (fireCountdown <= 0f)
            {
                Shoot();
                // 1초에 fireRate만큼 발사 (예: fireRate가 2면 0.5초마다 발사)
                fireCountdown = 1f / assembler.weaponPart.fireRate;
            }
        }

        fireCountdown -= Time.deltaTime;
    }

    void FindNearestEnemy()
    {
        // 사거리 내의 모든 콜라이더 검사
        float range = assembler.weaponPart.attackRange;
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, range);

        float shortestDistance = Mathf.Infinity;
        GameObject nearestEnemy = null;

        foreach (var hitCollider in hitColliders)
        {
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

        currentTarget = (nearestEnemy != null) ? nearestEnemy.transform : null;
    }

    void Shoot()
    {
        if (assembler.weaponPart.projectilePrefab == null) return;

        // 총알 생성
        GameObject bulletGO = Instantiate(assembler.weaponPart.projectilePrefab, firePoint.position, firePoint.rotation);

        // 총알 데미지 설정 (무기 데이터 기반)
        Projectile projectile = bulletGO.GetComponent<Projectile>();
        if (projectile != null)
        {
            projectile.damage = assembler.weaponPart.damage;
        }
    }

    // 에디터에서 사거리 눈으로 보기 (디버깅용)
    void OnDrawGizmosSelected()
    {
        if (assembler != null && assembler.weaponPart != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, assembler.weaponPart.attackRange);
        }
    }
}