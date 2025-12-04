using UnityEngine;
using UnityEngine.UI; // UI 사용

public class ResourceManager : MonoBehaviour
{
    public static ResourceManager Instance; // 어디서든 접근 가능하게 함 (싱글톤)

    [Header("Watt Settings")]
    public float maxWatt = 3000f;     // 최대 와트
    public float currentWatt;         // 현재 와트
    public float regenRate = 100f;    // 초당 회복량 (노바2 스타일)

    [Header("UI Linking")]
    public Text wattText;             // 화면에 표시할 텍스트
    public Image wattBarImage;        // (선택) 와트 게이지 바

    void Awake()
    {
        // 싱글톤 설정
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        currentWatt = 1000f; // 시작 자원
    }

    void Update()
    {
        // 1. 와트 자동 회복
        if (currentWatt < maxWatt)
        {
            currentWatt += regenRate * Time.deltaTime;

            // 최대치 넘지 않게 고정
            if (currentWatt > maxWatt) currentWatt = maxWatt;
        }

        // 2. UI 업데이트
        UpdateUI();
    }

    void UpdateUI()
    {
        // 텍스트 표시 (예: 1500 / 3000)
        if (wattText != null)
        {
            wattText.text = $"Watt: {(int)currentWatt} / {maxWatt}";
        }

        // 게이지 바 표시 (0.0 ~ 1.0)
        if (wattBarImage != null)
        {
            wattBarImage.fillAmount = currentWatt / maxWatt;
        }
    }

    // 외부에서 와트를 쓸 때 호출하는 함수
    public bool UseWatt(int amount)
    {
        if (currentWatt >= amount)
        {
            currentWatt -= amount;
            return true; // 사용 성공
        }
        return false; // 자원 부족
    }
}