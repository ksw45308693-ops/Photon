using UnityEngine;
using UnityEngine.AI;

public class UnitMovement : MonoBehaviour
{
    public NavMeshAgent agent;
    public GameObject selectedRing; // 선택 표시링 연결
    private bool isSelected = false; // 선택 상태 변수

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        // 시작할 때 선택 안 된 상태로 확정
        DeselectUnit();
    }

    // --- 선택 관련 함수 ---
    public void SelectUnit()
    {
        isSelected = true;
        if (selectedRing != null) selectedRing.SetActive(true); // 링 켜기
    }

    public void DeselectUnit()
    {
        isSelected = false;
        if (selectedRing != null) selectedRing.SetActive(false); // 링 끄기
    }

    // --- 이동 명령 ---
    public void MoveTo(Vector3 destination)
    {
        // 선택된 유닛만 이동 명령 수행
        if (isSelected)
        {
            agent.SetDestination(destination);
        }
    }
}