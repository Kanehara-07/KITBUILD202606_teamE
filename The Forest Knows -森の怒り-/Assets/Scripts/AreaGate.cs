using UnityEngine;

/**
 * エリア移動用ゲート
 * 
 * プレイヤーがゲートに接触すると、
 * MapManagerへ通知して次のエリアへ進ませる。
 * 
 * 例：
 * Battleエリアをクリア
 * ↓
 * ゲートが出現
 * ↓
 * プレイヤーがゲートに触れる
 * ↓
 * 次のエリアへ移動
 */
public class AreaGate : MonoBehaviour {
    /**
     * このゲートの遷移先エリアタイプ
     * Inspectorから設定する
     * 
     * 例：
     * Battle
     * Event
     * Companion
     * Boss
     */
    public AreaType targetAreaType;

    /**
     * Triggerにオブジェクトが侵入した時に呼ばれる
     * 
     * プレイヤーが触れた場合のみ
     * MapManagerにエリア遷移を依頼する
     */
    void OnTriggerEnter2D(Collider2D other) {
        // デバッグ用
        // ゲートに接触したオブジェクト名を表示する
        Debug.Log(
            $"[ゲート検証]「{gameObject.name}」に、何か（{other.name}）が物理的に触れました！"
        );

        // Playerタグを持つオブジェクトのみ反応
        if (other.CompareTag("Player")) {
            Debug.Log(
                "[ゲート検証] Playerタグの判定に成功！エリアを進めます。"
            );

            // 次のエリアへ進む処理をMapManagerへ依頼
            MapManager.Instance.AdvanceToNextArea(targetAreaType);
        }
    }
}