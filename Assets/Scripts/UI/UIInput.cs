using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIInput : MonoBehaviour
{
    // キー入力確認用
    [SerializeField] private Text AText = null;
    [SerializeField] private Text SText = null;
    [SerializeField] private Text DText = null;
    [SerializeField] private Text WText = null;
    [SerializeField] private Text JumpText = null;

    // キーログ確認
    [SerializeField] private Text[] rogText = new Text[10];

    // 鍵ログ
    [SerializeField] private Text remainKeyText = null;


    void Update()
    {
        // キー入力確認用
        CheckInputKeyCode();

        // キー入力のログを出す
        PopRog();

        // このステージの残り鍵取得数
        RemainingNumberKeys();
    }

    
    ///************************************************************************
    /// <summary>
    /// キーの入力確認用
    /// </summary>
    private void CheckInputKeyCode()
    {
        ChangeColor(KeyCode.A, AText);
        ChangeColor(KeyCode.S, SText);
        ChangeColor(KeyCode.D, DText);
        ChangeColor(KeyCode.W, WText);
        ChangeColor(KeyCode.Space, JumpText);
    }


    // 色変え
    private void ChangeColor(KeyCode code,Text text)
    {
        if (Input.GetKey(code)) text.color = Color.red;
        else text.color = Color.white;
    }
    ///************************************************************************



    ///************************************************************************
    /// <summary>
    /// ログを出す
    /// </summary>
    private void PopRog()
    {
        bool[] isInputs = new bool[5] { Input.GetKeyDown(KeyCode.A), Input.GetKeyDown(KeyCode.S), Input.GetKeyDown(KeyCode.D), Input.GetKeyDown(KeyCode.W), Input.GetKeyDown(KeyCode.Space) };

        if (isInputs[0]) PushLog("A");
        if (isInputs[1]) PushLog("S");
        if (isInputs[2]) PushLog("D");
        if (isInputs[3]) PushLog("W");
        if (isInputs[4]) PushLog("Jump");
    }

    /// <summary>
    /// ログを一つ進める
    /// </summary>
    /// <param name="word"> 今回押されたボタン </param>
    private void PushLog(string word)
    {
        string a = rogText[0].text;
        string b = rogText[1].text;
        rogText[0].text = GetDigitCount() + " " + word;
        for (int i = 1; i < rogText.Length; i++)
        {
            rogText[i].text = a;
            if (i == rogText.Length - 1) break;
            a = b;
            b = rogText[i + 1].text;
        }
    }

    /// <summary>
    /// frameCount%maxDigitから桁数が少ないと0を追加して返す
    /// </summary>
    /// <returns> 桁が4桁で揃った状態で返す </returns>
    private string GetDigitCount()
    {
        int maxDigit = 10000;
        int count = Time.frameCount % maxDigit;
        if (count < 10) return "000" + count;
        if (count < 100) return "00" + count;
        if (count < 1000) return "0" + count;
        return count.ToString();
    }
    ///************************************************************************



    ///************************************************************************
    /// <summary>
    /// このステージの残り鍵取得数
    /// </summary>
    private void RemainingNumberKeys()
    {
        if (KeyManager.instance == null) return;
        int getKeyCount = KeyManager.instance.getKeyCount;
        int startStageKeyCount = KeyManager.instance.startStageKeyCount;

        // 鍵全部取った
        if (getKeyCount == startStageKeyCount)
            remainKeyText.text = "ゴール可能";
        // 残り鍵数
        else
            remainKeyText.text = (startStageKeyCount - getKeyCount).ToString();
    }
    ///************************************************************************
}
