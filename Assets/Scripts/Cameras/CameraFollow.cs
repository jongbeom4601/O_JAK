using UnityEngine;


public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public float smoothSpeed = 5f;

    void LateUpdate()
    {
        if (target != null)
        {
            Vector3 newPos = transform.position;
            newPos.x = Mathf.Lerp(transform.position.x, target.position.x, smoothSpeed * Time.deltaTime);
            newPos.y = Mathf.Lerp(transform.position.y, target.position.y, smoothSpeed * Time.deltaTime);
            transform.position = new Vector3(newPos.x, newPos.y, -10f); // z = -10 °íÁ¤ (2D¿ë)
        }
    }
}
