using UnityEngine;
using TMPro;

/**
 * イベント管理クラス（修正版）
 *
 * 修正点：
 * BeginEvent() で MapCanvas を非表示にする。
 * EndEvent()   で MapCanvas を再表示する。
 */
public class EventManager : MonoBehaviour
{
    public static EventManager Instance;

    [Header("UI設定")]
    public GameObject eventCanvas;
    public GameObject choicePanel;
    public GameObject resultPanel;
    public TextMeshProUGUI eventText;
    public TextMeshProUGUI resultText;

    // ★ 追加: MapCanvas への参照
    [Header("マップUI（MapCanvasをセット）")]
    public GameObject mapCanvas;

    private GameObject playerMapObj;

    void Awake() { Instance = this; }

    void Start()
    {
        if (eventCanvas != null) eventCanvas.SetActive(false);
    }

    // ─── イベント開始 ─────────────────────────────────────────

    public void BeginEvent()
    {
        // ★ MapCanvas（エリア表示・MENUボタン含む）を非表示
        if (mapCanvas != null)
            mapCanvas.SetActive(false);
        // MapManager経由でも念のため
        if (MapManager.Instance != null)
        {
            MapManager.Instance.SetMapUIActive(false);
            MapManager.Instance.SetGatesActive(false);
        }

        // プレイヤーを一時非表示
        PlayerMapMovement movement = FindObjectOfType<PlayerMapMovement>();
        if (movement != null)
        {
            playerMapObj = movement.gameObject;
            playerMapObj.SetActive(false);
        }

        // イベント画面表示
        if (eventCanvas != null) eventCanvas.SetActive(true);
        choicePanel.SetActive(true);
        resultPanel.SetActive(false);

        eventText.text =
            "【運命の選択：トロッコ問題】\n" +
            "暴走したトロッコが走っている。\n" +
            "このままでは5人の作業員が犠牲になる。\n" +
            "レバーを引けば子供1人の犠牲で済むが…どうする？";
    }

    // ─── 選択肢 ───────────────────────────────────────────────

    public void OnChoice1Selected()
    {
        choicePanel.SetActive(false);
        PartyManager.Instance.AddCharacter("INTP", EndEvent);
    }

    public void OnChoice2Selected()
    {
        choicePanel.SetActive(false);
        PartyManager.Instance.AddCharacter("INFP", EndEvent);
    }

    // ─── イベント終了 ─────────────────────────────────────────

    void EndEvent()
    {
        if (eventCanvas != null) eventCanvas.SetActive(false);

        // プレイヤー再表示
        if (playerMapObj != null)
        {
            playerMapObj.SetActive(true);
            playerMapObj.GetComponent<PlayerMapMovement>().enabled = true;
        }

        // ★ MapCanvas を再表示
        if (mapCanvas != null)
            mapCanvas.SetActive(true);
        if (MapManager.Instance != null)
            MapManager.Instance.SetMapUIActive(true);

        MapManager.Instance.OnEventComplete();
    }
}