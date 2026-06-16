using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

/**
 * 初期シーン（タイトル画面）管理クラス
 *
 * 主な役割
 * ・プレイヤーにMBTIを選択させる
 * ・MapSceneへ遷移する
 * ・選択されたMBTIをMapSceneへ引き継ぐ
 * ・最初の仲間をパーティへ追加する
 * ・マップ探索を開始する
 *
 * ゲーム開始時のみ使用されるクラス。
 */
public class InitSceneChoice : MonoBehaviour {
    // UI設定

    [Header("最初の選択UIをまとめた親Canvas")]

    // MBTI選択画面全体
    public GameObject firstChoiceCanvas;

    [Header("MBTI選択ドロップダウン")]

    // MBTI選択用Dropdown
    public TMP_Dropdown mbtiDropdown;

    /**
     * プレイヤーが選択したMBTI
     *
     * 例:
     * INTP
     * ENTP
     * INFJ
     */
    private string selectedMBTI = "";

    // ゲーム開始ボタン

    /**
     * スタートボタン押下時に呼ばれる
     *
     * 処理の流れ
     * ---------------
     * MBTI取得
     * ↓
     * UI非表示
     * ↓
     * MapScene読込
     * ↓
     * 初期キャラ付与
     * ↓
     * マップ開始
     * ---------------
     */
    public void OnStartButtonClick() {
        // Dropdown未設定チェック
        if (mbtiDropdown == null) {
            Debug.LogError(
                "Dropdownがインスペクターで設定されていません"
            );
            return;
        }

        // Dropdownの選択中テキスト取得
        string fullText =
            mbtiDropdown.options[mbtiDropdown.value].text;

        /**
         * MBTIコードのみ取り出す
         *
         * 例:
         * "INTP 論理学者"
         * ↓
         * "INTP"
         */
        if (fullText.Length >= 4) {
            selectedMBTI = fullText.Substring(0, 4);
        }else{
            selectedMBTI = fullText;
        }

        Debug.Log($"選択されたMBTI: {selectedMBTI}");

        // 選択画面を閉じる
        if (firstChoiceCanvas != null) {
            firstChoiceCanvas.SetActive(false);
        }

        /**
         * シーン切り替え時に破棄されないよう保護
         *
         * MapSceneに移動するまで
         * selectedMBTI を保持する必要がある
         */
        DontDestroyOnLoad(gameObject);

        // シーン読込完了イベント登録
        SceneManager.sceneLoaded += OnSceneLoaded;

        // マップシーンへ移動
        SceneManager.LoadScene("MapScene");
    }

    // シーン読込完了

    /**
     * 新しいシーンが読み込まれた時に呼ばれる
     */
    void OnSceneLoaded(
        Scene scene,
        LoadSceneMode mode
    )
    {
        // 二重登録防止
        SceneManager.sceneLoaded -= OnSceneLoaded;

        /**
         * PartyManagerなどの初期化が終わるまで
         * 1フレーム待つ
         */
        StartCoroutine(WaitAndAddCharacter());
    }

    // 初期キャラ付与

    /**
     * 1フレーム待機後に実行
     *
     * MapScene内のManager群が
     * 完全に初期化されてから処理する
     */
    IEnumerator WaitAndAddCharacter() {
        // 1フレーム待機
        yield return null;

        if (PartyManager.Instance != null) {
            Debug.Log(
                $"PartyManagerに {selectedMBTI} の獲得を要求します。"
            );

            /**
             * 初期キャラ追加
             *
             * 獲得演出終了後に
             * StartMap() が呼ばれる
             */
            PartyManager.Instance.AddCharacter(
                selectedMBTI,
                StartMap
            );
        }else{
            Debug.LogError(
                "MapSceneに PartyManager が見つかりません！ヒエラルキーを確認してください。"
            );
        }
    }

    // マップ開始

    /**
     * 初期キャラ獲得演出終了後に呼ばれる
     *
     * プレイヤー操作を解放し、
     * マップ探索を開始する
     */
    void StartMap() {
        Debug.Log("マップ探索を開始します！");

        // マップUI表示
        if (MapManager.Instance != null) {
            MapManager.Instance.SetMapUIActive(true);
        }

        // プレイヤー再表示
        PlayerMapMovement movement =
            FindObjectOfType<PlayerMapMovement>(true);

        if (movement != null) {
            movement.gameObject.SetActive(true);

            // プレイヤー操作有効化
            movement.enabled = true;
        }

        /**
         * このオブジェクトは
         * ゲーム開始後は不要になるため削除
         */
        Destroy(gameObject);
    }
}