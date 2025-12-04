using UnityEngine;

public class UnitHealth : MonoBehaviour
{
    public int maxHealth = 100;
    private int currentHealth;

    // [추가 1] 체력바 프리팹 연결용
    public GameObject healthBarPrefab;
    private HealthBar healthBarInstance; // 생성된 체력바

    void Start()
    {
        currentHealth = maxHealth;

        // [추가 2] 게임 시작 시 체력바 생성
        if (healthBarPrefab != null)
        {
            // Canvas(체력바)를 게임 세상에 생성
            GameObject hb = Instantiate(healthBarPrefab);
            healthBarInstance = hb.GetComponent<HealthBar>();

            // 체력바에게 주인(나)을 알려줌
            healthBarInstance.targetUnit = this.transform;
            healthBarInstance.SetHealth(currentHealth, maxHealth);
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        Debug.Log($"{name} 피격! 남은 체력: {currentHealth}");

        // [추가 3] 체력바 갱신
        if (healthBarInstance != null)
        {
            healthBarInstance.SetHealth(currentHealth, maxHealth);
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log($"{name} 파괴됨!");
        Destroy(gameObject);
        // 체력바는 HealthBar 스크립트에서 targetUnit이 null이 되면 알아서 사라짐
    }
}