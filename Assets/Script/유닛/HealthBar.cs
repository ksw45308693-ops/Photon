using UnityEngine;
using UnityEngine.UI; // UI 다룰 때 필수

public class HealthBar : MonoBehaviour
{
    public Slider slider;
    public Transform targetUnit; // 체력바가 따라다닐 유닛
    public Vector3 offset = new Vector3(0, 2.5f, 0); // 머리 위 높이

    private Camera cam;

    void Start()
    {
        cam = Camera.main;
    }

    void Update()
    {
        // 1. 유닛 머리 위 따라다니기
        if (targetUnit != null)
        {
            transform.position = targetUnit.position + offset;
        }
        else
        {
            // 유닛이 죽어서 사라지면 체력바도 삭제
            Destroy(gameObject);
            return;
        }

        // 2. 항상 카메라 정면 바라보기 (빌보드 효과)
        // 체력바가 카메라와 같은 방향을 보게 만듦
        transform.rotation = cam.transform.rotation;
    }

    // 체력 비율 업데이트 (0.0 ~ 1.0)
    public void SetHealth(int current, int max)
    {
        if (slider != null)
        {
            slider.value = (float)current / max;
        }
    }
}