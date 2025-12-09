using UnityEngine;
using Photon.Pun; // [필수] RPC 사용을 위해 추가

public class UnitHealth : MonoBehaviourPun // MonoBehaviour -> MonoBehaviourPun 변경
{
    public int maxHealth = 100;
    private int currentHealth;

    [Header("UI Linking")]
    public GameObject healthBarPrefab;
    private HealthBar healthBarInstance;

    void Start()
    {
        if (currentHealth == 0) currentHealth = maxHealth;
        if (healthBarPrefab != null && healthBarInstance == null) CreateHealthBar();
    }

    void CreateHealthBar()
    {
        GameObject hb = Instantiate(healthBarPrefab);
        healthBarInstance = hb.GetComponent<HealthBar>();
        healthBarInstance.targetUnit = this.transform;
        healthBarInstance.SetHealth(currentHealth, maxHealth);
    }

    public void SetMaxHealth(int newMax)
    {
        maxHealth = newMax;
        currentHealth = maxHealth;
        if (healthBarInstance == null && healthBarPrefab != null) CreateHealthBar();
        else if (healthBarInstance != null) healthBarInstance.SetHealth(currentHealth, maxHealth);
    }

    // [핵심 수정] 외부에서는 이 함수를 부르지만, 실제로는 RPC를 쏩니다.
    public void TakeDamage(int damage)
    {
        // "모든 사람(All)들아, 내 체력 깎는 함수(RPC_TakeDamage) 실행해줘!"
        photonView.RPC("RPC_TakeDamage", RpcTarget.All, damage);
    }

    // [추가] 실제로 체력이 깎이는 곳 (네트워크 동기화됨)
    [PunRPC]
    public void RPC_TakeDamage(int damage)
    {
        currentHealth -= damage;

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
        if (healthBarInstance != null) Destroy(healthBarInstance.gameObject);

        // 내가 주인이면 포톤 파괴 명령, 아니면 로컬 파괴 (안전장치)
        if (photonView.IsMine) PhotonNetwork.Destroy(gameObject);
        else if (gameObject != null) gameObject.SetActive(false); // 혹은 로컬 삭제
    }
}