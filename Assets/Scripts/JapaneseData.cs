using System.Collections.Generic;

public static class JapaneseData
{
    // 1. Kho dữ liệu gốc (Private - để nội bộ class quản lý)
    // Sử dụng HashSet để đảm bảo không trùng lặp và tra cứu siêu tốc
    private static readonly HashSet<string> _basicHiragana = new HashSet<string>
    {
        // --- 5 Nguyên âm (A I U E O) ---
        "あ", "い", "う", "え", "お",

        // --- Hàng K (Ka Ki Ku Ke Ko) ---
        "か", "き", "く", "け", "こ",

        // --- Hàng S (Sa Shi Su Se So) ---
        "さ", "し", "す", "せ", "そ",

        // --- Hàng T (Ta Chi Tsu Te To) ---
        "た", "ち", "つ", "て", "と",

        // --- Hàng N (Na Ni Nu Ne No) ---
        "な", "に", "ぬ", "ね", "の",

        // --- Hàng H (Ha Hi Fu He Ho) ---
        "は", "ひ", "ふ", "へ", "ほ",

        // --- Hàng M (Ma Mi Mu Me Mo) ---
        "ま", "み", "む", "め", "も",

        // --- Hàng Y (Ya Yu Yo) ---
        "や", "ゆ", "よ",

        // --- Hàng R (Ra Ri Ru Re Ro) ---
        "ら", "り", "る", "れ", "ろ",

        // --- Hàng W (Wa Wo) ---
        "わ", "を",

        // --- Ký tự mũi (N) ---
        "ん"
    };
    public static IReadOnlyCollection<string> BasicHiragana => _basicHiragana;
  
}