using UnityEngine;

public class RTSCamera : MonoBehaviour
{
    [Header("Camera Settings")]
    public float panSpeed = 20f;       // 카메라 이동 속도
    public float panBorderThickness = 10f; // 화면 가장자리 마우스 이동 감지 두께
    public Vector2 panLimit;           // 맵 밖으로 못 나가게 제한 (X, Z)

    [Header("Zoom Settings")]
    public float scrollSpeed = 20f;    // 줌 속도
    public float minY = 10f;           // 최소 줌 (가까이)
    public float maxY = 40f;           // 최대 줌 (멀리)

    void Update()
    {
        Vector3 pos = transform.position;

        // --- 1. 키보드(WASD) 및 마우스 화면 가장자리 이동 ---
        if (Input.GetKey("w") || Input.mousePosition.y >= Screen.height - panBorderThickness)
        {
            pos.z += panSpeed * Time.deltaTime;
        }
        if (Input.GetKey("s") || Input.mousePosition.y <= panBorderThickness)
        {
            pos.z -= panSpeed * Time.deltaTime;
        }
        if (Input.GetKey("d") || Input.mousePosition.x >= Screen.width - panBorderThickness)
        {
            pos.x += panSpeed * Time.deltaTime;
        }
        if (Input.GetKey("a") || Input.mousePosition.x <= panBorderThickness)
        {
            pos.x -= panSpeed * Time.deltaTime;
        }

        // --- 2. 마우스 휠로 줌 인/아웃 ---
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        pos.y -= scroll * 1000 * scrollSpeed * Time.deltaTime;

        // 줌 제한 (너무 가깝거나 멀어지지 않게)
        pos.y = Mathf.Clamp(pos.y, minY, maxY);

        // 맵 밖으로 나가는 것 제한 (선택 사항)
        // pos.x = Mathf.Clamp(pos.x, -panLimit.x, panLimit.x);
        // pos.z = Mathf.Clamp(pos.z, -panLimit.y, panLimit.y);

        transform.position = pos;
    }
}