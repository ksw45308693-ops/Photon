using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon", menuName = "Nova2/Parts/Weapon")]
public class WeaponPartData : PartBase // [변경] PartBase 상속
{
    [Header("Weapon Stats")]
    public int damage;
    public float attackRange;
    public float fireRate;
    public AttackType attackType; // (기존에 정의된 Enum 사용)

    [Header("Prefab")]
    public GameObject projectilePrefab; // 발사체 프리팹

    private void OnEnable()
    {
        partType = PartType.Weapon;
    }
}