using UnityEngine;
using UnityEngine.Advertisements;

public class UnityAdsExample : MonoBehaviour
{
	
	public void OpenAds()
	{
		int rand = Random.Range( 0, 100 );
		if ( rand<= 30 )
			ShowRewardedAd();
	}
	public void ShowRewardedAd()
	{
		//*
		if ( Advertisement.IsReady( "rewardedVideo" ) )
		{
			var options = new ShowOptions
			{
				resultCallback = HandleShowResult
			};
			Advertisement.Show( "rewardedVideo", options );
		}
		/**/
	}
	//*
	private void HandleShowResult( ShowResult result )
	{
		switch ( result )
		{
		case ShowResult.Finished:	//유저가 동영상을 끝까지 본경우
		break;
		case ShowResult.Skipped:	//유저가 중간에 동영상을 스킵한경우
		break;
		case ShowResult.Failed:		//동영상 보여주기 실패한경우
		break;
		}
	}
	/**/

}