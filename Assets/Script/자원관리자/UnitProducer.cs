using UnityEngine;
using Photon.Pun;

public class UnitProducer : MonoBehaviour
{
    public string unitPrefabName = "Unit";
    public Transform spawnPoint;

    [System.Serializable]
    public class UnitRecipe
    {
        public string name;
        public int cost;
        public LegPartData leg;
        public CorePartData core;
        public WeaponPartData weapon;
    }

    public UnitRecipe[] recipes;

    public void ProduceUnit(int index)
    {
        if (index < 0 || index >= recipes.Length)
            return;

        UnitRecipe recipe = recipes[index];

        // 1) 자원 체크
        if (ResourceManager.Instance.UseWatt(recipe.cost) == false)
        {
            Debug.Log("와트 부족!");
            return;
        }

        // 2) 조립 데이터 준비
        UnitNetworkSync.UnitPartsData parts = new UnitNetworkSync.UnitPartsData
        {
            legID = recipe.leg.id,
            coreID = recipe.core.id,
            weaponID = recipe.weapon.id
        };

        string json = JsonUtility.ToJson(parts);

        // 3) InstantiationData에 태그 + 조립정보 적용
        object[] instantiationData = new object[]
        {
            gameObject.tag,
            json
        };

        PhotonNetwork.Instantiate(
            unitPrefabName,
            spawnPoint.position,
            spawnPoint.rotation,
            0,
            instantiationData
        );
    }
}
