using UnityEngine;
using Photon.Pun;

public class EnemySpawner : MonoBehaviourPun
{
    public string enemyPrefabName = "Unit";
    public Transform spawnPoint;
    public float spawnInterval = 10f;

    public LegPartData enemyLeg;
    public CorePartData enemyCore;
    public WeaponPartData enemyWeapon;

    private float timer = 0f;

    void Update()
    {
        if (PhotonNetwork.IsMasterClient == false)
            return;

        timer += Time.deltaTime;

        if (timer >= spawnInterval)
        {
            SpawnEnemy();
            timer = 0f;
        }
    }

    void SpawnEnemy()
    {
        // 1) 적 유닛 조립 JSON 생성
        UnitNetworkSync.UnitPartsData parts = new UnitNetworkSync.UnitPartsData
        {
            legID = enemyLeg.id,
            coreID = enemyCore.id,
            weaponID = enemyWeapon.id
        };

        string json = JsonUtility.ToJson(parts);

        // 2) InstantiationData 전달
        object[] instantiationData = new object[]
        {
            gameObject.tag,  // 예: "Enemy"
            json
        };

        PhotonNetwork.Instantiate(
            enemyPrefabName,
            spawnPoint.position,
            spawnPoint.rotation,
            0,
            instantiationData
        );

        Debug.Log("AI 적 유닛 생성");
    }
}
