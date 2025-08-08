using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 声音管理器 主要用于播放声音
/// 依赖 LocalAssetMgr
/// </summary>
public class SoundMgr : SingletonMonoBehavior<SoundMgr> {

    private AudioSource soundSource;
    private AudioSource bgSource;
    private Dictionary<string, float> playRecord = new Dictionary<string, float>();
    private float playCD = 0.1f;

	// Use this for initialization
	void Start () {
        GameObject bgGo = new GameObject();
        bgGo.name = "musicBg";
        bgGo.transform.SetParent(transform, false);
        bgSource = bgGo.AddMissingComponent<AudioSource>();
        bgSource.loop = true;
        //DontDestroyOnLoad(bgGo);

        GameObject soundGo = new GameObject();
        soundGo.name = "soundGo";
        soundGo.transform.SetParent(transform, false);
        soundSource = soundGo.AddMissingComponent<AudioSource>();
        //DontDestroyOnLoad(soundGo);
	}

    /// <summary>
    /// 播放背景音乐
    /// </summary>
    /// <param name="_name">音乐名</param>
    public void PlayMusic(string _name) {
        AudioClip clip = LocalAssetMgr.Instance.Load_Music(_name);
        if (clip == null)
            return;
        bgSource.clip = clip;
        bgSource.Play();
    }

    /// <summary>
    /// 获取背景音乐的时长
    /// </summary>
    /// <returns>时长</returns>
    public float GetMusicLength() {
        return bgSource.clip.length;
    }

    /// <summary>
    /// 播放音效
    /// </summary>
    /// <param name="_name">音乐名</param>
    public void PlaySound(string _name) {
        if (playRecord.ContainsKey(_name)) {
            if (Time.time - playRecord[_name] < playCD)
                return;
            else
                playRecord[_name] = Time.time;
        }
        else {
            playRecord.Add(_name, Time.time);
        }

        AudioClip clip = LocalAssetMgr.Instance.Load_Music(_name);
        if (clip == null)
            return;
        soundSource.PlayOneShot(clip);
    }

    /// <summary>
    /// 设置背景音乐的音量
    /// </summary>
    /// <param name="vol">声音大小(0.0 到1.0)</param>
    /// <param name="close">是否静音</param>
    public void SetMusicVol(float vol, bool close) {
        bgSource.volume = close ? 0 : vol;
    }

    /// <summary>
    /// 设置音效的音量
    /// </summary>
    /// <param name="vol">声音大小(0.0 到1.0)</param>
    /// <param name="close">是否静音</param>
    public void SetSoundVo(float vol, bool close) {
        soundSource.volume = close ? 0 : vol;
    }
}