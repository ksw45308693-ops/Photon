using UnityEngine;
using System.Collections.Generic;
using Photon.Pun; // [필수] 포톤 기능 추가

public class UnitSelectionManager : MonoBehaviour
{
    public LayerMask unitLayer;
    public LayerMask groundLayer;
    public RectTransform selectionBox; // UI 박스 연결

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
                SelectUnitsInBox();
                if (selectionBox != null) selectionBox.gameObject.SetActive(false);
                isDragging = false;
            }
            else
            {
                HandleSingleClick();
            }
        }

        // 4. 우클릭: 이동 명령
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
        selectionBox.anchoredPosition = startPos + new Vector2(width < 0 ? width : 0, height < 0 ? height : 0);
    }

    // 박스 안에 있는 유닛들 찾아서 선택
    void SelectUnitsInBox()
    {
        if (!Input.GetKey(KeyCode.LeftShift))
        {
            DeselectAll();
        }

        Vector2 min = selectionBox.anchoredPosition;
        Vector2 max = min + selectionBox.sizeDelta;

        // 씬에 있는 모든 유닛 검사
        UnitMovement[] allUnits = FindObjectsOfType<UnitMovement>();

        foreach (var unit in allUnits)
        {
            Vector3 screenPos = cam.WorldToScreenPoint(unit.transform.position);

            if (screenPos.x > min.x && screenPos.x < max.x &&
                screenPos.y > min.y && screenPos.y < max.y)
            {
                // [핵심 추가] 내 소유권(IsMine)이 있는 유닛인지 확인
                PhotonView pv = unit.GetComponent<PhotonView>();
                if (pv != null && pv.IsMine)
                {
                    Select(unit);
                }
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
                // [핵심 추가] 클릭했을 때도 내 유닛인지 확인
                PhotonView pv = unit.GetComponent<PhotonView>();
                if (pv != null && pv.IsMine)
                {
                    if (!Input.GetKey(KeyCode.LeftShift)) DeselectAll();
                    Select(unit);
                }
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
                // UnitMovement 내부에서도 IsMine 체크를 해주면 더 안전합니다.
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