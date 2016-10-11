using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace SichuanOpera
{
	public class Controller : MonoBehaviour {
		[SerializeField] private Slider m_RotationSlider;
		[SerializeField] private Slider m_ZoomSlider;
		[SerializeField] private Slider m_VerticalOffsetSlider;
		[SerializeField] private Toggle m_RotationInverse;
		[SerializeField] private Toggle m_AutoRotate;

		private float m_InitZPosition;
		private float m_InitVertOffset;
		private bool m_InitIsRotationInverseOn;

		private Animator m_RotateAnimator;
		private bool m_IsMouseRotate;
		private float m_TurnSpeed = 10f;
		private float m_TurnSmoothing = 0.1f;
		private Quaternion m_TransformTargetRot;

		// Use this for initialization
		void Start () {
			m_InitZPosition = transform.position.z;
			m_InitVertOffset = transform.position.y;
			m_InitIsRotationInverseOn = m_RotationInverse.isOn;

			m_ZoomSlider.value = m_InitZPosition;
			m_VerticalOffsetSlider.value = m_InitVertOffset;

			m_RotateAnimator = GetComponentInChildren<Animator> ();
		}
		
		// Update is called once per frame
		void Update () {
			if (Input.GetButtonDown("Fire2")) m_IsMouseRotate = true;
			if (Input.GetButtonUp("Fire2")) m_IsMouseRotate = false;

			if (m_IsMouseRotate) {
				// Read the user input
				var x = Input.GetAxis ("Mouse X");
				//var y = Input.GetAxis ("Mouse Y");
			
				// Adjust the look angle by an amount proportional to the turn speed and horizontal input.
				float rot = m_RotationSlider.value;
				rot -= x * m_TurnSpeed;
				if(rot > 360) rot -= 360;
				if(rot < 0) rot += 360;
				m_RotationSlider.value = rot;
			
				// Rotate the Root (the root of head) around Y axis only:
				m_TransformTargetRot = Quaternion.Euler (0f, m_RotationSlider.value, 0f);
			
				// on platforms with a mouse, we adjust the current angle based on Y mouse input and turn speed
				//m_TiltAngle -= y * m_TurnSpeed;
				// and make sure the new value is within the tilt range
				//m_TiltAngle = Mathf.Clamp (m_TiltAngle, -m_TiltMin, m_TiltMax);
				// Tilt input around X is applied to the pivot (the child of this object)
				//m_PivotTargetRot = Quaternion.Euler (m_TiltAngle, m_PivotEulers.y, m_PivotEulers.z);
				//m_Pivot.localRotation = Quaternion.Slerp (m_Pivot.localRotation, m_PivotTargetRot, m_TurnSmoothing * Time.deltaTime);
				transform.localRotation = Quaternion.Slerp (transform.localRotation, m_TransformTargetRot, m_TurnSmoothing * Time.deltaTime);
			}
		}

		public void OnChangeInverseRotation()
		{
			if (m_RotationInverse.isOn) {
				m_RotateAnimator.speed = 1;
			} else {
				m_RotateAnimator.speed = -1;
			}
		}

		public void OnChangeAutoRotate()
		{
			m_RotateAnimator.enabled = m_AutoRotate.isOn;
			if(!m_AutoRotate.isOn) m_RotateAnimator.gameObject.transform.rotation = transform.rotation;
		}

		public void OnChangeInitialRotation()
		{
			transform.rotation = Quaternion.Euler (0, m_RotationSlider.value, 0);
		}

		public void OnChangeZoom()
		{
			transform.position = new Vector3 (transform.position.x, transform.position.y, m_ZoomSlider.value);
		}

		public void OnChangeVerticalOffset()
		{
			transform.position = new Vector3 (transform.position.x, m_VerticalOffsetSlider.value, transform.position.z);
		}

		public void OnClickResetButton()
		{
			m_ZoomSlider.value = m_InitZPosition;
			m_VerticalOffsetSlider.value = m_InitVertOffset;

			m_RotationInverse.isOn = m_InitIsRotationInverseOn;

			m_RotationSlider.value = 0;
			transform.rotation = Quaternion.Euler (0, 0, 0);
			m_RotateAnimator.gameObject.transform.rotation = transform.rotation;
		}
	}
}
