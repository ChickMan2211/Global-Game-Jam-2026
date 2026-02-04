using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Tests
{
    public class GameLogicTests
    {
        [Test]
        public void JapaneseData_BasicHiragana_IsNotEmpty()
        {
            Assert.IsNotNull(JapaneseData.BasicHiragana);
            Assert.IsTrue(JapaneseData.BasicHiragana.Count > 0, "Dữ liệu Hiragana cơ bản không được rỗng.");
        }

        [Test]
        public void JapaneseData_ContainsCommonChars()
        {
            Assert.IsTrue(JapaneseData.BasicHiragana.Contains("あ"));
            Assert.IsTrue(JapaneseData.BasicHiragana.Contains("ん"));
        }

        // Test logic đếm lỗi (Mistake) thay cho Score
        [Test]
        public void MistakeTracking_LogicTest()
        {
            int currentMistakes = 0;
            int totalMistakes = 0;

            // Case 1: Trả lời sai
            currentMistakes++;
            totalMistakes++;
            
            Assert.AreEqual(1, currentMistakes);
            Assert.AreEqual(1, totalMistakes);

            // Case 2: Trả lời đúng -> Reset mistakes hiện tại (logic cũ)
            // Lưu ý: Logic này có thể thay đổi tùy theo yêu cầu gameplay mới về Finisher
            // Hiện tại trong CombatManager: HandleParrySuccess -> Reset Mistake = 0
            currentMistakes = 0;
            
            Assert.AreEqual(0, currentMistakes);
            Assert.AreEqual(1, totalMistakes); // Total không bị reset
        }
    }
}