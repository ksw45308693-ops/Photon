using UnityEngine;

[CreateAssetMenu(fileName = "New Leg", menuName = "Nova2/Parts/Leg")]
public class LegPartData : PartData
{
    [Header("Leg Stats")]
    public float moveSpeed;    // 이동 속도
    public int loadCapacity;   // 최대 하중 (이걸 넘으면 느려짐)

    // 생성자에서 타입 자동 설정
    private void OnEnable() => partType = PartType.Leg;
}