using UnityEngine;

/**
 * イベントシンボル
 *
 * マップ上に配置されるイベント用オブジェクト。
 * プレイヤーが接触すると EventManager に通知して
 * イベント画面を開始する。
 *
 * 責任：
 * ・プレイヤー接触の検知
 * ・EventManagerへの通知
 *
 * 責任外：
 * ・イベント内容の管理
 * ・報酬処理
 * ・イベントUI表示
 *
 * これらは EventManager が担当する。
 */
public class EventSymbol : MonoBehaviour {
    /**
     * Triggerにオブジェクトが侵入した時に呼ばれる
     */
    void OnTriggerEnter2D(Collider2D other) {
        // プレイヤーのみ反応
        if (other.CompareTag("Player")) {
            Debug.Log("イベントシンボルに接触！");

            // EventManagerへイベント開始を依頼
            if (EventManager.Instance != null) {
                EventManager.Instance.BeginEvent();
            }

            // 同じイベントを再度発生させないため無効化
            gameObject.SetActive(false);
        }
    }
}