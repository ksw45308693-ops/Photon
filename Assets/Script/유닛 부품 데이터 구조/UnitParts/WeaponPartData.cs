using UnityEngine;

public class WeaponPartData : PartData
{
    [Header("Weapon Stats")]
    public int damage;
    public float attackRange;
    public float fireRate;
    public AttackType attackType;

    // [Ãß°¡µÊ] ¹ß»çÇÒ ÃÑ¾Ë ÇÁ¸®ÆÕ
    public GameObject projectilePrefab;

    private void OnEnable() => partType = PartType.Weapon;
}