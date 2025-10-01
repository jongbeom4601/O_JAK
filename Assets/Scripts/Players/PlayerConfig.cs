using UnityEngine;

public enum ControlScheme {
    WASD,
    Arrows
}

public enum PlayerType {
    Geonwoo, // �߿�
    Jiknyeo  // ����
}

[System.Serializable]
public class PlayerConfig {
    [Header("�÷��̾� ����(�߿� 1, ���� 2)")]
    [SerializeField] private PlayerType playerType = PlayerType.Geonwoo;

    [Header("��Ʈ�� �÷��̾�")]
    [SerializeField] private GameObject partner;   // Inspector���� ���� ����

    [Header("��Ʈ�� ��Ʈ ����")]
    [SerializeField] private ControlScheme controlScheme = ControlScheme.WASD;

    [Header("��ų")]
    [SerializeField] private int maxUses = 3;

    [Header("������/����")]
    public bool hasKey = false;

    // �̵� Ű
    public KeyCode UpKey => controlScheme == ControlScheme.WASD ? KeyCode.W : KeyCode.UpArrow;
    public KeyCode DownKey => controlScheme == ControlScheme.WASD ? KeyCode.S : KeyCode.DownArrow;
    public KeyCode LeftKey => controlScheme == ControlScheme.WASD ? KeyCode.A : KeyCode.LeftArrow;
    public KeyCode RightKey => controlScheme == ControlScheme.WASD ? KeyCode.D : KeyCode.RightArrow;

    // ��ų Ű
    public KeyCode SkillKey => controlScheme == ControlScheme.WASD ? KeyCode.Space : KeyCode.Return;

    public int MaxUses => maxUses;
    public PlayerType Type => playerType;
    public GameObject Partner => partner;
}
