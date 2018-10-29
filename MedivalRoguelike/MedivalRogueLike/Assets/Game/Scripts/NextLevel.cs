using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NextLevel : MonoBehaviour {
    [SerializeField] private int m_LevelBuild;
    private void OnTriggerEnter(Collider other)
    {
        SceneManager.LoadScene(m_LevelBuild);
    }
}
