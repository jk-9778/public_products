using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TargetCursor : MonoBehaviour
{
    List<RectTransform> m_TCs = new List<RectTransform>();
    Image[] m_Images;

    static readonly float m_ZOOM_IN_DISTANCE = 50.0f;
    static readonly float m_ZOOM_OUT_DISTANCE = 150.0f;
    static readonly float m_ZOOM_TIME = 0.2f;
    float m_DistanceForCenter = m_ZOOM_OUT_DISTANCE;
    float m_ImageAlpha = 0.3f;

    void Start()
    {
        for (int i = 1; i <= 4; i++)
        {
            m_TCs.Add(transform.Find("TC" + i.ToString()).GetComponent<RectTransform>());
        }
        m_Images = GetComponentsInChildren<Image>();
    }

    void Update()
    {
        m_TCs[0].localPosition = new Vector3(-m_DistanceForCenter, m_DistanceForCenter, 0.0f);
        m_TCs[1].localPosition = new Vector3(m_DistanceForCenter, m_DistanceForCenter, 0.0f);
        m_TCs[2].localPosition = new Vector3(-m_DistanceForCenter, -m_DistanceForCenter, 0.0f);
        m_TCs[3].localPosition = new Vector3(m_DistanceForCenter, -m_DistanceForCenter, 0.0f);
        foreach (Image image in m_Images)
            image.color = new Color(1.0f, 1.0f, 1.0f, m_ImageAlpha);
    }

    public void SetZoomInPosition()
    {
        LeanTween.value(gameObject, m_DistanceForCenter, m_ZOOM_IN_DISTANCE, m_ZOOM_TIME)
            .setOnUpdate((float value) => { m_DistanceForCenter = value; })
            .setEase(LeanTweenType.linear);
        LeanTween.value(gameObject, m_ImageAlpha, 1.0f, m_ZOOM_TIME)
            .setOnUpdate((float value) => { m_ImageAlpha = value; })
            .setEase(LeanTweenType.easeOutQuint);
    }

    public void SetZoomOutPosition()
    {
        LeanTween.value(gameObject, m_DistanceForCenter, m_ZOOM_OUT_DISTANCE, m_ZOOM_TIME)
            .setOnUpdate((float value) => { m_DistanceForCenter = value; })
            .setEase(LeanTweenType.linear);
        LeanTween.value(gameObject, m_ImageAlpha, 0.3f, m_ZOOM_TIME)
            .setOnUpdate((float value) => { m_ImageAlpha = value; })
            .setEase(LeanTweenType.easeOutQuint);
    }
}
