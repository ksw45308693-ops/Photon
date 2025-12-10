using UnityEngine;

[CreateAssetMenu(fileName = "New Unit", menuName = "RTS/Unit Data")]
public class UnitData : ScriptableObject
{
    [Header("Basic Info")]
    public string unitName;
    public int cost; // 생산 비용
    public float buildTime; // 생산 시간 (필요시)

    [Header("Stats")]
    public int maxHealth;
    public float moveSpeed;
    public float turnSpeed = 120f;

    [Header("Combat")]
    public int damage;
    public float attackRange;
    public float fireRate;
    public bool isMelee; // 근접 유닛 여부

    [Header("Visual & Prefab")]
    public GameObject unitPrefab;       // 유닛의 본체 프리팹
    public GameObject projectilePrefab; // 원거리 유닛용 총알 (근접이면 비워둠)
    public GameObject deathEffect;      // 파괴 이펙트
}