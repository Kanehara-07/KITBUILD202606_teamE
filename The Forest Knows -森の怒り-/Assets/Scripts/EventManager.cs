using UnityEngine;
using TMPro;

/**
 * イベント管理クラス
 *
 * 主な役割
 * ・イベント画面の表示
 * ・選択肢の管理
 * ・イベント報酬の付与
 * ・イベント終了後のマップ復帰
 *
 * このゲームにおける
 * 「トロッコ問題」
 * 「MBTI獲得イベント」
 * 「診断イベント」
 * などを管理する。
 */
public class EventManager : MonoBehaviour {
    /**
     * Singleton
     *
     * 他クラスから
     * EventManager.Instance
     * でアクセスする
     */
    public static EventManager Instance;

    //==================================================
    // UI設定
    //==================================================

    [Header("UI設定")]

    // イベント画面全体
    public GameObject eventCanvas;

    // 選択肢ボタンをまとめた親オブジェクト
    public GameObject choicePanel;

    // 結果表示用パネル
    public GameObject resultPanel;

    // イベント本文
    public TextMeshProUGUI eventText;

    // 報酬や結果表示用テキスト
    public TextMeshProUGUI resultText;

    /**
     * イベント開始前に存在していた
     * マップ上のプレイヤーオブジェクト
     */
    private GameObject playerMapObj;

    /**
     * Singleton初期化
     */
    void Awake() {
        Instance = this;
    }

    /**
     * ゲーム開始時
     *
     * イベント画面は非表示にしておく
     */
    void Start() {
        if (eventCanvas != null) {
            eventCanvas.SetActive(false);
        }
    }

    //==================================================
    // イベント開始
    //==================================================

    /**
     * イベント開始処理
     *
     * ・マップUI非表示
     * ・プレイヤー操作停止
     * ・イベント画面表示
     * ・イベント内容設定
     */
    public void BeginEvent() {
        // マップUIを隠す
        if (MapManager.Instance != null) {
            MapManager.Instance.SetMapUIActive(false);

            // エリア移動用ゲートも無効化
            MapManager.Instance.SetGatesActive(false);
        }

        // プレイヤーを一時的に非表示
        PlayerMapMovement movement =
            FindObjectOfType<PlayerMapMovement>();

        if (movement != null) {
            playerMapObj = movement.gameObject;
            playerMapObj.SetActive(false);
        }

        // イベント画面表示
        if (eventCanvas != null) {
            eventCanvas.SetActive(true);
        }

        // 選択肢表示
        choicePanel.SetActive(true);

        // 結果画面は非表示
        resultPanel.SetActive(false);

        //==================================================
        // 仮イベント
        // トロッコ問題
        //==================================================

        eventText.text =
            "【運命の選択：トロッコ問題】\n" +
            "暴走したトロッコが走っている。\n" +
            "このままでは5人の作業員が犠牲になる。\n" +
            "レバーを引けば子供1人の犠牲で済むが…どうする？";
    }

    //==================================================
    // 選択肢処理
    //==================================================

    /**
     * 選択肢1
     *
     * レバーを引く
     *
     * 現在の仕様：
     * INTPを獲得
     */
    public void OnChoice1Selected() {
        // 選択肢を隠す
        choicePanel.SetActive(false);

        /**
         * キャラ獲得演出終了後に
         * EndEventが自動で呼ばれる
         */
        PartyManager.Instance.AddCharacter(
            "INTP",
            EndEvent
        );
    }

    /**
     * 選択肢2
     *
     * 何もしない
     *
     * 現在の仕様：
     * INFPを獲得
     */
    public void OnChoice2Selected() {
        choicePanel.SetActive(false);

        PartyManager.Instance.AddCharacter(
            "INFP",
            EndEvent
        );
    }

    //==================================================
    // 将来使う予定の結果表示システム
    //==================================================

    /*
    void ShowResult(string text)
    {
        // 選択肢を隠す
        choicePanel.SetActive(false);

        // 結果画面を表示
        resultPanel.SetActive(true);

        // 結果テキスト設定
        resultText.text = text;

        // 3秒後にイベント終了
        Invoke("EndEvent", 3.0f);
    }
    */

    //==================================================
    // イベント終了
    //==================================================

    /**
     * イベント終了処理
     *
     * ・イベント画面を閉じる
     * ・プレイヤーを再表示
     * ・マップUI再表示
     * ・MapManagerへ終了通知
     */
    void EndEvent() {
        // イベント画面を閉じる
        if (eventCanvas != null) {
            eventCanvas.SetActive(false);
        }

        // プレイヤー再表示
        if (playerMapObj != null) {
            playerMapObj.SetActive(true);

            playerMapObj
                .GetComponent<PlayerMapMovement>()
                .enabled = true;
        }

        // マップUI再表示
        if (MapManager.Instance != null) {
            MapManager.Instance.SetMapUIActive(true);
        }

        // イベント完了通知
        MapManager.Instance.OnEventComplete();
    }
}