using UnityEngine;

public class BackgroundScroller : MonoBehaviour
{
    [SerializeField] float scrollSpeed = 2f;
    [SerializeField] float resetPositionX = -4.5f;   // cuando sale, se teletransporta
    [SerializeField] float teleportDistance = 9f;  // ancho total (2 fondos)

    void Update()
    {
        transform.Translate(Vector2.left * scrollSpeed * Time.deltaTime);

        if (transform.position.x < resetPositionX)
        {
            Vector3 pos = transform.position;
            pos.x += teleportDistance;
            transform.position = pos;
        }
    }
}