using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMapMovement : MonoBehaviour {
    public float moveSpeed = 5f;
    private Rigidbody2D rb;
    private Vector2 moveInput;

    void Start() {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update() {
        float x = 0;
        float y = 0;

        if (Keyboard.current != null) {
            if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed) y = 1;
            if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed) y = -1;
            if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed) x = -1;
            if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed) x = 1;
        }

        moveInput = new Vector2(x, y).normalized;
    }

    void FixedUpdate() {
        rb.MovePosition(rb.position + moveInput * moveSpeed * Time.fixedDeltaTime);
    }
}