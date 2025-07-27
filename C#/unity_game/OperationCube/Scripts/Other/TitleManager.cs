using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class TitleManager : MonoBehaviour
{
    SceneMaker m_SceneMaker;        //シーンメーカー
    [SerializeField] SoundManager m_Sound;

    void Start()
    {
        m_SceneMaker = GetComponent<SceneMaker>();
    }

    void Update()
    {

    }

    //プレイボタンが押されたら
    public void InputPlayButton()
    {
        m_Sound.PlaySE(1);
        m_SceneMaker.ChangeNextScene(SceneMaker.SceneType.GamePlayScene);
        m_SceneMaker.LoadScene();
    }

    //チュートリアルボタンが押されたら
    public void InputTutorialButton()
    {
        m_Sound.PlaySE(1);
        m_SceneMaker.ChangeNextScene(SceneMaker.SceneType.TutorialScene);
        m_SceneMaker.LoadScene();
    }

    //終了ボタンが押されたら
    public void InputExitButton()
    {
        m_Sound.PlaySE(1);
        Application.Quit();
    }
}
