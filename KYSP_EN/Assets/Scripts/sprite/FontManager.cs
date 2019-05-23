using UnityEngine;
using System.Collections.Generic;

public class FontManager : MonoBehaviour {

	#region Methods

	public bool TryGetRectOf(char c, out Rect rect) {
		int index = 0;
		rect = new Rect();
		if (!mCharaSet.TryGetValue(c, out index)) {
			return false;
		}
		int px, py;
		py = System.Math.DivRem(index, (int)mCellCount.x, out px);
		rect.xMin = px * mCellTexel.x;
		rect.yMin = 1 - (py + 1) * mCellTexel.y;
		rect.width = mCellTexel.x;
		rect.height = mCellTexel.y;
		return true;
	}

	public bool IsReady {
		get {
			if (FontMaterial == null) { return false; }
			if (FontMaterial.mainTexture == null) { return false; }
			mFontTexture = FontMaterial.mainTexture;
			if (FontIndex == null) { return false; }
			if (mCharaSet == null) {
				mCharaSet = new Dictionary<char, int>();
				int length = FontIndex.text.Length;
				for (int i = 0; i < length; ++i) {
					char c = FontIndex.text[i];
					mCharaSet.Add(c, i);
				}
				mCellCount.x = mFontTexture.width / (int)CellSize.x;
				mCellCount.y = mFontTexture.height / (int)CellSize.y;
				mCellTexel.x = CellSize.x / (float)mFontTexture.width;
				mCellTexel.y = CellSize.y / (float)mFontTexture.height;
			}
			return true;
		}
	}

	#endregion

	#region Fields

	public Material FontMaterial = null;			//	フォント?テリアル
	public TextAsset FontIndex = null;				//	フォント情報
	public Vector2 CellSize = new Vector2(0,0);	//	セルサイズ //12,12

	private Texture mFontTexture = null;			//	フォントテクス?ャ
	private Dictionary<char, int> mCharaSet = null;	//	文字セット
	private Vector2 mCellCount = new Vector2(0, 0);	//	各軸のセルの個数
	private Vector2 mCellTexel = new Vector2(0, 0);	//	１セルのテクセル単位のサイズ

	#endregion
}
