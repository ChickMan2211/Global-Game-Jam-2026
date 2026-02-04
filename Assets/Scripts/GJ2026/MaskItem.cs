using UnityEngine;

public class MaskItem : MonoBehaviour
{
    public char TargetChar { get; private set; }
    [SerializeField] private UIFrameAnimationV2 animController; 

    public void Setup(char character)
    {
        TargetChar = char.ToLower(character);
        if (animController == null) animController = GetComponent<UIFrameAnimationV2>();
    }

    // Đổi tên hàm cho đúng ngữ nghĩa (Bỏ chữ Destroy đi)
    public void Break()
    {
        if (animController != null)
        {
            // Gọi Animation chạy
            animController.TriggerBreak(() => 
            {
                // TRƯỚC ĐÂY: Destroy(gameObject);
                // BÂY GIỜ: Không làm gì cả, hoặc làm các việc nhẹ nhàng hơn
                
                // Gợi ý: Nên tắt script này đi để nó không tốn hiệu năng
                this.enabled = false; 
            });
        }
    }
}