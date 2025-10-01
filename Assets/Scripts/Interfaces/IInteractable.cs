using UnityEngine;

public interface IInteractable {
    int Priority { get; }  // �� ������Ʈ�� ��ȣ�ۿ� ����
    void Interact(GameObject interactor, Vector2 dir);
}

public interface IOnEnter { void OnEnter(GameObject interactor, Vector2 dir); }
public interface IOnStay { void OnStay(GameObject interactor); }
public interface IOnExit { void OnExit(GameObject interactor); }