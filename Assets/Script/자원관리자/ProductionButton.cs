using UnityEngine;
using Photon.Pun;

public class ProductionButton : MonoBehaviour
{
    [Tooltip("생산할 유닛의 레시피 번호 (0: 기본, 1: 고급...)")]
    public int recipeIndex;

    public void OnClickProduce()
    {
        // 맵에 있는 모든 생산기지를 다 찾습니다.
        UnitProducer[] producers = FindObjectsOfType<UnitProducer>();

        bool foundMyBase = false;

        foreach (var producer in producers)
        {
            PhotonView pv = producer.GetComponent<PhotonView>();

            // [핵심] "내 소유(IsMine)"인 기지만 찾아서 명령을 내립니다.
            // GameSetupManager가 시작할 때 소유권을 넘겨줬으므로, 여기서 찾아집니다.
            if (pv != null && pv.IsMine)
            {
                producer.ProduceUnit(recipeIndex);
                foundMyBase = true;
                return; // 찾았으니 종료
            }
        }

        if (!foundMyBase)
        {
            Debug.LogWarning("내 소유의 생산 기지를 찾을 수 없습니다! (소유권 이전 확인 필요)");
        }
    }
}