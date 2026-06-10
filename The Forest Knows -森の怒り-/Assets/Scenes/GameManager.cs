using UnityEngine;
using TMPro; // TextMeshProを使うため
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {
    public static GameManager Instance;

    public List<MBTICharacterData> currentParty = new List<MBTICharacterData>();
    public List<MBTICharacterData> allCharacterDatabase;

    // ★ InputField から Dropdown に変更します
    public TMP_Dropdown mbtiDropdown; 

    void Awake() {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }else{
            Destroy(gameObject);
        }
    }

    // 決定ボタンを押した時に呼ばれる関数
    public void OnSubmitMBTI() {
        // ★ ドロップダウンで今選ばれている項目の「文字」を取得する
        string selectedMBTI = mbtiDropdown.options[mbtiDropdown.value].text;

        // データベースから一致するキャラを探す
        MBTICharacterData acquiredChar = allCharacterDatabase.Find(c => c.mbtiType.ToString() == selectedMBTI);

        if (acquiredChar != null) {
            Debug.Log($"{acquiredChar.mbtiType}のキャラを入手しました！");
            currentParty.Add(acquiredChar); // パーティに追加
            
            // マップ画面へ移動
            SceneManager.LoadScene("MapScene"); 
        }else{
            Debug.Log("キャラクターデータが見つかりません。Databaseに登録されているか確認してください。");
        }
    }
}