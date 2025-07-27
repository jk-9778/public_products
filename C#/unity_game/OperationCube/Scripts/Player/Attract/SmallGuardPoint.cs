using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmallGuardPoint : MonoBehaviour
{
    public enum Status
    {
        Enabled,
        Disabled,
    }

    [SerializeField]
    Status m_Status;

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    //使用可能にする
    public void SetEnabled()
    {
        m_Status = Status.Enabled;
    }

    //使用不可にする
    public void SetDisabled()
    {
        m_Status = Status.Disabled;
    }

    //状態を取得する
    public Status GetStatus()
    {
        return m_Status;
    }
}
