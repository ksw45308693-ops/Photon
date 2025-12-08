using UnityEngine;
using UnityEngine.EventSystems; // UI 클릭 이벤트를 위해 필수!

public class MinimapController : MonoBehaviour, IPointerDownHandler, IDragHandler
{
    [Header("연결할 오브젝트")]
    public Camera minimapCamera;    // 미니맵을 찍는 탑뷰 카메라
    public Transform mainCameraRig; // 메인 카메라 (또는 카메라를 담은 부모 오브젝트)

    private RectTransform minimapRect;

    void Awake()
    {
        minimapRect = GetComponent<RectTransform>();
    }

    // 미니맵을 '클릭'했을 때 호출
    public void OnPointerDown(PointerEventData eventData)
    {
        MoveCamera(eventData);
    }

    // 미니맵을 누른 채 '드래그'할 때도 호출 (원하는 위치로 쓱 훑기 위해)
    public void OnDrag(PointerEventData eventData)
    {
        MoveCamera(eventData);
    }

    void MoveCamera(PointerEventData eventData)
    {
        Vector2 localPoint;

        // 1. 클릭한 화면 좌표를 UI 내부의 로컬 좌표로 변환
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(minimapRect, eventData.position, eventData.pressEventCamera, out localPoint))
        {
            // 2. 로컬 좌표를 0 ~ 1 사이의 비율(Viewport 좌표)로 정규화
            // (RectTransform의 Pivot이 0.5, 0.5인 중앙 기준일 때 계산식)
            float xRatio = (localPoint.x / minimapRect.rect.width) + 0.5f;
            float yRatio = (localPoint.y / minimapRect.rect.height) + 0.5f;

            // 0~1 범위를 벗어나지 않게 고정 (미니맵 밖 클릭 방지)
            xRatio = Mathf.Clamp01(xRatio);
            yRatio = Mathf.Clamp01(yRatio);

            // 3. 미니맵 카메라를 기준으로 3D 월드 좌표 계산
            // 미니맵 카메라는 Orthographic(직교)이어야 정확하게 작동합니다.
            Vector3 targetWorldPos = minimapCamera.ViewportToWorldPoint(new Vector3(xRatio, yRatio, 0));

            // 4. 메인 카메라 이동 (높이 Y는 유지하고 X, Z만 변경)
            Vector3 newPosition = new Vector3(targetWorldPos.x, mainCameraRig.position.y, targetWorldPos.z);
            mainCameraRig.position = newPosition;
        }
    }
}