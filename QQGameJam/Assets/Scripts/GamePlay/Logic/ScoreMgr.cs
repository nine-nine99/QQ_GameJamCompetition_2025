using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 分数管理类
/// </summary>
public class ScoreMgr : Singleton<ScoreMgr> {
    private int m_score;
    public int Score {
        get {
            return m_score;
        }
        set {
            int changeValue = value - m_score;
            m_score = value;
            if (value> BestScore) {
                BestScore = value;
            }
            Send.SendMsg(SendType.ScoreChange, changeValue, m_score);
            //这里可以做best的实时判断 根据具体需求调整
        }
    }

    private int m_bestScore;
    public int BestScore {
        get {
            return m_bestScore;
        }
        set {
            m_bestScore = value;
            LocalSave.SetInt(TaskType.BestScore, value);
            Send.SendMsg(SendType.TaskValueChange, TaskType.BestScore);
        }
    }

	public void Init(){
        m_bestScore = LocalSave.GetInt(TaskType.BestScore);
	}
	
	public void Clear(){
      
    }
}
