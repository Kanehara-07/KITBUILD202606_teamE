using UnityEngine;
using UnityEngine.SceneManagement;

/**
 * メニュー開閉管理クラス
 *
 * アタッチ先: MenuCanvas > MenuConponent オブジェクト
 *
 * MenuCanvas 自体は常にアクティブにしておく。
 * Panel（中身）だけ Start() でOFFにする。
 */
public class MenuConponent : MonoBehaviour
{
    [Header("MenuCanvas > Panel をセット（MenuCanvas自体ではない）")]
    [SerializeField] private GameObject menuPanel;

    [Header("PartyEditPanel をセット")]
    [SerializeField] private PartyEditUI partyEditUI;

    [Header("タイトルシーン名")]
    [SerializeField] private string titleSceneName = "InitScene";

    private void Awake()
    {
        // Awake で閉じることで InitSceneChoice より先に確実に実行される
        if (menuPanel != null) menuPanel.SetActive(false);
    }

    private void Start()
    {
        // 設定ミスチェック
        if (menuPanel == null)
            Debug.LogError("[MenuConponent] menuPanel が未設定です。MenuCanvas > Panel をセットしてください。");
        if (partyEditUI == null)
            Debug.LogWarning("[MenuConponent] partyEditUI が未設定です。");
    }

    // ─── MENUボタン → ToggleMenu() ───────────────────────────

    public void ToggleMenu()
    {
        if (menuPanel == null) return;

        bool next = !menuPanel.activeSelf;
        menuPanel.SetActive(next);

        // チーム編成は閉じる時に一緒に閉じる
        if (!next && partyEditUI != null) partyEditUI.Hide();

        // プレイヤーの動きを止める / 再開
        PlayerMapMovement player = FindObjectOfType<PlayerMapMovement>();
        if (player != null) player.enabled = !next;
    }

    // ─── TeamButton → OpenPartyEdit() ────────────────────────

    public void OpenPartyEdit()
    {
        if (partyEditUI == null) return;
        partyEditUI.Show();
    }

    // ─── CloseButton → ClosePartyEdit() ──────────────────────

    public void ClosePartyEdit()
    {
        if (partyEditUI != null) partyEditUI.Hide();
    }

    // ─── TitleButton → GoToTitle()（後回しOK） ───────────────

    public void GoToTitle()
    {
        SceneManager.LoadScene(titleSceneName);
    }
}
