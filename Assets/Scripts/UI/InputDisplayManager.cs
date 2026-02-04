using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

/// <summary>
/// Quản lý giao diện nhập liệu (Input), bàn phím tổ ong/lục giác.
/// Xử lý logic trộn chữ và kiểm tra đáp án.
/// </summary>
public class InputDisplayManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private List<Button> buttonAnswers;
    [SerializeField] private TextMeshProUGUI inputFeedbackText; // Text hiển thị những gì user đã bấm
    [SerializeField] private TextMeshProUGUI vocabQuestionText; // Text câu hỏi/nghĩa
    [SerializeField] private List<GameObject> panels;

    [Header("Runtime State")]
    public bool IsReadyNextVocab { get; private set; }
    public bool IsLockedButton { get; private set; }
   

    // Data
    private VocabData _currentVocabData;
    private string _answerHira;
    private List<string> _targetCharList; // Danh sách ký tự đáp án chuẩn
    private int _currentIndexAnswer;
    
    // Cache
    private List<TextMeshProUGUI> _buttonTextComponents ;

    private float _lastInputTime;        // Anti-spam
    private void Awake()
    {
        CacheButtonComponents();
    }

    private void Start()
    {
        ResetFeedbackText();
    }

    private void Update()
    {
        if (GameManager.Instance.gameState == GameState.CompletedLevel) IsLockedButton = true;
    }

    private void CacheButtonComponents()
    {
        _buttonTextComponents = new List<TextMeshProUGUI>();
        foreach (var btn in buttonAnswers)
        {
            var tmp = btn.GetComponentInChildren<TextMeshProUGUI>();
            if (tmp != null)
            {
                _buttonTextComponents.Add(tmp);
            }
            else
            {
                // Debug.LogError($"[InputDisplayManager] Button {btn.name} thiếu TextMeshProUGUI!");
            }
        }
    }

    /// <summary>
    /// Nhận dữ liệu từ vựng mới và setup bàn phím.
    /// </summary>
    public void GetVocabData(VocabData vocabData)
    {
        IsReadyNextVocab = false;
        
        _currentVocabData = vocabData;
        _answerHira = _currentVocabData.Hiragana;
        
        ParseTargetAnswer(_answerHira);
        SetVocabQuestionText(_currentVocabData.Meaning);
        
        SetupButtons();

        IsReadyNextVocab = true;
    }

    public void NextVocab()
    {
        ResetFeedbackText();
        ActivePanel();
    }

    #region Text Handling

    private void UpdateFeedbackText(string text)
    {
        if (inputFeedbackText) 
            inputFeedbackText.text += text; 
    }

    private void ResetFeedbackText()
    {
        if (inputFeedbackText) 
            inputFeedbackText.text = ""; 
            
        _currentIndexAnswer = 0;
    }

    private void SetVocabQuestionText(string value)
    {
        if (vocabQuestionText)
            vocabQuestionText.text = value;
    }

    #endregion

    #region Input Logic

    private void CheckAnswer(string inputVal)
    {
        if(IsLockedButton) return;
        if (Time.time - _lastInputTime < 0.2f) return;
        _lastInputTime = Time.time;
        // Điều kiện thoát sớm
        if (_targetCharList == null || _currentIndexAnswer >= _targetCharList.Count) return;

        string targetChar = _targetCharList[_currentIndexAnswer];
        
        // Chuẩn hóa chuỗi để so sánh chính xác (tránh lỗi Unicode)
        bool isMatch = string.Equals(
            inputVal.Normalize(NormalizationForm.FormC),
            targetChar.Normalize(NormalizationForm.FormC)
        );

        if (isMatch)
        {
            HandleCorrectInput(inputVal);
        }
        else
        {
            HandleWrongInput();
        }
    }

    // ReSharper disable Unity.PerformanceAnalysis
    private void HandleCorrectInput(string inputVal)
    {
        _currentIndexAnswer++;
        UpdateFeedbackText(inputVal);

        bool isLastChar = _currentIndexAnswer >= _targetCharList.Count;
        GameEvents.OnCharCorrect?.Invoke();
        

        // Kiểm tra hoàn thành từ
        if (isLastChar)
        {
            _currentVocabData.isCorrect = true; // Gán đúng trước khi bắn sự kiện
            GameEvents.OnSubmitAnswer?.Invoke();
        }
    }

    // ReSharper disable Unity.PerformanceAnalysis
    private void HandleWrongInput()
    {
       
        bool isLastChar = _currentIndexAnswer >= _targetCharList.Count;
         GameEvents.OnCharWrong?.Invoke();
        

        // Kiểm tra hoàn thành từ
        if (isLastChar)
        {
            _currentVocabData.isCorrect = false;
            GameEvents.OnSubmitAnswer?.Invoke();
        }
    }

    #endregion

    #region Button Setup


    public void LockButton(bool isLocked)
    {
        SetPanelActive(!isLocked);
        IsLockedButton = isLocked;
    }
    private void SetupButtons()
    {
        // 1. Chuẩn bị danh sách ký tự (Đáp án + Nhiễu)
        List<string> displayChars = GenerateCharPool();

        // 2. Gán vào nút và đăng ký sự kiện
        for (int i = 0; i < buttonAnswers.Count; i++)
        {
            buttonAnswers[i].onClick.RemoveAllListeners();
            
            if (i >= _buttonTextComponents.Count) continue;

            // Nếu còn ký tự để hiển thị
            if (i < displayChars.Count)
            {
                string charToDisplay = displayChars[i];
                _buttonTextComponents[i].text = charToDisplay;
                
                // Reset trạng thái tương tác
                buttonAnswers[i].interactable = true;
                
                // Capture biến cho Lambda
                buttonAnswers[i].onClick.AddListener(() => CheckAnswer(charToDisplay));
                buttonAnswers[i].gameObject.SetActive(true);
            }
            else
            {
                // Ẩn các nút thừa
                buttonAnswers[i].gameObject.SetActive(false);
            }
        }
    }

    private List<string> GenerateCharPool()
    {
        List<string> pool = new List<string>(_targetCharList);
        int slotsRemaining = buttonAnswers.Count - pool.Count;

        // Lấy danh sách chữ cái nguồn để random
        var sourceData = JapaneseData.BasicHiragana.ToList(); 

        for (int i = 0; i < slotsRemaining; i++)
        {
            if (sourceData.Count > 0)
            {
                string randomChar = sourceData[Random.Range(0, sourceData.Count)];
                pool.Add(randomChar);
            }
        }

        pool.Shuffle();
        return pool;
    }

    private void ParseTargetAnswer(string rawValue)
    {
        if (string.IsNullOrEmpty(rawValue))
        {
            _targetCharList = new List<string>();
            return;
        }

        _targetCharList = rawValue.Trim()
            .Normalize(NormalizationForm.FormC)
            .Select(c => c.ToString())
            .ToList();
    }

    #endregion

    #region Panel Control

    private void SetPanelActive(bool isActive)
    {
        if (isActive && !IsReadyNextVocab) return;

        foreach (GameObject p in panels)
        {
            if(p) p.SetActive(isActive);
        }
    }
    
    public void DeActivePanel() => SetPanelActive(false);
    public void ActivePanel() => SetPanelActive(true);

    public void SetInteractable(bool isInteractable)
    {
        foreach (var btn in buttonAnswers)
        {
            if (btn != null) btn.interactable = isInteractable;
        }
    }

    #endregion
}
