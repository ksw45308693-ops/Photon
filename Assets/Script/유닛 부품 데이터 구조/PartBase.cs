using UnityEngine;

// [참고] PartType Enum은 UnitEnums.cs에 있으므로 여기엔 없어야 합니다.

public class PartBase : ScriptableObject
{
    [Header("Base Info")]
    // 인스펙터에서는 숨기고, 파일 이름을 ID로 사용
    [HideInInspector]
    public string id;

    [Tooltip("UnitEnums.cs에 정의된 타입을 선택하세요.")]
    public PartType partType;

    // [추가됨] 모든 부품이 공통으로 가지는 '무게' 속성
    // 이 변수가 없어서 UnitAssembler에서 오류가 났던 것입니다.
    [Header("Common Stats")]
    [Tooltip("이 부품의 무게입니다. 높으면 이동 속도가 느려질 수 있습니다.")]
    public int weight;

    // 에디터에서 스크립트가 로드되거나 값이 바뀔 때 실행
    protected virtual void OnValidate()
    {
        // 파일 이름을 ID로 자동 등록 (오타 방지)
        id = name;
    }
}