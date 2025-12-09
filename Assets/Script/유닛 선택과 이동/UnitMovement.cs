using UnityEngine;
using UnityEngine.AI;
using Photon.Pun; // [필수] 포톤 추가

// MonoBehaviour -> MonoBehaviourPun 변경 (photonView 사용을 위해)
public class UnitMovement : MonoBehaviourPun
{
    public NavMeshAgent agent;
    public GameObject selectedRing;
    private bool isSelected = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        DeselectUnit();
    }

    public void SelectUnit()
    {
        // [추가] 내 유닛이 아니면 선택 표시(링)도 띄우지 마라
        if (!photonView.IsMine) return;

        isSelected = true;
        if (selectedRing != null) selectedRing.SetActive(true);
    }

    public void DeselectUnit()
    {
        isSelected = false;
        if (selectedRing != null) selectedRing.SetActive(false);
    }

    public void MoveTo(Vector3 destination)
    {
        // [핵심] 내 것이 아니면 명령 무시! (해킹 방지 및 버그 방지)
        if (!photonView.IsMine) return;

        if (isSelected)
        {
            agent.SetDestination(destination);
        }
    }
}