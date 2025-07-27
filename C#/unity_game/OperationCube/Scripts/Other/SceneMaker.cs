using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneMaker : MonoBehaviour
{
    //シーンの種類(シーン名と同じものを登録する)
    public enum SceneType
    {
        TitleScene,         //タイトル
        TutorialScene,      //チュートリアル
        GamePlayScene,      //ゲームプレイ
        GameOverScene,      //ゲームオーバー
        CreditScene,        //クレジット
    }

    //遷移状態
    public enum State
    {
        FadeIn,         //フェードイン中
        FadeOut,        //フェードアウト中
        Completed,      //遷移完了中
        Change,         //遷移中
    }

    [SerializeField] SceneType m_NextScene = SceneType.GamePlayScene;   //次のシーン
    [SerializeField] State m_State = State.FadeIn;                      //シーンの状態

    InputChecker m_Input;       //入力情報
    Image m_BlackScreen;        //フェードに使う黒画像(※各シーンのキャンバスに設置しておくこと！)
    Image m_LoadLogoImage;      //ロード中に回す画像
    Text m_LoadTextImage;       //ロード中に表示するテキスト

    static readonly float m_FADE_TIME = 0.5f;     //フェードにかかる時間
    static readonly float m_FADE_DELAY = 1.0f;    //フェードの遅延

    void Start()
    {
        m_Input = GetComponent<InputChecker>();
        m_BlackScreen = GameObject.Find("BlackScreen").GetComponent<Image>();
        m_LoadLogoImage = GameObject.Find("LoadLogo").GetComponent<Image>();
        m_LoadTextImage = GameObject.Find("LoadText").GetComponent<Text>();
        //始めはどちらも非表示にする
        SetUIEnabled(false);
    }

    void Update()
    {
        switch (m_State)
        {
            case State.FadeIn: FadeInUpdate(); break;
            case State.FadeOut: FadeOutUpdate(); break;
            case State.Completed: CompletedUpdate(); break;
            case State.Change: ChangeUpdate(); break;

        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            LoadScene();
        }
    }

    //フェードイン中
    void FadeInUpdate()
    {
        LeanTween.alpha(m_BlackScreen.rectTransform, 0.0f, m_FADE_TIME).setEase(LeanTweenType.linear);
        if (m_BlackScreen.color.a <= 0.0f)
        {
            //フェードインが終わったら操作可能に
            m_Input.SetOperation(true);
            ChangeState(State.Completed);
        }
    }

    //フェードアウト中
    void FadeOutUpdate()
    {
        LeanTween.alpha(m_BlackScreen.rectTransform, 1.0f, m_FADE_TIME).setEase(LeanTweenType.linear).setDelay(m_FADE_DELAY);
        if (m_BlackScreen.color.a >= 1.0f)
        {
            SetUIEnabled(true);
            ChangeState(State.Change);
            //フェードアウト完了後にシーン遷移
            StartCoroutine(Load());
        }
    }

    //遷移完了
    void CompletedUpdate()
    {

    }

    //遷移t中
    void ChangeUpdate()
    {
    }

    //シーン切り替え
    public void LoadScene()
    {
        //フェードアウトに入ったら操作不能に
        m_Input.SetOperation(false);
        ChangeState(State.FadeOut);
    }

    //シーンをロードする
    IEnumerator Load()
    {
        AsyncOperation async = SceneManager.LoadSceneAsync(m_NextScene.ToString());

        while (!async.isDone)
        //while (LoadingGage.fillAmount != 1.0f)
        {
            //LoadingGage.fillAmount = async.progress;
            m_LoadLogoImage.transform.Rotate(new Vector3(0, 90, 0) * Time.deltaTime * 10.0f, Space.Self);
            yield return null;
        }
    }

    //次のシーンを変更する
    public void ChangeNextScene(SceneType type)
    {
        m_NextScene = type;
    }

    //状態遷移
    void ChangeState(State state)
    {
        m_State = state;
    }

    //UIの表示状態をセット
    void SetUIEnabled(bool enabled)
    {
        m_LoadLogoImage.enabled = enabled;
        m_LoadTextImage.enabled = enabled;
    }
}
