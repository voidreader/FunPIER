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

	public Material FontMaterial = null;			//	�t�H���g?�e���A��
	public TextAsset FontIndex = null;				//	�t�H���g���
	public Vector2 CellSize = new Vector2(0,0);	//	�Z���T�C�Y //12,12

	private Texture mFontTexture = null;			//	�t�H���g�e�N�X?��
	private Dictionary<char, int> mCharaSet = null;	//	�����Z�b�g
	private Vector2 mCellCount = new Vector2(0, 0);	//	�e���̃Z���̌�
	private Vector2 mCellTexel = new Vector2(0, 0);	//	�P�Z���̃e�N�Z���P�ʂ̃T�C�Y

	#endregion
}
