using UnityEngine;
using UnityEngine.AI;

public class UnitAssembler : MonoBehaviour
{
    [Header("Parts Slots")]
    public LegPartData legPart;
    public CorePartData corePart;
    public WeaponPartData weaponPart;

    [Header("Final Stats (Read Only)")]
    public int totalHealth;
    public float finalSpeed;
    public int totalWeight;

    private NavMeshAgent agent;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        // [수정] 부품이 이미 스크립트로 할당되었다면 Start에서는 조립하지 않음
        // (UnitProducer가 직접 AssembleUnit을 호출해주기 때문)
        // 하지만 에디터에서 미리 배치해둔 유닛은 자동으로 조립되어야 함.
        if (totalWeight == 0)
        {
            AssembleUnit();
        }
    }

    // 데이터를 기반으로 유닛의 능력을 결정하는 함수
    public void AssembleUnit()
    {
        if (legPart == null || corePart == null || weaponPart == null)
        {
            Debug.LogError("부품이 모두 장착되지 않았습니다!");
            return;
        }

        // 1. 무게 계산 (다리 + 코어 + 무기)
        totalWeight = legPart.weight + corePart.weight + weaponPart.weight;

        // 2. 체력 계산
        totalHealth = corePart.maxHealth;

        // 3. 이동 속도 계산 (노바2 핵심: 과적 패널티)
        if (totalWeight > legPart.loadCapacity)
        {
            // 하중 초과 시 속도 50% 감소 (예시 로직)
            finalSpeed = legPart.moveSpeed * 0.5f;
            Debug.LogWarning("경고: 하중 초과! 이동 속도가 감소합니다.");
        }
        else
        {
            finalSpeed = legPart.moveSpeed;
        }

        // 4. 실제 유닛(NavMeshAgent)에 속도 적용
        if (agent != null)
        {
            agent.speed = finalSpeed;
        }

        Debug.Log($"유닛 조립 완료: 체력({totalHealth}), 속도({finalSpeed}), 무게({totalWeight}/{legPart.loadCapacity})");
    }
}