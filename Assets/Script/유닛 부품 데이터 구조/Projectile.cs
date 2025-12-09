using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 20f;
    public int damage = 10;

    [HideInInspector] public int shooterTeamID = -1;
    public bool isRealBullet = false; // 진짜 총알 여부

    void Update()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
        Destroy(gameObject, 2f);
    }

    void OnTriggerEnter(Collider other)
    {
        // 1. 부딪힌 물체의 본체(부모)에서 명찰 찾기
        TeamEntity hitTeam = other.GetComponentInParent<TeamEntity>();

        // (예외) 명찰 없는 바닥/장식물에 닿으면? -> 그냥 삭제 (벽 충돌 구현)
        if (hitTeam == null)
        {
            // TeamEntity는 없지만 부딪히는 물체(벽 등)라면 삭제
            // (Tag 확인 등을 추가하면 더 좋음)
            // Destroy(gameObject); 
            return;
        }

        // 2. 아군(나를 쏜 팀)이면 통과 (팀킬 방지)
        if (hitTeam.GetTeamID() == shooterTeamID) return;

        // [Case A] 가짜 총알 (상대방 화면)
        if (!isRealBullet)
        {
            // 데미지는 안 주지만, "퍽" 하고 사라지는 연출을 위해 삭제
            Destroy(gameObject);
            return;
        }

        // [Case B] 진짜 총알 (내 화면) -> 데미지 권한 있음
        if (hitTeam.GetTeamID() != shooterTeamID)
        {
            // 부모 쪽에서 체력 스크립트 찾기 (중요!)
            UnitHealth health = other.GetComponentInParent<UnitHealth>();
            if (health != null)
            {
                // 로컬 함수 호출 -> 내부에서 RPC로 변환되어 모두에게 전달됨
                health.TakeDamage(damage);
            }
            // 적을 맞췄으니 총알 삭제
            Destroy(gameObject);
        }
    }
}