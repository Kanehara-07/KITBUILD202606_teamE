using UnityEngine;

/**
 * 初期キャラクター選択管理クラス
 *
 * ゲーム開始時にプレイヤーへ
 * 最初のMBTIキャラクターを選ばせる。
 *
 * 流れ
 * -----------------------
 * MBTI選択画面表示
 * ↓
 * プレイヤーが選択
 * ↓
 * PartyManagerへ追加
 * ↓
 * 獲得演出
 * ↓
 * マップ探索開始
 * -----------------------
 */
public class FirstChoiceManager : MonoBehaviour {
    [Header("最初の選択UIをまとめた親")]

    /**
     * 初期選択画面全体
     *
     * MBTIボタンなどを含むCanvas
     */
    public GameObject firstChoiceCanvas;

    //==================================================
    // 初期キャラクター選択
    //==================================================

    /**
     * プレイヤーがMBTIを選択した時に呼ばれる
     *
     * UIボタンの OnClick() から実行する。
     *
     * 例：
     * "INTP"
     * "ENTJ"
     * "INFP"
     */
    public void SelectInitialCharacter(string mbtiId) {
        // 選択画面を閉じる
        if (firstChoiceCanvas != null) {
            firstChoiceCanvas.SetActive(false);
        }

        /**
         * PartyManagerでキャラクター獲得
         *
         * 獲得演出が終わった後、
         * OnGetSequenceComplete が自動実行される
         */
        PartyManager.Instance.AddCharacter(
            mbtiId,
            OnGetSequenceComplete
        );
    }

    //==================================================
    // 初期キャラ獲得完了
    //==================================================

    /**
     * 最初のキャラ獲得演出終了後に呼ばれる
     *
     * この時点でゲーム開始となり、
     * プレイヤーがマップ探索可能になる。
     */
    void OnGetSequenceComplete() {
        Debug.Log(
            "最初のキャラ獲得。マップ行動開始！"
        );

        // マップUI表示
        if (MapManager.Instance != null) {
            MapManager.Instance.SetMapUIActive(true);

            /**
             * 将来的には
             * 最初のエリアへ強制移動させる処理などを
             * ここに追加できる
             */

            // MapManager.Instance.AdvanceToNextArea(
            //     AreaType.Battle
            // );
        }

        /**
         * 非表示状態だったプレイヤーを復帰
         */
        PlayerMapMovement movement =
            FindObjectOfType<PlayerMapMovement>(true);

        if (movement != null) {
            movement.gameObject.SetActive(true);

            // プレイヤー操作再開
            movement.enabled = true;
        }
    }
}