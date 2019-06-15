using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ServerDataManager : RPGSingleton<ServerDataManager>
{
    /// <summary>
    /// key = 서버에서의 맵 인덱스.
    /// value = 암호화된 맵 정보.
    /// </summary>
    Dictionary<string, string> m_MapList = new Dictionary<string, string>();

    public rpgames.game.Game NowGame { get; private set; }
    public rpgames.game.Map NowMap { get; private set; }

    public override void Init()
    {
        base.Init();
        reset();
    }

    public void reset()
    {
        NowGame = null;
        NowMap = null;
    }

    public void getMapData(string index, System.Action<string> selector)
    {
        if (m_MapList.ContainsKey(index))
            selector(m_MapList[index]);
        else
        {
            rpgames.game.RequestMapLoad request = new rpgames.game.RequestMapLoad();
            {
                request.request = WebConnector.Instance.CommonRequest;
                request.idx = index;
            }
            WebObject wo = new WebObject((w) =>
            {
                rpgames.game.ResponseMapLoad response = w.getResult<rpgames.game.ResponseMapLoad>();
                if (response.response.success)
                {
                    string mapData = response.data;

                    m_MapList[index] = mapData;
                    if (selector != null)
                        selector(mapData);
                }
                else
                {
                    Debug.LogError("map_load Error = " + response.response.error_code.ToString());
                }

            });
            wo.setData<rpgames.game.RequestMapLoad>(request);
            wo.setCommand("map_load");
            WebConnector.Instance.request(wo);
        }
    }

    public void playCustomGame(string index)
    {
        getMapData(index,
            (mapData) =>
            {
                rpgames.game.RequestGameStart request = new rpgames.game.RequestGameStart();
                {
                    request.request = WebConnector.Instance.CommonRequest;
                    request.map_idx = index;
                }
                WebObject wo = new WebObject((w) =>
                {
                    rpgames.game.ResponseGameStart response = w.getResult<rpgames.game.ResponseGameStart>();
                    if (response.response.success)
                    {
                        NENetworkManager.Instance.Gold = response.gold;
                        NowGame = response.game;
                        NowMap = response.map;

                        // 게임 실행.
                        BlockManager.Instance.loadCustom(mapData);
                        GameManager.Instance.startInGameReady(GameManager.ePlayMode.Custom);
                    }
                    else
                    {
                        Debug.LogError("game_start Error = " + response.response.error_code.ToString());
                    }

                });
                wo.setData<rpgames.game.RequestGameStart>(request);
                wo.setCommand("game_start");
                WebConnector.Instance.request(wo);
            });
    }

    /*
    float defaultTimeScale = 1.0f;
    /// <summary>
    /// 서버로 부터 받은 데이터로 플레이한 경우에 클리어 처리 합니다.
    /// </summary>
    public void Clear()
    {
        // 화면을 잠시 일시정지 상태로 만들어 줍니다.
        defaultTimeScale = Time.timeScale;
        Time.timeScale = 0;

        if (NowGame.rate == 0)
        {
            // 평점 주기 팝업을 띄웁니다.
            PopupRate.show((rate) =>
            {
                Debug.Log("rate = " + rate);
                EndGame(rate);
            });
        }
        else
        {
            EndGame(0);
        }

    }

    void EndGame(int rate)
    {
        rpgames.game.RequestGameEnd request = new rpgames.game.RequestGameEnd();
        {
            request.request = WebConnector.Instance.CommonRequest;
            request.map_idx = NowGame.map_idx;
            request.rate = rate;
            request.clear_sec = GameManager.Instance.PlayTime;
        }
        WebObject wo = new WebObject((w) =>
        {
            rpgames.game.ResponseGameEnd response = w.getResult<rpgames.game.ResponseGameEnd>();
            if (response.response.success)
            {
                NENetworkManager.Instance.Gold = response.gold;

                Time.timeScale = defaultTimeScale;
                GameManager.Instance.exitInGame();
                UICustomMain.show();
            }
            else
            {
                Debug.LogError("game_end Error = " + response.response.error_code.ToString());
            }

        });
        wo.setData<rpgames.game.RequestGameEnd>(request);
        wo.setCommand("game_end");
        WebConnector.Instance.request(wo);
    }
    */


}
