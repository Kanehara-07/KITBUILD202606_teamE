using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;
using System;

/**
 * MBTIキャラクターの基本情報
 *
 * 図鑑データとして利用する。
 */
[System.Serializable]
public class CharacterData {
    // MBTIコード
    // 例：INTP
    public string id;

    // 日本語名
    // 例：論理学者
    public string jpName;

    // ロール
    // 例：アタッカー
    public string role;
}

/**
 * プレイヤーが所持しているキャラクター情報
 */
[System.Serializable]
public class OwnedCharacter {
    // MBTIコード
    public string id;

    /**
     * ランク
     *
     * ★ = 1
     * ★★ = 2
     * ★★★ = 3
     */
    public int rank;
}


/**
 * パーティ管理クラス
 *
 * 主な役割
 * ・キャラクター図鑑管理
 * ・所持キャラクター管理
 * ・キャラクター獲得処理
 * ・ランクアップ処理
 * ・獲得演出表示
 * ・メニュー画面表示
 *
 * このゲームにおける
 * 「MBTIキャラ管理の中心クラス」
 */
public class PartyManager : MonoBehaviour {
    public static PartyManager Instance;

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
    public GameObject menuCanvas;       // メニュー画面全体
    public Transform menuContent;       // リストを並べる親枠
    public GameObject slotPrefab;       // 1キャラ分のパネル（プレハブ）

    private Action onSequenceComplete; 

    void Awake() { Instance = this; }

    void Start() {
        if (getCanvas != null) getCanvas.SetActive(false);
        if (menuCanvas != null) menuCanvas.SetActive(false);
    }
    
    
    /**
    * キャラクター獲得処理
    *
    * 新規獲得時
    * ・BOXへ追加
    * ・獲得演出表示
    *
    * 重複獲得時
    * ・同一キャラとして保存
    * ・後でランクアップ判定
    */
    public void AddCharacter(string mbtiId, Action onComplete) {
        onSequenceComplete = onComplete; 

        CharacterData data = characterDatabase.Find(c => c.id == mbtiId);
        if (data == null) {
            Debug.LogError(mbtiId + " が図鑑に登録されていません！");
            onComplete?.Invoke();
            return;
        }

        bool isNew = !ownedCharacters.Any(c => c.id == mbtiId);
        ownedCharacters.Add(new OwnedCharacter { id = mbtiId, rank = 1 });

        ShowGetUI(data, 1, isNew, false);
    }

    /**
    * 獲得・昇格演出表示
    *
    * 新規獲得
    * → MBTIキャラ獲得！
    *
    * 合成成功
    * → ランク昇格！！
    */
    void ShowGetUI(CharacterData data, int rank, bool isNew, bool isMerge) {
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

    void CloseGetUI() {
        getCanvas.SetActive(false);
        CheckMerge(); 
    }
    
    /**
    * ランクアップ判定
    *
    * 同一キャラ・同一ランクが
    * 3体揃った場合
    *
    * ★ ×3
    * ↓
    * ★★ ×1
    *
    * に変換する。
    *
    * 再帰的に実行されるため
    *
    * ★★ ×3
    * ↓
    * ★★★ ×1
    *
    * も自動処理される。
    */
    void CheckMerge() {
        var groups = ownedCharacters.GroupBy(c => new { c.id, c.rank })
                                    .Where(g => g.Count() >= 3)
                                    .FirstOrDefault();

        if (groups != null) {
            string mergeId = groups.Key.id;
            int mergeRank = groups.Key.rank;

            int removed = 0;
            for (int i = ownedCharacters.Count - 1; i >= 0; i--) {
                if (ownedCharacters[i].id == mergeId && ownedCharacters[i].rank == mergeRank) {
                    ownedCharacters.RemoveAt(i);
                    removed++;
                    if (removed == 3) break;
                }
            }

            int newRank = mergeRank + 1; 
            ownedCharacters.Add(new OwnedCharacter { id = mergeId, rank = newRank });

            CharacterData data = characterDatabase.Find(c => c.id == mergeId);
            ShowGetUI(data, newRank, false, true); 
        } else {
            if (onSequenceComplete != null) {
                onSequenceComplete.Invoke();
                onSequenceComplete = null;
            }
        }
    }
    
    /**
    * 獲得演出アニメーション
    *
    * 拡大
    * ↓
    * 少し縮小
    * ↓
    * 等倍
    *
    * のポップアップ演出を行う。
    */
    IEnumerator PopupAnimation(Transform target) {
        target.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        float time = 0;
        while (time < 0.3f) {
            time += Time.deltaTime;
            float scale = Mathf.Lerp(0.1f, 1.2f, time / 0.3f);
            target.localScale = new Vector3(scale, scale, scale);
            yield return null; 
        }
        time = 0;
        while (time < 0.15f) {
            time += Time.deltaTime;
            float scale = Mathf.Lerp(1.2f, 1.0f, time / 0.15f);
            target.localScale = new Vector3(scale, scale, scale);
            yield return null;
        }
        target.localScale = Vector3.one; 
    }

    // ========== メニュー用の新機能（位置ズレ対策・完全版） ==========
    /**
    * キャラクターメニュー表示
    *
    * 所持キャラクター一覧を
    * 動的生成して表示する。
    *
    * 表示内容
    * ・MBTI
    * ・日本語名
    * ・ランク
    */
    public void OpenMenu() {
        menuCanvas.SetActive(true);
        
        foreach (Transform child in menuContent) {
            Destroy(child.gameObject);
        }

        foreach (var character in ownedCharacters) {
            GameObject slot = Instantiate(slotPrefab, menuContent);
            
            // ★【新機能】生成されたスロットの奥行き(Z軸)を0に、サイズを1倍に強制固定して最手前に引っ張り出す
            RectTransform slotRect = slot.GetComponent<RectTransform>();
            if (slotRect != null) {
                slotRect.anchoredPosition3D = new Vector3(slotRect.anchoredPosition3D.x, slotRect.anchoredPosition3D.y, 0);
                slotRect.localScale = Vector3.one;
            }

            CharacterData data = characterDatabase.Find(c => c.id == character.id);
            
            Transform nameObj = slot.transform.Find("MenuNameText");
            Transform starObj = slot.transform.Find("MenuStarText");

            if (nameObj != null) {
                nameObj.GetComponent<TextMeshProUGUI>().text = $"{data.id} ({data.jpName})";
            } else {
                Debug.LogError("MenuSlotの中に 'MenuNameText' が見つかりません！");
            }

            if (starObj != null) {
                starObj.GetComponent<TextMeshProUGUI>().text = new string('★', character.rank);
            }
        }
    }
    
    /**
    * メニューを閉じる
    */
    public void CloseMenu() {
        menuCanvas.SetActive(false);
    }
}