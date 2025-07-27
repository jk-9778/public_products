using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectPlayer : MonoBehaviour
{
    public enum Effects
    {
        LandingShock,
    }
    [Header("enumEffectsの順番通りに入れてね！")]
    public ParticleSystem[] m_Efffects = new ParticleSystem[System.Enum.GetNames(typeof(Effects)).Length];
    [Header("AttractObjectを大きさ順に入れてね！")]
    [SerializeField] AttractObject[] m_Cube;                        //パーティクルに使用するキューブ
    Transform m_FieldObjects;                                       //放出したキューブを格納するオブジェクト
    List<GameObject> particles = new List<GameObject>();

    void Start()
    {
        m_FieldObjects = GameObject.Find("FieldObjects").transform;
    }

    void Update()
    {

    }

    //パーティクルシステム再生
    public void PlaySystem(Effects effects, Transform parent)
    {
        ParticleSystem playEffect = Instantiate(m_Efffects[(int)effects], parent);
    }

    //パーティクルシステム停止
    public void StopSystem(Effects effects)
    {
        m_Efffects[(int)effects].Stop();
    }

    /// <summary>
    /// キューブエフェクトを再生
    /// </summary>
    /// <param name="count">パーティクル数</param>
    /// <param name="t">発生位置</param>
    /// <param name="r">発生半径</param>
    /// <param name="size">キューブサイズ</param>
    /// <param name="force">放出力</param>
    public List<GameObject> PlayCube(int count, Transform t, float r, AttractObject.Size size, float force = 7.0f)
    {
        List<GameObject> particles = new List<GameObject>();
        for (int i = 0; i < count; i++)
        {
            //パーティクル生成
            AttractObject cube = Instantiate(m_Cube[(int)size], new Vector3(t.position.x + Random.Range(-r, r), t.position.y, t.position.z + Random.Range(-r, r)), t.rotation, m_FieldObjects);
            //ランダムな上方向に放出
            cube.GetComponent<Rigidbody>().velocity = new Vector3(Random.Range(-1.0f, 1.0f), Random.Range(0.5f, 1.0f), Random.Range(-1.0f, 1.0f)) * force;
            //生成したパーティクルを保存
            particles.Add(cube.gameObject);
        }
        return particles;
    }

    /// <summary>
    /// 消えるキューブエフェクトを再生
    /// </summary>
    /// <param name="count">パーティクル数</param>
    /// <param name="t">発生位置</param>
    /// <param name="r">発生半径</param>
    /// <param name="size">キューブサイズ</param>
    /// <param name="force">放出力</param>
    /// <param name="time">生存時間</param>
    public void PlayVanishCube(int count, Transform t, float r, AttractObject.Size size, float force = 7.0f, float time = 1.0f)
    {
        List<GameObject> particles = new List<GameObject>();
        for (int i = 0; i < count; i++)
        {
            //パーティクル生成
            AttractObject cube = Instantiate(m_Cube[(int)size], new Vector3(t.position.x + Random.Range(-r, r), t.position.y, t.position.z + Random.Range(-r, r)), t.rotation, m_FieldObjects);
            //プレイヤーが持てないようにタグを変更
            cube.tag = "EffectCube";
            //ランダムな上方向に放出
            cube.GetComponent<Rigidbody>().velocity = new Vector3(Random.Range(-1.0f, 1.0f), Random.Range(0.5f, 1.0f), Random.Range(-1.0f, 1.0f)) * force;
            //生成したパーティクルを保存
            particles.Add(cube.gameObject);
        }
        StartCoroutine(DestroyCube(particles, time));
    }

    //
    IEnumerator DestroyCube(List<GameObject> cubes, float time)
    {
        yield return new WaitForSeconds(time);
        for (int i = 0; i < cubes.Count; i++) Destroy(cubes[i]);
    }
}
