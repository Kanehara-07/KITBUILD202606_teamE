using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

/**
 * 戦闘画面のUI全体を管理する
 *
 * ── BattleCanvas 構成 ──────────────────────────────────────
 * BattleCanvas
 *   ├─ BattleBackground        背景画像（大樹など）
 *   ├─ EnemyArea
 *   │   ├─ EnemySprite         敵グラフィック
 *   │   └─ EnemyStatusPanel
 *   │       ├─ EnemyHPBar      (Slider)
 *   │       ├─ EnemyMPBar      (Slider) ★新規
 *   │       └─ EnemyNameText   (TMP)
 *   ├─ PlayerArea
 *   │   ├─ PlayerSprite        味方グラフィック（行動中キャラ）
 *   │   └─ PlayerStatusPanel
 *   │       ├─ PlayerHPBar     (Slider)
 *   │       ├─ PlayerMPBar     (Slider) ★新規
 *   │       └─ PlayerUltBar    (Slider) 必殺ゲージ
 *   ├─ TurnOrderPanel          次のターン表示（左上）
 *   │   ├─ NextLabel           "Next"
 *   │   ├─ NextCharIcon        (Image)
 *   │   └─ NextCharName        (TMP)  "→INTJ"
 *   ├─ ActionPanel             コマンド（右下）
 *   │   ├─ AttackButton        通常攻撃（青）
 *   │   ├─ SkillButton         スキル（黄）
 *   │   └─ UltButton           必殺技（赤）
 *   └─ BattleLogText           戦闘ログ（任意）
 * ────────────────────────────────────────────────────────────
 */
public class BattleUIManager : MonoBehaviour
{
    public static BattleUIManager Instance;

    [Header("敵ステータスUI")]
    public Slider enemyHPBar;
    public Slider enemyMPBar;
    public TextMeshProUGUI enemyNameText;

    [Header("味方ステータスUI")]
    public Slider playerHPBar;
    public Slider playerMPBar;
    public Slider playerUltBar;
    public Image  playerSprite;        // 行動中キャラのグラフィック

    [Header("次のターン表示（左上）")]
    public Image           nextCharIcon;
    public TextMeshProUGUI nextCharName;

    [Header("コマンドボタン")]
    public Button attackButton;
    public Button skillButton;
    public Button ultButton;
    public GameObject actionPanel;

    [Header("必殺技ボタン（使用不可時は暗くする）")]
    public Image ultButtonImage;
    public Color ultReadyColor   = new Color(0.95f, 0.35f, 0.40f); // 赤
    public Color ultNotReadyColor= new Color(0.45f, 0.25f, 0.28f); // 暗い赤

    [Header("戦闘ログ（任意）")]
    public TextMeshProUGUI battleLogText;

    void Awake() { Instance = this; }

    // ─── 敵UI更新 ─────────────────────────────────────────────

    public void UpdateEnemyStatus(string name, int hp, int maxHp, int mp, int maxMp)
    {
        if (enemyNameText != null) enemyNameText.text = name;
        SetSlider(enemyHPBar, hp, maxHp);
        SetSlider(enemyMPBar, mp, maxMp);
    }

    // ─── 味方UI更新 ───────────────────────────────────────────

    public void UpdatePlayerStatus(int hp, int maxHp, int mp, int maxMp, float ultGauge)
    {
        SetSlider(playerHPBar, hp, maxHp);
        SetSlider(playerMPBar, mp, maxMp);
        if (playerUltBar != null) playerUltBar.value = Mathf.Clamp01(ultGauge);
    }

    public void SetPlayerSprite(Sprite sprite)
    {
        if (playerSprite == null) return;
        playerSprite.sprite  = sprite;
        playerSprite.enabled = sprite != null;
    }

    // ─── 次のターン表示 ───────────────────────────────────────

    public void UpdateTurnOrder(string charName, Sprite icon = null)
    {
        if (nextCharName != null) nextCharName.text = $"→{charName}";
        if (nextCharIcon != null)
        {
            nextCharIcon.sprite  = icon;
            nextCharIcon.enabled = icon != null;
        }
    }

    // ─── コマンドパネル ───────────────────────────────────────

    public void ShowActionPanel(bool ultAvailable)
    {
        if (actionPanel != null) actionPanel.SetActive(true);

        // 必殺技ボタンの有効/無効
        if (ultButton != null) ultButton.interactable = ultAvailable;
        if (ultButtonImage != null)
            ultButtonImage.color = ultAvailable ? ultReadyColor : ultNotReadyColor;
    }

    public void HideActionPanel()
    {
        if (actionPanel != null) actionPanel.SetActive(false);
    }

    // ─── 戦闘ログ ─────────────────────────────────────────────

    public void Log(string message)
    {
        if (battleLogText == null) return;
        battleLogText.text = message;
        Debug.Log($"[Battle] {message}");
    }

    // ─── ユーティリティ ───────────────────────────────────────

    private void SetSlider(Slider slider, int current, int max)
    {
        if (slider == null || max <= 0) return;
        slider.maxValue = max;
        slider.value    = current;
    }
}
