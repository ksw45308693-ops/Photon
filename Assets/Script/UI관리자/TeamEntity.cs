using UnityEngine;
using Photon.Pun;

// 이 스크립트를 Unit 프리팹과 MainBase, EnemyBase에 모두 붙이세요!
public class TeamEntity : MonoBehaviourPun
{
    // 내 팀 번호를 알려주는 함수
    // (별도의 변수 동기화 없이, 포톤의 소유자 번호를 그대로 씁니다)
    public int GetTeamID()
    {
        if (photonView.Owner != null)
        {
            return photonView.Owner.ActorNumber;
        }

        // 주인이 없는 중립 오브젝트(혹은 에러)라면 -1 반환
        return -1;
    }
}