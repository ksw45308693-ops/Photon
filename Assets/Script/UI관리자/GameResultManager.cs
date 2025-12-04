using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement; // 씬(게임) 재시작을 위해 필수

public class GameResultManager : MonoBehaviour
{
    public static GameResultManager Instance;

    [Header("UI")]
    public GameObject gameOverPanel;
    public Text resultText;

    private bool isGameOver = false;

    void Awake()
    {
        if (Instance == null) Instance = this;
    }

    // 게임 오버 처리 함수 (isVictory: true면 승리, false면 패배)
    public void GameOver(bool isVictory)
    {
        if (isGameOver) return; // 이미 끝났으면 무시
        isGameOver = true;

        // 1. UI 켜기
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);

            if (isVictory)
            {
                resultText.text = "Victory!";
                resultText.color = Color.cyan;
            }
            else
            {
                resultText.text = "Defeat...";
                resultText.color = Color.red;
            }
        }

        // 2. 시간 멈추기 (선택 사항)
        Time.timeScale = 0f;
    }

    // 재시작 버튼이 누를 함수
    public void RestartGame()
    {
        // 멈춘 시간 다시 흐르게 하기
        Time.timeScale = 1f;

        // 현재 씬을 다시 로드 (초기화)
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}