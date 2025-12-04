using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 20f;
    public int damage = 10; // 무기 데이터에서 받아올 예정

    void Update()
    {
        // 앞으로 전진
        transform.Translate(Vector3.forward * speed * Time.deltaTime);

        // 2초 지나면 자동 삭제 (메모리 관리)
        Destroy(gameObject, 2f);
    }

    void OnTriggerEnter(Collider other)
    {
        // 부딪힌 대상이 '적'이라면
        if (other.CompareTag("Enemy"))
        {
            EnemyHealth enemy = other.GetComponent<EnemyHealth>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage); // 데미지 주기
            }
            Destroy(gameObject); // 총알 삭제
        }
    }
}