using UnityEngine;

/**
 * MBTI認知機能データ
 *
 * 全16タイプの専門認知機能（◎主、○副）と
 * トラウマ認知機能（劣等機能）を定義する。
 *
 * Excelの表に基づく。
 */
public static class MBTICognitiveFunctions {
    // 認知機能の種類
    public enum CogFunc { Ni, Ne, Ti, Te, Fi, Fe, Si, Se }

    [System.Serializable]
    public class FunctionSet
    {
        public CogFunc primary;   // ◎ 専門認知機能（主）
        public CogFunc secondary; // ○ 専門認知機能（副）
        public CogFunc trauma;    // × トラウマ認知機能（劣等機能）
    }

    // Excelの表に基づいた全16タイプのデータ
    private static readonly System.Collections.Generic.Dictionary<string, FunctionSet> data
        = new System.Collections.Generic.Dictionary<string, FunctionSet>
    {
        { "INTJ", new FunctionSet { primary = CogFunc.Ni, secondary = CogFunc.Te, trauma = CogFunc.Se } },
        { "INTP", new FunctionSet { primary = CogFunc.Ti, secondary = CogFunc.Ne, trauma = CogFunc.Fe } },
        { "ENTJ", new FunctionSet { primary = CogFunc.Te, secondary = CogFunc.Ni, trauma = CogFunc.Fi } },
        { "ENTP", new FunctionSet { primary = CogFunc.Ne, secondary = CogFunc.Ti, trauma = CogFunc.Si } },
        { "INFJ", new FunctionSet { primary = CogFunc.Ni, secondary = CogFunc.Fe, trauma = CogFunc.Se } },
        { "INFP", new FunctionSet { primary = CogFunc.Fi, secondary = CogFunc.Ne, trauma = CogFunc.Te } },
        { "ENFJ", new FunctionSet { primary = CogFunc.Fe, secondary = CogFunc.Ni, trauma = CogFunc.Ti } },
        { "ENFP", new FunctionSet { primary = CogFunc.Ne, secondary = CogFunc.Fi, trauma = CogFunc.Si } },
        { "ISTJ", new FunctionSet { primary = CogFunc.Si, secondary = CogFunc.Te, trauma = CogFunc.Ne } },
        { "ISFJ", new FunctionSet { primary = CogFunc.Si, secondary = CogFunc.Fe, trauma = CogFunc.Ne } },
        { "ESTJ", new FunctionSet { primary = CogFunc.Te, secondary = CogFunc.Si, trauma = CogFunc.Fi } },
        { "ESFJ", new FunctionSet { primary = CogFunc.Fe, secondary = CogFunc.Si, trauma = CogFunc.Ti } },
        { "ISTP", new FunctionSet { primary = CogFunc.Ti, secondary = CogFunc.Se, trauma = CogFunc.Fe } },
        { "ISFP", new FunctionSet { primary = CogFunc.Fi, secondary = CogFunc.Se, trauma = CogFunc.Te } },
        { "ESTP", new FunctionSet { primary = CogFunc.Se, secondary = CogFunc.Ti, trauma = CogFunc.Ni } },
        { "ESFP", new FunctionSet { primary = CogFunc.Se, secondary = CogFunc.Fi, trauma = CogFunc.Ni } },
    };

    /// <summary>MBTIタイプの認知機能セットを返す</summary>
    public static FunctionSet Get(string mbti)
    {
        if (data.TryGetValue(mbti, out var set)) return set;
        return null;
    }

    /// <summary>認知機能の表示色を返す</summary>
    public static Color GetColor(CogFunc func)
    {
        switch (func)
        {
            case CogFunc.Ni: return new Color(0.55f, 0.27f, 0.86f); // 濃紫
            case CogFunc.Ne: return new Color(0.78f, 0.47f, 0.95f); // 薄紫
            case CogFunc.Ti: return new Color(0.20f, 0.60f, 0.95f); // 青
            case CogFunc.Te: return new Color(0.10f, 0.78f, 0.90f); // シアン
            case CogFunc.Fi: return new Color(0.95f, 0.45f, 0.60f); // ピンク
            case CogFunc.Fe: return new Color(0.95f, 0.65f, 0.20f); // オレンジ
            case CogFunc.Si: return new Color(0.35f, 0.80f, 0.45f); // 緑
            case CogFunc.Se: return new Color(0.90f, 0.82f, 0.10f); // 黄
            default:         return Color.white;
        }
    }

    /// <summary>認知機能の文字列表現を返す（"Ni", "Fe" etc.）</summary>
    public static string ToString(CogFunc func) => func.ToString();
}
