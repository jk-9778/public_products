using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MenuCursor : MonoBehaviour
{
    public Transform[] m_Cursors;

    GameObject m_PrevSelectObj;

    [SerializeField] SoundManager m_Sound;

    void Awake()
    {
    }

    void Start()
    {
        m_PrevSelectObj = EventSystem.current.currentSelectedGameObject;
    }

    void LateUpdate()
    {
        //EventSystemから現在選択されているオブジェクトを取得する
        GameObject selectObject = EventSystem.current.currentSelectedGameObject;

        if (selectObject != m_PrevSelectObj)
        {
            m_Sound.PlaySE(0);
            m_PrevSelectObj = selectObject;
        }

        //何も選択されていなければ何もしない
        if (selectObject == null) return;

        //選択されているオブジェクトの場所にカーソルを移動する
        transform.position = selectObject.transform.position;
        transform.rotation = selectObject.transform.rotation;

        //子カーソルを回転させる
        for (int i = 0; i < m_Cursors.Length; i++)
        {
            Vector3 rot = new Vector3(90.0f, 90.0f, 90.0f);
            rot = i >= m_Cursors.Length / 2 ? rot : -rot;
            m_Cursors[i].transform.Rotate(rot * Time.deltaTime);
        }
    }
}
