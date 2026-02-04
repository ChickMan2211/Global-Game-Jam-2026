using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EnemyWaveData
{
    public EnemyProfile EnemyProfile;
    public List<VocabData> VocabList;
}

[CreateAssetMenu(fileName = "LevelData", menuName = "Scriptable Objects/LevelData")]
public class LevelData : ScriptableObject
{
    public int LevelID;
    public List<EnemyWaveData> Waves; // Thay thế cho 2 list rời rạc cũ
    public AnswerMode Mode;
}
