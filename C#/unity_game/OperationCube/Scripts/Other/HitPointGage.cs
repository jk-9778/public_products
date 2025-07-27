using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HitPointGage : MonoBehaviour
{
    Image m_Gage;
    int m_HP;
    int m_Max;

    public HitPointGage(Image gage, int max)
    {
        m_Gage = gage;
        m_Max = max;
        m_HP = max;
    }

    void Start()
    {

    }

    void Update()
    {
    }

    //増やす
    public void Increase(int point)
    {
        m_HP += point;
        if (m_HP > m_Max) m_HP = m_Max;
        m_Gage.fillAmount = (float)m_HP / (float)m_Max;
    }

    //減らす
    public void Decrease(int point)
    {
        m_HP -= point;
        if (m_HP < 0) m_HP = 0;
        m_Gage.fillAmount = (float)m_HP / (float)m_Max;
    }
}
