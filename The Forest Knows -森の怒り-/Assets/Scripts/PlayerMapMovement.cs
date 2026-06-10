using UnityEngine;

public class PlayerMapMovement : MonoBehaviour {
    public float moveSpeed = 5f;
    private Rigidbody2D rb;
    private Vector2 moveInput;

    void Start() {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update() {
        // 矢印キーまたはWASDの入力を取得
        moveInput.x = Input.GetAxisRaw("Horizontal");
        moveInput.y = Input.GetAxisRaw("Vertical");
    }

    void FixedUpdate() {
        // プレイヤーを移動させる
        rb.MovePosition(rb.position + moveInput.normalized * moveSpeed * Time.fixedDeltaTime);
    }
}
