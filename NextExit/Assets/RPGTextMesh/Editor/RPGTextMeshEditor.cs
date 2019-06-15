using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Reflection;
using UnityEditorInternal;
using System.Collections.Generic;
using System.Linq;


[CustomEditor (typeof (RPGTextMesh))]
[CanEditMultipleObjects]
public class RPGTextMeshEditor : Editor
{
    RPGTextMesh[] targetTextMeshes = new RPGTextMesh[0];
	
	private bool		wasPrefabModified;
	private string[]	sortingLayersNames;
	private int 		popupSortingLayersIndex;
    private bool        showRichTextHelp;

    void OnEnable()
    {
        targetTextMeshes = new RPGTextMesh[targets.Length];
        for (int i = 0; i < targets.Length; ++i)
            targetTextMeshes[i] = targets[i] as RPGTextMesh;

        sortingLayersNames = GetSortingLayerNames();
        //Initialize shorting layer index
        RPGTextMesh customFont = target as RPGTextMesh;
        for (int i = 0; i < sortingLayersNames.Length; i++)
        {
            if (sortingLayersNames[i] == customFont.textRenderer.sortingLayerName)
                popupSortingLayersIndex = i;
        }  
    }
	
	public override void OnInspectorGUI()
	{
		serializedObject.Update();

		DrawDefaultInspector();

        RPGTextMesh customFont = target as RPGTextMesh;

		SerializedProperty serializedText 				= serializedObject.FindProperty("_privateProperties.text");
        SerializedProperty serializedRichText = serializedObject.FindProperty("_privateProperties.richText");
        SerializedProperty serializedFontType = serializedObject.FindProperty("_privateProperties.font");
        SerializedProperty serializedUsePixelPerfect = serializedObject.FindProperty("_privateProperties.usePixelPerfect");
        SerializedProperty serializedUseTk2dUIMask = serializedObject.FindProperty("_privateProperties.useTk2dUIMask");
        SerializedProperty serializedDetailMaterial = serializedObject.FindProperty("_privateProperties.customDetailMaterial");
        SerializedProperty serializedFontTextureSize           = serializedObject.FindProperty("_privateProperties.fontSize");
		SerializedProperty serializedFontCharacterSize 		= serializedObject.FindProperty("_privateProperties.size");
        SerializedProperty serializedOverFlow = serializedObject.FindProperty("_privateProperties.overFlow");
        SerializedProperty serializedDrawSize = serializedObject.FindProperty("_privateProperties.drawSize");
		SerializedProperty serializedTextAnchor 		= serializedObject.FindProperty("_privateProperties.textAnchor");
		SerializedProperty serializedTextAlignment 		= serializedObject.FindProperty("_privateProperties.textAlignment");
		SerializedProperty serializedLineSpacing 		= serializedObject.FindProperty("_privateProperties.lineSpacing");
        SerializedProperty serializedWordSpacing = serializedObject.FindProperty("_privateProperties.wordSpacing");
        SerializedProperty serializedFontStyle = serializedObject.FindProperty("_privateProperties.fontStyle");
        SerializedProperty serializedFontFillType = serializedObject.FindProperty("_privateProperties.fillColorStyle");
		SerializedProperty serializedFontColorTop 		= serializedObject.FindProperty("_privateProperties.fontColorTop");
		SerializedProperty serializedFontColorBottom 	= serializedObject.FindProperty("_privateProperties.fontColorBottom");
		SerializedProperty serializedFillMaterial 		= serializedObject.FindProperty("_privateProperties.fillMaterial");
		SerializedProperty serializedEnableShadow 		= serializedObject.FindProperty("_privateProperties.enableShadow");
		SerializedProperty serializedShadowColor 		= serializedObject.FindProperty("_privateProperties.shadowColor");
		SerializedProperty serializedShadowDistance 	= serializedObject.FindProperty("_privateProperties.shadowDistance");
		SerializedProperty serializedEnableOutline 		= serializedObject.FindProperty("_privateProperties.enableOutline");
		SerializedProperty serializedOutlineColor 		= serializedObject.FindProperty("_privateProperties.outlineColor");
		SerializedProperty serializedOutlineWidth 		= serializedObject.FindProperty("_privateProperties.outLineWidth");
		SerializedProperty serializedOutLineQuality		= serializedObject.FindProperty("_privateProperties.outlineQuality");
		SerializedProperty serializedSortingLayerOrder	= serializedObject.FindProperty("_privateProperties.sortingLayerOrder");
		SerializedProperty serializedSortingLayerName	= serializedObject.FindProperty("_privateProperties.sortingLayerName");

        List<SerializedProperty> allSerializedProperties = new List<SerializedProperty>()
		{	
			serializedText, serializedRichText, serializedFontType, serializedUsePixelPerfect, serializedUseTk2dUIMask, serializedDetailMaterial, serializedFontTextureSize, serializedFontCharacterSize, serializedOverFlow, serializedDrawSize, serializedTextAnchor, serializedTextAlignment,
			serializedWordSpacing, serializedLineSpacing, serializedFontStyle, serializedFontFillType, serializedFontColorTop, serializedFontColorBottom, serializedFillMaterial, serializedEnableShadow, serializedShadowColor, serializedShadowDistance,
			serializedEnableOutline, serializedOutlineColor, serializedOutlineWidth, serializedOutLineQuality, serializedSortingLayerOrder, serializedSortingLayerName
		};
        
		#region properties

		GUIStyle textColor = new GUIStyle();

		//Render settings
    	SetBoldDefaultFont(serializedSortingLayerName);

		popupSortingLayersIndex = EditorGUILayout.Popup("Sorting Layer",popupSortingLayersIndex,sortingLayersNames);
		serializedSortingLayerName.stringValue = sortingLayersNames[popupSortingLayersIndex];

        //Sorting Layer Order
    	SetBoldDefaultFont(serializedSortingLayerOrder);
		EditorGUILayout.PropertyField(serializedSortingLayerOrder, new GUIContent("Order In layer", "Sets the Z shorting index in this layer"));

		//Text
		SetBoldDefaultFont(serializedText);
		
		EditorGUILayout.LabelField(new GUIContent("Text", "This is the text that is going to be used"));
		EditorGUILayout.BeginVertical("box");

		serializedText.stringValue = EditorGUILayout.TextArea(serializedText.stringValue);
		
		EditorGUILayout.EndVertical();

        //Rich Text
        GUILayout.BeginHorizontal();

    	SetBoldDefaultFont(serializedRichText);
        EditorGUILayout.PropertyField(serializedRichText, new GUIContent("Rich Text", "Enable/Disable Rich Text"));

        if (customFont.RichText)
        {
            showRichTextHelp = GUILayout.Toggle(showRichTextHelp, "?", EditorStyles.miniButton, GUILayout.ExpandWidth(false));
        }
        GUILayout.EndHorizontal();

        if (customFont.RichText && showRichTextHelp)
        {
            Color bg = GUI.backgroundColor;
            GUI.backgroundColor = new Color32(154, 176, 203, 255);
            string message = "Rich Text commands\n\n" +
                             "<b>text</b> - set text style bold\n" +
                             "<i>text</i> - set text style italic\n" +
                             "<bi>text</bi> - set text style bold and italic\n" +
                             "<color=#RGBA>text</color> - set text color\n" +
                             "<color=#RRGGBBAA>text</color> - set text color\n" +
                             "<gradient=#RGBA,#RGBA>text</gradient> - set text top and bottom colors\n" +
                             "<gradient=#RRGGBBAA,#RRGGBBAA>text</gradient> - set text top and bottom colors\n";
            EditorGUILayout.HelpBox(message, MessageType.Info);
            GUI.backgroundColor = bg;
        }

		//Font
		SetBoldDefaultFont(serializedFontType);

        //EditorGUILayout.PropertyField(serializedFontType, new GUIContent("Font", "The desired font type"));
        customFont.FontType = (Font)EditorGUILayout.ObjectField(new GUIContent("Font", "The desired font type"), customFont.FontType, typeof(Font), true);
		if (customFont.FontType == null)
		{
			customFont.FontType = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font; 
		}
		
        // Use Pixel Perfect
        SetBoldDefaultFont(serializedUsePixelPerfect);
        EditorGUILayout.PropertyField(serializedUsePixelPerfect, new GUIContent("Use Pixel Perfect", "Use Pixel Perfect"));
        if (!customFont.UsePixelPerfect)
        {
            EditorGUILayout.BeginVertical("box");
            ++EditorGUI.indentLevel;
            //Font TextureSize
            SetBoldDefaultFont(serializedFontTextureSize);
            EditorGUILayout.PropertyField(serializedFontTextureSize, new GUIContent("Texture Size", "Draw Texture Font Size"));
            --EditorGUI.indentLevel;
            EditorGUILayout.EndVertical();
        }

        // Use 2D Toolkit UIMask
        SetBoldDefaultFont(serializedUseTk2dUIMask);
        EditorGUILayout.PropertyField(serializedUseTk2dUIMask, new GUIContent("Use Tk2d UIMask", "Use 2D Toolkit UIMask"));

        if (customFont.UseTk2dUIMask)
        {
            string filename = customFont.FontType.name + "_tk2d";
            string fullpath = "Assets/RPGTextMesh/Material/" + filename + ".mat";
            if (!customFont.MainFontMaterial.name.Equals(filename))
            {
                Object asset = AssetDatabase.LoadAssetAtPath(fullpath, typeof(Material));
                if (asset)
                    customFont.MainFontMaterial = asset as Material;
                else
                {
                    Material mat = new Material(Shader.Find("RPG/RPGFont tk2d Text"));
                    mat.mainTexture = customFont.FontType.material.mainTexture;

                    string directory_name = System.IO.Path.GetDirectoryName(fullpath);
                    if (!System.IO.Directory.Exists(directory_name))
                        System.IO.Directory.CreateDirectory(directory_name);
                    AssetDatabase.CreateAsset(mat, fullpath);
                    Debug.Log(AssetDatabase.GetAssetPath(mat));
                    customFont.MainFontMaterial = mat;
                }
            }
        }
        else if (customFont.MainFontMaterial != null)
        {
            customFont.MainFontMaterial = null;
        }


		//Font Detail material
		SetBoldDefaultFont(serializedDetailMaterial);

		if (customFont.FillColorStyle != RPGTextMesh.FILL_COLOR_STYLE.textureGradient)
		{
			EditorGUILayout.PropertyField(serializedDetailMaterial, new GUIContent("Detail Material", "Used for additional FX"));
		}
		else
		{
			textColor.normal.textColor = Color.red;
			EditorGUILayout.BeginHorizontal();
			//EditorGUILayout.LabelField("Detail material is Disabled when TextureGradient Font Color Style is selected", textColor);
			EditorGUILayout.LabelField(new GUIContent( "Detail material","Disabled when TextureGradient Font Color Style is selected"), textColor);
			EditorGUILayout.LabelField(new GUIContent( "DISABLED", "Disabled when TextureGradient Font Color Style is selected"), textColor);
			EditorGUILayout.EndHorizontal();
		}

		//Font CharacterSize
        SetBoldDefaultFont(serializedFontCharacterSize);
        EditorGUILayout.PropertyField(serializedFontCharacterSize, new GUIContent("Font Size", "Draw Screen Font Size"));


        SetBoldDefaultFont(serializedOverFlow);
        EditorGUILayout.PropertyField(serializedOverFlow, new GUIContent("Overflow", "Use Type for DrawSize"));        

        //화면에 그려지는 사이즈.
        //Screen Demension
        SetBoldDefaultFont(serializedDrawSize);
        serializedDrawSize.vector2Value = EditorGUILayout.Vector2Field(new GUIContent("Draw Size", "Text Draw Size"), serializedDrawSize.vector2Value);

		//Text anchor
		SetBoldDefaultFont(serializedTextAnchor);
		EditorGUILayout.PropertyField(serializedTextAnchor, new GUIContent("Text Anchor", "Position of the texts pivot's point"));

		//Text alignment
		SetBoldDefaultFont(serializedTextAlignment);
		EditorGUILayout.PropertyField(serializedTextAlignment, new GUIContent("Text alignment", "Line alignment"));

		//Line spacing
		SetBoldDefaultFont(serializedLineSpacing);
		EditorGUILayout.PropertyField(serializedLineSpacing,  new GUIContent("Line spacing", "Distance between lines"));

        //Character spacing
        SetBoldDefaultFont(serializedWordSpacing);
        EditorGUILayout.PropertyField(serializedWordSpacing, new GUIContent("Word spacing", "word spacing"));

		//Font Style
        SetBoldDefaultFont(serializedFontStyle);
        EditorGUILayout.PropertyField(serializedFontStyle, new GUIContent("Font Style", "The font style"));

		//Font color
        SetBoldDefaultFont(serializedFontFillType);
		EditorGUILayout.PropertyField(serializedFontFillType, new GUIContent("Font Color Style", "The fill color style"));

        //fill color style
		EditorGUILayout.BeginVertical("box");
        ++EditorGUI.indentLevel;
		switch (customFont.FillColorStyle)
		{
            case RPGTextMesh.FILL_COLOR_STYLE.single:
    			SetBoldDefaultFont(serializedFontColorTop);
				EditorGUILayout.PropertyField(serializedFontColorTop, new GUIContent("Color", "Color for the top"));
			break;

            case RPGTextMesh.FILL_COLOR_STYLE.gradient:
				SetBoldDefaultFont(serializedFontColorTop);
				EditorGUILayout.PropertyField(serializedFontColorTop, new GUIContent("Top Color", "Color for the top"));
				
				SetBoldDefaultFont(serializedFontColorBottom);				
				EditorGUILayout.PropertyField(serializedFontColorBottom, new GUIContent("Bottom Color", "Color for the bottom"));	
			break;

            case RPGTextMesh.FILL_COLOR_STYLE.textureGradient:
				SetBoldDefaultFont(serializedFillMaterial);
				
				EditorGUILayout.PropertyField(serializedFillMaterial, new GUIContent("Fill material", "The desired fill material for the letters. Don't forget to assign the font texture correctly"));
				customFont.CustomDetailMaterial = null; //Detail material is not compatible with texture gradient because both uses second uv chanell
				
    			EditorGUILayout.HelpBox("Detail Material is not compatible with TextureGradient fill style. It will be automatically disabled", MessageType.Warning);
			break;
		}
        --EditorGUI.indentLevel;
		EditorGUILayout.EndVertical();	
		
		// Shadow
		SetBoldDefaultFont(serializedEnableShadow);		
		EditorGUILayout.PropertyField(serializedEnableShadow, new GUIContent("Enable Shadow", "Enable/Disable shadow"));
		
		if (customFont.EnableShadow) //Only show the options when enabled
		{
			EditorGUILayout.BeginVertical("box");
            ++EditorGUI.indentLevel;

			SetBoldDefaultFont(serializedShadowColor);			
			EditorGUILayout.PropertyField(serializedShadowColor, new GUIContent("Shadow color", "Sets the sahdow's color"));

			SetBoldDefaultFont(serializedShadowDistance);
			serializedShadowDistance.vector3Value = EditorGUILayout.Vector3Field(new GUIContent("Shadow distance", "The distance between the main characters and its shadow"), serializedShadowDistance.vector3Value);

            --EditorGUI.indentLevel;
			EditorGUILayout.EndVertical();
		}
		
		
		//Outline
		SetBoldDefaultFont(serializedEnableOutline);
		EditorGUILayout.PropertyField(serializedEnableOutline, new GUIContent("Enable Outline", "Enable/Disable the text's outline"));

		if (customFont.EnableOutline) //Only show the options when enabled
		{
			EditorGUILayout.BeginVertical("box");
            ++EditorGUI.indentLevel;

			SetBoldDefaultFont(serializedOutlineColor);
			EditorGUILayout.PropertyField(serializedOutlineColor, new GUIContent("Outline color", "Sets the ouline color"));

			SetBoldDefaultFont(serializedOutlineWidth);
			EditorGUILayout.PropertyField(serializedOutlineWidth,  new GUIContent("Outline width", "Sets the outline width"));

			SetBoldDefaultFont(serializedOutLineQuality);
			EditorGUILayout.PropertyField(serializedOutLineQuality, new GUIContent("High Quality", "Increase the number of vertex but gives better results"));

            --EditorGUI.indentLevel;
			EditorGUILayout.EndVertical();
		}
		
		#endregion
		
		#region buttons and info

        if (GUILayout.Button("Refresh"))
        {
            RefreshSelectedText();
        }
		
		if (GUILayout.Button("Refresh All"))
		{
			RefreshAllSceneText();
		}
		
		GUIStyle buttonStyleRed = new GUIStyle("button");
		buttonStyleRed.normal.textColor = Color.red;
		
		if (GUILayout.Button("Destroy Text component",buttonStyleRed))
		{
            Renderer tempRenderer = customFont.textRenderer;
			MeshFilter	tempMeshFilter = customFont.GetComponent<MeshFilter>();
			DestroyImmediate(customFont);
			DestroyImmediate(tempRenderer);
			DestroyImmediate(tempMeshFilter.sharedMesh);
			DestroyImmediate(tempMeshFilter);
			return;
		}
		

		textColor.normal.textColor = Color.green;
		EditorGUILayout.LabelField (string.Format("Vertex count {0}", customFont.GetVertexCount().ToString()),textColor);
        if (customFont.FontType != null)
        {
            if (customFont.FontType.material.mainTexture != null)
                EditorGUILayout.LabelField(string.Format("Font Texture Size {0} x {1}", customFont.FontType.material.mainTexture.width.ToString(), customFont.FontType.material.mainTexture.height.ToString()), textColor);
            else
            {
                textColor.normal.textColor = Color.red;
                EditorGUILayout.LabelField(string.Format("Font Texture Size {0}", "Texture is Null", textColor));
            }
        }
		#endregion
		
		#region prefab checks
		//Check if the prefab has changed to refresh the text
		bool checkCurrentPrefabModification = false;
		
		PropertyModification[] modifiedProperties = PrefabUtility.GetPropertyModifications((UnityEngine.Object)customFont);
		if (modifiedProperties != null && modifiedProperties.Length > 0)
		{
			for (int i = 0; i<modifiedProperties.Length; i++)
			{
				foreach (SerializedProperty serializerPropertyIterator in allSerializedProperties)
				{
					if (serializerPropertyIterator.propertyPath == modifiedProperties[i].propertyPath)
					{
						wasPrefabModified = true;
						checkCurrentPrefabModification = true;
					}
				}
			}
			
		}
		else
		{
			checkCurrentPrefabModification = false;			
		}
		
		if (wasPrefabModified && !checkCurrentPrefabModification)
		{
			RefreshAllSceneText();
			wasPrefabModified = false;
		}
		
		//Security check. If the mesh is null a prefab revert has been made
		if (customFont.GetComponent<MeshFilter>().sharedMesh == null)
			customFont.RefreshMeshEditor();
		#endregion

		serializedObject.ApplyModifiedProperties();

		//Track changes
		customFont.GUIChanged = GUI.changed;
		if (customFont.GUIChanged)
		{

			RefreshSelectedText();
			EditorUtility.SetDirty(customFont);
		}

		//If you undo with a multiple selection the GuiChange is not called.... So here it s a workaround
		if (Event.current.commandName == "UndoRedoPerformed") {
			RefreshSelectedText();
		}
	}
	
	
	void RefreshAllSceneText()
	{
        UnityEngine.Object[] customFonts = Resources.FindObjectsOfTypeAll(typeof(RPGTextMesh));
		
		if (customFonts.Length > 0)
		{
			for (int i= 0; i < customFonts.Length; i++)
			{
				if (AssetDatabase.GetAssetPath(customFonts[i]) == "") //Only affect the scene assets
				{
                    RPGTextMesh tempCustomFont = (RPGTextMesh)customFonts[i];	
					tempCustomFont.RefreshMeshEditor(); 
				}
			}
		}		
	}

	void RefreshSelectedText()
	{
		foreach(GameObject iteratorGameObject in Selection.gameObjects)
		{
            RPGTextMesh temp = iteratorGameObject.GetComponent<RPGTextMesh>();
			
			if (temp != null)
				temp.RefreshMeshEditor();
		}
	}

	private MethodInfo boldFontMethodInfo = null;
    private void SetBoldDefaultFont(SerializedProperty property)
    {
        if (property != null && property.isInstantiatedPrefab)
            SetBoldDefaultFont(property.prefabOverride);
    }

	private void SetBoldDefaultFont(bool value) {
	    
		boldFontMethodInfo = typeof(EditorGUIUtility).GetMethod("SetBoldDefaultFont", BindingFlags.Static | BindingFlags.NonPublic);
		boldFontMethodInfo.Invoke(null, new[] { value as object });
	}


	/// <summary>
	/// Gets the sorting layer names. This is a helper method because Unity doesn't expose the sorting layer names easily
	/// </summary>
	/// <returns>The sorting layer names.</returns>
	private string[] GetSortingLayerNames() 
	{
		System.Type internalEditorUtilityType = typeof(InternalEditorUtility);
		PropertyInfo sortingLayersProperty = internalEditorUtilityType.GetProperty("sortingLayerNames", BindingFlags.Static | BindingFlags.NonPublic);
		string[] sortingLayers = (string[])sortingLayersProperty.GetValue(null, new object[0]);
		
		return sortingLayers;

	}

    //int s_Hash = "RPGTextMesh".GetHashCode();

    public void OnSceneGUI()
    {
        RPGTextMesh customFont = target as RPGTextMesh;
        Transform t = customFont.transform;
        Vector2 localRectOrig = customFont.GetAnchorOffset();
        Rect localRect = new Rect(localRectOrig.x, localRectOrig.y, customFont.DrawSize.x, customFont.DrawSize.y);

        // Draw rect outline
        Handles.color = new Color(1, 1, 1, 0.5f);
        DrawRect(localRect, t);

        Handles.BeginGUI();

        // Resize handles
        if (RectControlsToggle())
        {
            EditorGUI.BeginChangeCheck();
            Rect resizeRect = RectControl(223192, localRect, t);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObjects(new UnityEngine.Object[] { t, customFont }, "Resize");
                customFont.ReshapeBounds(new Vector3(resizeRect.xMin, resizeRect.yMin) - new Vector3(localRect.xMin, localRect.yMin),
                    new Vector3(resizeRect.xMax, resizeRect.yMax) - new Vector3(localRect.xMax, localRect.yMax));
                EditorUtility.SetDirty(customFont);
            }
        }

        Handles.EndGUI();

        // Sprite selecting
        HandleSelectSprites();

        // Move targeted sprites
        HandleMoveSprites(t, localRect);

        if (GUI.changed)
        {
            EditorUtility.SetDirty(target);
        }
    }

    // Positive rect
    Rect PositiveRect(Rect r)
    {
        if (r.width < 0.0f) r = new Rect(r.xMax, r.yMin, -r.width, r.height);
        if (r.height < 0.0f) r = new Rect(r.xMin, r.yMax, r.width, -r.height);
        return r;
    }

    // Misc drawing
    void DrawRect(Rect r, Transform t)
    {
        Vector3 p0 = t.TransformPoint(new Vector3(r.xMin, r.yMin, 0));
        Vector3 p1 = t.TransformPoint(new Vector3(r.xMax, r.yMin, 0));
        Vector3 p2 = t.TransformPoint(new Vector3(r.xMax, r.yMax, 0));
        Vector3 p3 = t.TransformPoint(new Vector3(r.xMin, r.yMax, 0));
        Vector3[] worldPts = new Vector3[5] { p0, p1, p2, p3, p0 };
        Handles.DrawPolyLine(worldPts);
    }

    // Handle dragging sprite positions
    List<Transform> dragObjCachedTransforms = new List<Transform>();
    List<Vector3> dragObjStartPos = new List<Vector3>();
    Vector3 dragOrigin = Vector3.zero;
    Plane dragPlane = new Plane();
    void HandleMoveSprites(Transform t, Rect rect)
    {
        Event ev = Event.current;

        // Break out if vertex modifier is active
        if (IsSnapToVertexActive())
        {
            return;
        }

        int controlId = t.GetInstanceID();
        Ray mouseRay = HandleUtility.GUIPointToWorldRay(ev.mousePosition);
        rect = PositiveRect(rect);

        if (ev.type == EventType.MouseDown && ev.button == 0 && !ev.control && !ev.alt && !ev.command && !ev.shift)
        {
            float hitD = 0.0f;
            dragPlane = new Plane(t.forward, t.position);
            if (dragPlane.Raycast(mouseRay, out hitD))
            {
                Vector3 intersect = mouseRay.GetPoint(hitD);
                Vector3 pLocal = t.InverseTransformPoint(intersect);

                if (pLocal.x >= rect.xMin && pLocal.x <= rect.xMax &&
                    pLocal.y >= rect.yMin && pLocal.y <= rect.yMax)
                {
                    // Mousedown on our sprite

                    // Store current selected objects transforms
                    dragObjCachedTransforms.Clear();
                    dragObjStartPos.Clear();
                    for (int i = 0; i < Selection.gameObjects.Length; ++i)
                    {
                        Transform objTransform = Selection.gameObjects[i].transform;
                        dragObjCachedTransforms.Add(objTransform);
                        dragObjStartPos.Add(objTransform.position);
                    }
                    dragOrigin = intersect;

                    GUIUtility.hotControl = controlId;
                    ev.Use();
                }
            }
        }
        if (GUIUtility.hotControl == controlId)
        { // Handle drag / mouseUp
            switch (ev.GetTypeForControl(controlId))
            {
                case EventType.MouseDrag:
                    {
                        float hitD = 0.0f;
                        if (dragPlane.Raycast(mouseRay, out hitD))
                        {
                            Vector3 intersect = mouseRay.GetPoint(hitD);
                            Vector3 offset = intersect - dragOrigin;
                            if (ev.shift)
                            {
                                float x = Mathf.Abs(Vector3.Dot(offset, t.right));
                                float y = Mathf.Abs(Vector3.Dot(offset, t.up));
                                offset = Vector3.Project(offset, (x > y) ? t.right : t.up);
                            }

                            Undo.RecordObjects(dragObjCachedTransforms.ToArray(), "Move");

                            for (int i = 0; i < Selection.gameObjects.Length; ++i)
                            {
                                Selection.gameObjects[i].transform.position = dragObjStartPos[i] + offset;
                            }
                        }
                        break;
                    }

                case EventType.MouseUp:
                    {
                        GUIUtility.hotControl = 0;
                        ev.Use();
                        break;
                    }
            }
        }
    }

    static Vector2 mouseDownPos = Vector2.zero;

    // Handle selecting other sprites
    public static void HandleSelectSprites()
    {
        Event ev = Event.current;
        if (Tools.current == Tool.View)
        {
            if (ev.type == EventType.MouseDown && ev.button == 0)
            {
                mouseDownPos = ev.mousePosition;
            }
            if (ev.type == EventType.MouseUp && ev.button == 0 && ev.mousePosition == mouseDownPos)
            {

                bool changedSelection = false;

                List<Object> gos = new List<Object>(Selection.objects);
                Object go = HandleUtility.PickGameObject(ev.mousePosition, false);
                if (go != null)
                {
                    if (ev.shift)
                    {
                        if (gos.Contains(go))
                        {
                            gos.Remove(go);
                        }
                        else
                        {
                            gos.Add(go);
                        }
                        changedSelection = true;
                    }
                    else
                    {
                        if (!gos.Contains(go))
                        {
                            gos.Clear();
                            gos.Add(go);
                            changedSelection = true;
                        }
                    }
                }
                else
                {
                    if (!ev.shift)
                    {
                        gos.Clear();
                        changedSelection = true;
                    }
                }
                if (changedSelection)
                {
                    Selection.objects = gos.ToArray();
                    ev.Use();
                }
            }
        }
    }

    // Are we enabling resize controls, or rotate controls?
    bool RectControlsToggle()
    {
        bool result = true;
        if (Event.current.alt) result = !result;
        if (Tools.current == Tool.Rotate) result = !result;
        return result;
    }

    // Cursor stuff
    MouseCursor GetHandleCursor(Vector2 n, Transform objT)
    {
        n.Normalize();
        Vector3 worldN = new Vector3(n.x, n.y, 0);
        worldN = objT.TransformDirection(worldN);
        worldN = Vector3.Scale(worldN, objT.localScale);
        Vector3 screenN = worldN;

        bool useSceneCam = true;
        if (useSceneCam)
        {
            SceneView sceneview = SceneView.lastActiveSceneView;
            if (sceneview != null)
            {
                Camera sceneCam = sceneview.camera;
                if (sceneCam != null)
                {
                    screenN = sceneCam.transform.InverseTransformDirection(screenN);
                }
            }
        }

        Vector2[] cursorVec = new Vector2[] {
			new Vector2(1.0f, 0.0f), new Vector2(0.0f, 1.0f),
			new Vector2(1.0f, 1.0f), new Vector2(-1.0f, 1.0f)
		};
        MouseCursor[] cursors = new MouseCursor[] {
			MouseCursor.ResizeHorizontal, MouseCursor.ResizeVertical,
			MouseCursor.ResizeUpRight, MouseCursor.ResizeUpLeft
		};
        float maxDP = 0.0f;
        int maxInd = 0;
        for (int i = 0; i < 4; ++i)
        {
            Vector2 v = cursorVec[i];
            v.Normalize();
            float dp = Mathf.Abs(v.x * screenN.x + v.y * screenN.y);
            if (dp > maxDP)
            {
                maxDP = dp;
                maxInd = i;
            }
        }
        return cursors[maxInd];
    }

    const float handleClosenessClip = 10.0f; // Don't draw handles when the rect gets this thin (screenspace)
    bool vertexMoveModifierDown = false;

    bool IsSnapToVertexActive()
    {
        Event ev = Event.current;
        if (ev.isKey && ev.keyCode == KeyCode.V)
        {
            if (ev.type == EventType.KeyDown) vertexMoveModifierDown = true;
            else if (ev.type == EventType.KeyUp) vertexMoveModifierDown = false;
        }
        return (Tools.current == Tool.Move && vertexMoveModifierDown);
    }

    // For constrain proportions
    Rect constrainRectTemp = new Rect();
    Rect constrainRect = new Rect();
    Matrix4x4 constrainRectMatrixTemp = Matrix4x4.zero;
    Matrix4x4 constrainRectMatrix = Matrix4x4.zero;

    // A draggable point
    Vector3 MoveHandle(int id, Vector3 worldPos, Vector3 planeNormal, GUIStyle style, MouseCursor cursor)
    {
        // If handle is behind camera,
        SceneView sceneview = SceneView.lastActiveSceneView;
        if (sceneview != null)
        {
            Camera sceneCam = sceneview.camera;
            if (sceneCam != null)
            {
                Vector3 camSpace = sceneCam.transform.InverseTransformPoint(worldPos);
                if (camSpace.z < 0.0f)
                {
                    // then don't do this MoveHandle
                    return worldPos;
                }
            }
        }

        Event ev = Event.current;
        Vector2 guiPoint = HandleUtility.WorldToGUIPoint(worldPos);
        
        //int handleSize = (int)style.fixedWidth;
        int handleSize = (int)Mathf.Max(style.fixedWidth, style.fixedHeight);
        bool selected = GUIUtility.hotControl == id;
        Rect handleRect = new Rect(guiPoint.x - handleSize / 2, guiPoint.y - handleSize / 2, handleSize, handleSize);
        if (GUIUtility.hotControl == id || (GUIUtility.hotControl != id && !ev.shift))
        {
            EditorGUIUtility.AddCursorRect(handleRect, cursor);
        }

        if ((GUIUtility.hotControl == id || (GUIUtility.hotControl != id && !ev.shift)) && ev.type == EventType.Repaint)
        {
            style.Draw(handleRect, selected, false, false, false);
        }

        if (ev.type == EventType.MouseDown && ev.button == 0 && handleRect.Contains(ev.mousePosition) && !ev.shift)
        {
            constrainRect = constrainRectTemp;
            constrainRectMatrix = constrainRectMatrixTemp;
            GUIUtility.hotControl = id;
            ev.Use();
        }
        else if (GUIUtility.hotControl == id)
        {
            switch (ev.GetTypeForControl(id))
            {
                case EventType.MouseDrag:
                    {
                        Plane p = new Plane(planeNormal, worldPos);
                        Ray r = HandleUtility.GUIPointToWorldRay(ev.mousePosition);
                        float d = 0;
                        if (p.Raycast(r, out d))
                        {
                            Vector3 hitPoint = r.GetPoint(d);
                            GUI.changed = true;
                            worldPos = hitPoint;
                        }
                        ev.Use();
                        break;
                    }
                case EventType.MouseUp:
                    {
                        GUIUtility.hotControl = 0;
                        ev.Use();
                        break;
                    }
            }
        }
        return worldPos;
    }

    // 8 draggable points around the border (resizing)
    Rect RectControl(int controlId, Rect r, Transform t)
    {
        Event ev = Event.current;

        // Break out if vertex modifier is active
        if (IsSnapToVertexActive())
        {
            return r;
        }        
        bool guiChanged = false;
        GUIStyle style = "sv_label_1";
        
        Vector2 rSign = new Vector2(Mathf.Sign(r.width), Mathf.Sign(r.height));
        r = PositiveRect(r);

        constrainRectTemp = r;
        constrainRectMatrixTemp = t.localToWorldMatrix;

        Vector3[] localPts = new Vector3[] {
			new Vector3(r.xMin + r.width * 0.0f, r.yMin + r.height * 0.0f, 0),
			new Vector3(r.xMin + r.width * 0.5f, r.yMin + r.height * 0.0f, 0),
			new Vector3(r.xMin + r.width * 1.0f, r.yMin + r.height * 0.0f, 0),
			new Vector3(r.xMin + r.width * 0.0f, r.yMin + r.height * 0.5f, 0),
			new Vector3(r.xMin + r.width * 1.0f, r.yMin + r.height * 0.5f, 0),
			new Vector3(r.xMin + r.width * 0.0f, r.yMin + r.height * 1.0f, 0),
			new Vector3(r.xMin + r.width * 0.5f, r.yMin + r.height * 1.0f, 0),
			new Vector3(r.xMin + r.width * 1.0f, r.yMin + r.height * 1.0f, 0),
		};

        Vector3[] worldPts = new Vector3[8];
        Vector2[] guiPts = new Vector2[8];
        bool[] handleVisible = new bool[8];
        for (int i = 0; i < 8; ++i)
        {
            worldPts[i] = t.TransformPoint(localPts[i]);
            guiPts[i] = HandleUtility.WorldToGUIPoint(worldPts[i]);
            handleVisible[i] = true;
        }

        // Hide handles if screen distance gets too small
        {
            float edgeLengthBottom = (guiPts[0] - guiPts[2]).magnitude;
            float edgeLengthTop = (guiPts[5] - guiPts[7]).magnitude;
            float edgeLengthLeft = (guiPts[0] - guiPts[5]).magnitude;
            float edgeLengthRight = (guiPts[2] - guiPts[7]).magnitude;
            if (edgeLengthBottom < handleClosenessClip || edgeLengthTop < handleClosenessClip ||
                edgeLengthLeft < handleClosenessClip || edgeLengthRight < handleClosenessClip)
            {
                for (int i = 0; i < 8; ++i)
                {
                    handleVisible[i] = false;
                }
            }
            else
            {
                if (edgeLengthBottom < 2.0f * handleClosenessClip || edgeLengthTop < 2.0f * handleClosenessClip)
                {
                    handleVisible[1] = handleVisible[6] = false;
                }
                if (edgeLengthLeft < 2.0f * handleClosenessClip || edgeLengthRight < 2.0f * handleClosenessClip)
                {
                    handleVisible[3] = handleVisible[4] = false;
                }
            }
        }

        Vector2[] handleCursorN = new Vector2[] {
			new Vector2(-1.0f, -1.0f), new Vector2(0.0f, -1.0f), new Vector2(1.0f, -1.0f),
			new Vector2(-1.0f, 0.0f), new Vector2(1.0f, 0.0f),
			new Vector2(-1.0f, 1.0f), new Vector2(0.0f, 1.0f), new Vector2(1.0f, 1.0f)
		};

        for (int i = 0; i < 8; ++i)
        {
            if ((Event.current.type == EventType.Repaint || Event.current.type == EventType.MouseDown) && !handleVisible[i]) continue;

            Vector3 worldPt = worldPts[i];
            MouseCursor cursor = GetHandleCursor(handleCursorN[i], t);

            EditorGUI.BeginChangeCheck();
            Vector3 newWorldPt = MoveHandle(controlId + t.GetInstanceID() + "Handle".GetHashCode() + i, worldPt, t.forward, style, cursor);
            if (EditorGUI.EndChangeCheck())
            {
                Vector3 localPt = ev.shift ? constrainRectMatrix.inverse.MultiplyPoint(newWorldPt) : t.InverseTransformPoint(newWorldPt);
                Vector3 v0 = new Vector3(r.xMin, r.yMin, 0);
                Vector3 v1 = v0 + new Vector3(r.width, r.height, 0);

                // constrain axis
                if (i == 3 || i == 4) localPt.y = localPts[i].y;
                if (i == 1 || i == 6) localPt.x = localPts[i].x;

                // calculate new extrema
                if (!ev.shift)
                {
                    if (i == 0 || i == 3 || i == 5) v0.x = Mathf.Min(v1.x, localPt.x);
                    if (i == 0 || i == 1 || i == 2) v0.y = Mathf.Min(v1.y, localPt.y);
                    if (i == 2 || i == 4 || i == 7) v1.x = Mathf.Max(v0.x, localPt.x);
                    if (i == 5 || i == 6 || i == 7) v1.y = Mathf.Max(v0.y, localPt.y);
                }
                else
                {
                    // constrain proportions
                    v0 = new Vector3(constrainRect.xMin, constrainRect.yMin, 0);
                    v1 = v0 + new Vector3(constrainRect.width, constrainRect.height, 0);
                    if (i == 0 || i == 3 || i == 5)
                    {
                        v0.x = Mathf.Min(v1.x, localPt.x);
                        float sy0 = (i == 0) ? 1.0f : ((i == 3) ? 0.5f : 0.0f);
                        float dy = constrainRect.height * ((v1.x - v0.x) / constrainRect.width - 1.0f);
                        v0.y -= dy * sy0;
                        v1.y += dy * (1.0f - sy0);
                    }
                    if (i == 2 || i == 4 || i == 7)
                    {
                        v1.x = Mathf.Max(v0.x, localPt.x);
                        float sy0 = (i == 2) ? 1.0f : ((i == 4) ? 0.5f : 0.0f);
                        float dy = constrainRect.height * ((v1.x - v0.x) / constrainRect.width - 1.0f);
                        v0.y -= dy * sy0;
                        v1.y += dy * (1.0f - sy0);
                    }
                    if (i == 1 || i == 6)
                    {
                        if (i == 1) v0.y = Mathf.Min(v1.y, localPt.y);
                        else v1.y = Mathf.Max(v0.y, localPt.y);
                        float dx = constrainRect.width * ((v1.y - v0.y) / constrainRect.height - 1.0f);
                        v0.x -= dx * 0.5f;
                        v1.x += dx * 0.5f;
                    }

                    v0 = constrainRectMatrix.MultiplyPoint(v0);
                    v1 = constrainRectMatrix.MultiplyPoint(v1);
                    v0 = t.InverseTransformPoint(v0);
                    v1 = t.InverseTransformPoint(v1);
                }

                guiChanged = true;
                r.Set(v0.x, v0.y, v1.x - v0.x, v1.y - v0.y);
                HandleUtility.Repaint();
            }
        }

        if (guiChanged)
        {
            GUI.changed = true;
        }

        if (rSign.x < 0.0f) r = new Rect(r.xMax, r.yMin, -r.width, r.height);
        if (rSign.y < 0.0f) r = new Rect(r.xMin, r.yMax, r.width, -r.height);
        return r;
    }

}
