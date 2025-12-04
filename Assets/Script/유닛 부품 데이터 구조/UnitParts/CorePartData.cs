using UnityEngine;

[CreateAssetMenu(fileName = "New Core", menuName = "Nova2/Parts/Core")]
public class CorePartData : PartData
{
    [Header("Core Stats")]
    public int maxHealth;      // 체력
    public int defense;        // 방어력
    public float sightRange;   // 시야 거리

    private void OnEnable() => partType = PartType.Core;
}