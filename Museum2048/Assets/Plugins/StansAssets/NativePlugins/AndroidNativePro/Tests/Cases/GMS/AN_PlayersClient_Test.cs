using UnityEngine;
using System;
using SA.Android.Utilities;
using SA.Foundation.Tests;

using SA.Android.GMS.Games;

namespace SA.Android.Tests.GMS
{
    public class AN_PlayersClient_Test : SA_BaseTest
    {

        public override void Test() {
            AN_PlayersClient client = AN_Games.GetPlayersClient();
            client.GetCurrentPlayer((result) => {
                if (result.IsSucceeded) {
                    AN_Player player = result.Data;
                    //Printing player info:
                    AN_Logger.Log("player.Id: " + player.PlayerId);
                    AN_Logger.Log("player.Title: " + player.Title);
                    AN_Logger.Log("player.DisplayName: " + player.DisplayName);
                    AN_Logger.Log("player.HiResImageUri: " + player.HiResImageUri);
                    AN_Logger.Log("player.IconImageUri: " + player.IconImageUri);
                    AN_Logger.Log("player.HasIconImage: " + player.HasIconImage);
                    AN_Logger.Log("player.HasHiResImage: " + player.HasHiResImage);
                }

                SetAPIResult(result);
            });

        }
    }
}