using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//ボタンコード
public enum ButtonCode
{
    //各ボタンにキーボード操作時のキーを割り当てる
    A = KeyCode.Space,
    B = KeyCode.X,
    X = KeyCode.Z,
    Y = KeyCode.R,
    LB = 1,
    RB = 0,
    BACK = KeyCode.B,
    START = KeyCode.M,
    LS = KeyCode.LeftShift,
    RS = KeyCode.C,
    LT = KeyCode.Q,
    RT = KeyCode.E,
}

public class InputChecker : MonoBehaviour
{
    bool m_Operation = false;               //操作可能か？
    public bool m_UseController;            //コントローラーを使うか？
    public float m_TriggerSensitivity;      //トリガーボタンの感度
    public float m_StickDeadZone;           //スティックのデッドゾーン

    static readonly float m_FORCE_QUIT_DURATION = 120.0f;   //強制終了するまでの時間
    float m_ElapseTimeFromLastInout = 0.0f;                 //最後の入力からの経過時間

    void Awake()
    {
        //マウスカーソルを非表示・ロックする(※ビルドする時にコメントアウトを外すこと!!)
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        ForceQuit();
    }

    //操作権限を設定
    public void SetOperation(bool operation)
    {
        m_Operation = operation;
    }

    //強制終了コマンド
    void ForceQuit()
    {
        //STARTとBACKの同時押し、または、ESCAPEキーが押されたら強制終了
        if ((GetButtonState(ButtonCode.START) && GetButtonState(ButtonCode.BACK)) || Input.GetKey(KeyCode.Escape))
        {
            Application.Quit();
            return;
        }
        //入力があれば時間をリセット
        if (InoutEny())
        {
            m_ElapseTimeFromLastInout = 0.0f;
        }
        else
        {
            m_ElapseTimeFromLastInout += Time.deltaTime;
            //指定秒数以上入力が無ければ強制終了
            if(m_ElapseTimeFromLastInout >= m_FORCE_QUIT_DURATION)
            {
                Application.Quit();
                return;
            }
        }
    }

    /****************************** 入力チェック(キーボードとマウス操作に対応させる) ******************************/
    //移動入力されているか？
    public bool GetLeftStick()
    {
        if (!m_Operation) return false;

        if (m_UseController)
        {
            return Input.GetAxisRaw("Stick LH") > m_StickDeadZone ||
                Input.GetAxisRaw("Stick LH") < -m_StickDeadZone ||
                Input.GetAxisRaw("Stick LV") > m_StickDeadZone ||
                Input.GetAxisRaw("Stick LV") < -m_StickDeadZone;
        }
        else
        {
            return Input.GetKey(KeyCode.W) ||
                Input.GetKey(KeyCode.A) ||
                Input.GetKey(KeyCode.S) ||
                Input.GetKey(KeyCode.D);
        }
    }

    //右スティックが入力されているか？
    public bool GetRightStick()
    {
        if (!m_Operation) return false;

        if (m_UseController)
        {
            return Input.GetAxisRaw("Stick RH") > m_StickDeadZone ||
                Input.GetAxisRaw("Stick RH") < -m_StickDeadZone ||
                Input.GetAxisRaw("Stick RV") > m_StickDeadZone ||
                Input.GetAxisRaw("Stick RV") < -m_StickDeadZone;
        }
        else
        {
            return Input.GetAxisRaw("Mouse X") > m_StickDeadZone ||
                Input.GetAxisRaw("Mouse X") < -m_StickDeadZone ||
                Input.GetAxisRaw("Mouse Y") > m_StickDeadZone ||
                Input.GetAxisRaw("Mouse Y") < -m_StickDeadZone;
        }
    }

    //左スティックの垂直入力値を返す
    public float GetValueLV()
    {
        if (!m_Operation) return 0.0f;

        if (m_UseController)
            return GetLeftStick() ? Input.GetAxisRaw("Stick LV") : 0.0f;
        else
        {
            if (Input.GetKey(KeyCode.W)) return 1.0f;
            else if (Input.GetKey(KeyCode.S)) return -1.0f;
            else return 0.0f;
        }
    }

    //左スティックの水平入力値を返す
    public float GetValueLH()
    {
        if (!m_Operation) return 0.0f;

        if (m_UseController)
            return GetLeftStick() ? Input.GetAxisRaw("Stick LH") : 0.0f;
        else
        {
            if (Input.GetKey(KeyCode.D)) return 1.0f;
            else if (Input.GetKey(KeyCode.A)) return -1.0f;
            else return 0.0f;
        }
    }

    //右スティックの垂直入力値を返す
    public float GetValueRV()
    {
        if (!m_Operation) return 0.0f;

        if (m_UseController)
            return GetRightStick() ? Input.GetAxisRaw("Stick RV") : 0.0f;
        else
            return Input.GetAxisRaw("Mouse Y");
    }

    //右スティックの水平入力値を返す
    public float GetValueRH()
    {
        if (!m_Operation) return 0.0f;

        if (m_UseController)
            return GetRightStick() ? Input.GetAxisRaw("Stick RH") : 0.0f;
        else
            return Input.GetAxisRaw("Mouse X");
    }

    //LTが押されているか
    public bool GetLeftTrigger()
    {
        if (!m_Operation) return false;

        if (m_UseController)
            return Input.GetAxisRaw("LT") < -m_TriggerSensitivity;
        else
            return Input.GetKey((KeyCode)ButtonCode.LT);
    }

    //RTが押されているか
    public bool GetRightTrigger()
    {
        if (!m_Operation) return false;

        if (m_UseController)
            return Input.GetAxisRaw("RT") > m_TriggerSensitivity;
        else
            return Input.GetKey((KeyCode)ButtonCode.RT);
    }

    //ボタンが押されたか
    public bool GetButtonDown(ButtonCode code)
    {
        if (!m_Operation) return false;

        if (m_UseController)
            return Input.GetButtonDown(code.ToString());
        else
        {
            switch (code)
            {
                case ButtonCode.LB:
                case ButtonCode.RB:
                    return Input.GetMouseButtonDown((int)code);
                default:
                    return Input.GetKeyDown((KeyCode)code);
            }
        }
    }

    //ボタンが離されたか
    public bool GetButtonUp(ButtonCode code)
    {
        if (!m_Operation) return false;

        if (m_UseController)
            return Input.GetButtonUp(code.ToString());
        else
        {
            switch (code)
            {
                case ButtonCode.LB:
                case ButtonCode.RB:
                    return Input.GetMouseButtonUp((int)code);
                default:
                    return Input.GetKeyUp((KeyCode)code);
            }
        }
    }

    //ボタンが押されているか
    public bool GetButtonState(ButtonCode code)
    {
        if (!m_Operation) return false;

        if (m_UseController)
            return Input.GetButton(code.ToString());
        else
        {
            switch (code)
            {
                case ButtonCode.LB:
                case ButtonCode.RB:
                    return Input.GetMouseButton((int)code);
                default:
                    return Input.GetKey((KeyCode)code);
            }
        }
    }

    //何らかの入力があったか？
    public bool InoutEny()
    {
        return GetLeftStick() || GetRightStick();
    }
}
