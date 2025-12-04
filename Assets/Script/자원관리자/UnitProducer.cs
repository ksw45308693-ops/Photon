using UnityEngine;

public class UnitProducer : MonoBehaviour
{
    [Header("Settings")]
    public GameObject unitPrefab;   // 생성할 유닛의 껍데기 (Unit 프리팹)
    public Transform spawnPoint;    // 유닛이 태어날 위치 (기지 앞)

    // [중요] 유닛 레시피 정의 (부품 조합 + 가격)
    // Inspector에서 보이기 위해 System.Serializable 필요
    [System.Serializable]
    public struct UnitRecipe
    {
        public string name;          // 유닛 이름 (예: 정찰기)
        public int cost;             // 생산 비용
        public LegPartData leg;      // 다리 부품
        public CorePartData core;    // 몸통 부품
        public WeaponPartData weapon;// 무기 부품
    }

    public UnitRecipe[] recipes; // 생산 가능한 유닛 목록

    // 버튼에서 호출할 함수 (index: 0번 유닛, 1번 유닛...)
    public void ProduceUnit(int recipeIndex)
    {
        // 1. 레시피가 유효한지 확인
        if (recipeIndex < 0 || recipeIndex >= recipes.Length)
        {
            Debug.LogError("잘못된 유닛 번호입니다!");
            return;
        }

        UnitRecipe recipe = recipes[recipeIndex];

        // 2. 자원이 충분한지 확인 (ResourceManager 사용)
        if (ResourceManager.Instance.UseWatt(recipe.cost))
        {
            Spawn(recipe);
        }
        else
        {
            Debug.Log("와트가 부족합니다!");
        }
    }

    void Spawn(UnitRecipe recipe)
    {
        // 3. 유닛 생성 (Instantiate)
        GameObject newUnit = Instantiate(unitPrefab, spawnPoint.position, spawnPoint.rotation);

        // 4. 태그 설정 (기지와 같은 편으로 설정)
        newUnit.tag = gameObject.tag; // Player 기지면 Player 유닛, Enemy면 Enemy 유닛

        // 5. 부품 조립 (데이터 주입)
        UnitAssembler assembler = newUnit.GetComponent<UnitAssembler>();
        if (assembler != null)
        {
            assembler.legPart = recipe.leg;
            assembler.corePart = recipe.core;
            assembler.weaponPart = recipe.weapon;

            // 조립 실행! (Start보다 먼저 데이터를 넣었으므로 바로 적용됨)
            assembler.AssembleUnit();
        }

        Debug.Log($"{recipe.name} 생산 완료!");
    }
}