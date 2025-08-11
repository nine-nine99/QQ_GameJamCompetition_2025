using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AudioData", menuName = "ScriptableObjects/AudioData")]
public class AudioData_SO : ScriptableObject
{
    public List<AudioDetail> audioDetails; // 音频列表
    public AudioDetail GetAudioDetail(string name)
    {
        foreach (var audioDetail in audioDetails)
        {
            if (audioDetail.name == name)
            {
                return audioDetail;
            }
        }
        Debug.LogWarning("Audio not found: " + name);
        return null;
    }
}

[System.Serializable]
public class AudioDetail
{
    public string name; // 音频名称
    public AudioClip clip; // 音频剪辑
    public AudioType type; // 音频类型
    public bool loop; // 是否循环播放
    [Range(0f, 1f)]
    public float volume = 1f; // 音量
    [Range(0f, 1f)]
    public float MaxPitch = 1f; // 最大音调
    [Range(0f, 1f)]
    public float MinPitch = 1f; // 最小音调
}