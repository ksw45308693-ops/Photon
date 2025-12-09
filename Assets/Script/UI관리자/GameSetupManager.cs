using UnityEngine;
using Photon.Pun;

public class GameSetupManager : MonoBehaviourPunCallbacks
{
    [Header("Bases")]
    public GameObject mainBase;
    public GameObject enemyBase;

    [Header("Camera")]
    public Transform cameraRig;
    public Transform p2SpawnPos;
    public Transform minimapCamera;

    // [수정 1] Start 대신 이 함수를 씁니다.
    // "방 입장이 완료되었을 때" 자동으로 호출됩니다.
    public override void OnJoinedRoom()
    {
        SetupTeam();
    }

    // [수정 2] 혹시 로비 씬을 거쳐서 왔을 경우를 대비해 Start에도 안전장치 추가
    void Start()
    {
        // 이미 방에 들어와 있는 상태라면 바로 설정 (씬 이동 시 사용)
        if (PhotonNetwork.InRoom)
        {
            SetupTeam();
        }
    }

    void SetupTeam()
    {
        // 이제 접속이 완료된 상태이므로 방장 판정이 정확합니다.
        if (PhotonNetwork.IsMasterClient)
        {
            // Player 1 (방장) - 아무것도 안 함 (MainBase 위치 유지)
            mainBase.GetComponent<PhotonView>().TransferOwnership(PhotonNetwork.LocalPlayer);
            mainBase.tag = "Player";
            enemyBase.tag = "Enemy";
            Debug.Log("나는 방장(Player 1)입니다. MainBase에서 시작합니다.");
        }
        else
        {
            // Player 2 (게스트) - 카메라 이동
            enemyBase.GetComponent<PhotonView>().TransferOwnership(PhotonNetwork.LocalPlayer);
            enemyBase.tag = "Player";
            mainBase.tag = "Enemy";

            MoveCameraForP2();
            Debug.Log("나는 게스트(Player 2)입니다. EnemyBase로 이동합니다.");
        }
    }

    void MoveCameraForP2()
    {
        if (cameraRig == null || p2SpawnPos == null) return;

        cameraRig.position = new Vector3(p2SpawnPos.position.x, cameraRig.position.y, p2SpawnPos.position.z);
        cameraRig.rotation = Quaternion.Euler(0, 180, 0);

        if (minimapCamera != null)
        {
            minimapCamera.rotation = Quaternion.Euler(90, 0, 180);
        }
    }
}