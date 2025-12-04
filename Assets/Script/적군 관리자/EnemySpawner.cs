using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Spawn Info")]
    public GameObject enemyPrefab; // 생성할 적 유닛 프리팹
    public Transform spawnPoint;   // 생성 위치 (EnemyBase 앞)
    public float spawnInterval = 10f; // 몇 초마다 생성할지?

    [Header("Enemy Parts")]
    // 적 유닛에게 달아줄 부품들 (Inspector에서 설정)
    public LegPartData leg;
    public CorePartData core;
    public WeaponPartData weapon;

    private float timer = 0f;

    void Update()
    {
        timer += Time.deltaTime;

        // 시간이 되면 스폰!
        if (timer >= spawnInterval)
        {
            SpawnEnemy();
            timer = 0f; // 타이머 초기화
        }
    }

    void SpawnEnemy()
    {
        if (enemyPrefab == null || spawnPoint == null) return;

        // 1. 유닛 생성
        GameObject newEnemy = Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);

        // 2. 태그 설정 (중요: 적 기지의 태그를 따라감 -> Enemy)
        newEnemy.tag = gameObject.tag;

        // 3. 부품 조립
        UnitAssembler assembler = newEnemy.GetComponent<UnitAssembler>();
        if (assembler != null)
        {
            assembler.legPart = leg;
            assembler.corePart = core;
            assembler.weaponPart = weapon;
            assembler.AssembleUnit();
        }

        Debug.Log("적군 증원군 도착!");
    }
}