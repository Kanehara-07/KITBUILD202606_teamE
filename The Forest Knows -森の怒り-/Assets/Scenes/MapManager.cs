using UnityEngine;
using System.Collections.Generic;
using TMPro;

public enum AreaType { Battle, Event, Companion, Boss }

public class MapManager : MonoBehaviour {
    public static MapManager Instance;

    [Header("UIオブジェクト")]
    public GameObject mapCanvas; // ★ マップのCanvas全体を登録する枠を追加
    public TextMeshProUGUI areaProgressText;
    public TextMeshProUGUI areaTypeText;

    [Header("プレイヤー設定")]
    public Transform playerTransform;
    private Vector2 playerStartPosition = new Vector2(0f, -3f);

    [Header("エリア設定")]
    public int currentArea = 1;
    public int maxArea = 20;

    [Header("連動オブジェクト")]
    public GameObject battleField;
    public GameObject eventField; 
    public GameObject gateBattle;  
    public GameObject gateEvent;   

    void Awake() {
        Instance = this;
    }

    void Start() {
        if (battleField != null) battleField.SetActive(false);
        if (eventField != null) eventField.SetActive(false);
        SetGatesActive(true); 
        UpdateMapUI(AreaType.Companion);
    }

    public void AdvanceToNextArea(AreaType selectedType) {
        if (currentArea >= maxArea) {
            Debug.Log("ゲームクリア！");
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
        if (battleField != null) battleField.SetActive(false);
        if (eventField != null) eventField.SetActive(false);

        switch (type) {
            case AreaType.Battle:
                Debug.Log($"エリア {currentArea}: 敵が出現！上のゲートを一時的に隠します。");
                SetGatesActive(false);
                if (battleField != null) {
                    battleField.SetActive(true);
                    battleField.transform.position = new Vector3(0f, -0.5f, 0f); // 少し下げた位置
                    
                    // 非アクティブになって眠っている子供の敵シンボルを強制的に復活させる
                    EnemySymbol enemy = battleField.GetComponentInChildren<EnemySymbol>(true);
                    if (enemy != null) {
                        enemy.gameObject.SetActive(true);
                    }
                }
                break;

            case AreaType.Event:
                Debug.Log($"エリア {currentArea}: イベント発生");
                SetGatesActive(true); 
                if (eventField != null) {
                    eventField.SetActive(true);
                    eventField.transform.position = new Vector3(0f, -0.5f, 0f); 

                    EventSymbol evSymbol = eventField.GetComponentInChildren<EventSymbol>(true);
                    if (evSymbol != null) {
                        evSymbol.gameObject.SetActive(true);
                    }
                }
                break;
        }
    }

    public void SetGatesActive(bool isActive) {
        if (gateBattle != null) gateBattle.SetActive(isActive);
        if (gateEvent != null) gateEvent.SetActive(isActive);
    }

    // ★ バトル開始時に、マップの文字UI（MapCanvas）の表示・非表示を切り替える関数
    public void SetMapUIActive(bool isActive) {
        if (mapCanvas != null) mapCanvas.SetActive(isActive);
    }

    public void OnBattleWin() {
        Debug.Log("敵の撃破！次のエリアへのゲートを出現させます。");
        SetGatesActive(true); 
    }
    
    public void OnEventComplete() {
        Debug.Log("イベント完了！次のゲートを出現させます。");
        SetGatesActive(true); 
    }

    void UpdateMapUI(AreaType type) {
        areaProgressText.text = $"エリア {currentArea} / {maxArea}";
        
        switch (type) {
            case AreaType.Battle: areaTypeText.text = "エリア:バトル"; break;
            case AreaType.Event: areaTypeText.text = "エリア: イベント"; break;
            case AreaType.Companion: areaTypeText.text = "エリア: 始まり"; break;
            case AreaType.Boss: areaTypeText.text = "エリア: ボス"; break;
        }
    }
}