using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // Legacy UI 사용

public class ChatSystem : MonoBehaviour
{
    [Header("UI Components")]
    public InputField chatInput;      // 입력창
    public Transform chatContent;     // Scroll View -> Content
    public GameObject textPrefab;     // 채팅 글씨 프리팹
    public Button sendButton;         // [추가됨] 전송 버튼

    [Header("Settings")]
    public int maxMessages = 25;

    private List<GameObject> messageList = new List<GameObject>();

    void Start()
    {
        // 1. 엔터키를 쳤을 때 실행 (입력이 끝났을 때)
        chatInput.onEndEdit.AddListener(OnEndEditEvent);

        // 2. 버튼을 클릭했을 때 실행
        if (sendButton != null)
        {
            sendButton.onClick.AddListener(OnSendButtonClicked);
        }
    }

    void Update()
    {
        // 엔터키를 누르면 입력창에 포커스 가기 (편의성)
        if (Input.GetKeyDown(KeyCode.Return) && !chatInput.isFocused)
        {
            chatInput.ActivateInputField();
        }
    }

    // 엔터키 이벤트 처리
    void OnEndEditEvent(string text)
    {
        // 엔터키를 눌러서 입력이 끝난 경우에만 전송 (다른 곳 클릭해서 포커스 잃은 경우 제외)
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            SendMessageToChat();
        }
    }

    // 버튼 클릭 이벤트 처리
    public void OnSendButtonClicked()
    {
        SendMessageToChat();
    }

    // [핵심] 실제 메시지를 보내는 공통 함수
    void SendMessageToChat()
    {
        if (string.IsNullOrWhiteSpace(chatInput.text)) return; // 빈칸이면 무시

        // --- [여기서 텍스트 전송] ---
        AddChatMessage("나: " + chatInput.text);

        // 입력창 비우기 및 포커스 다시 잡기
        chatInput.text = "";
        chatInput.ActivateInputField(); // 전송 후 바로 다시 타자 칠 수 있게 함
    }

    // 화면에 텍스트 생성하는 함수
    public void AddChatMessage(string message)
    {
        if (messageList.Count >= maxMessages)
        {
            Destroy(messageList[0]);
            messageList.RemoveAt(0);
        }

        GameObject newTextObj = Instantiate(textPrefab, chatContent);
        Text txt = newTextObj.GetComponent<Text>();
        txt.text = message;

        messageList.Add(newTextObj);
    }
}