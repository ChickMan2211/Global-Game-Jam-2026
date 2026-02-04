using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "VocabData", menuName = "Scriptable Objects/VocabData")]
public class VocabData : ScriptableObject
{
    public String Meaning;
    public String Kanji;
    public String Hiragana;
    public String Katakana;

    public bool isCorrect;
}
