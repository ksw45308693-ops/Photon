using UnityEngine;
using Photon.Pun;

public class UnitAttack : MonoBehaviourPun
{
    private UnitAssembler assembler;
    public Transform firePoint;
    private float fireCountdown = 0f;
    private Transform currentTarget;
    private TeamEntity myTeam;

    void Start()
    {
        assembler = GetComponent<UnitAssembler>();
        myTeam = GetComponent<TeamEntity>();
    }

    void Update()
    {
        if (!photonView.IsMine) return;
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
                // [중요] 공격 시 네트워크로 발사 명령 전송
                photonView.RPC("RPC_Shoot", RpcTarget.All);
                fireCountdown = 1f / assembler.weaponPart.fireRate;
            }
        }
        fireCountdown -= Time.deltaTime;
    }

    [PunRPC]
    void RPC_Shoot()
    {
        if (assembler.weaponPart.projectilePrefab == null) return;

        GameObject bulletGO = Instantiate(assembler.weaponPart.projectilePrefab, firePoint.position, firePoint.rotation);

        Projectile projectile = bulletGO.GetComponent<Projectile>();
        if (projectile != null)
        {
            projectile.damage = assembler.weaponPart.damage;
            projectile.shooterTeamID = myTeam.GetTeamID();

            // [중요] 쏜 사람이 '나'라면 진짜 총알, 아니면 가짜 총알
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
            // [확인] 자식 콜라이더여도 부모의 명찰을 찾음
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
}