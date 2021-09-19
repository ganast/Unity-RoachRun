using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentManager : MonoBehaviour {

    [SerializeField]
    private AudioSource Radio;

    private static EnvironmentManager inst;

    public static EnvironmentManager GetEnvironmentManager() {
        return inst;
    }

    public void StopRadio() {
        Radio.Stop();
    }

    public void StartRadio() {
        Radio.loop = true;
        Radio.Play();
    }

    protected void Awake() {
        inst = this;
    }

    protected void Start () {
	
	}

    protected void Update () {
		
	}
}
