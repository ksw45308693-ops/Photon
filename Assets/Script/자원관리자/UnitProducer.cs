using UnityEngine;
using Photon.Pun;

public class UnitProducer : MonoBehaviour
{
    public Transform spawnPoint;

    // [변경] 복잡한 레시피 대신, 심플하게 유닛 데이터 리스트만 있으면 됨
    public UnitData[] availableUnits;

    public void ProduceUnit(int index)
    {
        if (index < 0 || index >= availableUnits.Length) return;

        UnitData data = availableUnits[index];

        // 1. 자원 체크
        if (ResourceManager.Instance.UseWatt(data.cost) == false)
        {
            Debug.Log("자원 부족!");
            return;
        }

        // 2. 유닛 생성 (이제 조립 데이터 Json 같은 건 필요 없음!)
        // 유닛 프리팹 이름은 UnitData 안에 있는 프리팹 이름을 사용하거나, 
        // Resources 폴더에 있는 프리팹 이름을 직접 써야 함.

        // 주의: PhotonNetwork.Instantiate는 Resources 폴더 안의 파일 이름(String)이 필요함.
        // UnitData에 prefabName string 변수를 추가해서 쓰거나, 
        // 프리팹 이름을 규칙적으로 지어서(예: "Unit_Marine") 사용해야 합니다.

        PhotonNetwork.Instantiate(
            data.unitPrefab.name, // 프리팹의 이름과 Resources 폴더 파일명이 같아야 함!
            spawnPoint.position,
            spawnPoint.rotation
        );
    }
}