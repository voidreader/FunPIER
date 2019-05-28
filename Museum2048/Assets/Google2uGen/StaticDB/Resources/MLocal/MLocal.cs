//----------------------------------------------
//    Google2u: Google Doc Unity integration
//         Copyright © 2015 Litteratus
//
//        This file has been auto-generated
//              Do not manually edit
//----------------------------------------------

using UnityEngine;
using System.Globalization;

namespace Google2u
{
	[System.Serializable]
	public class MLocalRow : IGoogle2uRow
	{
		public string _Korean;
		public string _English;
		public MLocalRow(string __ID, string __Korean, string __English) 
		{
			_Korean = __Korean.Trim();
			_English = __English.Trim();
		}

		public int Length { get { return 2; } }

		public string this[int i]
		{
		    get
		    {
		        return GetStringDataByIndex(i);
		    }
		}

		public string GetStringDataByIndex( int index )
		{
			string ret = System.String.Empty;
			switch( index )
			{
				case 0:
					ret = _Korean.ToString();
					break;
				case 1:
					ret = _English.ToString();
					break;
			}

			return ret;
		}

		public string GetStringData( string colID )
		{
			var ret = System.String.Empty;
			switch( colID )
			{
				case "Korean":
					ret = _Korean.ToString();
					break;
				case "English":
					ret = _English.ToString();
					break;
			}

			return ret;
		}
		public override string ToString()
		{
			string ret = System.String.Empty;
			ret += "{" + "Korean" + " : " + _Korean.ToString() + "} ";
			ret += "{" + "English" + " : " + _English.ToString() + "} ";
			return ret;
		}
	}
	public sealed class MLocal : IGoogle2uDB
	{
		public enum rowIds {
			TEXT1, TEXT2, TEXT3, TEXT4, TEXT5, TEXT6, TEXT7, TEXT8, TEXT9, TEXT10, TEXT11, TEXT12, TEXT13, TEXT14, TEXT15, TEXT16, TEXT17, TEXT18
			, TEXT19, TEXT20, TEXT21, TEXT22, TEXT23, TEXT24, TEXT25, TEXT26, TEXT27, TEXT28, TEXT29, TEXT30, TEXT31, TEXT32, TEXT33, TEXT34, TEXT35, TEXT36, TEXT37, TEXT38
			, TEXT39, TEXT40, TEXT41, TEXT42, TEXT43, TEXT44, TEXT45, TEXT46, TEXT47, TEXT48, TEXT49, TEXT50, TEXT51, TEXT52, TEXT53, TEXT54, TEXT55, TEXT56, TEXT57, TEXT58
			, TEXT59, TEXT60, TEXT61, TEXT62, TEXT63
		};
		public string [] rowNames = {
			"TEXT1", "TEXT2", "TEXT3", "TEXT4", "TEXT5", "TEXT6", "TEXT7", "TEXT8", "TEXT9", "TEXT10", "TEXT11", "TEXT12", "TEXT13", "TEXT14", "TEXT15", "TEXT16", "TEXT17", "TEXT18"
			, "TEXT19", "TEXT20", "TEXT21", "TEXT22", "TEXT23", "TEXT24", "TEXT25", "TEXT26", "TEXT27", "TEXT28", "TEXT29", "TEXT30", "TEXT31", "TEXT32", "TEXT33", "TEXT34", "TEXT35", "TEXT36", "TEXT37", "TEXT38"
			, "TEXT39", "TEXT40", "TEXT41", "TEXT42", "TEXT43", "TEXT44", "TEXT45", "TEXT46", "TEXT47", "TEXT48", "TEXT49", "TEXT50", "TEXT51", "TEXT52", "TEXT53", "TEXT54", "TEXT55", "TEXT56", "TEXT57", "TEXT58"
			, "TEXT59", "TEXT60", "TEXT61", "TEXT62", "TEXT63"
		};
		public System.Collections.Generic.List<MLocalRow> Rows = new System.Collections.Generic.List<MLocalRow>();

		public static MLocal Instance
		{
			get { return NestedMLocal.instance; }
		}

		private class NestedMLocal
		{
			static NestedMLocal() { }
			internal static readonly MLocal instance = new MLocal();
		}

		private MLocal()
		{
			Rows.Add( new MLocalRow("TEXT1", "자동차 박물관", "Automobile Museum"));
			Rows.Add( new MLocalRow("TEXT2", "와인 박물관", "Wine Museum"));
			Rows.Add( new MLocalRow("TEXT3", "바이킹 박물관", "Viking Museum"));
			Rows.Add( new MLocalRow("TEXT4", "상점", "Shop"));
			Rows.Add( new MLocalRow("TEXT5", "광고없애기", "No Ads"));
			Rows.Add( new MLocalRow("TEXT6", "되돌리기", "UNDO"));
			Rows.Add( new MLocalRow("TEXT7", "업그레이더", "UPGRADER"));
			Rows.Add( new MLocalRow("TEXT8", "클리너", "CLEANER"));
			Rows.Add( new MLocalRow("TEXT9", "특별한 패키지", "Special Package"));
			Rows.Add( new MLocalRow("TEXT10", "순위", "Rank"));
			Rows.Add( new MLocalRow("TEXT11", "효과음 ON", "SFX ON"));
			Rows.Add( new MLocalRow("TEXT12", "효과음 OFF", "SFX OFF"));
			Rows.Add( new MLocalRow("TEXT13", "배경음악 ON", "BGM ON"));
			Rows.Add( new MLocalRow("TEXT14", "배경음악 OFF", "BGM OFF"));
			Rows.Add( new MLocalRow("TEXT15", "언어 설정", "LANGUAGE"));
			Rows.Add( new MLocalRow("TEXT16", "도움말(튜토리얼)", "Help(Tutorial)"));
			Rows.Add( new MLocalRow("TEXT17", "설정", "Option"));
			Rows.Add( new MLocalRow("TEXT18", "취소", "Cancel"));
			Rows.Add( new MLocalRow("TEXT19", "축하합니다!", "Congratulation!"));
			Rows.Add( new MLocalRow("TEXT20", "박물관이 더욱 다채로워졌어요!", "Your museum has become more valuable!"));
			Rows.Add( new MLocalRow("TEXT21", "다시하기", "Retry"));
			Rows.Add( new MLocalRow("TEXT22", "게임 끝내기", "Finish"));
			Rows.Add( new MLocalRow("TEXT23", "별점주기", "Rate me"));
			Rows.Add( new MLocalRow("TEXT24", "『Midnight in Museum』을 즐기고 계신가요?", "Are you enjoying 『Midnight in Museum』?"));
			Rows.Add( new MLocalRow("TEXT25", "움직일 수 있는 블록이 없어요!", "No more exhibits to move."));
			Rows.Add( new MLocalRow("TEXT26", "아이템을 사용하여 게임을 계속하거나\n새로운 게임을 시작하세요.", "Use items to make higher exhibits or\nFinish this game."));
			Rows.Add( new MLocalRow("TEXT27", "Game Over!", "Game Over!"));
			Rows.Add( new MLocalRow("TEXT28", "당신의 박물관은 더 발전할 수 있습니다!", "The museum is not yet finished!"));
			Rows.Add( new MLocalRow("TEXT29", "9단계 미만의 전시품을 1단계 업그레이드 해줍니다.", "UPGRADER will upgrade selected exhibit under Lv9."));
			Rows.Add( new MLocalRow("TEXT30", "1,2단계의 모든 전시품을 제거합니다.", "CLEANER will remove all Lv1 and Lv2 exhibits."));
			Rows.Add( new MLocalRow("TEXT31", "아이템을 사용하시겠습니까?", "Would you like to use this item?"));
			Rows.Add( new MLocalRow("TEXT32", "모든 전시품의 상태를\n마지막 행동 이전으로 되돌립니다.", "UNDO will return the status of all exhibits\nto before the last action."));
			Rows.Add( new MLocalRow("TEXT33", "게임을 중단하고\n메인화면으로 돌아갈까요?", "Would you like to end this game?"));
			Rows.Add( new MLocalRow("TEXT34", "박물관에 필요한 모든 전시품을\n획득했어요!", "You\'ve acquired every exhibits\nof this museum!\n\nChallenge the next museum!"));
			Rows.Add( new MLocalRow("TEXT35", "전시물", "Exhibits"));
			Rows.Add( new MLocalRow("TEXT36", "더 크고 멋진 전시품을 모아보세요!", "Collect bigger and better exhibits"));
			Rows.Add( new MLocalRow("TEXT37", "", ""));
			Rows.Add( new MLocalRow("TEXT38", "깜짝 패키지", "Surprise Pack"));
			Rows.Add( new MLocalRow("TEXT39", "광고\n없애기", "No\nAds"));
			Rows.Add( new MLocalRow("TEXT40", "창을 닫으면 오늘은 더 이상 구매할 수 없습니다.", "If you close the window, \nyou can no longer purchase today."));
			Rows.Add( new MLocalRow("TEXT41", "광고를 보고 아이템을 받겠습니까?\n랜덤한 아이템을 최대 3개까지 획득할 수 있습니다.", "Would you like to watch video ads?\nUp to three random items will be given."));
			Rows.Add( new MLocalRow("TEXT42", "레드문이 떠올랐습니다", "The red moon has appeared."));
			Rows.Add( new MLocalRow("TEXT43", "광고를 보고 모든 블록을 \n정렬 하시겠습니까?", "Would you like to watch video ads\nand \'sort\' exhibits?\n\nSeriously, It will be ver" +
    "y helpful"));
			Rows.Add( new MLocalRow("TEXT44", "광고를 보고 {0}단계 전시품을 획득하고\n게임을 시작 하시겠습니까?", "Would you like to start\nwith a Level{0} exhibit?"));
			Rows.Add( new MLocalRow("TEXT45", "배너와 전면광고가 모두 제거 되었습니다.", "All banners are removed."));
			Rows.Add( new MLocalRow("TEXT46", "{0}개의 전시품을 더 획득 할 수 있어요!", "You can acquire {0} more top exhibits."));
			Rows.Add( new MLocalRow("TEXT47", "멋진 게임을 친구에게 공유하고\n아이템을 받으시겠어요?", "Would you like to share this cool game with\nyour friends and get useful items?"));
			Rows.Add( new MLocalRow("TEXT48", "축하합니다!\n{0} {1}개를 획득하였습니다.", "Congratulation!\nReceived {1} {0}(s)."));
			Rows.Add( new MLocalRow("TEXT49", "레드문이 떠오르면\n광고를 시청하고 유용한 아이템을\n받을 수 있습니다", "When the red moon is appeared, \nYou can watch video ads\nand get useful items."));
			Rows.Add( new MLocalRow("TEXT50", "같은 모양의 전시품을 합쳐서\n더 높은 레벨의 전시품을 만듭니다", "Swipe to move all the exhibits.\nMerge same exhibits to upgrade.\n\nThe higher the l" +
    "evels, \nthe more beautiful the museum."));
			Rows.Add( new MLocalRow("TEXT51", "튜토리얼", "How to play"));
			Rows.Add( new MLocalRow("TEXT52", "최소 Lv3 전시품이 하나 이상 존재해야 합니다", "At least one Lv3 exhibit must be present."));
			Rows.Add( new MLocalRow("TEXT53", "개인정보 보호정책", "Privacy Policy"));
			Rows.Add( new MLocalRow("TEXT54", "이미 구입했습니다.", "Purchase has been made."));
			Rows.Add( new MLocalRow("TEXT55", "상품을 구입 하였습니다.", "Purchae has been completed."));
			Rows.Add( new MLocalRow("TEXT56", "블록 정렬하기", "Align exhibits"));
			Rows.Add( new MLocalRow("TEXT57", "게임 계속하기", "Conniue game"));
			Rows.Add( new MLocalRow("TEXT58", "결제 서비스를 초기화하고 있습니다.", "Initializing payment service."));
			Rows.Add( new MLocalRow("TEXT59", "구매 복원", "Restore Purchase"));
			Rows.Add( new MLocalRow("TEXT60", "구매 복원이 완료되었습니다.", "Restore process is completed"));
			Rows.Add( new MLocalRow("TEXT61", "게임을 종료 하시겠습니까?", "Exit Game?"));
			Rows.Add( new MLocalRow("TEXT62", "겨울 박물관은 바이킹 박물관 완성 후\n플레이 할 수 있습니다.", "You need to complete \n\'Viking Museum\' first."));
			Rows.Add( new MLocalRow("TEXT63", "겨울 박물관", "Winter Museum"));
		}
		public IGoogle2uRow GetGenRow(string in_RowString)
		{
			IGoogle2uRow ret = null;
			try
			{
				ret = Rows[(int)System.Enum.Parse(typeof(rowIds), in_RowString)];
			}
			catch(System.ArgumentException) {
				Debug.LogError( in_RowString + " is not a member of the rowIds enumeration.");
			}
			return ret;
		}
		public IGoogle2uRow GetGenRow(rowIds in_RowID)
		{
			IGoogle2uRow ret = null;
			try
			{
				ret = Rows[(int)in_RowID];
			}
			catch( System.Collections.Generic.KeyNotFoundException ex )
			{
				Debug.LogError( in_RowID + " not found: " + ex.Message );
			}
			return ret;
		}
		public MLocalRow GetRow(rowIds in_RowID)
		{
			MLocalRow ret = null;
			try
			{
				ret = Rows[(int)in_RowID];
			}
			catch( System.Collections.Generic.KeyNotFoundException ex )
			{
				Debug.LogError( in_RowID + " not found: " + ex.Message );
			}
			return ret;
		}
		public MLocalRow GetRow(string in_RowString)
		{
			MLocalRow ret = null;
			try
			{
				ret = Rows[(int)System.Enum.Parse(typeof(rowIds), in_RowString)];
			}
			catch(System.ArgumentException) {
				Debug.LogError( in_RowString + " is not a member of the rowIds enumeration.");
			}
			return ret;
		}

	}

}
