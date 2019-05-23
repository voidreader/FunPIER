public class ConnectConstants
{
	/** 要求チェックコード  */
	public static int REQUEST_CHECK_CODE = 0x1234;
	/** 応答チェックコード  */
	public static int RESPONSE_CHECK_CODE = 0x1234;

	/** 共通通信プロトコルバージョン  */
	public static int CONNECT_PROTOCOL_VERSION = 1;

	/** ログインバージョン */
	public static int LOGIN_PROTCOL_VERSION = 1;
	/** GISバージョン */
	public static int GIS_PROTOCOL_VERSION = 1;
	/** ADバージョン */
	public static int AD_PROTCOL_VERSION = 1;

	/** 要求データ終端コード */
	public static int REQUEST_TERMINATION_CODE = 0x7e;

	/** 応答データ終端コード */
	public static int RESPONSE_TERMINATION_CODE = 0x7e;

	////////////////////////////////////////////////////////
	// クライアントからの要求コマンド

	// 共通
	/** 要求コマンド:起動時通信要求  */
	public static int REQCMD_STARTUP                  = 100;
	/** 要求コマンド:ログイン通信要求  */
	public static int REQCMD_LOGIN                    = 110;

	/** 要求コマンド:GIS取得通信要求  */
	public static int REQCMD_GIS_DATA                 = 200;
	/** 要求コマンド:AD取得通信要求  */
	public static int REQCMD_AD_DATA                  = 210;

	// コンテンツ固有
	/** 各コンテンツが持つ固有の要求コマンドの開始ID */
	public static int REQCMD_CONTENT_START_ID     = 10000;


	////////////////////////////////////////////////////////
	// サーバーからの応答タグ

	/** 応答タグ: サーバーサイドでのエラー */
	public static int RESTAG_SERVER_ERROR             = 100;

	/** 応答タグ: ログイン結果応答 */
	public static int RESTAG_LOGIN_RESULT             = 200;
	/** 応答タグ: 通信トランザクションID */
	public static int RESTAG_CONNECT_TRANSACTION_ID   = 210;
	/** 応答タグ: ユーザー情報 */
	public static int RESTAG_USERINFO                 = 220;
	/** 応答タグ: G-modeサービス情報 */
	public static int RESTAG_GMODE_SERVICE_INFO       = 230;
	/** 応答タグ: サーバ-サイド情報 */
	public static int RESTAG_SERVERSIDE_INFO          = 240;

	/** 応答タグ: GISデータ */
	public static int RESTAG_GIS_DATA                 = 300;
	/** 応答タグ: ADデータ */
	public static int RESTAG_AD_DATA                  = 310;


	/** 各コンテンツが持つ固有の応答タグの開始ID */
	public static int RESTAG_CONTENT_START_ID     = 10000;

	////////////////////////////////////////////////////////
	/** ログイン結果：成功 */
	public static int LOGIN_RESULT_FAILURE = 0;
	/** ログイン結果：失敗 */
	public static int LOGIN_RESULT_SUCCESS = 1;

	////////////////////////////////////////////////////////
	// GIS応答 表示後動作タイプ
	/** 表示後動作タイプ：停止 */
	public static int GIS_NEXT_ACTION_HALT            = 0;
	/** 表示後動作タイプ：継続 */
	public static int GIS_NEXT_ACTION_CONTINUE        = 1;

	// WEBタイプ
	/** WEBタイプ：URL */
	public static int WEB_TYPE_URL                    = 0;
	/** WEBタイプ：HTML */
	public static int WEB_TYPE_HTML                   = 1;
}
