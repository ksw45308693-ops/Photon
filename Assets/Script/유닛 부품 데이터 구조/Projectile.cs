using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 20f;
    public int damage = 10;

    [HideInInspector] // Inspector에서는 굳이 안 보여도 됨 (코드로 설정할 거라)
    public string targetTag; // 이 총알이 맞춰야 할 상대의 태그

    void Update()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
        Destroy(gameObject, 2f);
    }

    void OnTriggerEnter(Collider other)
    {
        // 1. 설정된 목표 태그와 부딪혔는지 확인
        if (other.CompareTag(targetTag))
        {
            // 2. 부딪힌 대상에게 'UnitHealth' (체력) 스크립트가 있는지 확인
            UnitHealth health = other.GetComponent<UnitHealth>();

            if (health != null)
            {
                health.TakeDamage(damage); // 데미지 주기
            }

            // 3. 총알 삭제
            Destroy(gameObject);
        }
    }
}