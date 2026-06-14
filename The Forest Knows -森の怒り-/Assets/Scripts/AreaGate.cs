using UnityEngine;

public class AreaGate : MonoBehaviour {
    public AreaType targetAreaType; 

    void OnTriggerEnter2D(Collider2D other) {
        // ★ 誰が触れても、当たった瞬間に必ずコンソールに名前を出すログ
        Debug.Log($"[ゲート検証]「{gameObject.name}」に、何か（{other.name}）が物理的に触れました！");

        if (other.CompareTag("Player")) {
            Debug.Log("[ゲート検証] Playerタグの判定に成功！エリアを進めます。");
            MapManager.Instance.AdvanceToNextArea(targetAreaType);
        }
    }
}