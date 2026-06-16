using UnityEngine;
using UnityEngine.InputSystem;

/**
 * マップ上のプレイヤー移動管理
 *
 * WASDキーまたは矢印キーで移動する。
 *
 * 使用コンポーネント
 * ・Rigidbody2D
 */
public class PlayerMapMovement : MonoBehaviour {
    // 移動速度
    public float moveSpeed = 5f;

    // Rigidbody参照
    private Rigidbody2D rb;

    // 入力方向
    private Vector2 moveInput;

    /**
     * 初期化
     */
    void Start() {
        rb = GetComponent<Rigidbody2D>();
    }

    /**
     * キー入力取得
     */
    void Update() {
        float x = 0;
        float y = 0;

        if (Keyboard.current != null) {
            // 上移動
            if (
                Keyboard.current.wKey.isPressed ||
                Keyboard.current.upArrowKey.isPressed
            )
            {
                y = 1;
            }

            // 下移動
            if (
                Keyboard.current.sKey.isPressed ||
                Keyboard.current.downArrowKey.isPressed
            )
            {
                y = -1;
            }

            // 左移動
            if (
                Keyboard.current.aKey.isPressed ||
                Keyboard.current.leftArrowKey.isPressed
            )
            {
                x = -1;
            }

            // 右移動
            if (
                Keyboard.current.dKey.isPressed ||
                Keyboard.current.rightArrowKey.isPressed
            )
            {
                x = 1;
            }
        }

        // 斜め移動速度を統一
        moveInput = new Vector2(x, y).normalized;
    }

    /**
     * 物理移動処理
     */
    void FixedUpdate() {
        rb.MovePosition(
            rb.position +
            moveInput *
            moveSpeed *
            Time.fixedDeltaTime
        );
    }
}