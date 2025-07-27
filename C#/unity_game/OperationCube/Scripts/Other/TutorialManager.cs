using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{
    [SerializeField] Image[] m_Images;
    int m_SetNum = 0;
    [SerializeField] Image[] m_Others;
    [SerializeField] GameObject[] m_Texts;
    InputChecker m_Input;
    [SerializeField] float m_ElapseTime = 0.0f;
    bool action = false;
    SceneMaker m_SceneMaker;
    [SerializeField] TutorialPlayer m_TutorialPlayer;

    void Start()
    {
        m_Input = GetComponent<InputChecker>();
        m_SceneMaker = GetComponent<SceneMaker>();
        foreach (Image image in m_Images)
        {
            image.gameObject.SetActive(false);
        }
        foreach (Image other in m_Others)
        {
            other.gameObject.SetActive(false);
        }
        foreach (GameObject text in m_Texts)
        {
            text.SetActive(false);
        }
    }

    void Update()
    {
        m_ElapseTime += Time.deltaTime;
        if (m_ElapseTime >= 2.0f && EnyPressButton() && action && m_SetNum <= 5)
        {
            InvisibleCapture();
            action = false;
        }
        else if (m_ElapseTime >= 2.0f && m_SetNum == 6)
        {
            m_SetNum++;
            m_SceneMaker.LoadScene();
        }
    }

    public void VisualozeCapture()
    {
        m_Images[m_SetNum].gameObject.SetActive(true);
        foreach (Image other in m_Others)
        {
            other.gameObject.SetActive(true);
        }
        m_Texts[m_SetNum].SetActive(true);
        m_ElapseTime = 0.0f;
        action = true;
    }

    public void InvisibleCapture()
    {
        m_Images[m_SetNum].gameObject.SetActive(false);
        foreach (Image other in m_Others)
        {
            other.gameObject.SetActive(false);
        }
        m_Texts[m_SetNum].SetActive(false);
        m_SetNum++;
        m_TutorialPlayer.ResetTime();
        m_TutorialPlayer.ChangeState((TutorialPlayer.TutorialState)m_SetNum);
    }

    bool EnyPressButton()
    {
        return m_Input.GetButtonDown(ButtonCode.A) ||
            m_Input.GetButtonDown(ButtonCode.B) ||
            m_Input.GetButtonDown(ButtonCode.X) ||
            m_Input.GetButtonDown(ButtonCode.Y) ||
            m_Input.GetButtonDown(ButtonCode.LB) ||
            m_Input.GetButtonDown(ButtonCode.RB) ||
            m_Input.GetLeftTrigger() ||
            m_Input.GetRightTrigger();
    }
}
