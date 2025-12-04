using UnityEngine;
using UnityEngine.AI; // 내비게이션 기능을 쓰기 위해 필수!

public class UnitMovement : MonoBehaviour
{
    public Camera cam; // 메인 카메라를 연결할 변수
    public NavMeshAgent agent; // 유닛의 내비게이션 요원

    void Start()
    {
        // 컴포넌트를 자동으로 찾아 연결해줍니다.
        if (agent == null)
            agent = GetComponent<NavMeshAgent>();

        // 만약 카메라를 수동으로 안 넣었으면 메인 카메라를 찾음
        if (cam == null)
            cam = Camera.main;
    }

    void Update()
    {
        // 마우스 오른쪽 버튼(1) 클릭 시
        if (Input.GetMouseButtonDown(1))
        {
            MoveToCursor();
        }
    }

    void MoveToCursor()
    {
        // 1. 카메라에서 마우스 위치로 레이저(Ray)를 쏨
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        // 2. 레이저가 무언가(땅)에 부딪혔다면
        if (Physics.Raycast(ray, out hit))
        {
            // 3. 그 위치(hit.point)로 이동 명령을 내림
            agent.SetDestination(hit.point);
        }
    }
}