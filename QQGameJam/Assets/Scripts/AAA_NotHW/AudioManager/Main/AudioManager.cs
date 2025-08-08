using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : SingletonMonoBehavior<AudioManager>
{
    // public Audio newAudioData; // 音频数据脚本对象
    // public Dictionary<int, AudioData> AudioDataDictionary => newAudioData.AudioDataDictionary; // 音频数据字典
    // // public AudioData_SO audioData; // 音频数据
    // public AudioMixer audioMixer; // 音频混音器
    // [Header("背景音乐设置")]
    // public int bgmID = 10002; // 背景音乐ID
    // private void Start()
    // {
    //     SetBGM(bgmID); // 设置背景音乐
    // }

    // // 设置背景音乐
    // public void SetBGM(int id)
    // {
    //     if (AudioDataDictionary.TryGetValue(id, out AudioData audioData))
    //     {
    //         string audioName = audioData.URL; // 获取音频名称
    //         AudioSource audioSource = transform.GetChild(1).GetComponent<AudioSource>();
    //         audioSource.clip = LoadAudioClipFromResources(audioName);
    //         audioSource.Play(); // 播放音频
    //     }
    //     else
    //     {
    //         Debug.LogWarning($"Audio ID {id} not found in AudioDataDictionary.");
    //     }
    // }

    // public AudioClip LoadAudioClipFromResources(string url)
    // {
    //     // url 例如 "Audio/BGM1"
    //     return Resources.Load<AudioClip>(url);
    // }
}
