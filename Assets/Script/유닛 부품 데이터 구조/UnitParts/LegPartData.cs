using UnityEngine;

[CreateAssetMenu(fileName = "New Leg", menuName = "Nova2/Parts/Leg")]
public class LegPartData : PartBase // [변경] PartBase 상속
{
    [Header("Leg Stats")]
    public float moveSpeed;    // 이동 속도
    public int loadCapacity;   // 최대 하중

    // 생성 시 자동으로 타입 설정
    private void OnEnable()
    {
        partType = PartType.Leg;
    }
}