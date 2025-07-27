using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialPlayer : Player
{
    public enum TutorialState
    {
        Wait,
        Move,
        Zoom,
        Attract,
        Shoot,
        Guard,
        End,
    }
    [SerializeField] TutorialState m_TState = TutorialState.Wait;
    TutorialManager m_Tutorial;
    [SerializeField] float m_ElapeTime = 0.0f;
    bool action = false;

    void Awake()
    {
        m_Tutorial = GameObject.Find("GameManager").GetComponent<TutorialManager>();
    }

    void LateUpdate()
    {
        m_ElapeTime += Time.deltaTime;

        if (m_TState == TutorialState.Wait && m_ElapeTime >= 1.0f && !action)
        {
            m_Tutorial.VisualozeCapture();
            action = true;
            //ChangeState(TutorialState.Move);
        }

        if (m_TState == TutorialState.Move && m_ElapeTime >= 7.0f && !action)
        {
            m_Tutorial.VisualozeCapture();
            action = true;
            //(TutorialState.Zoom);
        }

        if (m_TState == TutorialState.Zoom && m_Input.GetButtonUp(ButtonCode.LB) && !action)
        {
            m_Tutorial.VisualozeCapture();
            action = true;
            //ChangeState(TutorialState.Attract);
        }
    }

    public void ChangeState(TutorialState state)
    {
        m_ElapeTime = 0.0f;
        m_TState = state;
        action = false;
    }

    public void ResetTime()
    {
        //m_ElapeTime = 0.0f;
    }

    protected override void GuardUpdate()
    {
        base.GuardUpdate();
        if (m_TState == TutorialState.Guard && !m_Input.GetLeftTrigger())
        {
            m_Tutorial.VisualozeCapture();
            action = true;
            //ChangeState(TutorialState.End);
        }
    }

    protected override void AttractEndEvent()
    {
        base.AttractEndEvent();
        if (m_TState == TutorialState.Attract && !action)
        {
            m_Tutorial.VisualozeCapture();
            //ChangeState(TutorialState.Shoot);
        }
    }
    protected override void ShootEndEvent()
    {
        base.ShootEndEvent();
        if (m_TState == TutorialState.Shoot && !action)
        {
            m_Tutorial.VisualozeCapture();
            //ChangeState(TutorialState.Guard);
        }
    }
}
