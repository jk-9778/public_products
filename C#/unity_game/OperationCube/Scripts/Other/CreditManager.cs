using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreditManager : MonoBehaviour
{
    SceneMaker m_SceneMaker;                        //シーンメーカー
    [SerializeField] RectTransform m_CreditText;    //リザルトテキスト
    bool load = false;

    void Start()
    {
        m_SceneMaker = GetComponent<SceneMaker>();
    }

    void Update()
    {
        if (m_CreditText.position.y >= 15.0f && !load)
        {
            m_SceneMaker.LoadScene();
            load = true;
        }
        else if (!load)
            m_CreditText.position += Vector3.up * 2.0f * Time.deltaTime;
    }
}
