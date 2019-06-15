using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class TAB_HISTORY : TAB_Base
{
    bool IsAddLoading = false;

    public override void init()
    {
        if (!NENetworkManager.Instance.IsLogin)
            return;

        base.init();

        m_Scroll.m_scrollableArea.OnScroll += OnScroll;
    }

    public override void LoadAll(bool IsReset)
    {
        base.LoadAll(IsReset);

        Debug.Log("LoadAll Sort = " + Sort.ToString());

        rpgames.game.RequestMapHistory request = new rpgames.game.RequestMapHistory();
        {
            request.request = WebConnector.Instance.CommonRequest;
            request.page = Page;
            request.sort = rpgames.game.RequestMapHistory.Sort.CreateDate;
            request.sort_direction = rpgames.game.RequestMapHistory.SortDirection.Desc;
        }
        WebObject wo = new WebObject((w) =>
        {
            rpgames.game.ResponseMapHistory response = w.getResult<rpgames.game.ResponseMapHistory>();
            if (response.response.success)
            {
                Total_Page = response.total_page;

                List<rpgames.game.Map> list = response.map;
                List<object> scroll_list = new List<object>();
                for (int i = 0; i < list.Count; i++)
                {
                    rpgames.game.Map map = list[i];

                    float DiffcultyProgress = 0;
                    float ScoreProgress = 0;
                    if (map.clear_count > 0)
                        DiffcultyProgress = (float)((double)map.clear_count / (double)map.play_count);
                    if (map.rate_count > 0)
                        ScoreProgress = (float)((double)map.rate / (double)map.rate_count) / 50.0f;

                    bool is_first_play = map.is_first_play;
                    // 내가 만든맵이면 first_play를 강제로 false로 만든다.
                    if (map.owner.Equals(NENetworkManager.Instance.UserID))
                        is_first_play = false;
                    ItemMapIcon.ItemData icon = new ItemMapIcon.ItemData(-1,
                        map.idx,
                        map.insert_date.ToString(),
                        map.map_name,
                        map.owner,
                        map.nickname,
                        DiffcultyProgress,
                        ScoreProgress,
                        map.play_count,
                        map.clear_count,
                        map.best_score.user_id,
                        map.best_score.clear_sec,
                        map.best_score.death_count,
                        is_first_play, false);
                    scroll_list.Add(icon);
                }
                m_Scroll.Init(scroll_list);
                IsAddLoading = false;
            }
            else
            {
                Debug.LogError("map_history Error = " + response.response.error_code.ToString());
            }

        });
        wo.setData<rpgames.game.RequestMapHistory>(request);
        wo.setCommand("map_history");
        //wo.setParam("debug", "1");
        WebConnector.Instance.request(wo);
    }

    void OnScroll(tk2dUIScrollableArea scroll)
    {
        //Debug.Log("scroll = " + scroll.Value);
        // 스크롤 마지막을 체크하여 추가 페이지가 있으면 더 로딩합니다.
        if (!IsAddLoading && scroll.Value >= 1.0f && Page < Total_Page)
        {
            IsAddLoading = true;
            Page++;
            LoadAll(false);
        }
    }

    protected override void OnBtnPlay()
    {
        base.OnBtnPlay();

        ItemMapIcon.ItemData icon = getData();
        if (icon.IsFirstPlay)
        {
            int cost = int.Parse(RPGSheetManager.Instance.getSheetData("data.bin", "cost_download").ToString());
            MessageBox box = MessageBox.show();
            box.setMessage(DefineMessage.getMsg(30010, cost.ToString("N0")));
            box.addYesButton((b) =>
            {
                b.Close();
                if (CheckGold(cost))
                    ServerDataManager.Instance.playCustomGame(icon.ServerIndex);
            });
            box.addNoButton();
            box.addCloseButton();
        }
        else
        {
            MessageBox box = MessageBox.show();
            box.setMessage(DefineMessage.getMsg(30009));
            box.addYesButton((b) =>
            {
                b.Close();
                ServerDataManager.Instance.playCustomGame(icon.ServerIndex);
            });
            box.addNoButton();
            box.addCloseButton();
        }
    }



}
