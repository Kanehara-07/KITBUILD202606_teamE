using System.Collections.Generic;
using UnityEngine;

/**
 * 戦闘参加キャラクターのデータクラス
 *
 * 味方・敵の両方で使用する共通データ。
 * BattleManager が管理する。
 */
[System.Serializable]
public class BattleCharacter {
    // キャラクター名
    public string name;

    // 現在HP
    public int currentHP;

    // 最大HP
    public int maxHP;

    // 行動速度
    public int speed;

    /**
     * 行動値(Action Value)
     *
     * スタレ風の速度システムを簡易再現。
     * 値が小さいキャラから行動する。
     */
    public float actionValue;

    // 敵かどうか
    public bool isEnemy;

    /**
     * ターン終了後に次回行動までの待機値を設定
     *
     * speed が高いほど早く次のターンが回ってくる。
     */
    public void ResetActionValue() {
        actionValue = 10000f / speed;
    }
}

/**
 * 戦闘全体を管理するクラス
 *
 * 主な役割
 * ・戦闘開始
 * ・ターン管理
 * ・行動処理
 * ・ダメージ計算
 * ・戦闘終了
 */
public class BattleManager : MonoBehaviour {
    /**
     * 現在戦闘に参加しているキャラクター一覧
     *
     * 将来的には
     * 味方4人 + 敵複数
     * の構成になる予定
     */
    public List<BattleCharacter> participants = new List<BattleCharacter>();

    [Header("UI設定")]

    // 戦闘画面全体
    public GameObject battleCanvas;

    // プレイヤーコマンドUI
    public GameObject actionPanel;

    // 現在ターン中のキャラクター
    private BattleCharacter currentTurnCharacter;

    // 戦闘前にいたマップ上のプレイヤー
    private GameObject playerMapObj;

    /**
     * 初期化
     *
     * ゲーム開始時は戦闘画面を非表示にする
     */
    void Start() {
        if (battleCanvas != null)
            battleCanvas.SetActive(false);
    }

    //==================================================
    // 戦闘開始処理
    //==================================================

    /**
     * 戦闘開始
     *
     * マップを隠して戦闘画面へ切り替える
     */
    public void BeginBattle() {
        // マップUIを非表示
        if (MapManager.Instance != null) {
            MapManager.Instance.SetMapUIActive(false);
        }

        // プレイヤーを一時的に非表示
        PlayerMapMovement movement = FindObjectOfType<PlayerMapMovement>();

        if (movement != null) {
            playerMapObj = movement.gameObject;
            playerMapObj.SetActive(false);
        }

        // 戦闘画面表示
        if (battleCanvas != null)
            battleCanvas.SetActive(true);

        // 仮戦闘データ生成
        SetupTestBattle();

        // ターン進行開始
        ProgressTurn();
    }

    /**
     * テスト用戦闘データ生成
     *
     * 将来的にはPartyManagerやEnemyDataから
     * データを取得する予定
     */
    void SetupTestBattle() {
        participants.Clear();

        BattleCharacter player = new BattleCharacter {
            name = "ENTP(味方)",
            currentHP = 100,
            maxHP = 100,
            speed = 120,
            isEnemy = false
        };

        player.ResetActionValue();
        participants.Add(player);

        BattleCharacter enemy = new BattleCharacter {
            name = "森の動く切り株(敵)",
            currentHP = 50,
            maxHP = 50,
            speed = 80,
            isEnemy = true
        };

        enemy.ResetActionValue();
        participants.Add(enemy);
    }


    // ターン管理

    /**
     * 次の行動キャラクターを決定
     *
     * actionValueが最も小さいキャラが行動する
     */
    void ProgressTurn() {
        if (participants.Count == 0)
            return;

        BattleCharacter nextChar = participants[0];

        foreach (var c in participants) {
            if (c.actionValue < nextChar.actionValue) {
                nextChar = c;
            }
        }

        float reduction = nextChar.actionValue;

        foreach (var c in participants) {
            c.actionValue -= reduction;
        }

        currentTurnCharacter = nextChar;

        ExecuteCharacterTurn(currentTurnCharacter);
    }

    /**
     * キャラクターのターン実行
     */
    void ExecuteCharacterTurn(BattleCharacter character) {
        // 戦闘不能ならスキップ
        if (character.currentHP <= 0) {
            ProgressTurn();
            return;
        }

        Debug.Log(
            $"【{character.name}】のターン！ (残りHP: {character.currentHP})"
        );

        // プレイヤーターン
        if (!character.isEnemy) {
            actionPanel.SetActive(true);
        // 敵のターン
        }else{
            actionPanel.SetActive(false);

            Debug.Log("敵の攻撃！");

            BattleCharacter target =
                participants.Find(c => !c.isEnemy);

            target.currentHP -= 15;

            character.ResetActionValue();

            Invoke("ProgressTurn", 1.0f);
        }
    }

    //==================================================
    // プレイヤー操作
    //==================================================

    /**
     * 通常攻撃ボタン
     */
    public void OnAttackButtonPressed()
    {
        if (currentTurnCharacter == null ||
            currentTurnCharacter.isEnemy)
        {
            return;
        }

        Debug.Log("【通常攻撃】を実行！ 敵に20ダメージ！");

        DamageEnemy(20);
    }

    /**
     * スキルボタン
     *
     * 現在は固定ダメージ
     * 将来的にはMBTI固有スキルになる
     */
    public void OnSkillButtonPressed() {
        if (currentTurnCharacter == null ||
            currentTurnCharacter.isEnemy)
        {
            return;
        }

        Debug.Log(
            "【スキル：???】を発動！ 35ダメージ！"
        );

        DamageEnemy(35);
    }

    //==================================================
    // ダメージ処理
    //==================================================

    /**
     * 敵にダメージを与える
     */
    void DamageEnemy(int damage) {
        BattleCharacter enemyTarget =
            participants.Find(c => c.isEnemy);

        enemyTarget.currentHP -= damage;

        actionPanel.SetActive(false);

        // 敵撃破
        if (enemyTarget.currentHP <= 0) {
            EndBattle();
            return;
        }

        currentTurnCharacter.ResetActionValue();

        ProgressTurn();
    }

    //==================================================
    // 戦闘終了処理
    //==================================================

    /**
     * 勝利時の処理
     *
     * マップへ戻り、
     * MapManagerへ勝利通知を送る
     */
    void EndBattle() {
        Debug.Log("--- 戦闘終了！勝利！ ---");

        // 戦闘画面を閉じる
        if (battleCanvas != null) {
            battleCanvas.SetActive(false);
        }

        // プレイヤーを再表示
        if (playerMapObj != null) {
            playerMapObj.SetActive(true);

            playerMapObj
                .GetComponent<PlayerMapMovement>()
                .enabled = true;
        }

        // マップUIを再表示
        if (MapManager.Instance != null) {
            MapManager.Instance.SetMapUIActive(true);
        }

        // エリア管理へ勝利通知
        MapManager.Instance.OnBattleWin();
    }
}