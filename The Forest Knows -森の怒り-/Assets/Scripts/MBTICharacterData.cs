using UnityEngine;

// enum文字列格納でMBTIの種類を定義
public enum MBTIType { INTJ, INTP, ENTJ, ENTP, INFJ, ENFJ, INFP, ENFP, ISTP, ESTP, ISFP, ESFP, ISTJ, ISFJ, ESTJ, ESFJ }

[CreateAssetMenu(fileName = "NewMBTICharacter", menuName = "GameData/MBTI Character")]
public class MBTICharacterData : ScriptableObject {
    public string characterName; // キャラの名前
    public MBTIType mbtiType;    // MBTIタイプ
    public int maxHP;            // 最大HP
    public int attack;           // 攻撃力
    public int baseSpeed;        // 基礎速度（行動順に影響）
    public int starRank = 1;     // 星ランク（初期値1）
    
    // スキル等のデータも後々追加
}