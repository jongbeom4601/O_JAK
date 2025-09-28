using UnityEngine;

public enum ControlScheme {
    WASD,
    Arrows
}

public enum PlayerType {
    Geonwoo, // 견우
    Jiknyeo  // 직녀
}

[System.Serializable]
public class PlayerConfig {
    [Header("플레이어 구분(견우 1, 직녀 2)")]
    [SerializeField] private PlayerType playerType = PlayerType.Geonwoo;

    [Header("컨트롤 세트 선택")]
    [SerializeField] private ControlScheme controlScheme = ControlScheme.WASD;

    [Header("스킬")]
    [SerializeField] private int maxUses = 3;

    // 이동 키
    public KeyCode UpKey => controlScheme == ControlScheme.WASD ? KeyCode.W : KeyCode.UpArrow;
    public KeyCode DownKey => controlScheme == ControlScheme.WASD ? KeyCode.S : KeyCode.DownArrow;
    public KeyCode LeftKey => controlScheme == ControlScheme.WASD ? KeyCode.A : KeyCode.LeftArrow;
    public KeyCode RightKey => controlScheme == ControlScheme.WASD ? KeyCode.D : KeyCode.RightArrow;

    // 스킬 키
    public KeyCode SkillKey => controlScheme == ControlScheme.WASD ? KeyCode.Space : KeyCode.Return;

    public int MaxUses => maxUses;
    public PlayerType Type => playerType;
}
