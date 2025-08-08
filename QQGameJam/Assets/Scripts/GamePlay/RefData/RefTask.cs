using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RefTask : RefBase {

    public static Dictionary<int, RefTask> cacheMap = new Dictionary<int, RefTask>();

    public int TaskId;
    public TaskType TaskType;
    public int Condition;

    public override string GetFirstKeyName() {
        return "TaskId";
    }

    public override void LoadByLine(Dictionary<string, string> _value, int _line) {
        base.LoadByLine(_value, _line);
        TaskId = GetInt("TaskId");
        TaskType = (TaskType)GetEnum("TaskType", typeof(TaskType));
        Condition = GetInt("Condition");
    }

    public static RefTask GetRef(int taskid) {
        RefTask data = null;
        if (cacheMap.TryGetValue(taskid, out data)) {
            return data;
        }

        if (data == null) {
            Debug.LogError("error RefTask key:" + taskid);
        }
        return data;
    }
}
