using UnityEngine;
using System.Collections;
using System.Text.RegularExpressions;

public class SpriteTextCustom : Sprite {

	#region MonoBehaviour Methods

	void Awake() {
		mMesh = new Mesh();
		mMeshFilter = gameObject.AddComponent<MeshFilter>();
		gameObject.AddComponent<MeshRenderer>();

        
	}

	void Start () {
		mFont = StaticManager.GetInstanceOf(FontPrefab).GetComponent<FontManager>();
		if (!mFont.IsReady) { DebugUtil.Log("the font is not ready."); }

        Vertical = false;

		UpdateAll();
	}

	/*void Update() {
		if (mInvalidated) {
			UpdateAll();
			mInvalidated = false;
		}
	}*/

	#endregion

	#region Methods

	public override void UpdateAll() {
		if (mFont == null || !mFont.IsReady) {
			return;
		}
		mVertices = new Vector3[Text.Length * 4]; //4
		mUVs = new Vector2[Text.Length * 4];//4
        mColors = new Color[Text.Length * 4];//4
        mTriangles = new int[Text.Length * 6];//6

        Text = Text.Replace(System.Environment.NewLine, "\n");
		SetupBoundary();
		CursorIndex = 0;
		//	アンカーによる位置調整
		float ax = 0, ay = 0;
		if (AnchorX == SpriteAnchor.Middle) {
			ax = mBoundarySize.x / 2;
		} else if (AnchorX == SpriteAnchor.Maximum) {
			ax = mBoundarySize.x;
		}
		if (AnchorY == SpriteAnchor.Middle) {
			ay = mBoundarySize.y / 2;
		} else if (AnchorY == SpriteAnchor.Minimum) {
			ay = mBoundarySize.y;
		}
		//	初期カーソル位置の設定
		if (!Vertical) {
			mInitCursorPos.x = - ax + Offset.x;
			mInitCursorPos.y = ay + Offset.y - CharacterSize.y;
		} else {
			mInitCursorPos.x = ay + Offset.x - CharacterSize.x;
			mInitCursorPos.y = ax + Offset.y - CharacterSize.y;
		}
		mCursorPos.x = mInitCursorPos.x;
		mCursorPos.y = mInitCursorPos.y;
		//	文字ごとの処理
		foreach (char c in Text) {
			OnChar(c);
		}

		mMesh.Clear();
		mMesh.vertices = mVertices;
		mMesh.colors = mColors;
		mMesh.uv = mUVs;
		mMesh.triangles = mTriangles;
		mMeshFilter.sharedMesh = mMesh;
		GetComponent<Renderer>().material = mFont.FontMaterial;
	}

	public void SetColor(Color color) {
		if (mColors == null) { return; }
		int length = mColors.Length;
		for (int i = 0; i < length; ++i) {
			mColors[i] = color;
		}
		mMesh.colors = mColors;
	}

	private void SetupBoundary() {
		int charNum = 0;
		mLineMax = 1;
		mCharMax = 0;
		foreach (char c in Text) {
			if (c == '\n') {
				mLineMax++;
				mCharMax = Mathf.Max(mCharMax, charNum);
				charNum = 0;
			} else {
				charNum++;
			}
		}
		mCharMax = Mathf.Max(mCharMax, charNum);
		mBoundarySize = new Vector2(
			mCharMax * (CharacterSize.x + CharacterSpan.x) - CharacterSpan.x,
			mLineMax * (CharacterSize.y + CharacterSpan.y) - CharacterSpan.y
		);
	}


    /// <summary>
    /// 한글자씩 찍어..내는거야 왜...
    /// </summary>
    /// <param name="c"></param>
	protected void OnChar(char c) {
        int index = CursorIndex * 4; //4;
        int triex = CursorIndex * 6;//6;
		float x = mCursorPos.x;
		float y = mCursorPos.y;

		if (c == '\n') {
			if (!Vertical) {
				mCursorPos.x = mInitCursorPos.x;
				mCursorPos.y = mCursorPos.y - CharacterSize.y - CharacterSpan.y;
			} else {
				mCursorPos.x = mCursorPos.x - CharacterSize.x - CharacterSpan.x;
				mCursorPos.y = mInitCursorPos.y;
			}
			return;
		} 
        else {
            if(FontPrefab.name == "fontKurita") {
                switch(c) {
                    case 't':
                    case 'i':
                        x -= 1;
                        mCursorPos.x -= 1;
                        break;
                     
                    case 'n':
                        //x -= 1;
                        //mCursorPos.x -= 1;
                        break;

                    case '.':
                        x += 3;
                        mCursorPos.x += 3;
                        break;
                }
            }
        }

        /*
        if(c == '\'') {
            Debug.Log("Special Mark!");
        }
        */

		mVertices[index + 0] = new Vector3(x, y, 0);
		mVertices[index + 1] = new Vector3(x + CharacterSize.x, y, 0);
		mVertices[index + 2] = new Vector3(x, y + CharacterSize.y, 0);
		mVertices[index + 3] = new Vector3(x + CharacterSize.x, y + CharacterSize.y, 0);

		Rect rect;
		if (mFont.TryGetRectOf(c, out rect)) {
			if (Vertical && VerticalChar.IndexOf(c) >= 0) {
				mUVs[index + 3] = new Vector2(rect.xMin, rect.yMin);
				mUVs[index + 1] = new Vector2(rect.xMax, rect.yMin);
				mUVs[index + 2] = new Vector2(rect.xMin, rect.yMax);
				mUVs[index + 0] = new Vector2(rect.xMax, rect.yMax);
			}
            else if (Vertical && VerticalCharR90.IndexOf(c) >= 0) {
				mUVs[index + 2] = new Vector2(rect.xMin, rect.yMin);
				mUVs[index + 0] = new Vector2(rect.xMax, rect.yMin);
				mUVs[index + 3] = new Vector2(rect.xMin, rect.yMax);
				mUVs[index + 1] = new Vector2(rect.xMax, rect.yMax);
			}
            else {
				mUVs[index + 0] = new Vector2(rect.xMin, rect.yMin);
				mUVs[index + 1] = new Vector2(rect.xMax, rect.yMin);
				mUVs[index + 2] = new Vector2(rect.xMin, rect.yMax);
				mUVs[index + 3] = new Vector2(rect.xMax, rect.yMax);
			}
		} else {
            // DebugUtil.Log("the character "+ c + " cannot be found.");
            Debug.Log("the character ["+ c.ToString() + "] cannot be found.");
               
       
			mUVs[index + 0] = new Vector2(0, 0);
			mUVs[index + 1] = new Vector2(0, 0);
			mUVs[index + 2] = new Vector2(0, 0);
			mUVs[index + 3] = new Vector2(0, 0);
		}

		for (int i = 0; i < 4; ++i) {
			mColors[index + i] = FontColor;
		}

		mTriangles[triex + 0] = index + 0;
		mTriangles[triex + 1] = index + 2;
		mTriangles[triex + 2] = index + 3;
		mTriangles[triex + 3] = index + 0;
		mTriangles[triex + 4] = index + 3;
		mTriangles[triex + 5] = index + 1;

        
        if (!Vertical) { // 가로형태 

            // 폰트마다 다르게 조정 
            if(FontPrefab.name == "fontKurita") {

                switch (c) {

                    case ' ':
                        mCursorPos.x += CharacterSize.x + CharacterSpan.x - 15;
                        break;

                    case '.':
                        mCursorPos.x += CharacterSize.x + CharacterSpan.x - 15;
                        break;

                    case '\'':
                    case '\"':
                        mCursorPos.x += CharacterSize.x + CharacterSpan.x - 15;
                        // Debug.Log("Special mark in Kurita");
                        break;

                    case 'i':
                    case 'l':
                        mCursorPos.x += CharacterSize.x + CharacterSpan.x - 14;
                        break;


                    case 'r':
                        mCursorPos.x += CharacterSize.x + CharacterSpan.x - 14;
                        break;

                    case 't':
                        mCursorPos.x += CharacterSize.x + CharacterSpan.x - 12;
                        break;

                    default:
                        mCursorPos.x += CharacterSize.x + CharacterSpan.x - 12;
                        break;
                }
            }
            #region fontGrecoStd-DB
            else { // fontGrecoStd-DB


                switch(c) {

                    case '\'':
                    case '\"':
                        mCursorPos.x += CharacterSize.x + CharacterSpan.x - 15;
                        // Debug.Log("Special mark in G");
                        break;

                    case ' ':
                        mCursorPos.x += CharacterSize.x + CharacterSpan.x - 15;
                        break;

                    case 'i':
                    case 'r':
                        mCursorPos.x += CharacterSize.x + CharacterSpan.x - 12;
                        break;

                    case 't':
                        mCursorPos.x += CharacterSize.x + CharacterSpan.x - 13;
                        break;

                    default:
                        mCursorPos.x += CharacterSize.x + CharacterSpan.x - 10;
                        break;
                }

            }
            #endregion
        } // Vertical
        else {
			mCursorPos.y -= CharacterSize.y + CharacterSpan.y;
		}

        CursorIndex++;
    }

    bool IsUpper(string s)
    {
        return Regex.IsMatch(s, @"[^a-z]");
    }
	#endregion

	#region Properties

	public Color Color {
		set { SetColor(value); }
	}

	#endregion

	#region Fields

	public string Text = "";
	public Color FontColor = new Color(1, 1, 1, 1);
	public Vector2 Offset = new Vector2(0, 0);
	public Vector2 CharacterSize = new Vector2(12, 12);
	public Vector2 CharacterSpan = new Vector2(0, 0);
	public GameObject FontPrefab = null;
	public bool Vertical = false;
	public SpriteAnchor AnchorX = SpriteAnchor.Middle;
	public SpriteAnchor AnchorY = SpriteAnchor.Middle;

	private static readonly string VerticalChar = "ー-～~…、.,。→";
	private static readonly string VerticalCharR90 = "「」";

	//private bool mInvalidated = false;
	private int CursorIndex = 0;
	private Vector2 mCursorPos = new Vector2(0, 0);
	private Vector2 mInitCursorPos = new Vector2(0, 0);
	private Vector2 mBoundarySize = new Vector2(0, 0);
	private int mCharMax = 0;
	private int mLineMax = 0;

	private FontManager mFont;
	private MeshFilter mMeshFilter;
	private Mesh mMesh;
	private Vector3[] mVertices;
	private Vector2[] mUVs;
	private int[] mTriangles;
	private Color[] mColors;

	#endregion
}
