using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 等级管理类
/// </summary>
public class GradeMgr : Singleton<GradeMgr> {
    //config
    private const int DefaultMaxExp = 100;
    private const int GradeMaxExp = 100;

    private const string GRADE = "Grade";
    private int m_curGrade;
    public int CurGrade {
        get {
            return m_curGrade;
        }
        set {
            int changeValue = value - m_curGrade;
            m_curGrade = value;
            LocalSave.SetInt(GRADE, value);
            MaxExpCalculation();
            Send.SendMsg(SendType.GradeChange, changeValue, m_curGrade);
        }
    }

    private int m_maxExp;
    private int m_curExp;
    public int CurExp {
        get {
            return m_curExp;
        }
        set {
            int changeValue = value - m_curExp;
            m_curExp = value;
            if (m_curExp >= m_maxExp) {
                m_curExp -= m_maxExp;
                CurGrade++;
            }
            Send.SendMsg(SendType.ExpChange, changeValue, m_curExp, m_maxExp);
        }
    }

	public void Init(){
        m_curGrade = LocalSave.GetInt(GRADE, 1);
        m_curExp = 0;
        MaxExpCalculation();

	}
	
	public void Clear(){

	}

    private void OnExpAdd(object[] objs) {
        int addExp = (int)objs[0];
        CurExp += addExp;
    }

    private void MaxExpCalculation() {
        m_maxExp = m_curGrade * GradeMaxExp + DefaultMaxExp;
    }
}
