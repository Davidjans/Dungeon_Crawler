using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingScreens : MonoBehaviour {
    [SerializeField] private List<Sprite> m_LoadingScreens;
    private int currentimage;

	// Use this for initialization
	void Start () {
        currentimage = Mathf.RoundToInt(Random.Range(0, m_LoadingScreens.Count));
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
