using UnityEngine;
using TMPro; // TextMeshProを使う
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {
    public static GameManager Instance; // どこからでもアクセスできるようにする

    public List<MBTICharacterData> currentParty = new List<MBTICharacterData>(); // 現在のパーティ
    public List<MBTICharacterData> allCharacterDatabase; // 全キャラのデータベース（Inspectorでセット）

    public TMP_InputField mbtiInputField; // 入力欄

    void Awake() {
        // GameManagerをシーン移動で破壊されないようにする（シングルトン化）
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }else{
            Destroy(gameObject);
        }
    }

    // 決定ボタンを押した時に呼ばれる関数
    public void OnSubmitMBTI() {
        string input = mbtiInputField.text.ToUpper().Trim(); // 入力を大文字にして空白削除

        // データベースから入力されたMBTIと一致するキャラを探す
        MBTICharacterData acquiredChar = allCharacterDatabase.Find(c => c.mbtiType.ToString() == input);

        if (acquiredChar != null) {
            Debug.Log($"{acquiredChar.mbtiType}のキャラを入手しました！");
            currentParty.Add(acquiredChar); // パーティに追加
            
            SceneManager.LoadScene("MapScene");
        }else{
            Debug.Log("正しいMBTIを入力してください");
        }
    }
}