using UnityEngine;


//public class Key : MonoBehaviour, IInteractable {
//    public int Priority => 40; // 3¼øÀ§

//    public void Interact(GameObject interactor, Vector2 direction) {
//        var input = interactor.GetComponent<PlayerInput>();
//        var movement = interactor.GetComponent<PlayerMovement>();

//        if (input != null && movement != null) {
//            if (!input.Config.hasKey) {
//                input.Config.hasKey = true;
//                Debug.Log("¿­¼è È¹µæ!");
//                Destroy(gameObject);
//            }

//            movement.MoveTo(transform.position);
//        }
//    }
//}

public class Key : MonoBehaviour, IOnEnter {
    public void OnEnter(GameObject interactor, Vector2 dir) {
        var input = interactor.GetComponent<PlayerInput>();

        if (!input.Config.hasKey) {
            input.Config.hasKey = true;
            Debug.Log("¿­¼è È¹µæ!");
            Destroy(gameObject);
        }
    }
}