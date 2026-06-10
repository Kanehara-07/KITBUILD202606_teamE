using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BattleCharacter {
    public string name;
    public int currentHP;
    public int speed;
    public float actionValue;
    public bool isEnemy;

    public void ResetActionValue() {
        actionValue = 10000f / speed;
    }
}

public class BattleManager : MonoBehaviour {
    public List<BattleCharacter> participants = new List<BattleCharacter>();
    
    [Header("UI設定")]
    public GameObject actionPanel; // 先ほど作ったActionPanelをインスペクターで入れる

    private BattleCharacter currentTurnCharacter; // 現在ターンが回ってきたキャラを記憶する

    void Start() {
        // パネルは最初隠す
        actionPanel.SetActive(false);

        SetupTestBattle();
        ProgressTurn();
    }

    void SetupTestBattle() {
        BattleCharacter player = new BattleCharacter { name = "ENTP(味方)", currentHP = 100, speed = 120, isEnemy = false };
        player.ResetActionValue();
        participants.Add(player);

        BattleCharacter enemy = new BattleCharacter { name = "森の動く切り株(敵)", currentHP = 150, speed = 80, isEnemy = true };
        enemy.ResetActionValue();
        participants.Add(enemy);
        
        Debug.Log("--- 戦闘開始 ---");
    }

    void ProgressTurn() {
        if (participants.Count == 0) return;

        // 1. 一番AVが小さいキャラを探す
        BattleCharacter nextChar = participants[0];
        foreach (var c in participants) {
            if (c.actionValue < nextChar.actionValue) nextChar = c;
        }

        // 2. 時計を進める
        float reduction = nextChar.actionValue;
        foreach (var c in participants) {
            c.actionValue -= reduction;
        }

        // 現在のキャラを保持して行動開始
        currentTurnCharacter = nextChar;
        ExecuteCharacterTurn(currentTurnCharacter);
    }

    void ExecuteCharacterTurn(BattleCharacter character) {
        Debug.Log($"【{character.name}】のターン！ (残りHP: {character.currentHP})");

        if (!character.isEnemy) {
            // ★プレイヤーのターンの場合：ボタン（パネル）を表示して、プログラムをここでストップする
            Debug.Log("コマンド入力を待っています...");
            actionPanel.SetActive(true); 
        }else{
            // 敵のターンの場合：パネルを隠して、自動で攻撃して次のターンへ
            actionPanel.SetActive(false);
            Debug.Log("敵があなたを攻撃した！");
            
            BattleCharacter target = participants.Find(c => !c.isEnemy);
            target.currentHP -= 15;
            
            // 行動終了処理
            character.ResetActionValue();
            Invoke("ProgressTurn", 1.5f); // 1.5秒後に次のターンへ
        }
    }

    // ★通常攻撃ボタンが押された時に呼び出す関数
    public void OnAttackButtonPressed() {
        // プレイヤーのターン以外で誤作動しないようにガード
        if (currentTurnCharacter == null || currentTurnCharacter.isEnemy) return;

        Debug.Log("通常攻撃を実行しました！");
        
        // 敵にダメージを与える
        BattleCharacter enemyTarget = participants.Find(c => c.isEnemy);
        enemyTarget.currentHP -= 20;

        // 行動が終わったのでパネルを隠す
        actionPanel.SetActive(false);

        // 待ち時間をリセットして、次のターンへ進む
        currentTurnCharacter.ResetActionValue();
        ProgressTurn(); 
    }
}
