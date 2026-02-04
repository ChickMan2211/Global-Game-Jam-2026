bạn là senior unity developer bây giờ bạn sẽ là người đồng hành giúp tôi hoàn thiện con game này. Trong quá trình làm bạn có thể hỏi tôi những câu hỏi cần thiết để hiểu mục đích của tôi hơn nếu chưa rõ và trước khi chỉnh sửa điều gì phải hỏi tôi để tôi xác nhận trước. Mọi chỉnh sửa trong quá trình làm nêú có cái nào thay đổi bạn có thể thêm hoặc chỉnh sửa gdd dưới đây cho phù hợp với những gì game vừa cập nhập.
Khi fix lỗi hãy ưu tiên tìm ra nguyên nhân trước khi đưa ra giải pháp. Ví dụ thay vì thêm code ngăn chặn lỗi thì hãy tìm ra vị trí gây ra lỗi để khắc phục

Game design document
Tên game: (Chưa đặt)
Thể loại: 2D / Rhythm Combat / Educational
Nền tảng: Android
Đối tượng: Người học tiếng Nhật, Hardcore Gamers
Art: 2D Animation

1. Overview
   Đây là một trò chơi trên điện thoại giúp người chơi học từ vựng tiếng Nhật thông qua việc nhập vai làm một kiếm sĩ theo từng màn. Thay vì bấm nút đấm đá thông thường, người chơi phải ghép đúng các chữ cái để nhân vật chiến đấu.
   Mục tiêu: Tiêu diệt kẻ địch bằng kiến thức từ vựng để qua màn.
   Điểm độc đáo: Ghép chữ đến đâu, nhân vật đánh đến đó (Parry/Attack).

2. Core Loop (Cập nhật cơ chế mới)
   Mỗi màn chơi sẽ có danh sách từ vựng. Trò chơi diễn ra theo trình tự lặp lại:

   Bước 1: Chạm trán & Truy đuổi
   - Nhân vật chạy (Running) để tìm địch. Nếu địch bị đẩy lùi quá xa, nhân vật sẽ tự động chạy đuổi theo (Chase).
   - Khi gặp địch (trong tầm Raycast), cả hai chuyển sang thế chiến đấu (FightStand).
   - Câu hỏi hiện ra (Hình ảnh/Nghĩa). Bên dưới là các ô chữ cái lục giác.

   Bước 2: Combat & Phản xạ (Cơ chế Thanh Nộ - Tension Gauge)
   - Kẻ địch có một Thanh Nộ (Attack Timer) nạp dần theo thời gian.
   - Nhiệm vụ: Người chơi phải chọn ĐÚNG chữ cái tiếp theo trong từ vựng (Ví dụ: ひと) trước khi thanh nộ đầy.
   
   * Tương tác tức thì:
     - Chọn ĐÚNG: 
       + Nhân vật Parry (đỡ đòn).
       + Kẻ địch bị đẩy lùi (Knockback) và Background rung lắc tạo lực.
       + Thanh nộ của địch Reset về 0.
     - Chọn SAI (Hoặc để Thanh Nộ đầy):
       + Nhân vật bị địch chém trúng -> Mất Máu (Health).
       + Enemy thực hiện hoạt ảnh tấn công ngay lập tức.
       + Thanh nộ Reset về 0 để chuẩn bị cú chém tiếp theo.
       + Có cơ chế chống spam click (Cooldown 0.2s) để tránh mất máu oan.

   Bước 3: Kết thúc đòn đánh & Trạng thái Focus (Finisher)
   Sau khi hoàn thành từ vựng (trả lời hết các chữ cái), sẽ xảy ra các trường hợp:
   
   - Trường hợp A: Parry Hoàn Hảo (Perfect)
     + Điều kiện: Không sai lần nào trong quá trình ghép từ.
     + Diễn biến: Kẻ địch bị vỡ thế (BrokenStand - Quỳ xuống).
     + Chuyển sang trạng thái **FOCUS** (Tiêu điểm):
       * Người chơi trả lời một câu hỏi "Chí mạng" (Finisher Vocab).
       * Trong trạng thái Focus: Bấm sai KHÔNG mất máu.
       * Nếu trả lời ĐÚNG hết: Tung đòn kết liễu -> Địch chết (Die).
       * Nếu trả lời SAI quá giới hạn (ví dụ > 2 lần): Tung đòn kết liễu NHƯNG địch đỡ được/tỉnh dậy -> Địch đứng dậy (FightStand) -> Tiếp tục chiến đấu.

   - Trường hợp B: Parry Không Hoàn Hảo
     + Điều kiện: Đã sai trong quá trình ghép từ.
     + Diễn biến: Địch không vỡ thế ngay, hoặc cần thêm câu hỏi để phá thế. (Hiện tại logic game đang xử lý: Hoàn thành từ -> Địch Broken -> Vào Focus như trường hợp A nhưng có thể ít phần thưởng hơn).

   Bước 4: Hồi phục & Sinh tồn
   - Hệ thống Máu (Health) và Mạng (Life):
     + Hết Máu (Health = 0) -> Mất 1 Mạng (Life) -> Nhân vật ngã quỵ (BrokenStand).
     + Trong lúc ngã: Phải trả lời đúng từ vựng tiếp theo để hồi phục (Reset Health, Đứng dậy).
     + Nếu sai hoặc hết giờ trong lúc ngã -> Mất tiếp Mạng -> Hết Mạng (Life = 0) -> Game Over.

3. UI/UX
   - Phần trên (Sân khấu): Chiến đấu, Thanh máu, Thanh nộ địch (dạng vòng tròn hoặc thanh), Số ngọc (lần được phép sai).
   - Phần dưới (Bàn phím): Các ô lục giác tổ ong.

4. Các loại kẻ địch
   - Lính thường: Tốc độ nạp thanh nộ chậm. Từ vựng ngắn.
   - Lính giỏi: Tốc độ nạp nhanh. Knockback kháng cao hơn.
   - Trùm (Boss): Máu trâu, cần nhiều lần Finisher mới chết.

5. Luật Thắng/Thua
   - Thắng: Diệt hết địch.
   - Thua: Hết Mạng (Life).

6. Technical Notes (Cập nhật)
   - Input: Không khóa nút (Interactable luôn true) để đảm bảo flow nhanh.
   - Damage: Trừ máu tức thì (Instant) kết hợp Animation để tăng độ phản hồi (Responsiveness).
   - Movement: Đồng bộ vật lý (Acceleration/Friction) giữa Enemy và Background để tạo hiệu ứng Parallax mượt mà.

COMBAT SYSTEM
Dưới đây là bản mô tả đã được hệ thống hóa, chia theo State Machine để dễ code và dễ hiểu.
I. Cơ chế cơ bản (Core Mechanics)
Góc nhìn & Di chuyển: Player đứng yên (Stationary). Cảm giác di chuyển được tạo ra bằng cách di chuyển Enemy và Background ngược chiều.
Health System:
Max Mistakes (HP): 3.
Sai lần 1, 2: Nhận sát thương/Hiệu ứng xấu.
Sai lần 3: Die (Game Over).
Enemy Progression:
Enemy có $N$ từ vựng (Words).
Người chơi cần hoàn thành $N-1$ từ để đưa Enemy vào trạng thái yếu (Vulnerable/Broken).
Từ cuối cùng (Word $N$) là từ kết liễu (Finisher).
II. Tương tác cơ bản (Basic Interaction)
Diễn ra ở các từ vựng thông thường (chưa phải từ cuối).
Input Đúng (Correct Letter):
Action: Player & Enemy vung kiếm (Parry).
Visual: Particle tia lửa (Sparks) + Hiệu ứng rung màn hình nhẹ.
Movement: Player đứng yên. Enemy bị Small Knockback (lùi nhẹ). Background lùi nhẹ đồng bộ với Enemy.
Sound: Tiếng kiếm va chạm (Cling!).
Input Sai (Wrong Letter):
Action: Player bị chém trúng.
Visual: Player chạy Anim Take Damage. Màn hình nháy đỏ.
Logic: CurrentMistakes tăng lên 1.
Movement: Không có Knockback (Player đứng yên chịu đòn).
Sound: Tiếng nhân vật la đau/máu me.

III. Logic Nhánh (Branching Scenarios)
Hệ thống sẽ kiểm tra số lỗi (CurrentMistakes) ngay trước khi bước vào từ vựng cuối cùng (Finisher Word).
A. TRƯỜNG HỢP "PHONG ĐỘ TỐT" (Healthy State)
Điều kiện: CurrentMistakes $\le$ 1 (Máu còn nhiều).
Trigger: Hoàn thành từ áp chót ($N-1$).
Hiệu ứng chuyển giao: Kích hoạt Big Knockback (Enemy văng xa gấp đôi bình thường) $\rightarrow$ Player vào trạng thái Focusing (Tích nộ).
Giai đoạn Focus (Từ cuối cùng):
Input Đúng: Player chạy Anim tích lực, SFX nạp năng lượng (Charging).
Input Sai: Không bị trừ máu (hoặc trừ tùy design), nhưng làm gián đoạn nhịp độ.
Kết quả A1: PERFECT FINISHER (Win)
Điều kiện: Hoàn thành từ cuối và tổng số lỗi vẫn $\le$ 2 (chưa chết).
Diễn biến:
Enemy bị BrokenStand.
Dash Phase: Background & Enemy trôi thật nhanh về phía Player (Tạo cảm giác Player lướt tới tốc độ cao).
Impact: Player chém đường kiếm kết liễu (Slash VFX).
Aftermath: Enemy chạy Anim Die. Player chạy Anim StopAttack $\rightarrow$ Run.
Loop: Background tiếp tục trôi, Enemy mới xuất hiện.
Kết quả A2: COUNTERED (Fail Finisher)
Điều kiện: Trong lúc Focus hoặc Dash, người chơi phạm sai lầm khiến tổng lỗi chạm mốc 3 (hoặc quy định riêng cho việc fail finisher).
Diễn biến:
Dash Phase: Giống A1 (Lướt tới).
Impact: Khi va chạm, Enemy TẮT BrokenStand $\rightarrow$ Chuyển sang Attack.
Counter: Player bị chém văng ngược lại (Enemy & BG lùi ra xa gấp đôi khoảng cách thông thường).
Penalty: Player rơi vào trạng thái choáng (BrokenStand). Sau khi tỉnh lại, phải chạy lại gần Enemy và Reset Fight (đánh lại Enemy này từ đầu).
B. TRƯỜNG HỢP "HẤP HỐI" (Critical State)
Điều kiện: CurrentMistakes == 2 (Chỉ còn 1 giọt máu).
Trạng thái: Player vào trạng thái BrokenStand (Dáng đứng mệt mỏi/bị thương).
Tương tác:
Input Đúng: Player cố gắng gượng dậy chém (Weak Attack/Parry).
Input Sai: Bị chém trúng $\rightarrow$ Vẫn giữ BrokenStand (hoặc chết nếu tính là lỗi 3).
Kết quả B1: RECOVERY (Sống sót qua ải)
Điều kiện: Hoàn thành hết các từ nhưng vẫn đang ở trạng thái Critical (không đủ nộ để kết liễu hoành tráng).
Diễn biến:
Đòn cuối parry thành công. Enemy bị đẩy lùi xa (Big Knockback).
Player TẮT BrokenStand (Hồi phục lại tư thế thường).
Loop Penalty: Player phải chạy lại tiếp cận Enemy (Enemy & BG di chuyển lại gần).
Reset Fight: Cuộc chiến bắt đầu lại từ đầu (Do không kết liễu được, Enemy hồi phục).
Kết quả B2: DEATH (Game Over)
Điều kiện: Phạm sai lầm thứ 3 (CurrentMistakes $\ge$ 3).
Diễn biến: Enemy chém kết liễu. Player Anim Die. End Game.
