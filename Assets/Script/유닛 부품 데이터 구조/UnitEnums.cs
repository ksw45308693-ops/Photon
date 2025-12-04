// 부품의 종류
public enum PartType
{
    Leg,    // 기동부 (다리)
    Core,   // 몸통
    Arm,    // 팔 (옵션)
    Weapon  // 무기
}

// 공격 타입 (노바2 스타일 상성 구현용)
public enum AttackType
{
    Physical, // 물리 (총알)
    Explosive,// 폭발 (미사일)
    Beam,     // 빔
    Melee     // 근접
}