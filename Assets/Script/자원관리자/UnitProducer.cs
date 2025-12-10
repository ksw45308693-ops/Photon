using UnityEngine;
using Photon.Pun;

public class UnitProducer : MonoBehaviour
{
    public Transform spawnPoint;

    // [변경] 복잡한 Recipe 클래스 삭제 -> 단순 구조체 사용
    [System.Serializable]
    public class SimpleUnitInfo
    {
        public string unitName;        // UI 표시용 이름
        public string prefabName;      // Resources 폴더 안의 프리팹 파일 이름
        public int cost;               // 생산 비용
        public float buildTime = 2f;   // 생산 시간 (나중에 구현)
    }

    public SimpleUnitInfo[] unitList;

    public void ProduceUnit(int index)
    {
        if (index < 0 || index >= unitList.Length) return;

        SimpleUnitInfo info = unitList[index];

        // 1. 자원 체크
        if (ResourceManager.Instance.UseWatt(info.cost) == false)
        {
            Debug.Log("자원이 부족합니다!");
            return;
        }

        // 2. 유닛 생성 (데이터 전달 필요 없음 -> 기본 프리팹 그대로 생성)
        // instantiateData에 null을 넣어도 됩니다.
        PhotonNetwork.Instantiate(info.prefabName, spawnPoint.position, spawnPoint.rotation);

        Debug.Log($"{info.unitName} 생산 시작!");
    }
}