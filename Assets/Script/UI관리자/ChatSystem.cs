using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun; // [추가] 포톤 네임스페이스

// MonoBehaviour 대신 MonoBehaviourPun으로 변경하여 photonView 사용 가능하게 함
public class ChatSystem : MonoBehaviourPun
{
    [Header("UI Components")]
    public InputField chatInput;
    public Transform chatContent;
    public GameObject textPrefab;
    public Button sendButton;

    [Header("Settings")]
    public int maxMessages = 25;

    private List<GameObject> messageList = new List<GameObject>();

    void Start()
    {
        chatInput.onEndEdit.AddListener(OnEndEditEvent);
        if (sendButton != null) sendButton.onClick.AddListener(OnSendButtonClicked);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return) && !chatInput.isFocused)
        {
            chatInput.ActivateInputField();
        }
    }

    void OnEndEditEvent(string text)
    {
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            SendMessageToChat();
        }
    }

    public void OnSendButtonClicked()
    {
        SendMessageToChat();
    }

    void SendMessageToChat()
    {
        if (string.IsNullOrWhiteSpace(chatInput.text)) return;

        // [핵심 변경] 로컬 함수(AddChatMessage)를 직접 부르지 않고, RPC를 통해 모두에게 전송
        // RpcTarget.All : 나를 포함한 방 안의 모든 사람에게 실행하라
        photonView.RPC("RPC_AddChatMessage", RpcTarget.All, "User " + PhotonNetwork.LocalPlayer.ActorNumber + ": " + chatInput.text);

        chatInput.text = "";
        chatInput.ActivateInputField();
    }

    // [PunRPC] 속성을 붙여서 네트워크를 통해 호출될 수 있게 함
    [PunRPC]
    public void RPC_AddChatMessage(string message)
    {
        // 실제 텍스트 생성 로직은 동일
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