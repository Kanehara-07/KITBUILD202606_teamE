using UnityEngine;

/**
 * メニュー開閉管理クラス
 *
 * メニューボタンが押された時に
 * メニュー画面の表示・非表示を切り替える。
 */
public class MenuConponent : MonoBehaviour {
    [SerializeField]
    private GameObject menuPanel;

    /**
     * 初期状態ではメニューを閉じておく
     */
    private void Start() {
        if (menuPanel != null) {
            menuPanel.SetActive(false);
        }
    }

    /**
     * メニュー表示切り替え
     *
     * 開いている
     * → 閉じる
     *
     * 閉じている
     * → 開く
     */
    public void ToggleMenu() {
        if (menuPanel != null) {
            bool isActive = menuPanel.activeSelf;
            menuPanel.SetActive(!isActive);
        }
    }
}