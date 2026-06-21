using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * 戦闘全体管理（新仕様版）
 *
 * 仕様：
 *   HP/MPの2ゲージ制
 *   認知機能ストック（Ni〜Se）
 *   トラウマシステム（劣等機能）
 *   圧力（プレッシャー）
 *   人格崩壊
 *   スタレ風行動順（ActionValue方式）
 */
public class BattleManager : MonoBehaviour
{
    [Header("UI設定")]
    public GameObject battleCanvas;
    public GameObject actionPanel;

    // 戦闘参加キャラ一覧
    private List<BattleCharacterData> participants = new List<BattleCharacterData>();
    private BattleCharacterData currentActor;   // 現在行動中のキャラ
    private GameObject playerMapObj;

    // ─── ライフサイクル ───────────────────────────────────────

    void Start()
    {
        if (battleCanvas != null) battleCanvas.SetActive(false);
    }

    // ─── 戦闘開始 ─────────────────────────────────────────────

    public void BeginBattle()
    {
        if (MapManager.Instance != null) MapManager.Instance.SetMapUIActive(false);

        PlayerMapMovement movement = FindObjectOfType<PlayerMapMovement>();
        if (movement != null) { playerMapObj = movement.gameObject; playerMapObj.SetActive(false); }

        if (battleCanvas != null) battleCanvas.SetActive(true);

        SetupBattle();
        StartCoroutine(BattleLoop());
    }

    // ─── 戦闘データ構築 ───────────────────────────────────────

    void SetupBattle()
    {
        participants.Clear();

        // 味方：PartyManagerから編成済みキャラを取得
        if (PartyManager.Instance != null)
        {
            var slots = PartyManager.Instance.GetPartySlots();
            foreach (var oc in slots)
            {
                if (oc == null) continue;
                var cd = PartyManager.Instance.characterDatabase.Find(c => c.id == oc.id);

                var ally = new BattleCharacterData
                {
                    mbtiType    = oc.id,
                    displayName = cd != null ? cd.jpName : oc.id,
                    isEnemy     = false,
                    maxHP       = 100 + (oc.rank - 1) * 30,
                    maxMP       = 80,
                    attack      = 25 + (oc.rank - 1) * 8,
                    speed       = 100f,
                };
                ally.currentHP = ally.maxHP;
                ally.currentMP = ally.maxMP;

                var funcs = MBTICognitiveFunctions.Get(oc.id);
                if (funcs != null) ally.traumaFunc = funcs.trauma;

                ally.ResetActionValue();
                participants.Add(ally);
            }
        }

        // 敵：仮データ（後でEnemyDataSOから取得する）
        var enemy = new BattleCharacterData
        {
            mbtiType    = "ESTJ",
            displayName = "森の切り株",
            isEnemy     = true,
            maxHP       = 200,
            maxMP       = 100,
            attack      = 20,
            speed       = 80f,
        };
        enemy.currentHP = enemy.maxHP;
        enemy.currentMP = enemy.maxMP;
        var enemyFuncs  = MBTICognitiveFunctions.Get("ESTJ");
        if (enemyFuncs != null) enemy.traumaFunc = enemyFuncs.trauma;
        enemy.ResetActionValue();
        participants.Add(enemy);

        // UI初期化
        if (BattleUIManager.Instance != null)
        {
            BattleUIManager.Instance.UpdateEnemyStatus(
                enemy.displayName, enemy.currentHP, enemy.maxHP, enemy.currentMP, enemy.maxMP);
        }
    }

    // ─── 戦闘ループ ───────────────────────────────────────────

    IEnumerator BattleLoop()
    {
        while (true)
        {
            // 全員戦闘不能チェック
            bool alliesAlive  = participants.Exists(c => !c.isEnemy && !c.IsDefeated());
            bool enemiesAlive = participants.Exists(c =>  c.isEnemy && !c.IsDefeated());
            if (!alliesAlive || !enemiesAlive)
            {
                EndBattle(enemiesAlive == false);
                yield break;
            }

            // 次の行動者を決定
            currentActor = GetNextActor();
            AdvanceActionValues(currentActor.actionValue);
            currentActor.ResetActionValue();

            // UI更新（次の行動者）
            UpdateUI();

            if (!currentActor.isEnemy)
            {
                // 味方のターン → プレイヤー入力待ち
                bool ultReady = currentActor.IsUltReady();
                BattleUIManager.Instance?.ShowActionPanel(ultReady);
                yield return new WaitUntil(() => actionPanel == null || !actionPanel.activeSelf);
            }
            else
            {
                // 敵のターン → 自動行動
                BattleUIManager.Instance?.HideActionPanel();
                yield return new WaitForSeconds(1.0f);
                EnemyAction(currentActor);
            }

            yield return new WaitForSeconds(0.3f);
        }
    }

    // ─── 行動順計算（スタレ風ActionValue方式） ───────────────

    BattleCharacterData GetNextActor()
    {
        BattleCharacterData next = null;
        foreach (var c in participants)
        {
            if (c.IsDefeated()) continue;
            if (next == null || c.actionValue < next.actionValue) next = c;
        }
        return next;
    }

    void AdvanceActionValues(float reduction)
    {
        foreach (var c in participants)
            if (!c.IsDefeated()) c.actionValue -= reduction;
    }

    // ─── UI更新 ───────────────────────────────────────────────

    void UpdateUI()
    {
        if (BattleUIManager.Instance == null) return;

        // 行動中味方のステータス
        var ally = participants.Find(c => !c.isEnemy && !c.IsDefeated());
        if (ally != null)
        {
            BattleUIManager.Instance.UpdatePlayerStatus(
                ally.currentHP, ally.maxHP,
                ally.currentMP, ally.maxMP,
                ally.ultGauge / ally.ultCost);
            BattleUIManager.Instance.SetPlayerSprite(ally.sprite);
        }

        // 敵ステータス
        var enemy = participants.Find(c => c.isEnemy && !c.IsDefeated());
        if (enemy != null)
        {
            BattleUIManager.Instance.UpdateEnemyStatus(
                enemy.displayName, enemy.currentHP, enemy.maxHP, enemy.currentMP, enemy.maxMP);
        }

        // 次の行動者表示
        var nextActor = GetNextActor();
        if (nextActor != null && nextActor != currentActor)
            BattleUIManager.Instance.UpdateTurnOrder(nextActor.displayName, nextActor.sprite);
        else
            BattleUIManager.Instance.UpdateTurnOrder(currentActor?.displayName ?? "");
    }

    // ─── プレイヤーコマンド ───────────────────────────────────

    /// <summary>通常攻撃ボタンから呼ぶ</summary>
    public void OnAttackButtonPressed()
    {
        if (currentActor == null || currentActor.isEnemy) return;

        var target = participants.Find(c => c.isEnemy && !c.IsDefeated());
        if (target == null) return;

        // HP/MPダメージ
        int hpDmg = CalcDamage(currentActor.attack, pressureBonus: target.pressure);
        int mpDmg = Mathf.RoundToInt(hpDmg * 0.5f);
        target.TakeHPDamage(hpDmg);
        bool collapsed = target.TakeMPDamage(mpDmg);

        // 専門認知機能のストック蓄積
        var funcs = MBTICognitiveFunctions.Get(currentActor.mbtiType);
        if (funcs != null)
        {
            target.AddCogFuncStock(funcs.primary, 15f);
        }

        // 必殺ゲージ・圧力蓄積
        currentActor.AddUltGauge(0.15f);
        target.AddPressure(10f);

        BattleUIManager.Instance?.Log($"{currentActor.displayName} の通常攻撃！ HP-{hpDmg} MP-{mpDmg}");

        if (collapsed)
            BattleUIManager.Instance?.Log($"【人格崩壊】{target.displayName} が人格崩壊状態になった！");

        UpdateUI();
        BattleUIManager.Instance?.HideActionPanel();
    }

    /// <summary>スキルボタンから呼ぶ</summary>
    public void OnSkillButtonPressed()
    {
        if (currentActor == null || currentActor.isEnemy) return;

        var target = participants.Find(c => c.isEnemy && !c.IsDefeated());
        if (target == null) return;

        int hpDmg = CalcDamage(currentActor.attack * 2, pressureBonus: target.pressure);
        int mpDmg = Mathf.RoundToInt(hpDmg * 0.8f);
        target.TakeHPDamage(hpDmg);
        bool collapsed = target.TakeMPDamage(mpDmg);

        var funcs = MBTICognitiveFunctions.Get(currentActor.mbtiType);
        if (funcs != null)
        {
            target.AddCogFuncStock(funcs.primary, 30f);
            target.AddCogFuncStock(funcs.secondary, 15f);
        }

        currentActor.AddUltGauge(0.25f);
        target.AddPressure(20f);

        BattleUIManager.Instance?.Log($"{currentActor.displayName} のスキル発動！ HP-{hpDmg} MP-{mpDmg}");

        if (collapsed)
            BattleUIManager.Instance?.Log($"【人格崩壊】{target.displayName} が崩壊状態に！");

        UpdateUI();
        BattleUIManager.Instance?.HideActionPanel();
    }

    /// <summary>必殺技ボタンから呼ぶ</summary>
    public void OnUltButtonPressed()
    {
        if (currentActor == null || currentActor.isEnemy) return;
        if (!currentActor.IsUltReady()) return;

        var target = participants.Find(c => c.isEnemy && !c.IsDefeated());
        if (target == null) return;

        int hpDmg = CalcDamage(currentActor.attack * 3, pressureBonus: target.pressure);
        int mpDmg = Mathf.RoundToInt(hpDmg * 1.2f);
        target.TakeHPDamage(hpDmg);
        bool collapsed = target.TakeMPDamage(mpDmg);

        // トラウマチェック（必殺技の認知機能が敵のトラウマと一致するか）
        var funcs = MBTICognitiveFunctions.Get(currentActor.mbtiType);
        if (funcs != null && !target.traumaTriggered)
        {
            if (funcs.primary == target.traumaFunc || funcs.secondary == target.traumaFunc)
            {
                target.traumaRevealed  = true;
                target.traumaTriggered = true;
                target.AddPressure(40f);
                BattleUIManager.Instance?.Log($"【トラウマ発動】{target.displayName} のトラウマを刺激した！圧力+40！");
            }
        }

        currentActor.ultGauge = 0f; // ゲージリセット
        target.AddPressure(30f);

        BattleUIManager.Instance?.Log($"【必殺技】{currentActor.displayName}！ HP-{hpDmg} MP-{mpDmg}");

        if (collapsed)
            BattleUIManager.Instance?.Log($"【人格崩壊】{target.displayName} 崩壊！");

        UpdateUI();
        BattleUIManager.Instance?.HideActionPanel();
    }

    // ─── 敵の行動 ─────────────────────────────────────────────

    void EnemyAction(BattleCharacterData enemy)
    {
        var target = participants.Find(c => !c.isEnemy && !c.IsDefeated());
        if (target == null) return;

        int dmg = CalcDamage(enemy.attack, pressureBonus: 0);
        target.TakeHPDamage(dmg);
        target.TakeMPDamage(Mathf.RoundToInt(dmg * 0.3f));
        target.AddUltGauge(0.1f); // 被弾で必殺ゲージ増加

        BattleUIManager.Instance?.Log($"{enemy.displayName} の攻撃！ {target.displayName} に {dmg} ダメージ！");
        UpdateUI();
    }

    // ─── ダメージ計算 ─────────────────────────────────────────

    int CalcDamage(int baseAtk, float pressureBonus)
    {
        // 圧力が高いほどダメージUP（最大50%増）
        float pressureMult = 1f + (pressureBonus / 100f) * 0.5f;
        return Mathf.RoundToInt(baseAtk * pressureMult);
    }

    // ─── 戦闘終了 ─────────────────────────────────────────────

    void EndBattle(bool playerWon)
    {
        Debug.Log(playerWon ? "--- 勝利！ ---" : "--- 敗北... ---");

        if (battleCanvas != null) battleCanvas.SetActive(false);
        if (playerMapObj != null)
        {
            playerMapObj.SetActive(true);
            playerMapObj.GetComponent<PlayerMapMovement>().enabled = true;
        }
        if (MapManager.Instance != null) MapManager.Instance.SetMapUIActive(true);
        if (playerWon) MapManager.Instance?.OnBattleWin();
    }
}