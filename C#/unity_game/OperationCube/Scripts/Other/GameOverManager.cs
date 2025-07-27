using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverManager : MonoBehaviour
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

    //リトライボタンが押されたら
    public void InputRetryButton()
    {
        m_Sound.PlaySE(1);
        m_SceneMaker.ChangeNextScene(SceneMaker.SceneType.GamePlayScene);
        m_SceneMaker.LoadScene();
    }

    //タイトルボタンが押されたら
    public void InputTitleButton()
    {
        m_Sound.PlaySE(1);
        m_SceneMaker.ChangeNextScene(SceneMaker.SceneType.TitleScene);
        m_SceneMaker.LoadScene();
    }
}
