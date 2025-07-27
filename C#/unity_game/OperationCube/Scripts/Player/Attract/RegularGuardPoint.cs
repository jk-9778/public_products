using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RegularGuardPoint : MonoBehaviour
{
    public enum Status
    {
        Enabled,
        Disabled,
    }

    [SerializeField]
    Status m_Status;
    SmallGuardPoint[] m_SmallGuardPoint;

    void Start()
    {
        m_SmallGuardPoint = GetComponentsInChildren<SmallGuardPoint>();
    }

    void Update()
    {
        
    }

    //自分と全ての小ガードポイントを使用可能にする
    public void SetAllEnabled()
    {
        m_Status = Status.Enabled;
        for (int i = 0; i < m_SmallGuardPoint.Length; i++)
        {
            m_SmallGuardPoint[i].SetEnabled();
        }
    }

    //自分と全ての小ガードポイントを使用不可にする
    public void SetAllDisabled()
    {
        m_Status = Status.Disabled;
        for (int i = 0; i < m_SmallGuardPoint.Length; i++)
        {
            m_SmallGuardPoint[i].SetDisabled();
        }
    }

    //状態を取得する
    public Status GetStatus()
    {
        return m_Status;
    }
}
