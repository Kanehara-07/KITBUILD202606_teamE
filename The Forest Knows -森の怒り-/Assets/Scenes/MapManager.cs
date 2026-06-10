using UnityEngine;
using TMPro;
using System.Collections.Generic;

// エリアの種類を定義
public enum AreaType { Battle, Event, Companion, Boss }

public class MapManager : MonoBehaviour {
    [Header("UIテキスト")]
    public TextMeshProUGUI areaProgressText; // 「エリア 1 / 20」などの表示用
    public TextMeshProUGUI areaTypeText;     // 「戦闘エリア」などの表示用

    [Header("エリア設定")]
    public int currentArea = 1;
    public int maxArea = 20;
    
    // 全20エリアの種類を記憶するリスト
    private List<AreaType> mapRoute = new List<AreaType>();

    void Start() {
        GenerateMapRoute();
        UpdateMapUI();
    }

    // 20エリア分のルートを自動生成する（ランダム生成）
    void GenerateMapRoute() {
        mapRoute.Clear();

        for (int i = 1; i <= maxArea; i++) {
            if (i == 1) {
                // エリア1は「仲間エリア」（またはイベント）にして最初の準備期間にする
                mapRoute.Add(AreaType.Companion);
            }else if (i == maxArea){
                // 最後のエリア20は必ずボス（大樹）
                mapRoute.Add(AreaType.Boss);
            }else{
                // 2〜19エリアはランダム（戦闘かイベント）
                // Random.Range(0, 2) で 0か1 がランダムで出る
                AreaType randomType = (Random.Range(0, 2) == 0) ? AreaType.Battle : AreaType.Event;
                mapRoute.Add(randomType);
            }
        }
    }

    // 次のエリアに進むボタンを押した時の処理
    public void OnNextAreaButtonPressed() {
        if (currentArea >= maxArea) {
            Debug.Log("ゲームクリア");
            return;
        }

        currentArea++;
        UpdateMapUI();

        // 到着したエリアの種類に応じて処理を分岐
        AreaType currentType = mapRoute[currentArea - 1]; // 配列は0から始まるので -1
        TriggerAreaAction(currentType);
    }

    // エリアごとの処理（現状はログを出すだけ。ここに後で戦闘画面を開くなどの処理を入れます）
    void TriggerAreaAction(AreaType type) {
        switch (type) {
            case AreaType.Battle:
                Debug.Log($"エリア {currentArea}: 戦闘エリア");
                // ここでバトルUI（以前作ったもの）をオンにする
                break;
            case AreaType.Event:
                Debug.Log($"エリア {currentArea}: イベントエリア");
                break;
            case AreaType.Companion:
                Debug.Log($"エリア {currentArea}: 仲間エリア");
                break;
            case AreaType.Boss:
                Debug.Log($"エリア {currentArea}: ボスエリア");
                break;
        }
    }

    // 画面の文字を更新する関数
    void UpdateMapUI() {
        areaProgressText.text = $"エリア {currentArea} / {maxArea}";
        
        // 現在のエリアの種類を日本語にして表示
        AreaType currentType = mapRoute[currentArea - 1];
        switch (currentType) {
            case AreaType.Battle: areaTypeText.text = "種類：戦闘エリア"; break;
            case AreaType.Event: areaTypeText.text = "種類：イベントエリア"; break;
            case AreaType.Companion: areaTypeText.text = "種類：仲間エリア"; break;
            case AreaType.Boss: areaTypeText.text = "種類：ボスエリア（大樹）"; break;
        }
    }
}
