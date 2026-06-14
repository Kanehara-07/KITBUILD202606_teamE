using UnityEngine;

public class EnemySymbol : MonoBehaviour
{
    public GameObject battleUI; // BattleCanvas（またはActionPanel）を繋ぐ枠

    void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Player")) {
            Debug.Log($"敵とぶつかった！戦闘開始");
            
            // バトル用のUI（通常攻撃ボタンなど）を表示する
            if (battleUI != null) {
                battleUI.SetActive(true);
            }

            // ★ シーンにいる BattleManager を探して、戦闘をスタートさせる
            BattleManager battleManager = FindObjectOfType<BattleManager>();
            if (battleManager != null) {
                battleManager.BeginBattle();
            }

            // マップ上の敵シンボル（自分自身）は消す
            gameObject.SetActive(false);
        }
    }
    
}
