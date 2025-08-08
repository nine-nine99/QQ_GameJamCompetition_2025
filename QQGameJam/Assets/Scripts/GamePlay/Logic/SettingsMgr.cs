using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsMgr : Singleton<SettingsMgr> {
    private string MUSIC_KEY = "OpenMusic";
    private string VIRATE_KEY = "OpenVirate";

    public bool OpenMusic {
        get {
            return LocalSave.GetBool(MUSIC_KEY, true);
        }
        set {
            LocalSave.SetBool(MUSIC_KEY, value);
        }
    }

    public bool OpenVirate {
        get {
            return LocalSave.GetBool(VIRATE_KEY, true);
        }
        set {
            LocalSave.SetBool(VIRATE_KEY, value);
        }
    }

	public void Init(){
		
	}
	
	public void Clear(){
		
	}
}
