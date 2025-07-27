using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    AudioSource m_AudioSource;

    [SerializeField] AudioClip[] m_AuduioClip;    //アタッチしているオブジェクトで扱う音源リスト

    void Start()
    {
        m_AudioSource = GetComponent<AudioSource>();
    }

    void Update()
    {

    }

    public void PlaySE(int num)
    {
        m_AudioSource.PlayOneShot(m_AuduioClip[num]);
    }
}
