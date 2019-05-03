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
			, TEXT39, TEXT40, TEXT41
		};
		public string [] rowNames = {
			"TEXT1", "TEXT2", "TEXT3", "TEXT4", "TEXT5", "TEXT6", "TEXT7", "TEXT8", "TEXT9", "TEXT10", "TEXT11", "TEXT12", "TEXT13", "TEXT14", "TEXT15", "TEXT16", "TEXT17", "TEXT18"
			, "TEXT19", "TEXT20", "TEXT21", "TEXT22", "TEXT23", "TEXT24", "TEXT25", "TEXT26", "TEXT27", "TEXT28", "TEXT29", "TEXT30", "TEXT31", "TEXT32", "TEXT33", "TEXT34", "TEXT35", "TEXT36", "TEXT37", "TEXT38"
			, "TEXT39", "TEXT40", "TEXT41"
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
			Rows.Add( new MLocalRow("TEXT1", "자동차 박물관", "Car Museum"));
			Rows.Add( new MLocalRow("TEXT2", "와인 박물관", "Wine Museum"));
			Rows.Add( new MLocalRow("TEXT3", "바이킹 박물관", "Viking Museum"));
			Rows.Add( new MLocalRow("TEXT4", "상점", "Shop"));
			Rows.Add( new MLocalRow("TEXT5", "광고없애기", "No Ads"));
			Rows.Add( new MLocalRow("TEXT6", "되돌리기", "UNDO"));
			Rows.Add( new MLocalRow("TEXT7", "업그레이드", "UPGRADER"));
			Rows.Add( new MLocalRow("TEXT8", "클리너", "CLEANER"));
			Rows.Add( new MLocalRow("TEXT9", "특별한 패키지", "Special Package"));
			Rows.Add( new MLocalRow("TEXT10", "순위", "Rank"));
			Rows.Add( new MLocalRow("TEXT11", "효과음 켜기", "SFX ON"));
			Rows.Add( new MLocalRow("TEXT12", "효과음 끄기", "SFX OFF"));
			Rows.Add( new MLocalRow("TEXT13", "배경음악 켜기", "BGM ON"));
			Rows.Add( new MLocalRow("TEXT14", "배경음악 끄기", "BGM OFF"));
			Rows.Add( new MLocalRow("TEXT15", "언어 설정", "LANGUAGE"));
			Rows.Add( new MLocalRow("TEXT16", "도움말(튜토리얼)", "Help(Tutorial)"));
			Rows.Add( new MLocalRow("TEXT17", "설정", "Option"));
			Rows.Add( new MLocalRow("TEXT18", "취소", "Cancel"));
			Rows.Add( new MLocalRow("TEXT19", "축하합니다!", ""));
			Rows.Add( new MLocalRow("TEXT20", "박물관이 더욱 다채로워졌어요!", ""));
			Rows.Add( new MLocalRow("TEXT21", "다시하기", ""));
			Rows.Add( new MLocalRow("TEXT22", "나가기", ""));
			Rows.Add( new MLocalRow("TEXT23", "별점주기", ""));
			Rows.Add( new MLocalRow("TEXT24", "『Midnight in Museum』을 즐기고 계신가요?", ""));
			Rows.Add( new MLocalRow("TEXT25", "움직일 수 있는 블록이 없어요!", ""));
			Rows.Add( new MLocalRow("TEXT26", "아이템을 사용하여 게임을 계속하거나\n슬라이드 하여 새로운 게임을 시작하세요", ""));
			Rows.Add( new MLocalRow("TEXT27", "Game Over!", "Game Over!"));
			Rows.Add( new MLocalRow("TEXT28", "당신의 박물관은 더 발전할 수 있습니다!", ""));
			Rows.Add( new MLocalRow("TEXT29", "9단계 미만의 오브젝트를 1단계 업그레이드 해줍니다.", ""));
			Rows.Add( new MLocalRow("TEXT30", "1,2단계의 모든 오브젝트를 제거합니다.", ""));
			Rows.Add( new MLocalRow("TEXT31", "아이템을 사용하시겠습니까?", ""));
			Rows.Add( new MLocalRow("TEXT32", "모든 블록의 상태를 마지막 행동 이전으로 되돌립니다.", ""));
			Rows.Add( new MLocalRow("TEXT33", "게임을 중단하고\n메인화면으로 돌아갈까요?", ""));
			Rows.Add( new MLocalRow("TEXT34", "박물관에 필요한 모든 오브젝트를\n획득했어요!", ""));
			Rows.Add( new MLocalRow("TEXT35", "전시물", ""));
			Rows.Add( new MLocalRow("TEXT36", "더 크고 멋진 전시품을 모아보세요!", ""));
			Rows.Add( new MLocalRow("TEXT37", "모든 전시품을 획득하였습니다!\n다른 박물관 꾸미기에 도전하세요", ""));
			Rows.Add( new MLocalRow("TEXT38", "깜짝 패키지", ""));
			Rows.Add( new MLocalRow("TEXT39", "광고\n없애기", ""));
			Rows.Add( new MLocalRow("TEXT40", "창을 닫으면 오늘은 더 이상 구매할 수 없습니다.", ""));
			Rows.Add( new MLocalRow("TEXT41", "레드문이 떠올랐습니다.\n광고를 보고 아이템을 받겠습니까?\n\n랜덤한 아이템을 최대 3개까지 획득할 수 있습니다.", ""));
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
