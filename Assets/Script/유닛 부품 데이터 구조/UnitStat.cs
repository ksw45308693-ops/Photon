using UnityEngine;

public class UnitStat : MonoBehaviour
{
    [Header("Unit Stats")]
    public string unitName = "Marine";
    public int maxHealth = 100;
    public float moveSpeed = 5f;

    [Header("Attack Settings")]
    public int attackDamage = 10;
    public float attackRange = 7f;
    public float attackSpeed = 1.0f; // 초당 공격 횟수

    [Header("Projectile (원거리 유닛만)")]
    public GameObject projectilePrefab; // 총알 프리팹 (없으면 근접 공격)
    public Transform firePoint;         // 총알 나가는 위치
}