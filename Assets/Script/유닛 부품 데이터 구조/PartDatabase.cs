using UnityEngine;
using System.Collections.Generic;

public class PartDatabase : MonoBehaviour
{
    public static PartDatabase Instance;

    [Header("Debug View (Read Only)")]
    public LegPartData[] legs;
    public CorePartData[] cores;
    public WeaponPartData[] weapons;

    private Dictionary<string, LegPartData> legDict = new Dictionary<string, LegPartData>();
    private Dictionary<string, CorePartData> coreDict = new Dictionary<string, CorePartData>();
    private Dictionary<string, WeaponPartData> weaponDict = new Dictionary<string, WeaponPartData>();

    private void Awake()
    {
        if (Instance == null) { Instance = this; DontDestroyOnLoad(gameObject); }
        else { Destroy(gameObject); return; }

        LoadAllParts();
    }

    private void LoadAllParts()
    {
        // 1. 로드
        legs = Resources.LoadAll<LegPartData>("Parts/Legs");
        cores = Resources.LoadAll<CorePartData>("Parts/Cores");
        weapons = Resources.LoadAll<WeaponPartData>("Parts/Weapons");

        // 2. 딕셔너리 구축 (이제 dynamic 에러 안 남!)
        BuildDictionary(legs, legDict, "Leg");
        BuildDictionary(cores, coreDict, "Core");
        BuildDictionary(weapons, weaponDict, "Weapon");

        Debug.Log($"DB 로드 완료 - 다리: {legDict.Count}, 코어: {coreDict.Count}, 무기: {weaponDict.Count}");
    }

    // [핵심 변경] T가 이제 PartBase를 상속받는다고 명시함
    private void BuildDictionary<T>(T[] array, Dictionary<string, T> dict, string typeName) where T : PartBase
    {
        foreach (var item in array)
        {
            // dynamic 삭제! 이제 부모 클래스(PartBase)의 id를 바로 씀
            string itemId = item.id;

            if (string.IsNullOrEmpty(itemId))
            {
                Debug.LogError($"[ID 없음] {typeName}: {item.name}");
                continue;
            }

            if (dict.ContainsKey(itemId))
            {
                Debug.LogError($"[중복 ID] {typeName}: {itemId}");
            }
            else
            {
                dict.Add(itemId, item);
            }
        }
    }

    public LegPartData GetLeg(string id)
    {
        if (string.IsNullOrEmpty(id)) return null;
        if (legDict.TryGetValue(id, out LegPartData part)) return part;
        return null;
    }

    public CorePartData GetCore(string id)
    {
        if (string.IsNullOrEmpty(id)) return null;
        if (coreDict.TryGetValue(id, out CorePartData part)) return part;
        return null;
    }

    public WeaponPartData GetWeapon(string id)
    {
        if (string.IsNullOrEmpty(id)) return null;
        if (weaponDict.TryGetValue(id, out WeaponPartData part)) return part;
        return null;
    }
}