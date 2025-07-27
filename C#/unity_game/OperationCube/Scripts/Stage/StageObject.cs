using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageObject : MonoBehaviour
{
    enum State
    {
        Normal,
        Animation,
        Wait,
    }
    [SerializeField] State m_State = State.Normal;
    Animator m_Animator;                        //アニメーター

    float m_Timer = 0.0f;                       //タイマ
    float m_Scale = 0.0f;                       //揺れの規模
    Vector3 m_Target = Vector3.zero;                  //距離を測る対象
    AnimatorStateInfo m_Info;                   //アニメーター情報

    void Start()
    {
        m_Animator = GetComponent<Animator>();
        m_Info = m_Animator.GetCurrentAnimatorStateInfo(0);
    }

    void Update()
    {
        switch (m_State)
        {
            case State.Normal: NormalUpdate(); break;
            case State.Animation: AnimationUpdate(); break;
            case State.Wait: WaitUpdate(); break;
            default: break;
        }
        //タイマを更新
        m_Timer += Time.deltaTime;
    }

    //通常時
    void NormalUpdate()
    {

    }

    //アニメーション時
    void AnimationUpdate()
    {
        if (m_Timer >= Selector() * 0.05f)
        {
            m_Animator.SetTrigger("Anim" + Selector().ToString());
            ChangeState(State.Wait);
        }
    }

    //待機
    void WaitUpdate()
    {
        if (m_Timer >= m_Info.length - 0.2f)
        {
            LeanTween.moveY(gameObject, -0.5f, 0.2f)
                .setEase(LeanTweenType.linear);
        }
        ChangeState(State.Normal);
    }

    //アニメーションセット
    public void SetAnimation(Vector3 target, float scale)
    {
        m_Scale = scale;
        m_Target = target;
        if (m_State == State.Normal)
            ChangeState(State.Animation);
    }

    //再生するアニメーションを選択する
    int Selector()
    {
        //距離が近いほど強いアニメーションをセット
        float dis = Vector3.Distance(transform.position, m_Target);
        float unit = m_Scale / 12.0f;
        if (dis < unit) return 0;
        else if (dis < unit * 2.0f) return 1;
        else if (dis < unit * 3.0f) return 2;
        else if (dis < unit * 4.0f) return 3;
        else if (dis < unit * 5.0f) return 4;
        else return 5;
    }

    //状態遷移
    void ChangeState(State state)
    {
        m_Timer = 0.0f;
        m_State = state;
    }
}
