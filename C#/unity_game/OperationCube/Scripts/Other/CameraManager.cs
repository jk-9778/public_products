using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    InputChecker m_Input;                           //入力チェッカー
    Transform m_EyePoint;                           //プレイヤーの目の位置

    [SerializeField] float m_RotSpeed;              //通常の回転速度
    [SerializeField] float m_NormalDistance;        //通常時の距離
    [SerializeField] float m_ZoomDistance;          //ズーム時の距離
    [SerializeField] float m_ZoomTime;              //ズームにかかる時間
    [SerializeField] float m_ResetTime;             //カメラリセットにかかる時間

    float m_AngleH;                                 //水平の回転角度
    float m_AngleV;                                 //垂直の回転角度
    float m_Distance;                               //プレイヤーまでの距離

    static readonly float m_ANGLE_MAX = 50.0f;      //垂直の最大回転角度
    static readonly float m_ANGLE_MIN = -40.0f;     //垂直の最小回転角度

    Player m_Player;                                //プレイヤー
    float m_ResetStartTime = 0.0f;                  //カメラリセット開始時刻

    TargetCursor m_TargetCursor;


    void Start()
    {
        m_Distance = m_NormalDistance;
        //各コンポーネントを取得
        m_Input = GameObject.Find("GameManager").GetComponent<InputChecker>();
        m_Player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        m_EyePoint = m_Player.transform.Find("EyePoint").transform;
        m_TargetCursor = GameObject.Find("TargetCursors").GetComponent<TargetCursor>();
    }

    void Update()
    {
        //スティック入力
        if (m_Player.m_ActionState == Player.ActionState.Guard)
            //ガード時は遅く
            SetRotateSpeed(m_RotSpeed / 3.0f);
        else if(m_Distance <= m_ZoomDistance + 0.5)
            //ズーム中の遅く
            SetRotateSpeed(m_RotSpeed / 2.0f);
        else
            SetRotateSpeed(m_RotSpeed);
        //角度を丸める
        if (m_AngleH >= 360.0f) m_AngleH = 0.0f;
        if (m_AngleH <= -360.0f) m_AngleH = 0.0f;
        m_AngleV = Mathf.Clamp(m_AngleV, m_ANGLE_MIN, m_ANGLE_MAX);

        //入力された角度を反映
        Vector3 rotDir = Quaternion.Euler(m_AngleV, m_AngleH, 0f) * Vector3.back;
        transform.position = m_EyePoint.position + m_Distance * rotDir;

        // プレイヤーからカメラへの方向ベクトル
        Vector3 playerToCamera = (transform.position - m_EyePoint.position).normalized;

        // カメラのあるべき位置
        Vector3 correctPosition = m_EyePoint.transform.position + playerToCamera * m_Distance;

        // 壁とのめりこみチェック
        RaycastHit hit;
        Ray ray = new Ray(m_EyePoint.position, playerToCamera);
        int layerMask = 1 << LayerMask.NameToLayer("CollideObject");

        //プレイヤーからカメラまでの距離
        float playerToCameraDistance = Vector3.Distance(m_EyePoint.position, correctPosition);

        if (Physics.Raycast(ray, out hit, playerToCameraDistance, layerMask))
        {
            //　壁にめり込んでたら即座にめり込まない場所に移動
            transform.position = hit.point - playerToCamera.normalized * 0.2f;
        }
        else
        {
            // 壁にめり込んでなかったら補正しながら滑らかに移動
            transform.position = Vector3.Lerp(transform.position, correctPosition, 5f * Time.deltaTime);
        }


        if (m_Input.GetButtonDown(ButtonCode.RS))
            m_ResetStartTime = Time.time;
        if (Time.time <= m_ResetStartTime + m_ResetTime)
            CameraReset();

        //プレイヤーの方を向く
        transform.LookAt(m_EyePoint);
    }

    //回転速度をセット
    void SetRotateSpeed(float speed)
    {
        m_AngleH += m_Input.GetValueRH() * speed * Time.deltaTime;
        m_AngleV += m_Input.GetValueRV() * speed * Time.deltaTime;
    }

    //カメラリセット
    void CameraReset()
    {
        // 現在、プレイヤーから見てカメラがある方角
        Vector3 currentDirection = (transform.position - m_EyePoint.position).normalized;
        // 希望する方向（プレイヤーの背後の方向）
        Vector3 desiredDirection = -m_EyePoint.forward;
        // m_ResetTimeの時間かけて背後に行くようにLerpの強さを調整

        // 目標の方向へ少しずつ向ける
        Vector3 lerpedDirection = Vector3.Slerp(currentDirection, desiredDirection, m_ResetTime);

        // 場所を確定する
        transform.position = m_EyePoint.position + lerpedDirection * m_Distance;

        // プレイヤーからカメラへの方向ベクトル
        Vector3 playerToCameraDirection = transform.position - m_EyePoint.position;

        // ベクトルをQuatenionに変換してEulerに変換
        Vector3 rotation = Quaternion.FromToRotation(Vector3.back, playerToCameraDirection).eulerAngles;

        m_AngleH = rotation.y;
        m_AngleV = rotation.x;
    }

    //カメラ振動
    public void Shake(float magnitude, float duration, Vector3 position)
    {
        StartCoroutine(DoShake(magnitude, duration, position));
    }

    //カメラを振動させる
    IEnumerator DoShake(float magnitude, float duration, Vector3 position)
    {
        //振動を起こす対象とプレイヤーの距離
        float dis = Vector3.Distance(m_Player.transform.position, position);
        //距離が離れるほど振動を弱くする(2メートル離れるごとに減少)
        magnitude = magnitude * (1.0f - (dis / 2.0f * 0.1f));

        Vector3 shake = Vector3.zero;
        float elapsed = 0f;
        //指定された秒数だけ
        while (elapsed <= duration)
        {
            //ランダムな方向に振動する
            shake.x = Random.Range(-1f, 1f) * magnitude;
            shake.y = Random.Range(-1f, 1f) * magnitude;
            transform.position += shake;
            elapsed += Time.deltaTime;
            yield return null;
        }
    }

    public void ZoomIn()
    {
        LeanTween.value(gameObject, m_Distance, m_ZoomDistance, m_ZoomTime)
            .setOnUpdate((float value) => { m_Distance = value; })
            .setEase(LeanTweenType.easeOutQuint);
        m_TargetCursor.SetZoomInPosition();
    }

    public void ZoomOut()
    {
        LeanTween.value(gameObject, m_Distance, m_NormalDistance, m_ZoomTime)
            .setOnUpdate((float value) => { m_Distance = value; })
            .setEase(LeanTweenType.easeOutQuint);
        m_TargetCursor.SetZoomOutPosition();
    }
}
