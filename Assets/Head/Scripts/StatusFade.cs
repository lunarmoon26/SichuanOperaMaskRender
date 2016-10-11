using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace SichuanOpera
{
	public class StatusFade : MonoBehaviour {

		public bool show = false;
		public float flashSpeed = 5f;                              
		public Color panelColour = new Color(0f, 0f, 0f, 0.2f); 
		public Color textColour = new Color(1f, 1f, 1f, 1f); 

		private Image panelImage;
		private Text statusText;
		
		// Use this for initialization
		void Awake () {
			panelImage = GetComponent<Image> ();
			statusText = GetComponentInChildren<Text> ();
		}
		
		// Update is called once per frame
		void Update () {
			if(show)
			{
				panelImage.color = panelColour;
				statusText.color = textColour;
			}
			else
			{
				panelImage.color = Color.Lerp (panelImage.color, Color.clear, flashSpeed * Time.deltaTime);
				statusText.color = Color.Lerp (statusText.color, Color.clear, flashSpeed * Time.deltaTime);
			}
			
			// Reset the damaged flag.
			show = false;
		}

		public void ShowStatus(string msg)
		{
			statusText.text = msg;
			show = true;
		}
	}
}
