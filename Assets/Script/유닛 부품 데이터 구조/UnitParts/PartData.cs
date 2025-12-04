using UnityEngine;

// 이 스크립트는 게임 오브젝트에 붙이는 게 아니라, 파일로 존재합니다.
public class PartData : ScriptableObject
{
    [Header("Basic Info")]
    public string partName;      // 부품 이름 (예: 데빌클로)
    public PartType partType;    // 부품 종류
    public int weight;           // 무게 (중요: 다리 하중에 영향)
    public int wattCost;         // 생산 비용 (와트)
    public GameObject modelPrefab; // 실제 3D 모델 프리팹
}