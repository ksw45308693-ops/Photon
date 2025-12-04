using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon", menuName = "Nova2/Parts/Weapon")]
public class WeaponPartData : PartData
{
    [Header("Weapon Stats")]
    public int damage;           // 공격력
    public float attackRange;    // 사정거리
    public float fireRate;       // 연사 속도 (초당 발사 수)
    public AttackType attackType;// 공격 속성 (물리, 빔 등)

    private void OnEnable() => partType = PartType.Weapon;
}