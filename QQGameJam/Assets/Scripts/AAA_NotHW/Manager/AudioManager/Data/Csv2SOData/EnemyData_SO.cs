using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemySO", menuName = "ScriptableObjects/EnemySO")]
public class EnemyData_SO : ScriptableObject
{
    public List<EnemyData> data = new List<EnemyData>();
}
