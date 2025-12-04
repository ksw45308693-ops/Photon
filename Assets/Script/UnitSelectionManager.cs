using UnityEngine;
using System.Collections.Generic; // 리스트 사용을 위해 필요

public class UnitSelectionManager : MonoBehaviour
{
    public LayerMask unitLayer; // 유닛만 감지하기 위한 레이어 설정
    public LayerMask groundLayer; // 땅만 감지하기 위한 레이어 설정

    // 현재 선택된 유닛들을 담아두는 리스트
    private List<UnitMovement> selectedUnits = new List<UnitMovement>();
    private Camera cam;

    void Start()
    {
        cam = Camera.main;
    }

    void Update()
    {
        // 1. 좌클릭: 유닛 선택
        if (Input.GetMouseButtonDown(0))
        {
            HandleSelection();
        }

        // 2. 우클릭: 이동 명령
        if (Input.GetMouseButtonDown(1))
        {
            HandleMovement();
        }
    }

    void HandleSelection()
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        // 유닛을 클릭했는지 확인
        if (Physics.Raycast(ray, out hit, 1000f, unitLayer))
        {
            UnitMovement unit = hit.collider.GetComponent<UnitMovement>();
            if (unit != null)
            {
                // Shift 키를 안 눌렀으면 기존 선택 모두 해제 (다중 선택 아님)
                if (!Input.GetKey(KeyCode.LeftShift))
                {
                    DeselectAll();
                }

                // 유닛 선택 처리
                Select(unit);
            }
        }
        else
        {
            // 빈 땅을 클릭하면 모두 선택 해제
            // (Shift 누른 상태가 아닐 때만)
            if (!Input.GetKey(KeyCode.LeftShift))
            {
                DeselectAll();
            }
        }
    }

    void HandleMovement()
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        // 땅을 클릭했는지 확인
        if (Physics.Raycast(ray, out hit, 1000f, groundLayer))
        {
            // 선택된 모든 유닛에게 이동 명령 하달
            foreach (var unit in selectedUnits)
            {
                unit.MoveTo(hit.point);
            }
        }
    }

    void Select(UnitMovement unit)
    {
        if (!selectedUnits.Contains(unit))
        {
            selectedUnits.Add(unit);
            unit.SelectUnit(); // 유닛에게 "너 선택됐어"라고 알림
        }
    }

    void DeselectAll()
    {
        foreach (var unit in selectedUnits)
        {
            unit.DeselectUnit();
        }
        selectedUnits.Clear();
    }
}