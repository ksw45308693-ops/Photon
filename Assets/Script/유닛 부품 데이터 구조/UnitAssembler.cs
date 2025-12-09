using UnityEngine;
using UnityEngine.AI;

public class UnitAssembler : MonoBehaviour
{
    [Header("Visual Settings")]
    public MeshRenderer bodyRenderer; // 유닛의 색깔/외형을 바꿀 렌더러

    [Header("Current Parts (Data)")]
    // [수정 1] 우리가 만든 ScriptableObject 타입(LegPartData 등)을 사용해야 합니다.
    public LegPartData legPart;
    public CorePartData corePart;
    public WeaponPartData weaponPart;

    [Header("Final Stats (Read Only)")]
    public float finalSpeed;
    public int totalHealth;
    public int totalWeight;

    // 내부 컴포넌트
    private NavMeshAgent agent;
    private UnitHealth healthScript;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        healthScript = GetComponent<UnitHealth>();
    }

    // 외부(UnitNetworkSync, UnitProducer)에서 호출하는 조립 함수
    public void AssembleUnit()
    {
        // 안전장치: 부품이 하나라도 없으면 중단
        if (legPart == null || corePart == null || weaponPart == null)
        {
            Debug.LogError($"[UnitAssembler] 부품이 누락되었습니다! ({name})");
            return;
        }

        // --------------------------------------------------------
        // 1. 시각적 변경 (색상/재질)
        // --------------------------------------------------------
        if (bodyRenderer != null && corePart != null)
        {
            // bodyRenderer.material = corePart.unitMaterial; // (필요시 주석 해제)
        }

        // --------------------------------------------------------
        // 2. 능력치 계산 (체력)
        // --------------------------------------------------------
        totalHealth = corePart.maxHealth;

        // [수정 2] 계산한 체력을 UnitHealth 스크립트에 '진짜로' 적용
        if (healthScript != null)
        {
            // [수정됨] 줄바꿈 없이 한 줄로 작성
            healthScript.SetMaxHealth(totalHealth);
        }

        // --------------------------------------------------------
        // 3. 능력치 계산 (속도 및 무게)
        // --------------------------------------------------------
        totalWeight = legPart.weight + corePart.weight + weaponPart.weight;

        // [수정 3] 노바2 스타일 과적(Overweight) 패널티 적용
        if (totalWeight > legPart.loadCapacity)
        {
            // 무게가 한계보다 무거우면 속도 50% 감소
            finalSpeed = legPart.moveSpeed * 0.5f;
            Debug.LogWarning($"{name}: 과적 상태! 속도가 느려집니다.");
        }
        else
        {
            finalSpeed = legPart.moveSpeed;
        }

        // [수정 4] 계산한 속도를 NavMeshAgent에 '진짜로' 적용
        if (agent != null)
        {
            agent.speed = finalSpeed;
            // 회전 속도도 다리 부품에 따라 다르게 할 수 있음 (선택사항)
            agent.angularSpeed = 120f;
        }

        Debug.Log($"[조립 완료] HP:{totalHealth}, Speed:{finalSpeed}, Weight:{totalWeight}/{legPart.loadCapacity}");
    }
}