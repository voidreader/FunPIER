using UnityEngine;
using System.Collections;

public class KySceneTweet : KyScene {

	#region Inner Classes

	#endregion

	#region MonoBehaviour Methods

	void Start () {
		UserDataLoad();
		KyUtil.AlignTop(KyUtil.FindChild(gameObject, "navi"), 0);
		//Transform navi = transform.Find("rect");
		//Vector3 pos = navi.localPosition;
		//pos.y = Camera.main.orthographicSize;
		//navi.localPosition = pos;
		//navi = transform.Find("title");
		//pos.y -= 45;
		//navi.localPosition = pos;
		KyUtil.SetText(gameObject, "navi/label", KyText.GetText(20013));
		KyUtil.AlignBottom(KyUtil.FindChild(gameObject, "message"), 45);
		//pos.y = (0-Camera.main.orthographicSize) + 45;
		//navi = transform.Find("message");
		//navi.localPosition = pos;
		KyUtil.SetText(gameObject, "message", "");

		Communicating = KyUtil.FindChild(gameObject,"message");
		dispCommunicating(false);
		KySoftKeys.Instance.SetLabels(KySoftKeys.Label.None, KySoftKeys.Label.None);
		mState.ChangeState(StateTest);
	}

	public override void Update() {
		if (mState != null) {
			mState.Execute();
		}
		CommunicatingUpdate();
	}

	void OnGUI() {
		if ((Application.platform != RuntimePlatform.IPhonePlayer)&&(Application.platform != RuntimePlatform.Android)) {
			// PC上でのテスト用
			float ofsx = (100);
			float ofsy = (20);
			Rect rect = new Rect(ofsx,ofsy,230,60);
		
			input_str = GUI.TextField(rect, input_str);
		}
	}
	

	#endregion
	
	private	void UserDataLoad(){
		TWITTER_USER_ID				= KySaveData.PrefsData.TwitterID;
        TWITTER_USER_SCREEN_NAME	= KySaveData.PrefsData.TwitterName;
        TWITTER_USER_TOKEN			= KySaveData.PrefsData.TwitterToken;
        TWITTER_USER_TOKEN_SECRET	= KySaveData.PrefsData.TwitterTokenSecret;
        
        TWITTER_USER_TOKEN_SIZE = TWITTER_USER_TOKEN.Length;
        TWITTER_USER_TOKEN_SECRET_SIZE = TWITTER_USER_TOKEN_SECRET.Length;
	}
	
	private void UserDataSave(){
		KySaveData.PrefsData.TwitterID = TWITTER_USER_ID;
		KySaveData.PrefsData.TwitterName = TWITTER_USER_SCREEN_NAME;
		KySaveData.PrefsData.TwitterToken = TWITTER_USER_TOKEN;
		KySaveData.PrefsData.TwitterTokenSecret = TWITTER_USER_TOKEN_SECRET;
		KySaveData.Instance.Save(KySaveData.DataKind.Preferences);     	
	}		

	private void dispCommunicating(bool disp){
		if(disp){
//			KyUtil.SetText(gameObject, "message", "通信中");
			KyUtil.SetText(gameObject, "message", KyText.GetText(24040));
			CommunicatingAlpha = 1.0f;
		}else{
			KyUtil.SetText(gameObject, "message", "");
		}
		CommunicatingFlag = disp;
	}

	private void CommunicatingUpdate(){
		if( CommunicatingFlag ){
			if( Communicating!=null ){
				float alpha = Mathf.Sin(CommunicatingAlpha);
				CommunicatingAlpha += (Mathf.PI / 80.0f);
				Color col = new Color(0.0f,0.0f,0.0f,(alpha/3.0f)+0.66f);
				SpriteUtil.SetVerticesColor( Communicating, col);
			}else{
				print("Communicating NULL?");
			}
		}
	}

	#region State Methods

	private	int	StateTest(){
		if(mState.Sequence == 0){
			if((gobj == null)&&(TestPrefab != null)){
				gobj = (GameObject)GameObject.Instantiate(TestPrefab);
				gobj.transform.parent = transform;
				KyUtil.SetText(gobj, "message", "てすと" );
				KyUtil.SetText(gobj, "btnYes", KyText.GetText(22000)); // はい
				KyUtil.SetText(gobj, "btnNo", KyText.GetText(22001)); //いいえ

				ScreenFader.Main.FadeIn();
				mState.Sequence ++;
			}else{
				mState.ChangeState(StateCheckAcount);
				mState.Sequence=0;
				
			}
		}
		return 0;
	}

	
	/// <summary>
	/// ツイッターアカウント情報確認
	/// </summary>
	private	int	StateCheckAcount(){
		if(mState.Sequence == 0){
			mState.Sequence ++;
		}else	
		if(mState.Sequence == 1){
	        if(!CheckTwitterUserInfo()){
				// アカウント無し PINコード取得処理
				mState.ChangeState(StateUrlJump);
				mState.Sequence=0;
	        }else{
	        	// アカウント有り
				if( string.IsNullOrEmpty( tweetMessage ) ){
					tweetMessage = KyText.GetText(23000)+":"+Time.time;
				}

				mState.ChangeState(StateTweet);
				mState.Sequence=0;
	        }
			ScreenFader.Main.FadeIn();
		}
		return 0;
	}

	/// <summary>
	/// ブラウザ遷移確認
	/// </summary>
	private	int	StateUrlJump(){
		if(mState.Sequence == 0){
			if((gobj == null)&&(AuthConfirmPrefab != null)){
				gobj = (GameObject)GameObject.Instantiate(AuthConfirmPrefab);
				gobj.transform.parent = transform;

//				KyUtil.SetText(gobj, "message", "アプリの認承を行います。\nよろしいですか？");
				if(!pinError){
					KyUtil.SetText(gobj, "message", KyText.GetText(24000) );
					KyUtil.SetText(gobj, "btnYes", KyText.GetText(22000)); // はい
					KyUtil.SetText(gobj, "btnNo", KyText.GetText(22001)); //いいえ
				}else{
					KyUtil.SetText(gobj, "message", KyText.GetText(24011) );
					KyUtil.SetText(gobj, "btnYes", KyText.GetText(22002)); // OK
					KyUtil.SetText(gobj, "btnNo", KyText.GetText(22003)); //キャンセル
				}
//				KyUtil.SetText(gobj, "btnYes", KyText.GetText(22000)); // はい
//				KyUtil.SetText(gobj, "btnNo", KyText.GetText(22001)); //いいえ

				GuiButton ButtonYes = KyUtil.GetComponentInChild<GuiButton>(gobj, "btnYes");
				GuiButton ButtonNo = KyUtil.GetComponentInChild<GuiButton>(gobj, "btnNo");
				ButtonYes.GuiEnabled = false;
				ButtonNo.GuiEnabled = false;
				ButtonYes.ButtonSelected += StateUrlJumpYes;
				ButtonNo.ButtonSelected += StateUrlJumpNo;

//				ScreenFader.Main.FadeIn();
				mState.Sequence ++;
			}else{
				GetRequestToken();
				mState.ChangeState(StatePinInput);
				mState.Sequence=0;
			}	
		}else	
		if(mState.Sequence == 1){
			if (!ScreenFader.Main.FadeRunning) {
				GuiButton ButtonYes = KyUtil.GetComponentInChild<GuiButton>(gobj, "btnYes");
				GuiButton ButtonNo = KyUtil.GetComponentInChild<GuiButton>(gobj, "btnNo");
				ButtonYes.GuiEnabled = true;
				ButtonNo.GuiEnabled = true;
				mState.Sequence ++;
			}
		}else	
		if(mState.Sequence == 2){
		}else	
		if(mState.Sequence == 3){
			GetRequestToken();
			pinError = false;
			dispCommunicating(true);
			mState.Sequence ++;
		}else
		if (mState.Sequence == 5) {
//			ScreenFader.Main.FadeOut();
			mState.Sequence ++;
		}else
		if (mState.Sequence == 6) {
			if (!ScreenFader.Main.FadeRunning) {
				mState.ChangeState(StatePinInput);
				mState.Sequence=0;
				Destroy( gobj );
				gobj = null;

				if( string.IsNullOrEmpty( tweetMessage ) ){
					tweetMessage = KyText.GetText(23000)+":"+Time.time;
				}
			}
		}else
		if( mState.Sequence == 98 ){
			Destroy( gobj );
			gobj = null;
			mState.ChangeState(StateError);
			mState.Sequence=0;
		}else
		if( mState.Sequence == 99 ){
			GuiButton ButtonYes = KyUtil.GetComponentInChild<GuiButton>(gobj, "btnYes");
			GuiButton ButtonNo = KyUtil.GetComponentInChild<GuiButton>(gobj, "btnNo");
			ButtonYes.GuiEnabled = false;
			ButtonNo.GuiEnabled = false;
			mState.ChangeState(StateLeave);
			mState.Sequence=0;
		}
		return 0;
	}

	private int StateUrlJumpYes(object sender) {
		mState.Sequence = 3;
		return 0;
	}
	private int StateUrlJumpNo(object sender) {
		mState.Sequence = 99;
		return 0;
	}

	/// <summary>
	/// PIN 入力処理
	/// </summary>
	private int StatePinInput() {
		if(mState.Sequence == 0){
			if((gobj == null)&&(PinCodeInputPrefab != null)){
				gobj = (GameObject)GameObject.Instantiate(PinCodeInputPrefab);
				gobj.transform.parent = transform;
				
				input_str = "";

				KyUtil.SetText(gobj, "message", KyText.GetText(24010) );
				KyUtil.SetText(gobj, "pinerr", "" );
				KyUtil.SetText(gobj, "pincode", "" );
				KyUtil.SetText(gobj, "btnYes", KyText.GetText(22002)); // はい
				KyUtil.SetText(gobj, "btnNo", KyText.GetText(22003)); //いいえ

				GuiButton ButtonYes = KyUtil.GetComponentInChild<GuiButton>(gobj, "btnYes");
				GuiButton ButtonNo = KyUtil.GetComponentInChild<GuiButton>(gobj, "btnNo");
				ButtonYes.GuiEnabled = false;
				ButtonNo.GuiEnabled = false;
				ButtonYes.ButtonSelected += StatePinInputYes;
				ButtonNo.ButtonSelected += StatePinInputNo;

				GuiButton pinwaku = KyUtil.GetComponentInChild<GuiButton>(gobj, "pinwaku");
				pinwaku.GuiEnabled = false;
				pinwaku.ButtonSelected += StatePinWaku;

				dispCommunicating(false);

//				ScreenFader.Main.FadeIn();
				mState.Sequence ++;
			}else{
				mState.ChangeState(StateLeave);
				mState.Sequence=0;
			}	
		}else
		if(mState.Sequence == 1){
			if (!ScreenFader.Main.FadeRunning) {
				GuiButton ButtonYes = KyUtil.GetComponentInChild<GuiButton>(gobj, "btnYes");
				GuiButton ButtonNo = KyUtil.GetComponentInChild<GuiButton>(gobj, "btnNo");
				ButtonYes.GuiEnabled = true;
				ButtonNo.GuiEnabled = true;
				keyboard = TouchScreenKeyboard.Open("",TouchScreenKeyboardType.NumberPad);
				mState.Sequence ++;
			}
		}else
		if(mState.Sequence == 2){
			if(keyboard.active){
				input_str = keyboard.text;
				if(keyboard.done){
					keyboard.active = false;
				}
			}else{
				GuiButton pinwaku = KyUtil.GetComponentInChild<GuiButton>(gobj, "pinwaku");
				if(!pinwaku.GuiEnabled){
					pinwaku.ChangeState( GuiButton.ButtonState.Up );
					pinwaku.GuiEnabled = true;
				}
			}
			if(input_str.Length > 7){
				input_str = input_str.Substring(0,7);
				keyboard.text = input_str;
			}
			KyUtil.SetText(gobj, "pincode", input_str );
		}else
		if(mState.Sequence == 3){
			mState.Sequence ++;
			GetAccessToken( input_str );
			dispCommunicating(true);
			GuiButton pinwaku = KyUtil.GetComponentInChild<GuiButton>(gobj, "pinwaku");
			pinwaku.GuiEnabled = false;
		}else
		if(mState.Sequence == 4){
		}else
		if(mState.Sequence == 5){
			UserDataSave();

//			ScreenFader.Main.FadeOut();
			mState.Sequence ++;
		}else
		if (mState.Sequence == 6) {
			if (!ScreenFader.Main.FadeRunning) {
				mState.ChangeState(StateTweet);
				mState.Sequence=0;
				Destroy( gobj );
				gobj = null;
			}
		}else
		if( mState.Sequence == 90 ){
			pinError = true;
//			ScreenFader.Main.FadeOut();
			mState.Sequence++;
		}else
		if (mState.Sequence == 91) {
			if (!ScreenFader.Main.FadeRunning) {
				mState.ChangeState(StateUrlJump);
				mState.Sequence=0;
				Destroy( gobj );
				gobj = null;
			}
		}else
		if( mState.Sequence == 98 ){
			Destroy( gobj );
			gobj = null;
			mState.ChangeState(StateError);
			mState.Sequence=0;
		}else
		if( mState.Sequence == 99 ){
			mState.ChangeState(StateLeave);
			mState.Sequence=0;
		}
		
		return 0;
	}
	
	private int StatePinInputYes(object sender) {
		if(mState.Sequence == 2){
			mState.Sequence = 3;
		}else{
			mState.Sequence = 92;
		}
		return 0;
	}
	private int StatePinInputNo(object sender) {
		mState.Sequence = 99;
		return 0;
	}

	private int StatePinWaku(object sender) {
		keyboard = TouchScreenKeyboard.Open(input_str,TouchScreenKeyboardType.NumberPad);
		GuiButton pinwaku = KyUtil.GetComponentInChild<GuiButton>(gobj, "pinwaku");
		pinwaku.GuiEnabled = false;
		print("PinWaku Push");
		return 0;
	}


	/// <summary>
	/// Tweet 投稿処理
	/// </summary>
	private int StateTweet() {
		if (mState.Sequence == 0) {
			if((gobj == null)&&(PostConfirmPrefab != null)){
				gobj = (GameObject)GameObject.Instantiate(PostConfirmPrefab);
				gobj.transform.parent = transform;


				string	editstr = tweetMessage;
				tweetMsgDisp = "";
				while( editstr.Length > TweetLFCount ){
					tweetMsgDisp = tweetMsgDisp + editstr.Substring(0,TweetLFCount) + "\n";
					editstr = editstr.Substring(TweetLFCount,editstr.Length - TweetLFCount);
				}
				tweetMsgDisp = tweetMsgDisp + editstr;

				KyUtil.SetText(gobj, "message", KyText.GetText(24020) );
				KyUtil.SetText(gobj, "userid", TWITTER_USER_SCREEN_NAME );
				KyUtil.SetText(gobj, "tweet", tweetMsgDisp );
				KyUtil.SetText(gobj, "btnYes", KyText.GetText(22000)); // はい
				KyUtil.SetText(gobj, "btnNo", KyText.GetText(22001)); //いいえ
				KyUtil.SetText(gobj, "btnOther", KyText.GetText(24021)); //

				GuiButton ButtonYes = KyUtil.GetComponentInChild<GuiButton>(gobj, "btnYes");
				GuiButton ButtonNo = KyUtil.GetComponentInChild<GuiButton>(gobj, "btnNo");
				GuiButton ButtonOth = KyUtil.GetComponentInChild<GuiButton>(gobj, "btnOther");
				ButtonYes.GuiEnabled = false;
				ButtonNo.GuiEnabled = false;
				ButtonOth.GuiEnabled = false;
				ButtonYes.ButtonSelected += StateTweetYes;
				ButtonNo.ButtonSelected += StateTweetNo;
				ButtonOth.ButtonSelected += StateTweetOther;

//				ScreenFader.Main.FadeIn();

				mState.Sequence ++;
			}else{
				mState.ChangeState(StateLeave);
				mState.Sequence=0;
			}
		}else
		if(mState.Sequence == 1){
			if (!ScreenFader.Main.FadeRunning) {
				GuiButton ButtonYes = KyUtil.GetComponentInChild<GuiButton>(gobj, "btnYes");
				GuiButton ButtonNo = KyUtil.GetComponentInChild<GuiButton>(gobj, "btnNo");
				GuiButton ButtonOth = KyUtil.GetComponentInChild<GuiButton>(gobj, "btnOther");
				ButtonYes.GuiEnabled = true;
				ButtonNo.GuiEnabled = true;
				ButtonOth.GuiEnabled = true;
				mState.Sequence ++;
			}
		}else
		if(mState.Sequence == 2){
		}else
		if(mState.Sequence == 3){
			PostTweet( tweetMessage );
			dispCommunicating(true);
			GuiButton ButtonOth = KyUtil.GetComponentInChild<GuiButton>(gobj, "btnOther");
			ButtonOth.GuiEnabled = false;
			mState.Sequence ++;
		}else
		if(mState.Sequence == 4){
		}else
		if(mState.Sequence == 5){
//			ScreenFader.Main.FadeOut();
			mState.Sequence ++;
		}else
		if (mState.Sequence == 6) {
			if (!ScreenFader.Main.FadeRunning) {
				mState.ChangeState(StateTweetEnd);
				mState.Sequence=0;
				Destroy( gobj );
				gobj = null;
			}
		}else
		if( mState.Sequence == 60 ){
//			ScreenFader.Main.FadeOut();
			mState.Sequence ++;
		}else
		if (mState.Sequence == 61) {
			if (!ScreenFader.Main.FadeRunning) {
				mState.ChangeState(StateUrlJump);
				mState.Sequence=0;
				Destroy( gobj );
				gobj = null;
			}
		}else
		if( mState.Sequence == 98 ){
			Destroy( gobj );
			gobj = null;
			mState.ChangeState(StateError);
			mState.Sequence=0;
		}else
		if( mState.Sequence == 99 ){
			mState.ChangeState(StateLeave);
			mState.Sequence=0;
		}

		return 0;
	}
	private int StateTweetYes(object sender) {
		mState.Sequence = 3;
		return 0;
	}
	private int StateTweetNo(object sender) {
		mState.Sequence = 99;
		return 0;
	}
	private int StateTweetOther(object sender) {
		mState.Sequence = 60;
		return 0;
	}

	/// <summary>
	/// 投稿終了処理
	/// </summary>
	private int StateTweetEnd() {
		if (mState.Sequence == 0) {
			if((gobj == null)&&(PostEndPrefab != null)){
				gobj = (GameObject)GameObject.Instantiate(PostEndPrefab);
				gobj.transform.parent = transform;

//				KyUtil.SetText(gobj, "message", "投稿を完了しました。" );
				KyUtil.SetText(gobj, "message", KyText.GetText(24030) );
				KyUtil.SetText(gobj, "btnYes", KyText.GetText(22002)); // はい

				GuiButton ButtonYes = KyUtil.GetComponentInChild<GuiButton>(gobj, "btnYes");
				ButtonYes.GuiEnabled = false;
				ButtonYes.ButtonSelected += StateTweetEndYes;

//				ScreenFader.Main.FadeIn();
				mState.Sequence ++;
			}else{
				mState.ChangeState(StateLeave);
				mState.Sequence=0;
			}
		}else
		if(mState.Sequence == 1){
			if (!ScreenFader.Main.FadeRunning) {
				GuiButton ButtonYes = KyUtil.GetComponentInChild<GuiButton>(gobj, "btnYes");
				ButtonYes.GuiEnabled = true;
				mState.Sequence ++;
			}
		}else
		if( mState.Sequence == 99 ){
			print("tweet exit");
			mState.ChangeState(StateLeave);
			mState.Sequence=0;
		}
		return 0;
	}
	private int StateTweetEndYes(object sender) {
		mState.Sequence = 99;
		return 0;
	}


	/// <summary>
	/// シーン終了
	/// </summary>
	private int StateLeave() {
		if (mState.Sequence == 0) {
			// とりあえずタイトルにでも戻す
			ScreenFader.Main.FadeOut();
			mState.Sequence ++;
		}else
		if (mState.Sequence == 1) {
			if (!ScreenFader.Main.FadeRunning) {
				if( NextScene == null ){
					ChangeScene("KySceneTitle");
				}else{
					ChangeScene(NextScene);
				}			
				mState.Sequence ++;
			}
		}
		return 0;
	}

	/// <summary>
	/// エラー表示処理
	/// </summary>
	private int StateError() {
		if (mState.Sequence == 0) {
			if((gobj == null)&&(ErrorPrefab != null)){
				gobj = (GameObject)GameObject.Instantiate(PostEndPrefab);
				gobj.transform.parent = transform;
				
				if(errCode == -1){
					KyUtil.SetText(gobj, "message", KyText.GetText(24100) );
				}else{
					KyUtil.SetText(gobj, "message", KyText.GetText(24101) );
				}
				KyUtil.SetText(gobj, "btnYes", KyText.GetText(22002)); // はい

				GuiButton ButtonYes = KyUtil.GetComponentInChild<GuiButton>(gobj, "btnYes");
				ButtonYes.GuiEnabled = false;
				ButtonYes.ButtonSelected += StateErrorYes;

//				ScreenFader.Main.FadeIn();
				mState.Sequence ++;
			}else{
				mState.ChangeState(StateLeave);
				mState.Sequence=0;
			}
		}else
		if(mState.Sequence == 1){
			if (!ScreenFader.Main.FadeRunning) {
				GuiButton ButtonYes = KyUtil.GetComponentInChild<GuiButton>(gobj, "btnYes");
				ButtonYes.GuiEnabled = true;
				mState.Sequence ++;
			}
		}else
		if( mState.Sequence == 99 ){
			print("tweet exit");
			mState.ChangeState(StateLeave);
			mState.Sequence=0;
		}
		return 0;
	}
	private int StateErrorYes(object sender) {
		mState.Sequence = 99;
		return 0;
	}


	#endregion




	#region Twitter Methods

	// ユーザ認証情報チェック
	public	bool	CheckTwitterUserInfo(){
		m_AccessTokenResponse = new Twitter.AccessTokenResponse();

		m_AccessTokenResponse.UserId = TWITTER_USER_ID;
		m_AccessTokenResponse.ScreenName = TWITTER_USER_SCREEN_NAME;
		m_AccessTokenResponse.Token = TWITTER_USER_TOKEN;
		m_AccessTokenResponse.TokenSecret = TWITTER_USER_TOKEN_SECRET;
	
		if( !string.IsNullOrEmpty(m_AccessTokenResponse.Token) &&
			!string.IsNullOrEmpty(m_AccessTokenResponse.ScreenName) &&
			!string.IsNullOrEmpty(m_AccessTokenResponse.Token) &&
			!string.IsNullOrEmpty(m_AccessTokenResponse.TokenSecret) ) {
			return true;
		}
		return false;
	}

	// ユーザ認証要求
	public	void	GetRequestToken(){
		StartCoroutine(	Twitter.API.GetRequestToken(CONSUMER_KEY,CONSUMER_SECRET,
						new Twitter.RequestTokenCallback(this.OnRequestTokenCallback)));
	}
	
	// ユーザ認証コールバック
	void	OnRequestTokenCallback(bool success,Twitter.RequestTokenResponse response ,string err,int errcode){
		if(success){
			m_RequestTokenResponse=response;
			Twitter.API.OpenAuthorizationPage(response.Token);
			if(( mState.State == StateUrlJump ) &&
			   ( mState.Sequence == 4 )){
				mState.Sequence++;
			}
		}else{
			dispCommunicating(false);
			
			print("OnRequestTokenCallback-failed.");
			errStr = "RequestToken\n" + err;
			errCode = errcode;
			if(( mState.State == StateUrlJump ) &&
			   ( mState.Sequence == 4 )){
				mState.Sequence=98;
			}
		}
	}
	
	
	
	// PINコード送信アクセストークン取得
	public	void	GetAccessToken(string m_PIN){
		print("pin code = "+m_PIN);
		StartCoroutine(	Twitter.API.GetAccessToken(CONSUMER_KEY,CONSUMER_SECRET,
						m_RequestTokenResponse.Token,m_PIN,
						new Twitter.AccessTokenCallback(this.OnAccessTokenCallback)));
	}

	// PINコード送信アクセストークン取得コールバック
	void	OnAccessTokenCallback(bool success,Twitter.AccessTokenResponse response ,string err,int errcode){
		dispCommunicating(false);
		
		if(success){

			m_AccessTokenResponse=response;

			TWITTER_USER_ID				=m_AccessTokenResponse.UserId;
			TWITTER_USER_SCREEN_NAME	=m_AccessTokenResponse.ScreenName;
			TWITTER_USER_TOKEN			=m_AccessTokenResponse.Token;
			TWITTER_USER_TOKEN_SECRET	=m_AccessTokenResponse.TokenSecret;

	        TWITTER_USER_TOKEN_SIZE = TWITTER_USER_TOKEN.Length;
    	    TWITTER_USER_TOKEN_SECRET_SIZE = TWITTER_USER_TOKEN_SECRET.Length;

			print("AcT OK State="+mState.Sequence);
			if(( mState.State == StatePinInput ) &&
			   ( mState.Sequence == 4 )){
				mState.Sequence++;
			}
		}else{
			print("OnAccessTokenCallback-failed.");
			errCode = errcode;
			errStr = "AccessToken\n" + err;
			if(( mState.State == StatePinInput ) &&
			   ( mState.Sequence == 4 )){
			   	if( errCode == -2 ){
					mState.Sequence=90;
			   	}else{
					mState.Sequence=98;
			   	}
			}
		}
	}
	
	// ツイート
	public	void PostTweet(string m_Tweet){
		StartCoroutine(	Twitter.API.PostTweet(m_Tweet,CONSUMER_KEY,CONSUMER_SECRET,m_AccessTokenResponse,
						new Twitter.PostTweetCallback(this.OnPostTweet)));
	}

	// ツイート コールバック
	void	OnPostTweet(bool success ,string err,int errcode){
		dispCommunicating(false);
		if(success){
			if(( mState.State == StateTweet ) &&
			   ( mState.Sequence == 4 )){
				mState.Sequence++;
			}
		}else{
			errCode = errcode;
			errStr = "PostTweet\n" + err;
			if( errcode!= -5){
				if(( mState.State == StateTweet ) &&
				   ( mState.Sequence == 4 )){
					mState.Sequence=98;
				}
			}else{
				if(( mState.State == StateTweet ) &&
				   ( mState.Sequence == 4 )){
					mState.Sequence++;
				}
			}
		}
		print("OnPostTweet-"+(success?"succedded.":"failed."));
	}

	#endregion
	

	#region Fields

	public	GameObject TestPrefab = null;

	public	GameObject AuthConfirmPrefab = null;
	public	GameObject PinCodeInputPrefab = null;
	public	GameObject PostConfirmPrefab = null;
	public	GameObject PostEndPrefab = null;
	public	GameObject ErrorPrefab = null;

	private	GameObject	gobj=null;

	private GameObject			Communicating = null;
	private bool				CommunicatingFlag = false;
	private float				CommunicatingAlpha = 0.0f;

	private KyStateManager mState = new KyStateManager();	//	ステート管理

	private TouchScreenKeyboard		keyboard=null;
	
	private	bool			pinError = false;
	
	private	string			input_str = "";
	private	string			tweetMsgDisp = "";
	
	// Twitterへアプリの登録でもらうID類
	// ( http://twitter.com/apps/new )
	/* テスト用 Consumer_key & secret
	  NydicpuIXg4Zq7tVVNQwQ
	  3fsC5fCVuXs0dUJZRcvymWkG1G11BWQukA88zBkMHU
	*/
	public	string	CONSUMER_KEY;
	public	string	CONSUMER_SECRET;
	
	
	// 認証後手に入れるユーザ情報
	private	string	TWITTER_USER_ID=null;
	private	string	TWITTER_USER_SCREEN_NAME=null;
	private	string	TWITTER_USER_TOKEN=null;
	private	string	TWITTER_USER_TOKEN_SECRET=null;

	private	int		TWITTER_USER_TOKEN_SIZE = 0;
	private	int		TWITTER_USER_TOKEN_SECRET_SIZE = 0;

	// リクエスト・トークン
	private	Twitter.RequestTokenResponse	m_RequestTokenResponse;
	// アクセス・トークン
	private	Twitter.AccessTokenResponse		m_AccessTokenResponse;
	

	private GuiButton mButtonYes = null;	//	YESボタン
	private GuiButton mButtonNo = null;		//	NOボタン
	private GuiButton mButtonOther = null;		//	Otherボタン
	private GameObject mDialogBox = null;	//	ダイアログボックス
	
	public	string	errStr = null;
	public	int		errCode = 0;
	
	public	int		TweetLFCount = 20;
	
	public	string		tweetMessage =""; // Tweet する文言
	public	string		NextScene=null;	// 終了後に移行するシーン
	

	
	#endregion

}
