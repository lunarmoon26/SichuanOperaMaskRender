using UnityEngine;
using System.Collections;
using System.IO;
//using UnityEditor;
using UnityEngine.UI;

namespace SichuanOpera
{
	public class GameMaster : MonoBehaviour {
		[SerializeField] private StatusFade m_StatusPanel;
		[SerializeField] private Text m_ConsoleText;
		//[SerializeField] private Slider m_RenderProcessBar;
		[SerializeField] private Material m_MaskMaterial;
		[SerializeField] private Slider m_QualitySlider;
		[SerializeField] private Text m_QualityText;
		[SerializeField] private Slider m_FrameNumberSlider;
		[SerializeField] private Text m_FrameNumberText;
		[SerializeField] private Camera m_RenderCamera;
		[SerializeField] private Camera m_MainCamera;
		[SerializeField] private Animator m_HeadAnimator;
		[SerializeField] private Toggle m_Inverse;
		[SerializeField] private Transform m_HeadRoot;

		private float m_QualityWidthMultiplier = 80f;
		private float m_QualityHeightMultiplier = 100f;
		private float m_FrameNumberMultiplier = 36f;
		private Rect m_CaptureRect;
		private string m_Prefix = "New_Sequence";
		private string m_LoadDir;

		//skins and textures
		public GUISkin skin;
		public Texture2D file,folder,back,drive;
		
		//initialize file browser
		FileBrowser fb = new FileBrowser();
		bool showfb = false;

		// Use this for initialization
		void Start () {
			m_FrameNumberText.text = (m_FrameNumberSlider.value * m_FrameNumberMultiplier).ToString ();
			m_QualityText.text = m_QualitySlider.value.ToString ();
			m_CaptureRect = new Rect (0, 0, m_QualitySlider.value * m_QualityWidthMultiplier, m_QualitySlider.value * m_QualityHeightMultiplier);
			m_LoadDir = Application.dataPath;

			//setup file browser style
			fb.guiSkin = skin; //set the starting skin
			//set the various textures
			fb.fileTexture = file; 
			fb.directoryTexture = folder;
			fb.backTexture = back;
			fb.driveTexture = drive;
			fb.searchPattern = "*.png";
			//show the search bar
			//fb.showSearch = true;
			//search recursively (setting recursive search may cause a long delay)
			//fb.searchRecursively = true;
		}

		void OnGUI()
		{
			if (showfb) {
				if (fb.draw ()) { //true is returned when a file has been selected
					showfb = false;
					//the output file is a member if the FileInfo class, if cancel was selected the value is null
					string path = (fb.outputFile == null) ? "" : fb.outputFile.FullName;
					if (path.Length != 0) {
						m_ConsoleText.text = path;
						m_Prefix = Path.GetFileNameWithoutExtension(path) + "_Sequence";
						m_LoadDir = Path.GetDirectoryName(path);

						WWW www = new WWW("file:///" + path);
						m_MaskMaterial.mainTexture = www.texture;
					}
				}
			}
		}

		public void OnFrameNumberChange()
		{
			m_FrameNumberText.text = (m_FrameNumberSlider.value * m_FrameNumberMultiplier).ToString ();
		}

		public void OnQualityChange()
		{
			m_QualityText.text = m_QualitySlider.value.ToString ();
			m_CaptureRect = new Rect (0, 0, m_QualitySlider.value * m_QualityWidthMultiplier, m_QualitySlider.value * m_QualityHeightMultiplier);
		}

		public void OnClickLoadTexture()
		{
			showfb = true;

			/*
			string path = EditorUtility.OpenFilePanel(
				"选择贴图",
				m_LoadDir,
				"png");
			
			if (path.Length != 0) {
				m_Prefix = Path.GetFileNameWithoutExtension(path);
				m_LoadDir = Path.GetDirectoryName(path);

				WWW www = new WWW("file:///" + path);
				m_MaskMaterial.mainTexture = www.texture;
			}
			*/
		}

		public void OnClickStartRender()
		{
			if (!Directory.Exists (m_LoadDir + "/" + m_Prefix)) Directory.CreateDirectory(m_LoadDir + "/" + m_Prefix);
			string path = m_LoadDir + "/" + m_Prefix;

			if (path.Length != 0) {

				// Initialize render setups
				RenderTexture currentRT = RenderTexture.active;
				RenderTexture currentTT = m_RenderCamera.targetTexture;
				bool isAutoRotating = m_HeadAnimator.enabled;
				Quaternion headRotation = m_HeadAnimator.gameObject.transform.rotation;

				// Start rendering...
				m_HeadAnimator.enabled = false;
				float frameNumber = m_FrameNumberSlider.value * m_FrameNumberMultiplier;
				float deltaAngle = m_Inverse.isOn ? -360/frameNumber : 360/frameNumber;
				RenderTexture rt = new RenderTexture ((int)m_CaptureRect.width, (int)m_CaptureRect.height, 16);  
				m_RenderCamera.targetTexture = rt; 
				RenderTexture.active = rt;
				Texture2D image = new Texture2D ((int)m_CaptureRect.width, (int)m_CaptureRect.height, TextureFormat.ARGB32, false);

				for(int i = 0; i < (int)frameNumber; i++)
				{
					//m_RenderPercentage = i/frameNumber;
					m_HeadAnimator.gameObject.transform.rotation = Quaternion.Euler(0, m_HeadRoot.rotation.eulerAngles.y + deltaAngle * i, 0);
					m_RenderCamera.Render (); 

					image.ReadPixels (m_CaptureRect, 0, 0);
					image.Apply ();
					byte[] bytes = image.EncodeToPNG ();  
					string filename = path + "/" + m_Prefix + "_" + (i + 1) + ".png";
					System.IO.File.WriteAllBytes (filename, bytes);
				}
				GameObject.Destroy (rt);

				// Reset viewport
				m_HeadAnimator.enabled = isAutoRotating;
				m_HeadAnimator.gameObject.transform.rotation = headRotation;
				m_RenderCamera.targetTexture = currentTT;
				RenderTexture.active = currentRT;

				m_StatusPanel.gameObject.SetActive(true);
				//EditorUtility.DisplayDialog("渲染完成", "图片序列已渲染完成并保存到指定文件夹", "OK");
			}
		}

		public void OnClickQuit()
		{
			Application.Quit ();
		}
	}
}
