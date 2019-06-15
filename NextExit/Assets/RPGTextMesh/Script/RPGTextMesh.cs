/// <summary>
/// Custom font. Author Cesar Rios 2013
/// Modified By Seok-Yeon Yoon 2016
/// </summary>

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using RPG.Text;

[RequireComponent (typeof (MeshRenderer))]
[RequireComponent (typeof (MeshFilter))]
[ExecuteInEditMode]
public class RPGTextMesh : MonoBehaviour
{
    public class CustomFontParsing
    {
        public Color ColorTop;
        public Color ColorBottom;
        public FontStyle Style = FontStyle.Normal;
        public char Char;
        public CustomFontParsing(char c, Color colorTop, Color colorBottom, FontStyle style)
        {
            Char = c;
            ColorTop = colorTop;
            ColorBottom = colorBottom;
            Style = style;
        }
    }
    
	public enum TEXT_ANCHOR 
	{
		UpperLeft,
		UpperRight,
		UpperCenter,
		MiddleLeft,
		MiddleRight,
		MiddleCenter,
		LowerLeft,
		LowerRight,
		LowerCenter
	}
	
	public enum TEXT_ALIGNMENT
	{
		left,
		right,
		center
	}

	public enum OUTLINE_QUALITY
	{
		low,
		medium,
		high
	}

	public enum FILL_COLOR_STYLE
	{
		single,
		gradient,
		textureGradient
	}
	
	
	/// <summary>
	/// Define if we are drawing the main font, the shadow or the outline
	/// </summary>
	private enum TEXT_COMPONENT 
	{
		Main,
		Shadow,
		Outline
	}

    public enum TEXT_OVERFLOW
    {
        ResizeFreely,
        ClampContent,
        ResizeHeight,
    }
	
	[System.Serializable]
	public class TextProperties
	{
		public	string 				text = "Hello World!";
        public bool richText = true;
		public	Font				font;
        public bool usePixelPerfect = true;
        public bool useTk2dUIMask = false;
		public	Material 			customDetailMaterial;
		public	int					fontSize = 16;
		public	float				size = 16;
        public TEXT_OVERFLOW overFlow;
        public Vector2 drawSize = new Vector2(100, 100);
		public 	TEXT_ANCHOR			textAnchor;
		public	TEXT_ALIGNMENT		textAlignment;
		public	float				lineSpacing = 1;
        public float wordSpacing = 0;
        public FontStyle fontStyle = FontStyle.Normal;
		public	FILL_COLOR_STYLE	fillColorStyle;
		public	Color				fontColorTop = new Color(1,1,1,1);
		public	Color				fontColorBottom = new Color(1,1,1,1);
		public 	Material 			fillMaterial;
		public 	bool				enableShadow;
		public 	Color				shadowColor = new Color(0,0,0,1);
		public  Vector3				shadowDistance = new Vector3(0,-1,0);
		public 	bool				enableOutline;
		public 	Color				outlineColor = new Color(0,0,0,1);
		public 	float				outLineWidth = 0.3f; 
		public	OUTLINE_QUALITY		outlineQuality;
		public	int					sortingLayerIndex;
		public	int					sortingLayerOrder;
		public	string 				sortingLayerName;
		
	}
	
	/// <summary>
	/// DO NOT CHANGE THIS DIRECTLY
	/// </summary>
	[HideInInspector]
	public TextProperties 	_privateProperties;  //WARNING!: do not change it directly
	
	#region properties

	// If you have some problematic text that appears corrupt when enabling it, try to enable this variable
	public bool				updateAlwaysOnEnable; 
	
	
	/// <summary>
	/// For complex setups with a lot of materials override the auto setting may not be usefull. WARNING!: You will 
	/// have to setup all the materials by hand.
	/// </summary>
	public bool				dontOverrideMaterials; 


	/// <summary>
	/// Gets or sets the text to show
	/// </summary>
	
	public	string			Text
	{
		get { return 		_privateProperties.text;}
        set { _privateProperties.text = value; RefreshMesh(true); }
	}

    public bool RichText
    {
        get { return _privateProperties.richText; }
        set { _privateProperties.richText = value; RefreshMesh(true); }
    }
	
	/// <summary>
	/// Gets or sets the Font Type
	/// </summary>
	public	Font			FontType
	{
		get { return _privateProperties.font;}
		set { _privateProperties.font = value; ChangeFont();}
	}

    /// <summary>
    /// Gets or sets the Use Pixel Perfect
    /// Only Editor
    /// </summary>
    public bool UsePixelPerfect
    {
        get { return _privateProperties.usePixelPerfect; }
        set { _privateProperties.usePixelPerfect = value; }
    }

    /// <summary>
    /// Gets or sets the Use 2D Toolkit UIMask
    /// Only Editor
    /// </summary>
    public bool UseTk2dUIMask
    {
        get { return _privateProperties.useTk2dUIMask; }
        set { _privateProperties.useTk2dUIMask = value; }
    }

	/// <summary>
	/// Gets or sets the filling material (for having patterns inside the letters) 
	/// </summary>
	public	Material		CustomDetailMaterial
	{
		get { return _privateProperties.customDetailMaterial;}
        set { _privateProperties.customDetailMaterial = value; RefreshMesh(false); }
	}
    /*
	/// <summary>
    /// 텍스쳐에 저장되는 사이즈임. 실제 화면에 찍히는 사이즈를 구하고 싶으면 Size를 가져다 쓸것.
	/// Gets or sets the font size. This will increase the resolution of the text and the font texure size
	/// </summary>
	public	int				FontSize
	{
		get { return _privateProperties.fontSize;}
        set { _privateProperties.fontSize = value; RefreshMesh(false); }
	}
    */
	
	
	/// <summary>
	/// Gets or sets the size of the letters. The proportional quad size for the letters
	/// </summary>
	public	float			Size
	{
		get { return _privateProperties.size;}
        set { _privateProperties.size = value; RefreshMesh(true); }
	}
	
    /// <summary>
    /// overflow text
    /// </summary>
    public TEXT_OVERFLOW Overflow
    {
        get { return _privateProperties.overFlow; }
        set { _privateProperties.overFlow = value; RefreshMesh(false); }
    }


    /// <summary>
    /// 텍스트가 그려질 공간의 크기.
    /// Dimension Text
    /// </summary>
    public Vector2 DrawSize
    {
        get { return _privateProperties.drawSize; }
        set { _privateProperties.drawSize = value; RefreshMesh(false); }
    }

	/// <summary>
	/// Gets or sets the Text anchor
	/// </summary>
	public	TEXT_ANCHOR		Textanchor
	{
		get { return _privateProperties.textAnchor;}
        set { _privateProperties.textAnchor = value; RefreshMesh(false); }
	}
	
	/// <summary>
	/// Gets or sets the text alignment. Only for paragraphs
	/// </summary>
	public	TEXT_ALIGNMENT	Textalignment
	{
		get { return _privateProperties.textAlignment;}
        set { _privateProperties.textAlignment = value; RefreshMesh(false); }
	}
	
	/// <summary>
	/// Gets or sets the space between lines of a paragraph
	/// </summary>
	public	float			LineSpacing
	{
		get { return _privateProperties.lineSpacing;}
        set { _privateProperties.lineSpacing = value; RefreshMesh(false); }
	}

    /// <summary>
    /// 자간 수정.
    /// Gets or sets the space between characters of a paragraph
    /// </summary>
    public float WordSpacing
    {
        get { return _privateProperties.wordSpacing; }
        set { _privateProperties.wordSpacing = value; RefreshMesh(false); }
    }

    /// <summary>
    /// 폰트에 기본적용된 폰트 스타일을 지정합니다.
    /// Get or sets the font style
    /// </summary>
    public FontStyle FontStyle
    {
        get { return _privateProperties.fontStyle; }
        set { _privateProperties.fontStyle = value; RefreshMesh(false); }
    }


	/// <summary>
	/// Gets or sets the fill color style
	/// </summary>
	public	FILL_COLOR_STYLE	FillColorStyle
	{
		get { return _privateProperties.fillColorStyle;}
        set { _privateProperties.fillColorStyle = value; SetColor(_privateProperties.fontColorTop,_privateProperties.fontColorBottom); }
	}

	/// <summary>
	/// Gets or sets the top font color
	/// </summary>
	public	Color			FontColorTop
	{
		get { return _privateProperties.fontColorTop;}
        set { SetColor(value, _privateProperties.fontColorBottom); }
	}
	
	/// <summary>
	/// Gets or sets the bottom font color
	/// </summary>
	public	Color			FontColorBottom
	{
		get { return _privateProperties.fontColorBottom;}
        set { SetColor(_privateProperties.fontColorTop, value); }
	}

	public	Material		FillMaterial
	{
		get { return _privateProperties.fillMaterial;}
        set { _privateProperties.fillMaterial = value; SetFontMaterial(); }
	}
	
	/// <summary>
	/// Enable or deisable proyected shadow. This will draw the text twice
	/// </summary>
	public	bool			EnableShadow
	{
		get { return _privateProperties.enableShadow;}
        set { _privateProperties.enableShadow = value; RefreshMesh(false); }
	}
	
	/// <summary>
	/// Gets or sets the shadow color
	/// </summary>
	public	Color			ShadowColor
	{
		get { return _privateProperties.shadowColor;}
        set { SetShadowColor(value); }
	}
	
	/// <summary>
	/// Gets or sets the shadow offset distance. Note: Normally you don't want to change the Z coordinate.
	/// </summary>
	public	Vector3			ShadowDistance
	{
		get { return _privateProperties.shadowDistance;}
        set { _privateProperties.shadowDistance = value; RefreshMesh(false); }
	}
	
	/// <summary>
	/// Enable or disable the font outline. This will draw the text 4 times more
	/// </summary>
	public	bool			EnableOutline
	{
		get { return _privateProperties.enableOutline;}
        set { _privateProperties.enableOutline = value; RefreshMesh(false); }
	}
	
	/// <summary>
	/// Gets or sets the outline color
	/// </summary>
	public	Color			OutlineColor
	{
		get { return _privateProperties.outlineColor;}
        set { SetOutlineColor(value); }
	}
	
	/// <summary>
	/// Gets or sets the outline width
	/// </summary>
	public	float			OutLineWidth
	{
		get { return _privateProperties.outLineWidth;}
        set { _privateProperties.outLineWidth = value; RefreshMesh(false); }
	}

	
	/// <summary>
	/// Gets or sets the highquality outline option. Will increase the vertex count per letter
	/// </summary>
	/// <value><c>true</c> if high quality outline; otherwise, <c>false</c>.</value>
	public	OUTLINE_QUALITY			OutlineQuality
	{
		get { return _privateProperties.outlineQuality;}
        set { _privateProperties.outlineQuality = value; RefreshMesh(false); }
	}


	public	int 			SortingLayerOrder
	{
		get { return _privateProperties.sortingLayerOrder;}
		set { _privateProperties.sortingLayerOrder = value; RefreshRenderLayerSettings();}
	}


	/// <summary>
	/// Gets or sets the name of the sorting layer.
	/// </summary>
	/// <value>The name of the sorting layer.</value>
	public	string 			SortingLayerName
	{
		get { return _privateProperties.sortingLayerName;}
		set { _privateProperties.sortingLayerName = value; RefreshRenderLayerSettings();}
	}
		
	#endregion
		
	
	
	#region Private vars
	
	//Cache vars
    public Camera textCamera;
    [HideInInspector]
    [SerializeField]
    private Material fontMaterial;
    private Mesh textMesh;
    private MeshFilter      textMeshFilter;
    [HideInInspector]
    public Renderer textRenderer;
    private CustomFontParsing[] parsingList;

    public Material MainFontMaterial
    {
        get
        {
            if (fontMaterial)
                return fontMaterial;
            else
                return _privateProperties.font.material;
        }
        set
        {
            fontMaterial = value;
            SetFontMaterial();
        }
    }

	/// <summary>
	/// The current line break.
	/// </summary>
	private int currentLineBreak = 0;
    /// <summary>
    /// The current character count
    /// </summary>
    private int characterLength = 0;

	/// <summary>
	/// Store in wich character there is a line break
	/// </summary>
	private List<int> lineBreakCharCounter 			= new List<int>();

	/// <summary>
	/// The distance between characters accumulated before each line break
	/// </summary>
	private List<float> lineBreakAccumulatedDistance 	= new List<float>();

	//Mesh data
	Vector3[]				vertices;
	int[]					triangles;
	Vector2[]				uv;
	Vector2[]				uv2;
	Color[]					colors;
	
	#endregion
	
	[HideInInspector]
	public bool GUIChanged = false;
	
	#region special char codes
	private char	LINE_BREAK = Convert.ToChar(10);  //This is the character code for the alt+enter character that Unity includes in the text
	#endregion
	
	
	void Awake()
	{
#if UNITY_EDITOR
		if (_privateProperties == null)
			_privateProperties = new TextProperties();
		
		if (_privateProperties.font == null)
			_privateProperties.font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;

        //_privateProperties.font = UnityEditor.AssetDatabase.LoadAssetAtPath<Font>("Assets/Font/arialbd.ttf");

#endif

		CacheTextVars();
		RefreshMesh(true);

        if (Application.isPlaying)
            Font.textureRebuilt += FontTexureRebuild;
	}

    void OnDestroy()
    {
        if (Application.isPlaying)
            Font.textureRebuilt -= FontTexureRebuild;

        DestoryTextMesh();
    }


	void OnEnable()
	{
        if (!Application.isPlaying)
            Font.textureRebuilt += FontTexureRebuild;

		if (updateAlwaysOnEnable)
			RefreshMesh(true);

        RefreshRenderLayerSettings();
    }

    void OnDisable()
    {
        if (!Application.isPlaying)
            Font.textureRebuilt -= FontTexureRebuild;
    }

#if UNITY_EDITOR
    void OnValidate()
    {
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        if (meshFilter != null)
        {
            meshFilter.sharedMesh = textMesh;
        }
    }
#endif

    void LateUpdate()
    {
        PixelPerfect(true);
    }

    void DestoryTextMesh()
    {
        if (textMeshFilter == null)
            textMeshFilter = GetComponent<MeshFilter>();
        if (textMeshFilter != null)
            textMesh = textMeshFilter.sharedMesh;

        if (textMesh)
        {
            DestroyImmediate(textMesh, true);
            textMeshFilter.mesh = null;
        }
    }

    /// <summary>
    /// Fonts the texure rebuild.
    /// </summary>
    void FontTexureRebuild(Font changedFont)
    {
        if (_privateProperties.font != changedFont)
            return;

        RefreshMesh(true);
    }

    public Camera getTextCamera()
    {
        if (textCamera == null)
            textCamera = Camera.main;
        return textCamera;
    }

    void ChangeFont()
    {
#if UNITY_EDITOR
        //if (!Application.isPlaying && GUIChanged == false)
        //    return;
#endif

        if (!dontOverrideMaterials)
        {
            SetFontMaterial();
        }
        RefreshMesh(true);
    }

	/// <summary>
	/// Initialize the text variables
	/// </summary>
	void CacheTextVars()
	{
        if (_privateProperties.font == null)
            return;
        if (!_privateProperties.useTk2dUIMask)
            fontMaterial = null;

		if (textMesh == null)
		{
            textMeshFilter = GetComponent<MeshFilter>();
            if (textMeshFilter == null)
                textMeshFilter = gameObject.AddComponent<MeshFilter>();

			textMesh 		= new Mesh();
			textMesh.name	= GetInstanceID().ToString(); //Rename to something
            textMesh.hideFlags = HideFlags.DontSave;
            textMeshFilter.sharedMesh = textMesh;
            //textMeshFilter.mesh = textMesh;
		}
        if (textRenderer == null)
        {
            if ((textRenderer = GetComponent<MeshRenderer>()) == null)            
                textRenderer = gameObject.AddComponent<MeshRenderer>();
        }

		//Set materials
        SetFontMaterial();
	}

	/// <summary>
	/// Sets the font material.
	/// </summary>
	void SetFontMaterial()
	{
		if (!dontOverrideMaterials)
		{
            Material customMaterial = null;
            if (_privateProperties.customDetailMaterial != null)
                customMaterial = _privateProperties.customDetailMaterial;
            else if (_privateProperties.fillColorStyle == FILL_COLOR_STYLE.textureGradient && _privateProperties.fillMaterial != null)
                customMaterial = _privateProperties.fillMaterial;

            if (customMaterial != null)
            {
                if (customMaterial.mainTexture != _privateProperties.font.material.mainTexture) //Check if the assigned font texture is OK
                    customMaterial.mainTexture = _privateProperties.font.material.mainTexture;
                if (_privateProperties.enableShadow || _privateProperties.enableOutline)
                    textRenderer.sharedMaterials = new Material[2] { MainFontMaterial, customMaterial };
                else
                    textRenderer.sharedMaterials = new Material[1] { customMaterial };
            }
            else
                textRenderer.sharedMaterials = new Material[1] { MainFontMaterial };
		}
	}
    
    /// <summary>
    /// 텍스트를 파싱합니다.
    /// _updateTexureInfo 가 true이면 Font.RequestCharactersInTexture도 함께 실행합니다.
    /// Parsing Font Style, Color, Characters.
    /// <b>Bold</b>
    /// <i>Italic</i>
    /// <bi>Bold and Italic</bi>
    /// <color=#FFFFFF>Color</color>
    /// <gradient=#FF0000,#FFFF00>Gradient</gradient>
    /// </summary>
    /// <param name="text"></param>
    /// <param name="_updateTexureInfo"></param>
    /// <returns></returns>
    CustomFontParsing[] ParsingText(string text, Color colorTop, Color colorBottom, FontStyle fontStyle = FontStyle.Normal)
    {
        text = text.Replace(System.Environment.NewLine, "\n");
        if (fontStyle == FontStyle.Normal)
            fontStyle = _privateProperties.fontStyle;

        List<CustomFontParsing> ret = new List<CustomFontParsing>();

        int index = 0;
        int length = text.Length;
        if (_privateProperties.richText)
        {
            string outText;
            string outValue;
            while (index < length)
            {
                if (CustomRichText.findTextIndex(text, "b", ref index, out outText, out outValue))
                    ret.AddRange(ParsingText(outText, colorTop, colorBottom, FontStyle.Bold));
                else if (CustomRichText.findTextIndex(text, "i", ref index, out outText, out outValue))
                    ret.AddRange(ParsingText(outText, colorTop, colorBottom, FontStyle.Italic));
                else if (CustomRichText.findTextIndex(text, "bi", ref index, out outText, out outValue))
                    ret.AddRange(ParsingText(outText, colorTop, colorBottom, FontStyle.BoldAndItalic));
                // 컬러 체크.
                // check color
                else if (CustomRichText.findTextIndex(text, "color", ref index, out outText, out outValue))
                {
                    Color parsingColor = CustomRichText.ConvertColor(outValue);
                    ret.AddRange(ParsingText(outText, parsingColor, parsingColor, fontStyle));
                }
                // 그라데이션 체크.
                // check gradient
                else if (CustomRichText.findTextIndex(text, "gradient", ref index, out outText, out outValue))
                {
                    string[] split = outValue.Split(",".ToCharArray());
                    if (split.Length > 1)
                    {
                        Color parsingColorTop = CustomRichText.ConvertColor(split[0]);
                        Color parsingColorBottom = CustomRichText.ConvertColor(split[1]);
                        ret.AddRange(ParsingText(outText, parsingColorTop, parsingColorBottom, fontStyle));
                    }
                }
                else
                    ret.Add(new CustomFontParsing(text[index], colorTop, colorBottom, fontStyle));
                index++;
            }
        }
        else
        {
            while (index < length)
            {
                ret.Add(new CustomFontParsing(text[index], colorTop, colorBottom, fontStyle));
                index++;
            }
        }
        AnalizeText(ret); //Check for special characters

        return ret.ToArray();
    }

    /// <summary>
    /// 해당 폰트스타일의 텍스쳐를 업데이트 합니다.
    /// update Texture Font Sytle
    /// </summary>
    /// <param name="fontStyle"></param>
    void updateTextureInfo(FontStyle fontStyle)
    {
        List<char> requestList = new List<char>();
        for (int i = 0; i < parsingList.Length ; i++)
        {
            CustomFontParsing parsing = parsingList[i];
            if (fontStyle == parsing.Style)
                requestList.Add(parsing.Char);
        }
        if (requestList.Count > 0)
            _privateProperties.font.RequestCharactersInTexture(new string(requestList.ToArray()), _privateProperties.fontSize, fontStyle);
    }

    void PixelPerfect(bool isRefreshMesh)
    {
        if (_privateProperties.usePixelPerfect)
        {
            Vector3 cameraScreenPoints = getTextCamera().WorldToScreenPoint(new Vector3(0f, 0f, transform.position.z));
            Vector3 cameraScreenPoints2 = getTextCamera().WorldToScreenPoint(new Vector3(0f, _privateProperties.size, transform.position.z));
            float px = cameraScreenPoints2.y - cameraScreenPoints.y;
            float maxScale = Mathf.Max(transform.lossyScale.x, transform.lossyScale.y);
            int fontSize = Mathf.RoundToInt(px * maxScale);
            if (_privateProperties.fontSize != fontSize)
            {
                _privateProperties.fontSize = fontSize;
                if (isRefreshMesh)
                    RefreshMesh(true);
            }
        }
    }

	/// <summary>
	/// Refreshs the mesh.
	/// </summary>
	/// <param name='_updateTexureInfo'>
	/// _update texure info. 
	/// </param>
	public void RefreshMesh(bool _updateTexureInfo)
	{
        if (_privateProperties.font == null)
            return;
        if (_privateProperties.size <= 0)
            return;

        PixelPerfect(false);

        if (_privateProperties.fontSize <= 0)
            return;

        // 그라데이션이 아니면 컬러값을 맞춰준다.
        if (_privateProperties.fillColorStyle == FILL_COLOR_STYLE.single)
            _privateProperties.fontColorBottom = _privateProperties.fontColorTop;

        // 텍스쳐를 파싱합니다.
        // Parsing Text
        parsingList = ParsingText(_privateProperties.text, _privateProperties.fontColorTop, _privateProperties.fontColorBottom);
		        
        // 텍스쳐를 생성합니다.
        // Create Texture
        if (_updateTexureInfo)
        {
            updateTextureInfo(FontStyle.Normal);
            updateTextureInfo(FontStyle.Bold);
            updateTextureInfo(FontStyle.Italic);
            updateTextureInfo(FontStyle.BoldAndItalic);
        }
		
		//The vertex count must increase if we are going to use high quality outline
		float angleIncrement = 90;
		if (_privateProperties.enableOutline)
		{
			switch (_privateProperties.outlineQuality)
			{
				case OUTLINE_QUALITY.medium:    angleIncrement = 45; break;
				case OUTLINE_QUALITY.high:      angleIncrement = 22.5f; break;
				default:    angleIncrement = 90; break;
			}
		}
        int vertexNumberModifier = GetVertexIndexPosition() + 1;

        vertices = new Vector3[parsingList.Length * 4 * vertexNumberModifier];
        triangles = new int[parsingList.Length * 6 * vertexNumberModifier];
        uv = new Vector2[parsingList.Length * 4 * vertexNumberModifier];
        uv2 = new Vector2[parsingList.Length * 4 * vertexNumberModifier];
        colors = new Color[parsingList.Length * 4 * vertexNumberModifier];


		int characterPosition = 0;
		int alignmentPass = 0;

		//Shadow
        if (_privateProperties.enableShadow)
            CreateCharacters(ref alignmentPass, ref characterPosition, _privateProperties.shadowDistance, TEXT_COMPONENT.Shadow);

		//Outline
        if (_privateProperties.enableOutline)
        {
            for (float ang = 0.0f; ang < 360.0f; ang += angleIncrement)
            {
                Vector3 dir = Vector3.right;
                dir.x = Mathf.Cos(ang * Mathf.Deg2Rad);
                dir.y = Mathf.Sin(ang * Mathf.Deg2Rad);

                CreateCharacters(ref alignmentPass, ref characterPosition, dir * _privateProperties.outLineWidth, TEXT_COMPONENT.Outline);
            }
        }

        //Normal text
        CreateCharacters(ref alignmentPass, ref characterPosition, Vector3.zero, TEXT_COMPONENT.Main);
 
		if (textMesh != null)
		{
			textMesh.Clear(true);
			SetAnchor();
			textMesh.vertices 	= vertices;
			textMesh.uv 		= uv;
			textMesh.uv2		= uv2;
		
			if ((_privateProperties.customDetailMaterial != null || _privateProperties.fillColorStyle == FILL_COLOR_STYLE.textureGradient) && 
			    (_privateProperties.enableShadow || _privateProperties.enableOutline))
				SetTrianglesForMultimesh();
			else
	        	textMesh.triangles 	= triangles;

			textMesh.colors		= colors;
		}
	}

	
	void ResetHelperVariables()
	{
	 	lineBreakAccumulatedDistance.Clear();
		lineBreakCharCounter.Clear();
		currentLineBreak 	= 0;
        characterLength = 0;
	}


	
	/// <summary>
	/// Analizes the text for keycodes and prepare it for rendering. Right now only \n is supported
	/// </summary>
	void AnalizeText(List<CustomFontParsing> texts)
	{		
		//Test characters for know keycodes
		bool recheckCharArray = true;
		while (recheckCharArray)
		{
			recheckCharArray = false;
            for (int i = 0; i < texts.Count - 1; i++)
            {
                if (texts[i].Char == '\\' && texts[i + 1].Char == 'n')
                {
                    texts[i].Char = LINE_BREAK;
                    texts.RemoveAt(i + 1);
                    recheckCharArray = true;
                    break;
                }
            }
		}		
	}

    /// <summary>
    /// check over Dimension for ClampContent
    /// </summary>
    /// <returns></returns>
    bool CheckCharacterLine()
    {
        // ClampContent and Y Dimension over is return
        if (_privateProperties.overFlow == TEXT_OVERFLOW.ClampContent &&
            (_privateProperties.drawSize.y + 0.01f) < _privateProperties.size + _privateProperties.size * currentLineBreak * _privateProperties.lineSpacing)
        {
            currentLineBreak = (int)((_privateProperties.drawSize.y + 0.01f - _privateProperties.size) / _privateProperties.lineSpacing / _privateProperties.size);
            return true;
        }
        return false;
    }

    /// <summary>
    /// Creates the characters.
    /// </summary>
    /// <param name="alignmentPass"></param>
    /// <param name="characterPosition"></param>
    /// <param name="_offset"></param>
    /// <param name="_textComponent"></param>
    void CreateCharacters(ref int alignmentPass, ref int characterPosition, Vector3 _offset, TEXT_COMPONENT _textComponent)
    {
        ResetHelperVariables();
        for (int i = 0; i < parsingList.Length; i++)
        {
            CustomFontParsing parsing = parsingList[i];
            Color topColor = parsing.ColorTop;
            Color bottomColor = parsing.ColorBottom;
            switch (_textComponent)
            {
                case TEXT_COMPONENT.Shadow: topColor = bottomColor = _privateProperties.shadowColor; break;
                case TEXT_COMPONENT.Outline: topColor = bottomColor = _privateProperties.outlineColor; break;
            }
            if (!CreateCharacter(parsing.Char, characterPosition, _offset, topColor, bottomColor, parsing.Style))
                break;
            characterPosition++;
        }
        SetAlignment(alignmentPass++, characterLength);
    }

    /// <summary>
    /// Creates the character.
    /// </summary>
    /// <param name="_character"></param>
    /// <param name="_arrayPosition"></param>
    /// <param name="_offset"></param>
    /// <param name="_colorTop"></param>
    /// <param name="_colorBottom"></param>
    /// <param name="fontStyle"></param>
	bool CreateCharacter(char _character, int _arrayPosition, Vector3 _offset, Color _colorTop, Color _colorBottom, FontStyle fontStyle )
	{
		if (lineBreakAccumulatedDistance.Count == 0)
			lineBreakAccumulatedDistance.Add(0);		
		if (lineBreakCharCounter.Count == 0)
			lineBreakCharCounter.Add(0);

        float sizeModifier = _privateProperties.size / _privateProperties.fontSize;
        _offset *= _privateProperties.size * 0.1f;

		CharacterInfo charInfo = new CharacterInfo();
        // character is line break
        if (!_privateProperties.font.GetCharacterInfo(_character, out charInfo, _privateProperties.fontSize, fontStyle))
        {
            lineBreakCharCounter.Add(lineBreakCharCounter[currentLineBreak]);
            lineBreakAccumulatedDistance.Add(0);
            currentLineBreak++;
            lineBreakCharCounter[currentLineBreak]++;

            if (CheckCharacterLine())
                return false;
            characterLength++;
            return true;
        }
        
        if ((_privateProperties.overFlow == TEXT_OVERFLOW.ClampContent || _privateProperties.overFlow == TEXT_OVERFLOW.ResizeHeight)
             && (lineBreakAccumulatedDistance[currentLineBreak] + charInfo.advance) * sizeModifier > _privateProperties.drawSize.x)
        {
            lineBreakCharCounter.Add(lineBreakCharCounter[currentLineBreak]);
            lineBreakAccumulatedDistance.Add(0);
            currentLineBreak++;

            if (CheckCharacterLine())
                return false;
        }
        lineBreakCharCounter[currentLineBreak]++;
        characterLength++;

		//print("Character: " + _character  +" Vertex: " +   charInfo.vert + "UV: " +charInfo.uv + "Char size: " + charInfo.size + "Char width: " + charInfo.width + "  Is flipped: "+ charInfo.flipped);		
        Vector3 currentAcumulatedCharacterDistance = new Vector3(lineBreakAccumulatedDistance[currentLineBreak] * sizeModifier, -_privateProperties.size * currentLineBreak * _privateProperties.lineSpacing, 0);
		
		//Create a quad with the size of the character
        vertices[4 * _arrayPosition + 0] = currentAcumulatedCharacterDistance + _offset + new Vector3(charInfo.minX, charInfo.maxY, 0) * sizeModifier;
        vertices[4 * _arrayPosition + 1] = currentAcumulatedCharacterDistance + _offset + new Vector3(charInfo.maxX, charInfo.maxY, 0) * sizeModifier;
        vertices[4 * _arrayPosition + 2] = currentAcumulatedCharacterDistance + _offset + new Vector3(charInfo.maxX, charInfo.minY, 0) * sizeModifier;
        vertices[4 * _arrayPosition + 3] = currentAcumulatedCharacterDistance + _offset + new Vector3(charInfo.minX, charInfo.minY, 0) * sizeModifier;
        
		//lineBreakAccumulatedDistance[currentLineBreak] += charInfo.width;
        lineBreakAccumulatedDistance[currentLineBreak] += charInfo.advance + _privateProperties.wordSpacing;
		
		//Set triangles
		triangles[6*_arrayPosition] 	= _arrayPosition*4;
		triangles[6*_arrayPosition+1] 	= _arrayPosition*4+1;
		triangles[6*_arrayPosition+2] 	= _arrayPosition*4+2;
		triangles[6*_arrayPosition+3] 	= _arrayPosition*4;
		triangles[6*_arrayPosition+4] 	= _arrayPosition*4+2;
		triangles[6*_arrayPosition+5] 	= _arrayPosition*4+3;
		
		//Set UVs
        uv[4 * _arrayPosition] = charInfo.uvTopLeft;
        uv[4 * _arrayPosition + 1] = charInfo.uvTopRight;
        uv[4 * _arrayPosition + 2] = charInfo.uvBottomRight;
        uv[4 * _arrayPosition + 3] = charInfo.uvBottomLeft;

		//Set uv2
		if (_privateProperties.customDetailMaterial != null && _privateProperties.fillColorStyle != FILL_COLOR_STYLE.textureGradient)  //Only if we need them
		{
			Vector2 uvOffset = new Vector2(_offset.x,_offset.y);
			Vector2 uvAccumulatedDistance = new Vector2(currentAcumulatedCharacterDistance.x,currentAcumulatedCharacterDistance.y);
            
            uv2[4 * _arrayPosition + 0] = uvAccumulatedDistance + uvOffset + new Vector2(charInfo.minX, charInfo.maxY) * sizeModifier;
            uv2[4 * _arrayPosition + 1] = uvAccumulatedDistance + uvOffset + new Vector2(charInfo.maxX, charInfo.maxY) * sizeModifier;
            uv2[4 * _arrayPosition + 2] = uvAccumulatedDistance + uvOffset + new Vector2(charInfo.maxX, charInfo.minY) * sizeModifier;
            uv2[4 * _arrayPosition + 3] = uvAccumulatedDistance + uvOffset + new Vector2(charInfo.minX, charInfo.minY) * sizeModifier;
		}	
		else
		{
			//UV2 for textureGradient
			uv2[4*_arrayPosition] 			=   new Vector2(0,0);
			uv2[4*_arrayPosition+1] 		=   new Vector2(1,0);
			uv2[4*_arrayPosition+2] 		=  	new Vector2(1,1);
			uv2[4*_arrayPosition+3] 		=  	new Vector2(0,1);
		}

        colors[4 * _arrayPosition] = _colorTop;
        colors[4 * _arrayPosition + 1] = _colorTop;
        colors[4 * _arrayPosition + 2] = _colorBottom;
        colors[4 * _arrayPosition + 3] = _colorBottom;

        return true;
	}

	/// <summary>
	/// Sets the anchor.
	/// </summary>
	void SetAnchor()
	{
		Vector2 textOffset = Vector2.zero;

        float sizeModifier = _privateProperties.size / _privateProperties.fontSize;
		float maxDistance = 0;
        float maxHeight = _privateProperties.size * 0.8f;

				
		for (int i = 0; i< lineBreakAccumulatedDistance.Count; i++)
		{
			if (lineBreakAccumulatedDistance[i] > maxDistance)
				maxDistance = lineBreakAccumulatedDistance[i];
		}

        if (_privateProperties.overFlow == TEXT_OVERFLOW.ResizeFreely)
            _privateProperties.drawSize.x = Mathf.Ceil(maxDistance * sizeModifier);
        if (_privateProperties.overFlow == TEXT_OVERFLOW.ResizeFreely ||
            _privateProperties.overFlow == TEXT_OVERFLOW.ResizeHeight)
        {
            _privateProperties.drawSize.y = Mathf.Ceil(_privateProperties.size + _privateProperties.size * currentLineBreak * _privateProperties.lineSpacing);
        }

        // set text X position
		switch (_privateProperties.textAnchor)
		{
			case TEXT_ANCHOR.MiddleLeft:
			case TEXT_ANCHOR.UpperLeft:
			case TEXT_ANCHOR.LowerLeft:
				switch(_privateProperties.textAlignment)
				{
					case TEXT_ALIGNMENT.right:
                        textOffset.x = maxDistance * sizeModifier;	
					break;
				
					case TEXT_ALIGNMENT.center:
                        textOffset.x += maxDistance * 0.5f * sizeModifier;
					break;
				}
			break;
			
			case TEXT_ANCHOR.MiddleRight:
			case TEXT_ANCHOR.UpperRight:
			case TEXT_ANCHOR.LowerRight:
				switch(_privateProperties.textAlignment)
				{
					case TEXT_ALIGNMENT.left:
                        textOffset.x -= maxDistance * sizeModifier;
					break;
					case TEXT_ALIGNMENT.center:
                    textOffset.x -= maxDistance * 0.5f * sizeModifier;
					break;
				}			
			break;
			
			case TEXT_ANCHOR.MiddleCenter:
			case TEXT_ANCHOR.UpperCenter:
			case TEXT_ANCHOR.LowerCenter:
				switch(_privateProperties.textAlignment)
				{
					case TEXT_ALIGNMENT.left:
						textOffset.x -= maxDistance * 0.5f * sizeModifier;
					break;
				
					case TEXT_ALIGNMENT.right:
                        textOffset.x = maxDistance * 0.5f * sizeModifier;		
					break;
				}			
			break;	
		}
		
        // set text Y position
        switch (_privateProperties.textAnchor)
        {
            case TEXT_ANCHOR.UpperLeft:
            case TEXT_ANCHOR.UpperRight:
            case TEXT_ANCHOR.UpperCenter:
                textOffset.y = -maxHeight;
                break;
            case TEXT_ANCHOR.MiddleCenter:
            case TEXT_ANCHOR.MiddleLeft:
            case TEXT_ANCHOR.MiddleRight:
                textOffset.y = -maxHeight + (_privateProperties.size * (currentLineBreak + 1) * _privateProperties.lineSpacing * 0.5f);
                break;
            case TEXT_ANCHOR.LowerLeft:
            case TEXT_ANCHOR.LowerRight:
            case TEXT_ANCHOR.LowerCenter:
                textOffset.y = -maxHeight + (_privateProperties.size * (currentLineBreak + 1) * _privateProperties.lineSpacing);
                break;
        }
		
		for (int i = 0; i<vertices.Length; i++) 
		{
			vertices[i].x += textOffset.x;
			vertices[i].y += textOffset.y;
		}
	}
	
	/// <summary>
	/// Sets the alignment.
	/// </summary>
	/// <param name='_pass'>
	/// _pass. The pass set what are we drawing (shadow, main, oituline up, outline down...)
	/// </param>
	void SetAlignment(int _pass, int _count)
	{
        if (_count == 0)
            _count = parsingList.Length;
        int vertexPassOffset = _pass * _count * 4; // We have to align the outline,shadow and main.
		float charOffset = 0;
		
		for (int  i = 0; i<lineBreakCharCounter.Count; i++)
		{
			switch (_privateProperties.textAlignment)
			{
				case TEXT_ALIGNMENT.left:
						//This is how the text is created by default. No changes are needed
				break;
					
				case TEXT_ALIGNMENT.right:
					charOffset = -lineBreakAccumulatedDistance[i]*_privateProperties.size/_privateProperties.fontSize;
				break;
				
				case TEXT_ALIGNMENT.center:
					charOffset = -lineBreakAccumulatedDistance[i]*0.5f*_privateProperties.size/_privateProperties.fontSize;
				break;
			}
			
			int firstCharVertex;			
			if (i == 0)
				firstCharVertex = 0;
			else
				firstCharVertex = lineBreakCharCounter[i-1]*4;			
			int lastCharVertex = lineBreakCharCounter[i]*4-1;
			
            for (int j = firstCharVertex + vertexPassOffset; j <= lastCharVertex + vertexPassOffset; j++) // 
            {
				vertices[j].x += charOffset;	
			}
		}
	}
	
	
	/// <summary>
	/// Sets the triangles for multimesh. This is used for the fill material
	/// </summary>
	void SetTrianglesForMultimesh()
	{
        int triangleMultiplier = GetVertexIndexPosition();
        int firstTriangleNormalText = triangleMultiplier * 6 * parsingList.Length;
        int[] mainTriangleSubmesh = new int[parsingList.Length * 6];
		
		for (int i = firstTriangleNormalText, j=0; i<triangles.Length; i++, j++)
			mainTriangleSubmesh[j] = triangles[i];

        int styleTextTriangleNumber = parsingList.Length * triangleMultiplier * 6;
		int[] secondaryTriangleStyleText =  new int[styleTextTriangleNumber];
		for (int i = 0; i<styleTextTriangleNumber; i++)
			secondaryTriangleStyleText[i] = triangles[i];
		
		textMeshFilter.sharedMesh.subMeshCount = 2;
		textMeshFilter.sharedMesh.SetTriangles(mainTriangleSubmesh, 1);
		textMeshFilter.sharedMesh.SetTriangles(secondaryTriangleStyleText, 0);
	}
	
	/// <summary>
	/// Refreshs the mesh editor. Only used by the custom inspector
	/// </summary>
	public void RefreshMeshEditor()
	{
        //DestroyImmediate(textMesh);
        DestoryTextMesh();
        CacheTextVars();
        RefreshRenderLayerSettings();
        RefreshMesh(true);
	}

	/// <summary>
	/// Confiugures the render order
	/// </summary>
	private void RefreshRenderLayerSettings()
	{
        textRenderer.sortingOrder = _privateProperties.sortingLayerOrder;
        textRenderer.sortingLayerName = _privateProperties.sortingLayerName;
	}
	
	
	public int GetVertexCount()
	{
		if (vertices != null)
			return vertices.Length;
		else
			return 0;
	}
	
	/// <summary>
	/// Sets the color hidden. This will not change the values in inspector, but its more efficent for changing vertex colors
	/// </summary>
	/// <param name='_topColor'>
	/// _top color.
	/// </param>
	public void SetColor(Color _topColor, Color _bottomColor)
	{
        switch (_privateProperties.fillColorStyle)
        {
            case FILL_COLOR_STYLE.single:
                _bottomColor = _topColor;
                break;

            case FILL_COLOR_STYLE.gradient:
                //Defulat behaviour. Nothing to do here	
                break;

            case FILL_COLOR_STYLE.textureGradient:
                _topColor = Color.white;
                _bottomColor = Color.white;
                break;
        }
        _privateProperties.fontColorTop = _topColor;
        _privateProperties.fontColorBottom = _bottomColor;

		if (colors == null || textMesh == null)
			return;

#if UNITY_EDITOR        
		if (!Application.isPlaying && GUIChanged == false)
			return;
#endif
        parsingList = ParsingText(_privateProperties.text, _topColor, _bottomColor);

        int initalVertex = GetInitialVertexToColorize(TEXT_COMPONENT.Main);
        for (int _arrayPosition = 0; _arrayPosition < parsingList.Length; _arrayPosition++)
        {
            CustomFontParsing parsing = parsingList[_arrayPosition];
            colors[_arrayPosition + initalVertex] = parsing.ColorTop;
            colors[_arrayPosition + initalVertex + 1] = parsing.ColorTop;
            colors[_arrayPosition + initalVertex + 2] = parsing.ColorBottom;
            colors[_arrayPosition + initalVertex + 3] = parsing.ColorBottom;
            initalVertex += 3;
        }
        textMesh.colors = colors; 
	}	

    public void SetColor(Color _topColor)
    {
        SetColor(_topColor, _topColor);
    }
	
	/// <summary>
	/// Sets the shadow's color
	/// </summary>
	/// <param name='_color'>
	/// _color.
	/// </param>
	public void SetShadowColor(Color _color)
	{
        _privateProperties.shadowColor = _color;

		#if UNITY_EDITOR	
		if (!Application.isPlaying && GUIChanged == false)
			return;
		#endif
		
		if (colors == null || textMesh == null )
			return;
        if (!_privateProperties.enableShadow)
            return;
		
		int initalVertex = GetInitialVertexToColorize(TEXT_COMPONENT.Shadow);
		
		for (int i = initalVertex; i<GetFinalVertexToColorize(TEXT_COMPONENT.Shadow); i ++ )
		{
			colors[i] = _color;			
		}	
		
		textMesh.colors = colors; 	
	}
	
	/// <summary>
	/// Sets the outline colour
	/// </summary>
	/// <param name='_color'>
	/// _color.
	/// </param>
	public void SetOutlineColor(Color _color)
	{
        _privateProperties.outlineColor = _color;

		#if UNITY_EDITOR	
		if (!Application.isPlaying && GUIChanged == false)
			return;
		#endif
				
		if (colors == null || textMesh == null)
			return;
        if (!_privateProperties.enableOutline)
            return;
		
		int initalVertex = GetInitialVertexToColorize(TEXT_COMPONENT.Outline);
		
		for (int i = initalVertex; i<GetFinalVertexToColorize(TEXT_COMPONENT.Outline); i ++ )
		{
			colors[i] = _color;			
		}	
		
		textMesh.colors = colors; 	
	}
	
	/// <summary>
	/// Gets the initial vertex to colorize.
	/// </summary>
	/// <returns>
	/// The initial vertex to colorize.
	/// </returns>
	/// <param name='_textComponent'>
	/// _text component.
	/// </param>
	private int GetInitialVertexToColorize(TEXT_COMPONENT _textComponent)
	{		
		int meshOptionMultipler = 0;
		
		switch (_textComponent)
		{
			case TEXT_COMPONENT.Main:		
				meshOptionMultipler += GetVertexIndexPosition();
			break;

			case TEXT_COMPONENT.Shadow:
				meshOptionMultipler = 0;
			break;
			
			case TEXT_COMPONENT.Outline:
				if (_privateProperties.enableShadow)	
					meshOptionMultipler = 1;
				else
					meshOptionMultipler = 0;
			break;
		}

        return parsingList.Length * 4 * meshOptionMultipler;
    }
	
	private int GetFinalVertexToColorize(TEXT_COMPONENT _textComponent)
	{		
		int lastVertex = 0;
		int meshOptionMultipler = 0;
		
		switch (_textComponent)
		{
			case TEXT_COMPONENT.Main:
				meshOptionMultipler = GetVertexIndexPosition() + 1;

                lastVertex = parsingList.Length * 4 * meshOptionMultipler;			
			break;
			
			case TEXT_COMPONENT.Shadow:
                lastVertex = parsingList.Length * 4;
			break;
			
			case TEXT_COMPONENT.Outline:
                meshOptionMultipler = GetVertexIndexPosition();

                lastVertex = parsingList.Length * 4 * meshOptionMultipler;
			break;
		}
		
		return lastVertex;
			
	}
    
	int GetVertexIndexPosition()
	{
		int vertexPosition = 0;

		if (_privateProperties.enableOutline)
		{
			switch (_privateProperties.outlineQuality)
			{
			case OUTLINE_QUALITY.low:
				vertexPosition += 4; 
				break;
				
			case OUTLINE_QUALITY.medium:
				vertexPosition += 8; 
				break;
				
			case OUTLINE_QUALITY.high:
				vertexPosition += 16; 
				break;
			}
		}
		if (_privateProperties.enableShadow)
			vertexPosition += 1;

		return vertexPosition;
	}

    public Vector2 GetAnchorOffset()
    {
        Vector2 anchorOffset = Vector3.zero;        
        switch (_privateProperties.textAnchor)
        {
            case TEXT_ANCHOR.LowerLeft:
            case TEXT_ANCHOR.MiddleLeft:
            case TEXT_ANCHOR.UpperLeft:
                break;
            case TEXT_ANCHOR.LowerCenter:
            case TEXT_ANCHOR.MiddleCenter:
            case TEXT_ANCHOR.UpperCenter:
                anchorOffset.x = -(_privateProperties.drawSize.x / 2.0f); break;
            case TEXT_ANCHOR.LowerRight:
            case TEXT_ANCHOR.MiddleRight:
            case TEXT_ANCHOR.UpperRight:
                anchorOffset.x = -(_privateProperties.drawSize.x); break;
        }
        switch (_privateProperties.textAnchor)
        {
            case TEXT_ANCHOR.LowerLeft:
            case TEXT_ANCHOR.LowerCenter:
            case TEXT_ANCHOR.LowerRight:
                break;
            case TEXT_ANCHOR.MiddleLeft:
            case TEXT_ANCHOR.MiddleCenter:
            case TEXT_ANCHOR.MiddleRight:
                anchorOffset.y = -(_privateProperties.drawSize.y / 2.0f); break;
            case TEXT_ANCHOR.UpperLeft:
            case TEXT_ANCHOR.UpperCenter:
            case TEXT_ANCHOR.UpperRight:
                anchorOffset.y = -(_privateProperties.drawSize.y); break;
        }
        return anchorOffset;
    }

    public void ReshapeBounds(Vector3 dMin, Vector3 dMax)
    {
        float minDrawSize = 0.1f;
        
        Vector3 oldSize = new Vector3(_privateProperties.drawSize.x, _privateProperties.drawSize.y);
        Vector3 oldMin = Vector3.zero;
        switch (_privateProperties.textAnchor)
        {
            case TEXT_ANCHOR.LowerLeft: oldMin.Set(0, 0, 0); break;
            case TEXT_ANCHOR.LowerCenter: oldMin.Set(0.5f, 0, 0); break;
            case TEXT_ANCHOR.LowerRight: oldMin.Set(1, 0, 0); break;
            case TEXT_ANCHOR.MiddleLeft: oldMin.Set(0, 0.5f, 0); break;
            case TEXT_ANCHOR.MiddleCenter: oldMin.Set(0.5f, 0.5f, 0); break;
            case TEXT_ANCHOR.MiddleRight: oldMin.Set(1, 0.5f, 0); break;
            case TEXT_ANCHOR.UpperLeft: oldMin.Set(0, 1, 0); break;
            case TEXT_ANCHOR.UpperCenter: oldMin.Set(0.5f, 1, 0); break;
            case TEXT_ANCHOR.UpperRight: oldMin.Set(1, 1, 0); break;
        }
        oldMin = Vector3.Scale(oldMin, oldSize) * -1;
        Vector3 newScale = oldSize + dMax - dMin;
        newScale.x /= _privateProperties.drawSize.x;
        newScale.y /= _privateProperties.drawSize.y;
        Vector2 scaleFactor = new Vector3(newScale.x, newScale.y);
        Vector3 scaledMin = new Vector3(oldMin.x * scaleFactor.x, oldMin.y * scaleFactor.y);

        Vector3 offset = dMin + oldMin - scaledMin;
        offset.z = 0;
        transform.position = transform.TransformPoint(offset);
        
        float drawWidth = _privateProperties.drawSize.x * scaleFactor.x;
        float drawHeight = _privateProperties.drawSize.y * scaleFactor.y;
        if (drawWidth < minDrawSize) drawWidth = minDrawSize;
        if (drawHeight < minDrawSize) drawHeight = minDrawSize;
        _privateProperties.drawSize = new Vector2(drawWidth, drawHeight);
        RefreshMesh(false);
    }
		
}
