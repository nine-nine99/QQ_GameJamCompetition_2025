using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 任务管理器
/// </summary>
public class TaskMgr : Singleton<TaskMgr> {
    private List<TaskInfo> taskInfoList = new List<TaskInfo>();

	public void Init(){
        InitList();
        Send.RegisterMsg(SendType.TaskValueChange, OnTaskValueChange);
	}
	
	public void Clear(){
        Send.UnregisterMsg(SendType.TaskValueChange, OnTaskValueChange);
	}

    private void InitList() {
        taskInfoList.Clear();
        foreach (RefTask refTask in RefTask.cacheMap.Values) {
            taskInfoList.Add(new TaskInfo(refTask));
        }
    }

    private void OnTaskValueChange(object[] objs) {
        TaskType taskType = (TaskType)objs[0];
        for (int index = 0; index < taskInfoList.Count; index++) {
            TaskInfo taskInfo = taskInfoList[index];
            if (taskInfo.refTask.TaskType == taskType) {
                taskInfo.Refresh();
            }
        }
    }

    public void TaskValueChange(TaskType taskType, int value) {
        LocalSave.SetInt(taskType, value);
        Send.SendMsg(SendType.TaskValueChange);
    }

    public bool TaskHasComplete(int taskId) {
        TaskInfo info = GetTaskInfo(taskId);
        if (info == null) {
            Debug.LogError("taskinfo is null:" + taskId);
            return false;
        }
        return info.taskState == TaskState.Complete;
    }

    public TaskInfo GetTaskInfo(int taskId) {
        for (int index = 0; index < taskInfoList.Count; index++) {
            TaskInfo taskInfo = taskInfoList[index];
            if (taskInfo.refTask.TaskId == taskId) {
                return taskInfo;
            }
        }

        return null;
    }
}

/// <summary>
/// 任务信息
/// </summary>
public class TaskInfo {
    private const string TASK_COMPLETE = "TaskComplete";
    public RefTask refTask;
    public TaskState taskState;

    private int m_curValue;
    public int CurValue {
        get {
            return m_curValue;
        }
        set {
            m_curValue = value;
            if (taskState == TaskState.Doing && m_curValue >= refTask.Condition) {
                CompleteTask();
            }
        }
    }

    public TaskInfo(RefTask _refTask) {
        refTask = _refTask;
        Refresh();
    }

    public void Refresh() {
        taskState = LocalSave.GetBool(TASK_COMPLETE + refTask.TaskId) ? TaskState.Complete : TaskState.Doing;
        CurValue = LocalSave.GetInt(refTask.TaskType);
    }

    public void CompleteTask() {
        LocalSave.SetBool(TASK_COMPLETE + refTask.TaskId, true);
        Refresh();
        Send.SendMsg(SendType.TaskComplete, refTask.TaskId);
        //如果完成任务有奖励 在这里发送广播
    }
}