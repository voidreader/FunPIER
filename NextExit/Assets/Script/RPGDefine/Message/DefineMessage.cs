using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DefineMessage {

    //static Dictionary<int, string> MessageList;
    /*
    static string Language
    {
        get
        {
            switch (GameConfig.Language)
            {
                case 
            }
            return "english";
        }
    }
    */

    /// <summary>
    /// 
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public static string getMsg(int index)
    {
        string language = "korea";
		switch ( Application.systemLanguage ) {
            case SystemLanguage.Korean:
                language = "korea"; break;

            default:
                language = "english"; break;

        }
		/*
        switch(GameConfig.Language)
        {
            case 0: language = "korea"; break;  // 한국어.
            case 1: language = "english"; break;  // 영어.
            case 2: language = "japan"; break;  // 일본어.
            case 3: language = "china"; break;  // 중국어.
        }
		*/

        /*
        if (MessageList == null)
        {
            MessageList = new Dictionary<int, string>()
            {
                {10001, "해당 위치에 이미 배치된 블럭이 있습니다."},
                {10002, "블럭을 놓을 수 없는 위치 입니다."},
                {10003, "입구는 한개만 배치할 수 있습니다."},
                {10004, "출구는 한개만 배치할 수 있습니다."},
                {10005, "입구를 배치해야 합니다."},
                {10006, "출구를 배치해야 합니다."},
                {10007, "근접한 블록이 있어야 합니다."},

                {20001, "스테이지 이름을 입력하세요"},
                {20002, "스테이지 이름은 반드시 입력해야 합니다."},
                {20003, "존재하지 않는 스테이지 입니다."},

                {30001, "저장된 게임이 존재합니다.\n이어 하시겠습니까?"},
            };
        }
        return MessageList[index];
        */
		Debug.Log( "language : " + language );
		Debug.Log( "index : " + index );
		Dictionary<string, object> dic = RPGSheetManager.Instance.getInfo( "text.bin", "index", index.ToString() );

       // string t = (RPGSheetManager.Instance.getSheetData("text.bin", index.ToString()) as Dictionary<string, object>)[language].ToString();
		string t = dic[language].ToString();
		t = t.Replace("\\\\n", System.Environment.NewLine);
        //t = t.Replace("\\n", System.Environment.NewLine);
        return t;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="index"></param>
    /// <param name="args"></param>
    /// <returns></returns>
    public static string getMsg(int index, params object[] args)
    {
        return string.Format(getMsg(index), args);
    }
}
