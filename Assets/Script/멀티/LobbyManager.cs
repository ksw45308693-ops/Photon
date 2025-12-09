using UnityEngine;
using Photon.Pun; // 포톤 핵심 기능
using Photon.Realtime; // 포톤 실시간 기능

public class LobbyManager : MonoBehaviourPunCallbacks
{
    // 게임 시작 시 자동 실행
    void Start()
    {
        Debug.Log("서버 접속 시도 중...");
        // 1. 포톤 서버에 접속 (설정 파일 내용대로)
        PhotonNetwork.ConnectUsingSettings();
    }

    // 서버 접속 성공 시 호출되는 콜백
    public override void OnConnectedToMaster()
    {
        Debug.Log("서버 접속 완료! 방 입장 시도...");
        // 2. 방이 있으면 들어가고, 없으면 만듦 (방 이름: "NovaRoom")
        PhotonNetwork.JoinOrCreateRoom("NovaRoom", new RoomOptions { MaxPlayers = 4 }, null);
    }

    // 방 입장 성공 시 호출되는 콜백
    public override void OnJoinedRoom()
    {
        Debug.Log("방 입장 성공! 현재 플레이어 수: " + PhotonNetwork.CurrentRoom.PlayerCount);
        // 여기서부터 게임 로직이나 채팅이 작동하면 됩니다.
    }
}