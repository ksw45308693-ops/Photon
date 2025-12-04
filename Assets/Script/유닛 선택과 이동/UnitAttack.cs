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

        // --- [중요] 내 태그를 보고 적 태그를 자동 결정 ---
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
            // 태그가 없으면 기본값으로 Enemy 설정 (안전장치)
            enemyTag = "Enemy";
        }
    }

    void Update()
    {
        if (assembler.weaponPart == null) return;

        FindNearestEnemy();

        if (currentTarget != null)
        {
            Vector3 dir = currentTarget.position - transform.position;
            Quaternion lookRot = Quaternion.LookRotation(dir);
            Vector3 rotation = Quaternion.Lerp(transform.rotation, lookRot, Time.deltaTime * 10f).eulerAngles;
            transform.rotation = Quaternion.Euler(0f, rotation.y, 0f);

            if (fireCountdown <= 0f)
            {
                Shoot();
                fireCountdown = 1f / assembler.weaponPart.fireRate;
            }
        }

        fireCountdown -= Time.deltaTime;
    }

    void FindNearestEnemy()
    {
        float range = assembler.weaponPart.attackRange;
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, range);

        float shortestDistance = Mathf.Infinity;
        GameObject nearestEnemy = null;

        foreach (var hitCollider in hitColliders)
        {
            // --- [수정] 위에서 결정한 enemyTag를 가진 애만 찾음 ---
            if (hitCollider.CompareTag(enemyTag))
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

        GameObject bulletGO = Instantiate(assembler.weaponPart.projectilePrefab, firePoint.position, firePoint.rotation);

        Projectile projectile = bulletGO.GetComponent<Projectile>();
        if (projectile != null)
        {
            projectile.damage = assembler.weaponPart.damage;

            // --- [핵심] 총알에게 "누구를 맞춰야 하는지" 알려줌 ---
            projectile.targetTag = enemyTag;
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