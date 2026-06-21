using UnityEngine;
using UnityEngine.UI;
using TMPro;

/**
 * キャラクタースロット1マス分のUI部品（認知機能表示対応版）
 *
 * ── Prefab 構成 ────────────────────────────────────────────
 * Purple_Slot（ルート: Image + Button + このスクリプト）
 *   ├─ CharacterIcon      Image  キャラグラフィック
 *   ├─ IdText             TMP    "INTJ"
 *   ├─ NameText           TMP    "建築家"
 *   ├─ StarText           TMP    "★★"
 *   ├─ InPartyBadge       TMP    "編成中"
 *   ├─ PrimaryFuncBadge   Image  ◎認知機能の背景
 *   │   └─ PrimaryFuncText TMP   "Ni"
 *   └─ SecondaryFuncBadge Image  ○認知機能の背景
 *       └─ SecondaryFuncText TMP "Te"
 * ────────────────────────────────────────────────────────────
 */
public class CharacterSlotUI : MonoBehaviour
{
    [Header("背景（ルートのImageをセット）")]
    [SerializeField] private Image backgroundImage;

    [Header("キャラアイコン")]
    [SerializeField] private Image characterIcon;

    [Header("テキスト")]
    [SerializeField] private TextMeshProUGUI idText;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI starText;
    [SerializeField] private TextMeshProUGUI inPartyBadge;

    [Header("認知機能バッジ（◎主専門）")]
    [SerializeField] private Image           primaryFuncBadge;   // 背景Image
    [SerializeField] private TextMeshProUGUI primaryFuncText;    // "Ni" などのテキスト

    [Header("認知機能バッジ（○副専門）")]
    [SerializeField] private Image           secondaryFuncBadge;
    [SerializeField] private TextMeshProUGUI secondaryFuncText;

    [Header("ボタン（ルートのButtonをセット）")]
    [SerializeField] private Button slotButton;

    public OwnedCharacter OwnedChar { get; private set; }
    public int SlotIndex { get; private set; } = -1;
    private System.Action<CharacterSlotUI> onClick;

    // ─── セットアップ ──────────────────────────────────────────

    public void Setup(OwnedCharacter character, System.Action<CharacterSlotUI> clickCallback)
    {
        OwnedChar = character;
        SlotIndex = -1;
        onClick   = clickCallback;
        Refresh();
    }

    public void SetupAsPartySlot(int index, OwnedCharacter character, System.Action<CharacterSlotUI> clickCallback)
    {
        OwnedChar = character;
        SlotIndex = index;
        onClick   = clickCallback;
        Refresh();
    }

    public void UpdateCharacter(OwnedCharacter character)
    {
        OwnedChar = character;
        Refresh();
    }

    public void RefreshInPartyBadge()
    {
        if (inPartyBadge == null) return;
        bool inParty = OwnedChar != null
                       && PartyManager.Instance != null
                       && PartyManager.Instance.IsInParty(OwnedChar);
        inPartyBadge.gameObject.SetActive(inParty);
    }

    // ─── 表示更新 ─────────────────────────────────────────────

    public void Refresh()
    {
        bool hasChar = OwnedChar != null;

        // 背景色（MBTIカラー）
        if (backgroundImage != null)
            backgroundImage.color = hasChar
                ? GetMBTIColor(OwnedChar.id)
                : new Color(0.25f, 0.30f, 0.37f);

        // キャラアイコン：Spriteが設定済みの時だけON
        if (characterIcon != null)
            characterIcon.gameObject.SetActive(hasChar && characterIcon.sprite != null);

        // MBTIコード
        if (idText != null)
            idText.text = hasChar ? OwnedChar.id
                        : SlotIndex >= 0 ? $"スロット{SlotIndex + 1}" : "";

        // 日本語名
        if (nameText != null)
        {
            string jp = "";
            if (hasChar && PartyManager.Instance != null)
            {
                var d = PartyManager.Instance.characterDatabase.Find(c => c.id == OwnedChar.id);
                jp = d != null ? d.jpName : "";
            }
            nameText.text = hasChar ? jp : SlotIndex >= 0 ? "空き" : "";
        }

        // ランク★
        if (starText != null)
            starText.text = hasChar ? new string('★', OwnedChar.rank) : "";

        // 編成中バッジ
        RefreshInPartyBadge();

        // 認知機能バッジ
        RefreshCognitiveFunctionBadges(hasChar ? OwnedChar.id : null);

        // ボタン登録
        Button b = slotButton != null ? slotButton : GetComponent<Button>();
        if (b != null)
        {
            if (slotButton == null) slotButton = b;
            slotButton.onClick.RemoveAllListeners();
            slotButton.onClick.AddListener(() => onClick?.Invoke(this));
        }
    }

    // ─── 認知機能バッジ更新 ───────────────────────────────────

    private void RefreshCognitiveFunctionBadges(string mbti)
    {
        var funcs = string.IsNullOrEmpty(mbti) ? null : MBTICognitiveFunctions.Get(mbti);
        bool show = funcs != null;

        // ◎ 主専門認知機能
        if (primaryFuncBadge != null)
        {
            primaryFuncBadge.gameObject.SetActive(show);
            if (show)
                primaryFuncBadge.color = MBTICognitiveFunctions.GetColor(funcs.primary);
        }
        if (primaryFuncText != null && show)
            primaryFuncText.text = MBTICognitiveFunctions.ToString(funcs.primary);

        // ○ 副専門認知機能
        if (secondaryFuncBadge != null)
        {
            secondaryFuncBadge.gameObject.SetActive(show);
            if (show)
                secondaryFuncBadge.color = MBTICognitiveFunctions.GetColor(funcs.secondary);
        }
        if (secondaryFuncText != null && show)
            secondaryFuncText.text = MBTICognitiveFunctions.ToString(funcs.secondary);
    }

    // ─── 選択ハイライト ───────────────────────────────────────

    public void SetSelected(bool selected)
    {
        if (backgroundImage == null) return;
        backgroundImage.color = selected
            ? new Color(1f, 0.85f, 0.2f)
            : (OwnedChar != null ? GetMBTIColor(OwnedChar.id) : new Color(0.25f, 0.30f, 0.37f));
    }

    // ─── MBTIカラー ───────────────────────────────────────────

    public static Color GetMBTIColor(string mbti)
    {
        if (string.IsNullOrEmpty(mbti)) return Color.gray;
        switch (mbti)
        {
            case "INTJ": case "INTP": case "ENTJ": case "ENTP":
                return new Color(0.486f, 0.361f, 0.749f);
            case "INFJ": case "INFP": case "ENFJ": case "ENFP":
                return new Color(0.353f, 0.620f, 0.290f);
            case "ISTJ": case "ISFJ": case "ESTJ": case "ESFJ":
                return new Color(0.290f, 0.557f, 0.667f);
            case "ISTP": case "ISFP": case "ESTP": case "ESFP":
                return new Color(0.722f, 0.659f, 0.125f);
            default:
                return new Color(0.4f, 0.4f, 0.4f);
        }
    }
}
