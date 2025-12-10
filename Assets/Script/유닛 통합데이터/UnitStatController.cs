using UnityEngine;
using UnityEngine.AI;
using Photon.Pun;

public class UnitStatController : MonoBehaviourPun
{
    // 유닛의 스탯 정보가 담긴 데이터 파일
    public UnitData unitData;

    [Header("Components")]
    public UnitHealth healthScript;
    public NavMeshAgent agent;

    void Start()
    {
        // 컴포넌트 자동 찾기
        if (healthScript == null) healthScript = GetComponent<UnitHealth>();
        if (agent == null) agent = GetComponent<NavMeshAgent>();

        InitializeUnit();
    }

    void InitializeUnit()
    {
        if (unitData == null)
        {
            Debug.LogError($"{name}: UnitData가 연결되지 않았습니다!");
            return;
        }

        // 1. 체력 설정
        if (healthScript != null)
        {
            healthScript.SetMaxHealth(unitData.maxHealth);
        }

        // 2. 이동 속도 설정
        if (agent != null)
        {
            agent.speed = unitData.moveSpeed;
            agent.angularSpeed = unitData.turnSpeed;
        }
    }
}