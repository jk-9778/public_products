using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy01BodyCollider : MonoBehaviour
{
    //部位
    enum Part
    {
        Head,   //頭
        Body,   //胴体
    }
    [SerializeField] Part m_Part;
    Enemy01 m_Root;     //本体

    void Start()
    {
        m_Root = transform.root.GetComponent<Enemy01>();
    }

    void Update()
    {

    }

    //ダメージを本体に伝える
    void ConveyDamage(AttractObject.Size size)
    {
        int damage = 0;
        switch (size)
        {
            case AttractObject.Size.Small: damage = 1; break;
            case AttractObject.Size.Regular: damage = 2; break;
            case AttractObject.Size.Large: damage = 5; break;
        }
        switch (m_Part)
        {
            case Part.Head: m_Root.ToDamage(damage * 2); break;
            case Part.Body: m_Root.ToDamage(damage); break;
        }

        StartCoroutine(ChangeColorForSeconds());
    }

    //ダメージを受けた際の色変化
    IEnumerator ChangeColorForSeconds()
    {
        GetComponent<MeshRenderer>().material.color = Color.red;
        yield return new WaitForSeconds(1.0f);
        GetComponent<MeshRenderer>().material.color = Color.white;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "PlayerAmmo")
        {
            ConveyDamage(collision.gameObject.GetComponent<AttractObject>().GetSize());
        }
    }
}
