using UnityEngine;

public class UnitHealth : MonoBehaviour
{
    public int maxHealth = 100;
    private int currentHealth;

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        // 누가 맞았는지 이름과 남은 체력 로그 출력
        Debug.Log($"{name} 피격! 데미지: {damage}, 남은 체력: {currentHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log($"{name} 파괴됨!");
        Destroy(gameObject);
    }
}