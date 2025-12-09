using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon", menuName = "Nova2/Parts/Weapon")]
public class WeaponPartData : PartBase
{
    [Header("Weapon Stats")]
    public int damage;
    public float attackRange;
    public float fireRate;
    public AttackType attackType; // (±âÁ¸¿¡ Á¤ÀÇµÈ Enum »ç¿ë)

    [Header("Prefab")]
    public GameObject projectilePrefab; // ¹ß»çÃ¼ ÇÁ¸®ÆÕ (ÃÑ¾Ë)

    // --- [Ãß°¡µÊ] ¹«±â ÀÚÃ¼ÀÇ 3D ¸ðµ¨ ÇÁ¸®ÆÕ (Ä®, ÃÑ µî) ---
    [Header("Visual")]
    public GameObject weaponModelPrefab;

    private void OnEnable()
    {
        partType = PartType.Weapon;
    }
}