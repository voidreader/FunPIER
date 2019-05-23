using UnityEngine;
using System.Collections;
using System;
using System.IO;
using System.Text;
using System.Xml;

public class Banner : MonoBehaviour
{
	// 位置
	public static int POSITION_TOP = 0;
	public static int POSITION_BOTTOM = 1;
	// 上記の数値をInspectorから指定してください
	public int position = POSITION_TOP;
	
	// 幅
	private int width;
	// 高さ
	private int height;
	
	// 通信先URL。変更する場合はInspectorから指定してください
	public string connectUrl = "https://ssl.sp.g-mode.jp/gm/ad_entry";
	// G-modeフルコンテンツコード。Inspectorから指定してください
	public string gmodeCodeIos = "";
	public string gmodeCodeAndroid = "";
	
	// 通信で取得する画像のURL
	private string srcUrl;
	// リンク先URL。通信で取得した場合上書き
	// デフォルトのURLを変更する場合はInspectorから指定してください
	public string linkUrl = "http://www.g-mode.co.jp/";
	
	private int xmlGetType;
	private string xmlUrl;
	private string[] xmlData;
	
	// アニメーション状態
	public static int HIDDEN = 0;
	public static int ANIMATION_OPENING = 1;
	public static int ANIMATION_CLOSING = 2;
	public static int SHOWN = 3;
	// アニメーションにかかるフレーム数。変更する場合はInspectorから指定してください
	public int ANIMATION_FRAME = 60;
	// 現在のアニメーション状態
	private int animationState;
	// アニメーション用カウンタ
	private int animationCount;
	// Y軸移動量
	private float animationMoveHeight;
	
	// 準備完了フラグ
	private bool ready;
	
	// CRC32算出用テーブル
	private uint[] crc_table = new uint[256];
	
	// CRC32算出用テーブルの初期化
	private void InitCRC32()
	{
		uint c;
		
		for (int i = 0; i < 256; i++)
		{
			c = (uint) i;
			for (int j = 0; j < 8; j++)
			{
				if ((c & 0x1) != 0)
				{
					c = 0xEDB88320 ^ (c >> 1);
				}
				else
				{
					c = c >> 1;
				}
			}
			crc_table[i] = c;
		}
	}
	
	// CRC32の取得
	private uint GetCRC32(byte[] buffer)
	{
		long result = 0xFFFFFFFF;
		int length =buffer.Length;
		for(int i = 0; i < length; i++)
		{
			result = (result >> 8) ^ crc_table[(buffer[i] ^ result) & 0xFF];
			result &= 0xFFFFFFFF;
		}
		return (uint)(result ^ 0xFFFFFFFF);
	}
	
	// 初期化
	void Start ()
	{
		// 縦横を比較して短い方を幅として使用
		width = (Screen.width > Screen.height) ? Screen.height : Screen.width;
		height = (width * 50) / 320;
		
		// アニメーション時の移動量を算出
		animationMoveHeight = (float)height / Screen.height;
		
		if (position == POSITION_TOP)
		{
			// 座標の基準点を上部に設定
			GetComponent<GUITexture>().pixelInset = new Rect(-width / 2, -height, width, height); 
		}
		else
		{
			// 座標の基準点を下部に設定
			GetComponent<GUITexture>().pixelInset = new Rect(-width / 2, 0, width, height); 
		}
		
		// 初期状態では隠す
		GetComponent<GUITexture>().color = new Color(0.5f, 0.5f, 0.5f, 0.0f);
		animationState = HIDDEN;
		
		// CRC32算出用テーブルの初期化
		InitCRC32();
		
		// 準備の初期化
		ready = false;
		
		StartCoroutine(Connect());
	}
	
	// 更新処理
	void Update ()
	{
		// 出現アニメーション時
		if (animationState == ANIMATION_OPENING)
		{
			// アニメーション用カウンタを進める
			animationCount++;
			// 全体フレーム数から割合を求める
			float currentFrame = (float)animationCount / ANIMATION_FRAME;
			
			// アルファ値を徐々に増加させる
			GetComponent<GUITexture>().color = new Color(0.5f, 0.5f, 0.5f, 0.5f * currentFrame);
			
			if (position == POSITION_TOP)
			{
				// カウンタの二乗に比例して徐々に座標を下ろす
				transform.position = new Vector3(0.5f, 1.0f + animationMoveHeight * (1.0f - (currentFrame * currentFrame)), 0.0f);
			}
			else
			{
				// カウンタの二乗に比例して徐々に座標を上げる
				transform.position = new Vector3(0.5f, 0.0f - animationMoveHeight * (1.0f - (currentFrame * currentFrame)), 0.0f);
			}
			
			// アニメーションが完了した
			if (animationCount == ANIMATION_FRAME)
			{
				// アニメーション状態を表示中に変更
				animationState = SHOWN;
			}
		}
		// 退場アニメーション時
		else if (animationState == ANIMATION_CLOSING)
		{
			// アニメーション用カウンタを進める
			animationCount++;
			// 全体フレーム数から逆順で割合を求める
			float currentFrame = (float)(ANIMATION_FRAME - animationCount) / ANIMATION_FRAME;
			
			// アルファ値を徐々に減少させる
			GetComponent<GUITexture>().color = new Color(0.5f, 0.5f, 0.5f, 0.5f * currentFrame);
			
			if (position == POSITION_TOP)
			{
				// カウンタの二乗に比例して徐々に座標を上げる
				transform.position = new Vector3(0.5f, 1.0f + animationMoveHeight * ((1.0f - currentFrame) * (1.0f - currentFrame)), 0.0f);
			}
			else
			{
				// カウンタの二乗に比例して徐々に座標を下ろす
				transform.position = new Vector3(0.5f, 0.0f - animationMoveHeight * ((1.0f - currentFrame) * (1.0f - currentFrame)), 0.0f);
			}
			
			// アニメーションが完了した
			if (animationCount == ANIMATION_FRAME)
			{
				// アニメーション状態を非表示に変更
				animationState = HIDDEN;
			}
		}
		
		// タッチされた場合
		if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
		{
			// タッチされた座標を取得
			Vector3 pos = Input.GetTouch(0).position;
			// 座標がテクスチャ内か判定
			if (GetComponent<GUITexture>().HitTest(pos))
			{
				// ブラウザを起動させる
				Application.OpenURL(linkUrl);
			}
		}
		
		// 動作確認用　クリック時
		if (Input.GetMouseButtonDown(0))
		{
			// クリックされた座標を取得
			Vector3 pos = Input.mousePosition;
			// 座標がテクスチャ内か判定
			if (GetComponent<GUITexture>().HitTest(pos))
			{
				// ブラウザを起動させる
				Application.OpenURL(linkUrl);
			}
		}
	}
	
	// 通信
	private IEnumerator Connect()
	{
		// GIS通信
		WWW www = new WWW(connectUrl, WriteRequest());
		yield return www;
		
		if (www.error != null)
		{
			print(www.error);
			xmlUrl = null;
			xmlData = null;
		}
		else
		{
			// 応答データの解析
			if (!ReadResponse(www.bytes))
			{
				xmlUrl = null;
				xmlData = null;
			}
		}
		
		string useXmlData = null;
		if (xmlUrl != null || xmlData != null)
		{
			// XMLデータのURLが送られてきた場合
			if (xmlGetType == ConnectConstants.WEB_TYPE_URL)
			{
				// 通信でXMLデータを取得
				WWW xmlWww = new WWW(xmlUrl);
				yield return xmlWww;
				
				if (xmlWww.error != null)
				{
					print(xmlWww.error);
					useXmlData = null;
				}
				else
				{
					useXmlData = xmlWww.text;
				}
			}
			// XMLデータ自体が送られてきた場合
			else
			{
				useXmlData = xmlData[0];
			}
		}
		
		if (useXmlData != null)
		{
			// XMLデータの解析
			ParseXML(useXmlData);
		}
		
		// 画像データのURLが送られてきた場合
		if (srcUrl != null)
		{
			// URLを指定し通信開始
			WWW srcWww = new WWW(srcUrl);
			// 通信が完了するまで待つ
			yield return srcWww;
			
			if (srcWww.error != null)
			{
				print(srcWww.error);
			}
			else
			{
				// 取得したテクスチャの設定
				GetComponent<GUITexture>().texture = srcWww.texture;
			}
		}
		
		// 初期化完了
		ready = true;
		
		// バナー表示
		Open();
	}
	
	// リクエストデータの書き出し
	private byte[] WriteRequest()
	{
		MemoryStream crcOutputStream = new MemoryStream(1024);
		BigEndianWriter crcWriter = new BigEndianWriter(crcOutputStream);
		
		try
		{
			// ヘッダーの出力
			crcWriter.WriteInt(ConnectConstants.CONNECT_PROTOCOL_VERSION);
			crcWriter.WriteInt(ConnectConstants.GIS_PROTOCOL_VERSION);
			crcWriter.WriteString("UTF-8");	// encode
			crcWriter.WriteString(Application.platform == RuntimePlatform.IPhonePlayer ? gmodeCodeIos : gmodeCodeAndroid);
			crcWriter.WriteString("");	// seriesCode
			crcWriter.WriteInt(1);	// prgverMajor
			crcWriter.WriteInt(0);	// prgverMinor
			crcWriter.WriteString("");	// justiceCode
			crcWriter.WriteString("");	// userID
			crcWriter.WriteString("");	// loginKey
			crcWriter.WriteLong(0);	// transactionID
			crcWriter.WriteString("ja");	// langCode
			crcWriter.WriteString("");	// deviceID
			crcWriter.WriteInt(0);	// deviceInfoMap.size()

			// 要求コマンドIDの出力
			crcWriter.WriteInt(ConnectConstants.REQCMD_AD_DATA);

			// 要求コマンドの出力
			crcWriter.WriteInt(ConnectConstants.AD_PROTCOL_VERSION);
		}
		catch (Exception)
		{
			return null;
		}
		finally
		{
			crcWriter.Close();
			crcOutputStream.Close();
		}
		
		byte[] crcData = crcOutputStream.ToArray();
		
		long crcValue = (long) GetCRC32(crcData);
		int crcDataSize = crcData.Length;
		
		MemoryStream outputStream = new MemoryStream(1024);
		BigEndianWriter writer = new BigEndianWriter(outputStream);
		
		try
		{
			// チェックコード
			writer.WriteInt(ConnectConstants.REQUEST_CHECK_CODE);

			// CRC
			writer.WriteLong(crcValue);

			// CRCデータサイズ
			writer.WriteInt(crcDataSize);

			// CRDデータ
			writer.Write(crcData);

			// 終端コード
			writer.WriteInt(ConnectConstants.REQUEST_TERMINATION_CODE);
		}
		catch (Exception)
		{
			return null;
		}
		finally
		{
			writer.Close();
			outputStream.Close();
		}
		
		return outputStream.ToArray();
	}
	
	// レスポンスデータの読み込み
	private bool ReadResponse(byte[] response)
	{
		MemoryStream inputStream = new MemoryStream(response);
		BigEndianReader reader = new BigEndianReader(inputStream);
		byte[] crcData = null;
		try
		{
			// チェックコードの確認
			int checkCode = reader.ReadInt();
			if (checkCode != ConnectConstants.RESPONSE_CHECK_CODE)
			{
				return false;
			}

			// CRC値
			long crcValue = reader.ReadLong();
			// CRCデータサイズ
			int crcDataSize = reader.ReadInt();

			// データボディ
			crcData = new byte[crcDataSize];
			reader.Read(crcData, 0, crcDataSize);

			// 終端コード
			int terminationCode = reader.ReadInt();
			if (terminationCode != ConnectConstants.RESPONSE_TERMINATION_CODE)
			{
				return false;
			}

			// CRCデータ比較
			if (crcValue != GetCRC32(crcData))
			{
				return false;
			}
		}
		catch (Exception)
		{
			return false;
		}
		finally
		{	
			reader.Close();
			inputStream.Close();
		}
		
		MemoryStream crcInputStream = new MemoryStream(crcData);
		BigEndianReader crcReader = new BigEndianReader(crcInputStream);
		
		try
		{
			// データブロック数
			int dataNum = crcReader.ReadInt();
			for (int i = 0; i < dataNum; i++)
			{
				// データブロックサイズ
				int size = crcReader.ReadInt();
				// データブロックタグ
				int tag = crcReader.ReadInt();

				// AD応答
				if (tag == ConnectConstants.RESTAG_AD_DATA)
				{
					// シーン数
					int sceneNum = crcReader.ReadInt();
					for (int j = 0; j < sceneNum; j++)
					{
						// シーンキー
						crcReader.ReadString();
						// WEBタイトル
						crcReader.ReadString();
						// WEBタイプ
						xmlGetType = crcReader.ReadInt();
						// URL　WEBタイプが0=URLの場合のみ使用する
						xmlUrl = crcReader.ReadString();
						// MIMEタイプ　WEBタイプが1=HTMLの場合のみ使用する
						crcReader.ReadString();
						// エンコード　WEBタイプが1=HTMLの場合のみ使用する
						crcReader.ReadString();
						// HTMLデータ数　WEBタイプが1=HTMLの場合のみ使用する
						int dataSize = crcReader.ReadInt();
						if (dataSize > 0)
						{
							// HTMLデータ　WEBタイプが1=HTMLの場合のみ使用する
							xmlData = new string[dataSize];
							for (int k = 0; k < dataSize; k++)
							{
								xmlData[k] = crcReader.ReadString();
							}
						}
					}
				}
				// それ以外の応答は読み捨てる
				else
				{
					byte[] temp = new byte[size];
					crcReader.Read(temp, 0, size);
				}
			}
		}
		catch (Exception)
		{
			return false;
		}
		finally
		{
			crcReader.Close();
			crcInputStream.Close();
		}
		
		return true;
	}
	
	// XMLデータの解析
	private void ParseXML(string xml)
	{
		try
		{
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.LoadXml(xml);
			
			// <link>タグの検索
			XmlNodeList nodeList = xmlDocument.SelectNodes("//link");
			
			ArrayList linkUrlArrayList = new ArrayList();
			ArrayList linkSrcArrayList = new ArrayList();
			int linkCount = 0;
			
			for (int i = 0; i < nodeList.Count; i++)
			{
				// リンクの取得
				XmlNode urlNode = nodeList[i].Attributes["url"];
				XmlNode srcNode = nodeList[i].Attributes["src"];
				
				if (urlNode != null && srcNode != null)
				{
					linkUrlArrayList.Add(urlNode.InnerText);
					linkSrcArrayList.Add(srcNode.InnerText);
					linkCount++;
				}
			}
			
			if (linkCount > 0)
			{
				System.Random rand = new System.Random();
				int linkIndex = rand.Next(linkCount);
				
				linkUrl = (string) linkUrlArrayList[linkIndex];
				srcUrl = (string) linkSrcArrayList[linkIndex];
			}
			else
			{
				srcUrl = null;
			}
		}
		catch (Exception)
		{
			srcUrl = null;
		}
	}
	
	// バナーを表示させる
	public void Open()
	{
		// 初期化が完了していなければリターン
		if (!ready) return;
		// 表示中やアニメーション中ならばリターン
		if (animationState != HIDDEN) return;
		
		// アニメーション用カウンタのリセット
		animationCount = 0;
		// 出現アニメーションの開始
		animationState = ANIMATION_OPENING;
	}
	
	// バナーを退場させる
	public void Close()
	{
		// 初期化が完了していなければリターン
		if (!ready) return;
		// 非表示中やアニメーション中ならばリターン
		if (animationState != SHOWN) return;
		
		// アニメーション用カウンタのリセット
		animationCount = 0;
		// 退場アニメーションの開始
		animationState = ANIMATION_CLOSING;
	}
	
	// 初期化フラグの取得
	public bool IsReady()
	{
		return ready;
	}
	
	// アニメーション状態の取得
	public int GetAnimationState()
	{
		return animationState;
	}
	
	// ビッグエンディアンでのストリーム読み込み
	public class BigEndianReader
	{
		private Stream stream;
		private bool isLittleEndian;
		private UTF8Encoding utf8 = new UTF8Encoding();
		
		public BigEndianReader(Stream inputStream)
		{
			stream = inputStream;
			isLittleEndian = BitConverter.IsLittleEndian;
		}
		
		public void Read(byte[] buffer)
		{
			Read(buffer, 0, buffer.Length);
		}
		
		public void Read(byte[] buffer, int offset, int length)
		{
			stream.Read(buffer, offset, length);
		}
		
		public short ReadShort()
		{
			return BitConverter.ToInt16(ReadBytesInternal(2), 0);
		}
		
		public int ReadInt()
		{
			return BitConverter.ToInt32(ReadBytesInternal(4), 0);
		}
		
		public long ReadLong()
		{
			return BitConverter.ToInt64(ReadBytesInternal(8), 0);
		}
		
		public string ReadString()
		{
			int length = (int) ReadShort();
			byte[] buffer = new byte[length];
			Read(buffer, 0, length);
			return utf8.GetString(buffer, 0, length);
		}
		
		public void Close()
		{
			stream = null;
		}
		
		private byte[] ReadBytesInternal(int count)
		{
			byte[] buffer = new byte[count];
			Read(buffer, 0, count);
			
			if (isLittleEndian)
			{
				Array.Reverse(buffer);
			}
			
			return buffer;
		}
	}
	
	// ビッグエンディアンでのストリーム書き込み
	public class BigEndianWriter
	{
		private Stream stream;
		private bool isLittleEndian;
		private UTF8Encoding utf8 = new UTF8Encoding();
		
		public BigEndianWriter(Stream outputStream)
		{
			stream = outputStream;
			isLittleEndian = BitConverter.IsLittleEndian;
		}
		
		public void Write(byte[] buffer)
		{
			Write(buffer, 0, buffer.Length);
		}
		
		public void Write(byte[] buffer, int offset, int length)
		{
			stream.Write(buffer, offset, length);
		}
		
		public void WriteShort(short val)
		{
			WriteBytesInternal(BitConverter.GetBytes(val));
		}
		
		public void WriteInt(int val)
		{
			WriteBytesInternal(BitConverter.GetBytes(val));
		}
		
		public void WriteLong(long val)
		{
			WriteBytesInternal(BitConverter.GetBytes(val));
		}
		
		public void WriteString(string str)
		{
			short length = (short)str.Length;
			WriteShort(length);
			Write(utf8.GetBytes(str), 0, length);
		}
		
		public void Close()
		{
			stream = null;
		}
		
		private void WriteBytesInternal(byte[] buffer)
		{
			if (isLittleEndian)
			{
				Array.Reverse(buffer);
			}
			
			Write(buffer);
		}
	}
}
