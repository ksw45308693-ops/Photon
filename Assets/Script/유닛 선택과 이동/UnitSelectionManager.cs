using UnityEngine;
using System.Collections.Generic;

public class UnitSelectionManager : MonoBehaviour
{
    public LayerMask unitLayer;
    public LayerMask groundLayer;
    public RectTransform selectionBox; // [추가] UI 박스 연결

    private List<UnitMovement> selectedUnits = new List<UnitMovement>();
    private Camera cam;

    // 드래그 관련 변수
    private Vector2 startPos;
    private bool isDragging = false;

    void Start()
    {
        cam = Camera.main;
        // 시작 시 박스 안 보이게
        if (selectionBox != null) selectionBox.gameObject.SetActive(false);
    }

    void Update()
    {
        // 1. 마우스 누름: 드래그 시작
        if (Input.GetMouseButtonDown(0))
        {
            startPos = Input.mousePosition;
            isDragging = false;
        }

        // 2. 마우스 누르고 있음: 박스 그리기
        if (Input.GetMouseButton(0))
        {
            // 살짝이라도 움직였으면 드래그로 간주
            if ((Vector2)Input.mousePosition != startPos)
            {
                isDragging = true;
                UpdateSelectionBox(Input.mousePosition);
            }
        }

        // 3. 마우스 뗌: 선택 확정
        if (Input.GetMouseButtonUp(0))
        {
            if (isDragging)
            {
                // 드래그 선택 수행
                SelectUnitsInBox();
                // 박스 끄기
                if (selectionBox != null) selectionBox.gameObject.SetActive(false);
                isDragging = false;
            }
            else
            {
                // 그냥 클릭 선택 수행 (기존 기능)
                HandleSingleClick();
            }
        }

        // 4. 우클릭: 이동 명령 (기존 기능)
        if (Input.GetMouseButtonDown(1))
        {
            HandleMovement();
        }
    }

    // UI 박스 크기 및 위치 업데이트
    void UpdateSelectionBox(Vector2 curMousePos)
    {
        if (!selectionBox) return;

        if (!selectionBox.gameObject.activeInHierarchy)
            selectionBox.gameObject.SetActive(true);

        float width = curMousePos.x - startPos.x;
        float height = curMousePos.y - startPos.y;

        selectionBox.sizeDelta = new Vector2(Mathf.Abs(width), Mathf.Abs(height));

        // 너비/높이가 음수일 때(왼쪽/아래로 드래그) 위치 보정
        selectionBox.anchoredPosition = startPos + new Vector2(width < 0 ? width : 0, height < 0 ? height : 0);
    }

    // 박스 안에 있는 유닛들 찾아서 선택
    void SelectUnitsInBox()
    {
        // Shift 안 눌렀으면 기존 선택 해제
        if (!Input.GetKey(KeyCode.LeftShift))
        {
            DeselectAll();
        }

        // 드래그 박스 영역 (UI 좌표)
        // Min/Max 계산으로 뒤집힌 드래그도 정상 처리
        Vector2 min = selectionBox.anchoredPosition;
        Vector2 max = min + selectionBox.sizeDelta;

        // 씬(Scene)에 있는 모든 유닛을 검사 (최적화하려면 리스트 관리 추천)
        UnitMovement[] allUnits = FindObjectsOfType<UnitMovement>();

        foreach (var unit in allUnits)
        {
            // 유닛의 월드 좌표를 화면(Screen) 좌표로 변환
            Vector3 screenPos = cam.WorldToScreenPoint(unit.transform.position);

            // 화면 좌표가 박스 범위 안에 있는지 확인
            if (screenPos.x > min.x && screenPos.x < max.x &&
                screenPos.y > min.y && screenPos.y < max.y)
            {
                Select(unit);
            }
        }
    }

    void HandleSingleClick()
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 1000f, unitLayer))
        {
            UnitMovement unit = hit.collider.GetComponent<UnitMovement>();
            if (unit != null)
            {
                if (!Input.GetKey(KeyCode.LeftShift)) DeselectAll();
                Select(unit);
            }
        }
        else
        {
            if (!Input.GetKey(KeyCode.LeftShift)) DeselectAll();
        }
    }

    void HandleMovement()
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 1000f, groundLayer))
        {
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
            unit.SelectUnit();
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