using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityStandardAssets.Effects;

namespace UnityStandardAssets.SceneUtils
{
	public class ParticleSceneControls : MonoBehaviour
	{
		public enum Mode
		{
			Activate,
			Instantiate,
			Trail
		}

		public enum AlignMode
		{
			Normal,
			Up
		}

		[Serializable]
		public class DemoParticleSystem
		{
			public Transform transform;

			public Mode mode;

			public AlignMode align;

			public int maxCount;

			public float minDist;

			public int camOffset = 15;

			public string instructionText;
		}

		[Serializable]
		public class DemoParticleSystemList
		{
			public DemoParticleSystem[] items;
		}

		public DemoParticleSystemList demoParticles;

		public float spawnOffset = 0.5f;

		public float multiply = 1f;

		public bool clearOnChange;

		public Text titleText;

		public Transform sceneCamera;

		public Text instructionText;

		public Button previousButton;

		public Button nextButton;

		public GraphicRaycaster graphicRaycaster;

		public EventSystem eventSystem;

		private ParticleSystemMultiplier m_ParticleMultiplier;

		private List<Transform> m_CurrentParticleList = new List<Transform>();

		private Transform m_Instance;

		private static int s_SelectedIndex;

		private Vector3 m_CamOffsetVelocity = Vector3.zero;

		private Vector3 m_LastPos;

		private static DemoParticleSystem s_Selected;

		private void Awake()
		{
			Select(s_SelectedIndex);
			previousButton.onClick.AddListener(Previous);
			nextButton.onClick.AddListener(Next);
		}

		private void OnDisable()
		{
			previousButton.onClick.RemoveListener(Previous);
			nextButton.onClick.RemoveListener(Next);
		}

		private void Previous()
		{
			s_SelectedIndex--;
			if (s_SelectedIndex == -1)
			{
				s_SelectedIndex = demoParticles.items.Length - 1;
			}
			Select(s_SelectedIndex);
		}

		public void Next()
		{
			s_SelectedIndex++;
			if (s_SelectedIndex == demoParticles.items.Length)
			{
				s_SelectedIndex = 0;
			}
			Select(s_SelectedIndex);
		}

		private void Update()
		{
			sceneCamera.localPosition = Vector3.SmoothDamp(sceneCamera.localPosition, Vector3.forward * -s_Selected.camOffset, ref m_CamOffsetVelocity, 1f);
			if (s_Selected.mode == Mode.Activate || CheckForGuiCollision())
			{
				return;
			}
			bool flag = Input.GetMouseButtonDown(0) && s_Selected.mode == Mode.Instantiate;
			bool flag2 = Input.GetMouseButton(0) && s_Selected.mode == Mode.Trail;
			if (!flag && !flag2)
			{
				return;
			}
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hitInfo;
			if (!Physics.Raycast(ray, out hitInfo))
			{
				return;
			}
			Quaternion rotation = Quaternion.LookRotation(hitInfo.normal);
			if (s_Selected.align == AlignMode.Up)
			{
				rotation = Quaternion.identity;
			}
			Vector3 vector = hitInfo.point + hitInfo.normal * spawnOffset;
			if (!((vector - m_LastPos).magnitude > s_Selected.minDist))
			{
				return;
			}
			if (s_Selected.mode != Mode.Trail || m_Instance == null)
			{
				m_Instance = (Transform)UnityEngine.Object.Instantiate(s_Selected.transform, vector, rotation);
				if (m_ParticleMultiplier != null)
				{
					m_Instance.GetComponent<ParticleSystemMultiplier>().multiplier = multiply;
				}
				m_CurrentParticleList.Add(m_Instance);
				if (s_Selected.maxCount > 0 && m_CurrentParticleList.Count > s_Selected.maxCount)
				{
					if (m_CurrentParticleList[0] != null)
					{
						UnityEngine.Object.Destroy(m_CurrentParticleList[0].gameObject);
					}
					m_CurrentParticleList.RemoveAt(0);
				}
			}
			else
			{
				m_Instance.position = vector;
				m_Instance.rotation = rotation;
			}
			if (s_Selected.mode == Mode.Trail)
			{
				m_Instance.transform.GetComponent<ParticleSystem>().enableEmission = false;
				m_Instance.transform.GetComponent<ParticleSystem>().Emit(1);
			}
			m_Instance.parent = hitInfo.transform;
			m_LastPos = vector;
		}

		private bool CheckForGuiCollision()
		{
			PointerEventData pointerEventData = new PointerEventData(eventSystem);
			pointerEventData.pressPosition = Input.mousePosition;
			pointerEventData.position = Input.mousePosition;
			List<RaycastResult> list = new List<RaycastResult>();
			graphicRaycaster.Raycast(pointerEventData, list);
			return list.Count > 0;
		}

		private void Select(int i)
		{
			s_Selected = demoParticles.items[i];
			m_Instance = null;
			DemoParticleSystem[] items = demoParticles.items;
			foreach (DemoParticleSystem demoParticleSystem in items)
			{
				if (demoParticleSystem != s_Selected && demoParticleSystem.mode == Mode.Activate)
				{
					demoParticleSystem.transform.gameObject.SetActive(false);
				}
			}
			if (s_Selected.mode == Mode.Activate)
			{
				s_Selected.transform.gameObject.SetActive(true);
			}
			m_ParticleMultiplier = s_Selected.transform.GetComponent<ParticleSystemMultiplier>();
			multiply = 1f;
			if (clearOnChange)
			{
				while (m_CurrentParticleList.Count > 0)
				{
					UnityEngine.Object.Destroy(m_CurrentParticleList[0].gameObject);
					m_CurrentParticleList.RemoveAt(0);
				}
			}
			instructionText.text = s_Selected.instructionText;
			titleText.text = s_Selected.transform.name;
		}
	}
}
