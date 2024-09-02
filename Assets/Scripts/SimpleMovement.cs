using UnityEngine;

public class SimpleMovement : MonoBehaviour
{
    public float speed = 5f; // Speed of the movement

    void Update()
    {
        // Get input from arrow keys and W, A, S, D
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        // Calculate movement direction
        Vector2 movement = new Vector2(horizontal, vertical) * speed * Time.deltaTime;

        // Move the sprite
        transform.Translate(movement);
    }
}