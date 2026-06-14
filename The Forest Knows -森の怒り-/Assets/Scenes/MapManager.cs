using UnityEngine;
using TMPro;
using System.Collections.Generic;

public enum AreaType { Battle, Event, Companion, Boss }

public class MapManager : MonoBehaviour {
    public static MapManager Instance;

    [Header("UIテキスト")]
    public TextMeshProUGUI areaProgressText;
    public TextMeshProUGUI areaTypeText;

    [Header("プレイヤー設定")]
    public Transform playerTransform;
    private Vector2 playerStartPosition = new Vector2(0f, -3f);

    [Header("エリア設定")]
    public int currentArea = 1;
    public int maxArea = 20;

    // ★ 隠してある「BattleField」をインスペクターで繋ぐための枠
    [Header("連動オブジェクト")]
    public GameObject battleField; 

    void Awake() {
        Instance = this;
    }

    void Start() {
        // 最初は敵フィールドを隠しておく
        if (battleField != null) battleField.SetActive(false);

        UpdateMapUI(AreaType.Companion);
    }

    public void AdvanceToNextArea(AreaType selectedType) {
        if (currentArea >= maxArea) {
            Debug.Log("ゲームクリア");
            return;
        }

        currentArea++;
        UpdateMapUI(selectedType);
        TriggerAreaAction(selectedType);

        if (playerTransform != null) {
            playerTransform.position = playerStartPosition;
        }
    }

    void TriggerAreaAction(AreaType type) {
        // 次のエリアに移動した時、一旦敵フィールドは非表示にする
        if (battleField != null) battleField.SetActive(false);

        switch (type) {
            case AreaType.Battle:
                Debug.Log($"エリア {currentArea}: 敵が出現");
                // ★ 戦闘エリアに入ったので、隠していた敵（BattleField）を自動で表示！
                if (battleField != null) battleField.SetActive(true);
                break;

            case AreaType.Event:
                Debug.Log($"エリア {currentArea}: イベント発生");
                break;
        }
    }

    void UpdateMapUI(AreaType type) {
        // 日本語をやめて英語表記にする（これで四角が消えます！）
        areaProgressText.text = $"AREA {currentArea} / {maxArea}";
    
        switch (type) {
            case AreaType.Battle: areaTypeText.text = "NEXT: BATTLE"; break;
            case AreaType.Event: areaTypeText.text = "NEXT: EVENT"; break;
            case AreaType.Companion: areaTypeText.text = "STATUS: START"; break;
            case AreaType.Boss: areaTypeText.text = "NEXT: BOSS"; break;
        }
    }
    
}