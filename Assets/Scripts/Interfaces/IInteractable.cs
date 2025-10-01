using UnityEngine;

public interface IInteractable {
    int Priority { get; }  // 각 오브젝트의 상호작용 순위
    void Interact(GameObject interactor, Vector2 dir);
}

public interface IOnEnter { void OnEnter(GameObject interactor, Vector2 dir); }
public interface IOnStay { void OnStay(GameObject interactor); }
public interface IOnExit { void OnExit(GameObject interactor); }