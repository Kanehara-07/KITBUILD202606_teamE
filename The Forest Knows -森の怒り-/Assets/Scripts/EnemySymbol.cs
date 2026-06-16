using UnityEngine;

public class EnemySymbol : MonoBehaviour {
    void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Player")) {
            Debug.Log($"敵シンボルに接触！バトルに移行します。");
            
            BattleManager battleManager = FindObjectOfType<BattleManager>();
            if (battleManager != null) {
                battleManager.BeginBattle();
            }

            // マップ上の敵シンボル（自分自身）は消す
            gameObject.SetActive(false);
        }
    }
}