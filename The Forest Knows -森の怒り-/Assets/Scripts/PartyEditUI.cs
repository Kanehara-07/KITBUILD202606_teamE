using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/**
 * チーム編成画面
 *
 * 重要：panelRoot には PartyEditPanel をセットすること。
 *       MenuCanvas をセットすると MenuCanvas ごとOFFになってしまう。
 */
public class PartyEditUI : MonoBehaviour
{
    [Header("このパネル自身（PartyEditPanel をセット）")]
    [SerializeField] private GameObject panelRoot;

    [Header("パーティスロット 4つ")]
    [SerializeField] private CharacterSlotUI[] partySlotUIs = new CharacterSlotUI[4];

    [Header("所持キャラ一覧")]
    [SerializeField] private Transform  ownedGrid;
    [SerializeField] private GameObject characterSlotPrefab;

    [Header("テキスト")]
    [SerializeField] private TextMeshProUGUI bonusText;
    [SerializeField] private TextMeshProUGUI infoText;

    private CharacterSlotUI selectedOwnedSlot;
    private List<CharacterSlotUI> ownedSlotList = new List<CharacterSlotUI>();

    // ─── ライフサイクル ───────────────────────────────────────

    private void Awake()
    {
        // Awake で閉じることで Start の実行順問題を回避
        // panelRoot が MenuCanvas になっていないか自動チェック
        if (panelRoot != null)
        {
            // Canvas コンポーネントを持つ = MenuCanvas 等のルートCanvas
            // それをOFFにしてしまう設定ミスを防ぐ
            Canvas c = panelRoot.GetComponent<Canvas>();
            if (c != null)
            {
                Debug.LogError(
                    "[PartyEditUI] panelRoot に Canvas ルートオブジェクトがセットされています！" +
                    "PartyEditPanel をセットし直してください。MenuCanvas 全体が消える原因です。"
                );
                panelRoot = null; // 誤設定を無効化して被害を防ぐ
                return;
            }
            panelRoot.SetActive(false);
        }
    }

    private void Start()
    {
        // パーティスロットの初期セットアップ
        for (int i = 0; i < partySlotUIs.Length; i++)
        {
            if (partySlotUIs[i] == null) continue;
            int idx = i;
            partySlotUIs[i].SetupAsPartySlot(idx, null, OnPartySlotClicked);
        }
    }

    // ─── 表示制御 ─────────────────────────────────────────────

    public void Show()
    {
        if (panelRoot == null)
        {
            Debug.LogError("[PartyEditUI] panelRoot が未設定です。PartyEditPanel をセットしてください。");
            return;
        }
        panelRoot.SetActive(true);
        RefreshAll();
        SetInfo("編成したいキャラを選んでください");
    }

    public void Hide()
    {
        if (panelRoot != null) panelRoot.SetActive(false);
        ClearSelection();
    }

    // ─── 更新 ─────────────────────────────────────────────────

    private void RefreshAll()
    {
        RefreshPartySlots();
        RefreshOwnedList();
        RefreshBonus();
    }

    private void RefreshPartySlots()
    {
        if (PartyManager.Instance == null) return;
        var slots = PartyManager.Instance.GetPartySlots();
        for (int i = 0; i < partySlotUIs.Length; i++)
        {
            if (partySlotUIs[i] == null) continue;
            partySlotUIs[i].UpdateCharacter(i < slots.Length ? slots[i] : null);
        }
    }

    private void RefreshOwnedList()
    {
        if (ownedGrid == null || characterSlotPrefab == null) return;
        foreach (var s in ownedSlotList)
            if (s != null) Destroy(s.gameObject);
        ownedSlotList.Clear();

        if (PartyManager.Instance == null) return;
        foreach (var oc in PartyManager.Instance.ownedCharacters)
        {
            var go   = Instantiate(characterSlotPrefab, ownedGrid);
            var slot = go.GetComponent<CharacterSlotUI>();
            if (slot == null) continue;
            slot.Setup(oc, OnOwnedSlotClicked);
            ownedSlotList.Add(slot);
        }
    }

    private void RefreshBonus()
    {
        if (bonusText == null || PartyManager.Instance == null) return;
        float b   = PartyManager.Instance.GetPartyCompatibilityBonus();
        int   pct = Mathf.RoundToInt((b - 1f) * 100f);
        bonusText.text = pct > 0 ? $"編成ボーナス: 攻撃力 +{pct}%" : "編成ボーナス: なし";
    }

    // ─── クリック処理 ─────────────────────────────────────────

    private void OnOwnedSlotClicked(CharacterSlotUI slot)
    {
        if (selectedOwnedSlot == slot) { ClearSelection(); SetInfo("編成したいキャラを選んでください"); return; }
        if (selectedOwnedSlot != null) selectedOwnedSlot.SetSelected(false);
        selectedOwnedSlot = slot;
        slot.SetSelected(true);
        ShowDetail(slot.OwnedChar);
        SetInfo("セットしたいスロットを選んでください");
    }

    private void OnPartySlotClicked(CharacterSlotUI slot)
    {
        if (selectedOwnedSlot != null)
        {
            PartyManager.Instance.SetPartySlot(slot.SlotIndex, selectedOwnedSlot.OwnedChar);
            ClearSelection();
            RefreshAll();
            SetInfo("編成しました！");
        }
        else if (slot.OwnedChar != null)
        {
            PartyManager.Instance.ClearPartySlot(slot.SlotIndex);
            RefreshAll();
            SetInfo("スロットを空にしました");
        }
        else { SetInfo("まずキャラを選んでください"); }
    }

    // ─── ユーティリティ ───────────────────────────────────────

    private void ClearSelection()
    {
        if (selectedOwnedSlot != null) selectedOwnedSlot.SetSelected(false);
        selectedOwnedSlot = null;
        foreach (var s in ownedSlotList) s.RefreshInPartyBadge();
    }

    private void ShowDetail(OwnedCharacter oc)
    {
        if (infoText == null || oc == null) return;
        var data = PartyManager.Instance?.characterDatabase.Find(c => c.id == oc.id);
        infoText.text = $"{oc.id}（{data?.jpName}）{new string('★', oc.rank)}\n役職: {data?.role}";
    }

    private void SetInfo(string msg) { if (infoText != null) infoText.text = msg; }
}