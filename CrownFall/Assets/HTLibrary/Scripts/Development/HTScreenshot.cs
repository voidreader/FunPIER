#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;


namespace HT
{
	/////////////////////////////////////////
	//---------------------------------------
	[ExecuteInEditMode]
	public class HTScreenShot : EditorWindow
	{
		//---------------------------------------
		public Camera _myCamera;
		public string _lastScreenshot = "";

		//---------------------------------------
		private int _resWidth = Screen.width * 4;
		private int _resHeight = Screen.height * 4;
		private int _scale = 1;

		//---------------------------------------
		private string _path = "";
		private RenderTexture _renderTexture;

		//---------------------------------------
		private bool _isTransparent = false;
		private bool _takeHiResShot = false;

		//---------------------------------------
		[MenuItem("Window/HTLibrary/Screenshot")]
		public static void ShowWindow()
		{
			//Show existing window instance. If one doesn't exist, make one.
			EditorWindow editorWindow = EditorWindow.GetWindow(typeof(HTScreenShot));
			editorWindow.autoRepaintOnSceneChange = true;
			editorWindow.Show();
			editorWindow.titleContent = new GUIContent("ScreenShot");
		}

		//---------------------------------------
		void OnGUI()
		{
			EditorGUILayout.LabelField("Resolution", EditorStyles.boldLabel);
			_resWidth = EditorGUILayout.IntField("Width", _resWidth);
			_resHeight = EditorGUILayout.IntField("Height", _resHeight);

			//-----
			EditorGUILayout.Space();
			_scale = EditorGUILayout.IntSlider("Scale", _scale, 1, 15);

			EditorGUILayout.HelpBox("The default mode of screenshot is crop - so choose a proper width and height. The scale is a factor " +
				"to multiply or enlarge the renders without loosing quality.", MessageType.None);
			
			//-----
			EditorGUILayout.Space();
			GUILayout.Label("Save Path", EditorStyles.boldLabel);

			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.TextField(_path, GUILayout.ExpandWidth(false));
			if (GUILayout.Button("Browse", GUILayout.ExpandWidth(false)))
				_path = EditorUtility.SaveFolderPanel("Path to Save Images", _path, Application.dataPath);

			EditorGUILayout.EndHorizontal();

			EditorGUILayout.HelpBox("Choose the folder in which to save the screenshots ", MessageType.None);

			//-----
			EditorGUILayout.Space();
			GUILayout.Label("Select Camera", EditorStyles.boldLabel);

			_myCamera = EditorGUILayout.ObjectField(_myCamera, typeof(Camera), true, null) as Camera;
			if (_myCamera == null)
				_myCamera = HTFramework.LastCamera;

			_isTransparent = EditorGUILayout.Toggle("Transparent Background", _isTransparent);

			EditorGUILayout.HelpBox("Choose the camera of which to capture the render. You can make the background transparent using the transparency option.", MessageType.None);

			//-----
			EditorGUILayout.Space();
			EditorGUILayout.BeginVertical();
			EditorGUILayout.LabelField("Default Options", EditorStyles.boldLabel);

			if (GUILayout.Button("Set To Screen Size"))
			{
				_resHeight = (int)Handles.GetMainGameViewSize().y;
				_resWidth = (int)Handles.GetMainGameViewSize().x;
			}

			if (GUILayout.Button("Default Size"))
			{
				_resHeight = 1440;
				_resWidth = 2560;
				_scale = 1;
			}

			EditorGUILayout.EndVertical();

			//-----
			EditorGUILayout.Space();
			EditorGUILayout.LabelField("Screenshot will be taken at " + _resWidth * _scale + " x " + _resHeight * _scale + " px", EditorStyles.boldLabel);

			if (GUILayout.Button("Take Screenshot", GUILayout.MinHeight(60)))
			{
				if (_path == "")
				{
					_path = EditorUtility.SaveFolderPanel("Path to Save Images", _path, Application.dataPath);
					Debug.Log("Path Set");
					TakeHiResShot();
				}
				else
				{
					TakeHiResShot();
				}
			}

			//-----
			EditorGUILayout.Space();
			EditorGUILayout.BeginHorizontal();

			if (GUILayout.Button("Open Last Screenshot", GUILayout.MaxWidth(160), GUILayout.MinHeight(40)))
			{
				if (_lastScreenshot != "")
				{
					Application.OpenURL("file://" + _lastScreenshot);
					Debug.Log("Opening File " + _lastScreenshot);
				}
			}

			if (GUILayout.Button("Open Folder", GUILayout.MaxWidth(100), GUILayout.MinHeight(40)))
				Application.OpenURL("file://" + _path);

			EditorGUILayout.EndHorizontal();

			//-----
			if (_takeHiResShot)
			{
				int resWidthN = _resWidth * _scale;
				int resHeightN = _resHeight * _scale;
				RenderTexture rt = new RenderTexture(resWidthN, resHeightN, 24);
				_myCamera.targetTexture = rt;

				TextureFormat tFormat;
				if (_isTransparent)
					tFormat = TextureFormat.ARGB32;
				else
					tFormat = TextureFormat.RGB24;

				Texture2D screenShot = new Texture2D(resWidthN, resHeightN, tFormat, false);
				_myCamera.Render();
				RenderTexture.active = rt;
				screenShot.ReadPixels(new Rect(0, 0, resWidthN, resHeightN), 0, 0);
				_myCamera.targetTexture = null;
				RenderTexture.active = null;
				byte[] bytes = screenShot.EncodeToPNG();
				string filename = ScreenShotName(resWidthN, resHeightN);

				System.IO.File.WriteAllBytes(filename, bytes);
				Debug.Log(string.Format("[ScreenShot] Took screenshot to: {0}", filename));
				Application.OpenURL(filename);
				_takeHiResShot = false;
			}
		}

		//---------------------------------------
		public string ScreenShotName(int width, int height)
		{
			string szDateTime = System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
			_lastScreenshot = string.Format("{0}/screen_{1}x{2}_{3}.png", _path, width, height, szDateTime);

			return _lastScreenshot;
		}
		
		public void TakeHiResShot()
		{
			Debug.Log("[ScreenShot] Taking screenshot...");
			_takeHiResShot = true;
		}

		//---------------------------------------
	}


	/////////////////////////////////////////
	//---------------------------------------
}

#endif // UNITY_EDITOR