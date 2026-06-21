using UnityEngine;

public enum MBTIType { INTJ, INTP, ENTJ, ENTP, INFJ, ENFJ, INFP, ENFP, ISTP, ESTP, ISFP, ESFP, ISTJ, ISFJ, ESTJ, ESFJ }

[CreateAssetMenu(fileName = "NewMBTICharacter", menuName = "GameData/MBTI Character")]
public class MBTICharacterData : ScriptableObject
{
    public string characterName;
    public MBTIType mbtiType;
    public int maxHP;
    public int attack;
    public int baseSpeed;
    public int starRank = 1;

    [Header("グラフィック")]
    public Sprite characterSprite; // ★追加：キャラ画像をここにセット
}
