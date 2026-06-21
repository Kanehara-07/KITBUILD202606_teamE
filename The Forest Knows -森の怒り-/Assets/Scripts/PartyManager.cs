using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;
using System;

/**
 * MBTIキャラクターの基本情報
 * 図鑑データとして利用する。
 */
[System.Serializable]
public class CharacterData
{
    public string id;     // MBTIコード（例：INTP）
    public string jpName; // 日本語名（例：論理学者）
    public string role;   // ロール（例：アタッカー）
}

/**
 * プレイヤーが所持しているキャラクター情報
 */
[System.Serializable]
public class OwnedCharacter
{
    public string id;   // MBTIコード
    public int rank;    // ★1〜★3
}

/**
 * パーティ管理クラス（拡張版）
 *
 * 既存機能（変更なし）
 * ・キャラクター図鑑管理
 * ・所持キャラクター管理
 * ・キャラクター獲得処理
 * ・ランクアップ処理
 * ・獲得演出表示
 * ・メニュー画面表示
 *
 * 追加機能
 * ・パーティスロット4枠の管理
 * ・スロットへのセット／解除
 * ・編成ボーナス計算
 */
public class PartyManager : MonoBehaviour
{
    public static PartyManager Instance;

    // ─── 既存フィールド（変更なし） ──────────────────────────

    [Header("全キャラクター図鑑")]
    public List<CharacterData> characterDatabase = new List<CharacterData>();

    [Header("所持キャラクターBOX")]
    public List<OwnedCharacter> ownedCharacters = new List<OwnedCharacter>();

    [Header("獲得・昇格UI設定")]
    public GameObject getCanvas;
    public GameObject popUpPanel;
    public TextMeshProUGUI getTitleText;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI roleText;
    public TextMeshProUGUI starText;
    public GameObject newBadge;

    [Header("メニューUI設定")]
    public GameObject menuCanvas;
    public Transform menuContent;
    public GameObject slotPrefab;

    private Action onSequenceComplete;

    // ─── 追加フィールド ───────────────────────────────────────

    /// <summary>
    /// パーティスロット 4枠。
    /// null = 空きスロット。
    /// OwnedCharacter の id で管理する。
    /// </summary>
    private OwnedCharacter[] partySlots = new OwnedCharacter[4];

    // ─── ライフサイクル（既存と同じ） ────────────────────────

    void Awake() { Instance = this; }

    void Start()
    {
        if (getCanvas != null) getCanvas.SetActive(false);
        if (menuCanvas != null) menuCanvas.SetActive(false);
    }

    // ─── 既存メソッド（変更なし） ─────────────────────────────

    public void AddCharacter(string mbtiId, Action onComplete)
    {
        onSequenceComplete = onComplete;

        CharacterData data = characterDatabase.Find(c => c.id == mbtiId);
        if (data == null)
        {
            Debug.LogError(mbtiId + " が図鑑に登録されていません！");
            onComplete?.Invoke();
            return;
        }

        bool isNew = !ownedCharacters.Any(c => c.id == mbtiId);
        ownedCharacters.Add(new OwnedCharacter { id = mbtiId, rank = 1 });

        ShowGetUI(data, 1, isNew, false);
    }

    void ShowGetUI(CharacterData data, int rank, bool isNew, bool isMerge)
    {
        getCanvas.SetActive(true);
        getTitleText.text = isMerge ? "ランク昇格！！" : "MBTIキャラ獲得！";
        nameText.text = $"{data.id} ({data.jpName})";
        roleText.text = data.role;
        starText.text = new string('★', rank);

        if (newBadge != null) newBadge.SetActive(isNew && !isMerge);
        if (popUpPanel != null) StartCoroutine(PopupAnimation(popUpPanel.transform));

        CancelInvoke("CloseGetUI");
        Invoke("CloseGetUI", 3.0f);
    }

    void CloseGetUI()
    {
        getCanvas.SetActive(false);
        CheckMerge();
    }

    void CheckMerge()
    {
        var groups = ownedCharacters.GroupBy(c => new { c.id, c.rank })
                                    .Where(g => g.Count() >= 3)
                                    .FirstOrDefault();

        if (groups != null)
        {
            string mergeId   = groups.Key.id;
            int    mergeRank = groups.Key.rank;

            int removed = 0;
            for (int i = ownedCharacters.Count - 1; i >= 0; i--)
            {
                if (ownedCharacters[i].id == mergeId && ownedCharacters[i].rank == mergeRank)
                {
                    ownedCharacters.RemoveAt(i);
                    removed++;
                    if (removed == 3) break;
                }
            }

            int newRank = mergeRank + 1;
            ownedCharacters.Add(new OwnedCharacter { id = mergeId, rank = newRank });

            CharacterData data = characterDatabase.Find(c => c.id == mergeId);
            ShowGetUI(data, newRank, false, true);
        }
        else
        {
            if (onSequenceComplete != null)
            {
                onSequenceComplete.Invoke();
                onSequenceComplete = null;
            }
        }
    }

    IEnumerator PopupAnimation(Transform target)
    {
        target.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        float time = 0;
        while (time < 0.3f)
        {
            time += Time.deltaTime;
            float scale = Mathf.Lerp(0.1f, 1.2f, time / 0.3f);
            target.localScale = new Vector3(scale, scale, scale);
            yield return null;
        }
        time = 0;
        while (time < 0.15f)
        {
            time += Time.deltaTime;
            float scale = Mathf.Lerp(1.2f, 1.0f, time / 0.15f);
            target.localScale = new Vector3(scale, scale, scale);
            yield return null;
        }
        target.localScale = Vector3.one;
    }

    public void OpenMenu()
    {
        menuCanvas.SetActive(true);

        foreach (Transform child in menuContent)
            Destroy(child.gameObject);

        foreach (var character in ownedCharacters)
        {
            GameObject slot = Instantiate(slotPrefab, menuContent);

            RectTransform slotRect = slot.GetComponent<RectTransform>();
            if (slotRect != null)
            {
                slotRect.anchoredPosition3D = new Vector3(
                    slotRect.anchoredPosition3D.x,
                    slotRect.anchoredPosition3D.y,
                    0);
                slotRect.localScale = Vector3.one;
            }

            CharacterData data = characterDatabase.Find(c => c.id == character.id);

            Transform nameObj = slot.transform.Find("MenuNameText");
            Transform starObj = slot.transform.Find("MenuStarText");

            if (nameObj != null)
                nameObj.GetComponent<TextMeshProUGUI>().text = $"{data.id} ({data.jpName})";
            else
                Debug.LogError("MenuSlotの中に 'MenuNameText' が見つかりません！");

            if (starObj != null)
                starObj.GetComponent<TextMeshProUGUI>().text = new string('★', character.rank);
        }
    }

    // ─── 追加メソッド：パーティスロット管理 ──────────────────

    /// <summary>パーティスロット4枠を返す（PartyEditUIから参照）</summary>
    public OwnedCharacter[] GetPartySlots() => partySlots;

    /// <summary>
    /// スロット index（0〜3）にキャラをセットする。
    /// 同じキャラが別スロットに居たら先に外す。
    /// </summary>
    public void SetPartySlot(int index, OwnedCharacter character)
    {
        if (index < 0 || index >= partySlots.Length) return;

        // 同キャラが他のスロットにいれば外す
        if (character != null)
        {
            for (int i = 0; i < partySlots.Length; i++)
                if (partySlots[i] == character)
                    partySlots[i] = null;
        }

        partySlots[index] = character;
    }

    /// <summary>スロットを空にする</summary>
    public void ClearPartySlot(int index)
    {
        if (index >= 0 && index < partySlots.Length)
            partySlots[index] = null;
    }

    /// <summary>そのキャラがパーティに編成済みか</summary>
    public bool IsInParty(OwnedCharacter character)
    {
        foreach (var s in partySlots)
            if (s == character) return true;
        return false;
    }

    /// <summary>
    /// MBTITypeデータを元に相性ボーナスを計算して返す。
    /// パーティ内の2キャラ間で相性が良ければ攻撃力+5%/組。
    /// ※ MBTICharacterData の goodCompatibility を参照する場合は
    ///   ScriptableObject を別途用意してください。
    ///   ここでは簡易版として同じMBTIグループ（E/I, N/S）で判定します。
    /// </summary>
    public float GetPartyCompatibilityBonus()
    {
        float bonus = 1.0f;
        for (int i = 0; i < partySlots.Length; i++)
        {
            if (partySlots[i] == null) continue;
            for (int j = i + 1; j < partySlots.Length; j++)
            {
                if (partySlots[j] == null) continue;
                if (AreCompatible(partySlots[i].id, partySlots[j].id))
                    bonus += 0.05f;
            }
        }
        return bonus;
    }

    /// <summary>
    /// 簡易相性判定。
    /// 同じ第1字（E/I）かつ同じ第3字（T/F）なら相性良とする暫定ルール。
    /// 本実装では MBTICharacterData.goodCompatibility[] を参照してください。
    /// </summary>
    private bool AreCompatible(string a, string b)
    {
        if (a.Length < 4 || b.Length < 4) return false;
        return a[0] == b[0] && a[2] == b[2];
    }
}
