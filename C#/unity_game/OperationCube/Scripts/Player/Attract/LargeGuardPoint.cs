using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LargeGuardPoint : MonoBehaviour
{
    public enum Status
    {
        Enabled,
        Disabled,
    }

    [SerializeField]
    Status m_Status;
    RegularGuardPoint[] m_RegularGuardPoint;

    void Start()
    {
        //中ガードポイントを取得
        m_RegularGuardPoint = GetComponentsInChildren<RegularGuardPoint>();
    }

    void Update()
    {

    }

    //自分と全ての中ガードポイントを使用可能にする
    public void SetAllEnabled()
    {
        m_Status = Status.Enabled;
        for (int i = 0;i < m_RegularGuardPoint.Length; i++)
        {
            m_RegularGuardPoint[i].SetAllEnabled();
        }
    }

    //自分と全ての中ガードポイントを使用不可にする
    public void SetAllDisabled()
    {
        m_Status = Status.Disabled;
        for (int i = 0; i < m_RegularGuardPoint.Length; i++)
        {
            m_RegularGuardPoint[i].SetAllDisabled();
        }
    }

    //状態を取得する
    public Status GetStatus()
    {
        return m_Status;
    }
}
