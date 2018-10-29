using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIButtons : MonoBehaviour
{
    public void Iwanttodie()
    {
        Application.Quit();
    }
    public void IwanttobeBIG()
    {
        Screen.fullScreen = true;
    }
    public void SceneSwitching(int DesiredScene)
    {
        SceneManager.LoadScene(DesiredScene);
    }
    public void Unpauzing()
    {
        GameObject.Find("GameManager").GetComponent<GameManager>().m_CurrentlyPauzed = false;
        GameObject.Find("GameManager").GetComponent<GameManager>().m_PauseMenu.SetActive(false);
    }
}
