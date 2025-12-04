using UnityEngine;

public class RTSCamera : MonoBehaviour
{
    [Header("Camera Settings")]
    public float panSpeed = 20f;
    public float panBorderThickness = 10f;
    public Vector2 panLimit;

    // 이 변수를 추가하세요! (체크박스로 끄고 켤 수 있게)
    public bool enableEdgeScroll = true;

    [Header("Zoom Settings")]
    public float scrollSpeed = 20f;
    public float minY = 10f;
    public float maxY = 40f;

    void Update()
    {
        Vector3 pos = transform.position;

        // --- 1. 키보드 이동 (항상 작동) ---
        if (Input.GetKey("w")) pos.z += panSpeed * Time.deltaTime;
        if (Input.GetKey("s")) pos.z -= panSpeed * Time.deltaTime;
        if (Input.GetKey("d")) pos.x += panSpeed * Time.deltaTime;
        if (Input.GetKey("a")) pos.x -= panSpeed * Time.deltaTime;

        // --- 2. 마우스 가장자리 이동 (스위치가 켜져 있을 때만 작동) ---
        if (enableEdgeScroll)
        {
            if (Input.mousePosition.y >= Screen.height - panBorderThickness)
                pos.z += panSpeed * Time.deltaTime;
            if (Input.mousePosition.y <= panBorderThickness)
                pos.z -= panSpeed * Time.deltaTime;
            if (Input.mousePosition.x >= Screen.width - panBorderThickness)
                pos.x += panSpeed * Time.deltaTime;
            if (Input.mousePosition.x <= panBorderThickness)
                pos.x -= panSpeed * Time.deltaTime;
        }

        // --- 3. 마우스 휠 줌 ---
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        pos.y -= scroll * 1000 * scrollSpeed * Time.deltaTime;
        pos.y = Mathf.Clamp(pos.y, minY, maxY);

        transform.position = pos;
    }
}