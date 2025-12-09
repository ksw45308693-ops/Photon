using UnityEngine;
using Photon.Pun;
using System;

public class UnitNetworkSync : MonoBehaviourPun, IPunInstantiateMagicCallback
{
    [Serializable]
    public class UnitPartsData
    {
        public string legID;
        public string coreID;
        public string weaponID;
    }

    private UnitAssembler assembler;

    private void Awake()
    {
        assembler = GetComponent<UnitAssembler>();
    }

    // PhotonNetwork.Instantiate 할 때 전달된 InstantiationData를 받는 콜백
    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        object[] data = info.photonView.InstantiationData;

        if (data == null || data.Length < 2)
        {
            Debug.LogError("Unit InstantiationData 누락!");
            return;
        }

        // 1) 태그 설정
        string tagName = (string)data[0];
        gameObject.tag = tagName;

        // 2) JSON 조립정보 파싱
        string json = (string)data[1];
        UnitPartsData parts = JsonUtility.FromJson<UnitPartsData>(json);

        // 3) ID로 실제 ScriptableObject 가져오기
        assembler.legPart = PartDatabase.Instance.GetLeg(parts.legID);
        assembler.corePart = PartDatabase.Instance.GetCore(parts.coreID);
        assembler.weaponPart = PartDatabase.Instance.GetWeapon(parts.weaponID);

        // 4) 조립 실행
        assembler.AssembleUnit();
    }
}
