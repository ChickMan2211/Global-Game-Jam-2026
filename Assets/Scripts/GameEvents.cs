using System;
using UnityEngine;

// Đảm bảo bạn đã import namespace chứa Data và UI Prefab
// using Game.Data;
// using Game.UI; 

public static class GameEvents
{
    // ========================================================================
    // 1. GAME FLOW (Vòng đời game)
    // ========================================================================
    
    /// <summary>
    /// Bắn ra khi bắt đầu màn chơi.
    /// Payload: Dữ liệu của Level đó (để Spawner biết sinh quái gì, UI biết mode gì).
    /// </summary>
    public static Action OnLevelStart;

    /// <summary>
    /// Bắn ra khi kết thúc màn chơi.
    /// Payload (bool): True = Thắng, False = Thua.
    /// </summary>
    public static Action<bool> OnLevelComplete;

    /// <summary>
    /// Tạm dừng hoặc tiếp tục game.
    /// Payload (bool): True = Pause, False = Resume.
    /// </summary>
    public static Action<bool> OnGamePause;


    // ========================================================================
    // 2. COMBAT & DATA (Dữ liệu trận đấu)
    // ========================================================================

    /// <summary>
    /// Bắn ra khi xuất hiện kẻ địch mới hoặc câu hỏi mới.
    /// Payload: Từ vựng cần trả lời.
    /// </summary>
    public static Action OnNextVocab;

    /// <summary>
    /// Cập nhật số lần sai (viên ngọc/trái tim).
    /// Payload (int): Số lỗi hiện tại của câu hỏi này.
    /// </summary>
    public static Action<int> OnMistakeCountChanged;
   


    // ========================================================================
    // 3. INPUT SYSTEM (Giao tiếp giữa uGUI và Logic)
    // ========================================================================

    /// <summary>
    /// Khi người chơi nhấp (Click) vào một chữ cái (thay vì kéo thả).
    /// Payload: Script của chữ cái đó (để UI Manager biết chữ nào mà tự bay vào ô).
    /// </summary>
    //public static Action<DraggableLetter> OnLetterClicked;
    

    /// <summary>
    /// Khi một chữ cái được thả thành công vào ô đáp án.
    /// (Không cần payload, chỉ báo hiệu để Manager check xem đầy ô chưa).
    /// </summary>
    public static Action OnSlotFilled;
    
    
    /// <summary>
    /// chuẩn bị chiến đấu 
    /// </summary>
    public static Action OnReadyToFight;

    /// <summary>
    /// Khi kẻ địch bị đẩy lùi xa, Player cần chạy để đuổi theo (Kết thúc combat tạm thời)
    /// </summary>
    // public static Action OnChaseStart;

    /// <summary>
    /// Khi người chơi đã điền đủ các ô -> Gửi đáp án về CombatManager check.
    /// Payload (string): Chuỗi ký tự người chơi đã ghép (VD: "neko").
    /// </summary>
    public static Action OnSubmitAnswer;


    // ========================================================================
    // 4. FEEDBACK (Hiệu ứng Âm thanh/Hình ảnh)
    // ========================================================================

    /// <summary>
    /// Trả lời ĐÚNG (CombatManager báo về).
    /// Dùng để phát âm thanh 'Ding', hiện hiệu ứng xanh, chém quái.
    /// Payload (bool): True nếu là ký tự cuối cùng của từ (Finish Blow).
    /// </summary>
    public static Action OnCharCorrect;
    
  
    
    
    
    /// <summary>
    /// Trả lời SAI (CombatManager báo về).
    /// Dùng để rung màn hình, hiện màu đỏ, trừ máu.
    /// </summary>
    public static Action OnCharWrong;
    public static Action<bool> OnPlayerBroken;

    public static Action OnFinisherFail;

    public static Action OnEndGame;

}
