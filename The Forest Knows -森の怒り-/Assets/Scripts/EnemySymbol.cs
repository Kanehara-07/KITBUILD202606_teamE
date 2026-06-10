using UnityEngine;

public class EnemySymbol : MonoBehaviour {
    // インスペクターでBattleManagerのオブジェクトを結びつける
    public GameObject battleUI; 

    // 何かがこの敵（Trigger）に触れた瞬間に実行されるUnityの基本機能
    void OnTriggerEnter2D(Collider2D other) {
        // ぶつかってきた相手のタグが「Player」だった場合
        if (other.CompareTag("Player")) {
            Debug.Clone($"【接触】敵とぶつかった！バトル開始！");
            
            // バトル画面（前回作ったActionPanelなど）を表示する
            if (battleUI != null) {
                battleUI.SetActive(true);
            }

            // 戦闘が始まったので、マップ上の敵シンボルは消す
            gameObject.SetActive(false);
        }
    }
}