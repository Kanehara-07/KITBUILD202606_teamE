using UnityEngine;

/**
 * 戦闘に参加する1キャラクター分のランタイムデータ
 *
 * 仕様書に基づく新パラメータ：
 *   HP   : 身体的体力。0で戦闘不能。
 *   MP   : 精神的体力。0で人格崩壊。
 *   UltGauge : 必殺ゲージ（0〜1）
 *   Pressure : ストレス（プレッシャー）段階
 *   CogFuncStock[] : Ni〜Se各認知機能の蓄積度
 *   TraumaFunc : トラウマ認知機能（劣等機能）
 *   IsCollapsed : 人格崩壊中フラグ
 */
[System.Serializable]
public class BattleCharacterData
{
    [Header("基本情報")]
    public string mbtiType;      // "INTJ" など
    public string displayName;   // "建築家" など
    public bool   isEnemy;
    public Sprite sprite;

    [Header("HP / MP")]
    public int currentHP;
    public int maxHP;
    public int currentMP;
    public int maxMP;

    [Header("戦闘パラメータ")]
    public int   attack;
    public float speed;
    public float actionValue;    // スタレ風行動値（小さい順に行動）

    [Header("必殺ゲージ（0〜1）")]
    public float ultGauge = 0f;
    public float ultCost  = 1f;  // 必殺技に必要なゲージ量

    [Header("ストレス・圧力（0〜100）")]
    public float pressure = 0f;

    [Header("認知機能蓄積（Ni=0, Ne=1, Ti=2, Te=3, Fi=4, Fe=5, Si=6, Se=7）")]
    public float[] cogFuncStock = new float[8]; // インデックスはCogFunc enum順

    [Header("人格崩壊フラグ")]
    public bool isCollapsed = false;

    [Header("トラウマ")]
    public MBTICognitiveFunctions.CogFunc traumaFunc;
    public bool traumaRevealed  = false; // プレイヤーが見破ったか
    public bool traumaTriggered = false; // 発動済みか
    public float traumaGauge   = 0f;    // トラウマゲージ（0〜100）

    // ─── ユーティリティ ───────────────────────────────────────

    /// <summary>行動値をリセット（速度が高いほど次の行動が早い）</summary>
    public void ResetActionValue()
    {
        actionValue = 10000f / Mathf.Max(speed, 1f);
    }

    /// <summary>HPダメージ処理</summary>
    public void TakeHPDamage(int damage)
    {
        currentHP = Mathf.Max(0, currentHP - damage);
    }

    /// <summary>MPダメージ処理（人格崩壊チェック含む）</summary>
    public bool TakeMPDamage(int damage)
    {
        currentMP = Mathf.Max(0, currentMP - damage);
        if (currentMP <= 0 && !isCollapsed)
        {
            isCollapsed = true;
            return true; // 人格崩壊発生
        }
        return false;
    }

    /// <summary>認知機能ストックを加算</summary>
    public void AddCogFuncStock(MBTICognitiveFunctions.CogFunc func, float amount)
    {
        int idx = (int)func;
        if (idx >= 0 && idx < cogFuncStock.Length)
            cogFuncStock[idx] = Mathf.Min(100f, cogFuncStock[idx] + amount);
    }

    /// <summary>必殺ゲージを加算（0〜1）</summary>
    public void AddUltGauge(float amount)
    {
        ultGauge = Mathf.Min(ultCost, ultGauge + amount);
    }

    /// <summary>必殺技が使用可能か</summary>
    public bool IsUltReady() => ultGauge >= ultCost;

    /// <summary>圧力を加算</summary>
    public void AddPressure(float amount)
    {
        pressure = Mathf.Min(100f, pressure + amount);
    }

    /// <summary>戦闘不能か</summary>
    public bool IsDefeated() => currentHP <= 0;
}
