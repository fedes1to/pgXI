using System.Collections.Generic;
using UnityEngine;

namespace Rilisoft
{
	public class PetIndicationController : MonoBehaviour
	{
		public Camera LookToCamera;

		public GameObject LabelObj;

		public TextMesh TextMesh;

		public GameObject IconObject;

		public GameObject DirectionObj;

		public Color ColorPetMy = new Color(0f, 1f, 0f);

		public Color ColorPetEnemy = new Color(1f, 0f, 0f);

		public Color ColorPetMyTeam = new Color(0f, 0f, 1f);

		public Color ColorPetCoop = new Color(1f, 1f, 1f);

		public Material MaterialMy;

		public Material MaterialOur;

		public bool isUpdateNameFromInfo;

		private MeshRenderer _meshRenderer;

		private PetEngine _engineValue;

		private List<GameObject> labelsFrame = new List<GameObject>();

		private Vector3[] deltaPos = new Vector3[8]
		{
			new Vector3(0f, 0.2f, 0f),
			new Vector3(0f, -0.2f, 0f),
			new Vector3(0.2f, 0.2f, 0f),
			new Vector3(-0.2f, 0.2f, 0f),
			new Vector3(0.2f, 0f, 0f),
			new Vector3(-0.2f, 0f, 0f),
			new Vector3(0.2f, -0.2f, 0f),
			new Vector3(-0.2f, 0.2f, 0f)
		};

		private PetEngine _engine
		{
			get
			{
				if (_engineValue == null)
				{
					_engineValue = base.gameObject.GetComponentInParents<PetEngine>();
				}
				return _engineValue;
			}
		}

		public void CreateFrameLabel()
		{
			if (labelsFrame.Count < deltaPos.Length)
			{
				for (int i = 0; i < deltaPos.Length; i++)
				{
					labelsFrame.Add(Object.Instantiate(LabelObj));
				}
			}
			for (int j = 0; j < labelsFrame.Count; j++)
			{
				labelsFrame[j].transform.SetParent(LabelObj.transform);
				labelsFrame[j].transform.localScale = Vector3.one;
				labelsFrame[j].transform.localRotation = Quaternion.identity;
				labelsFrame[j].transform.localPosition = deltaPos[j];
				labelsFrame[j].GetComponent<TextMesh>().color = Color.black;
				labelsFrame[j].GetComponent<TextMesh>().text = TextMesh.text;
				labelsFrame[j].GetComponent<TextMesh>().offsetZ = 0.1f;
			}
		}

		private void Awake()
		{
			_meshRenderer = TextMesh.gameObject.GetComponent<MeshRenderer>();
		}

		private void Update()
		{
			if (LookToCamera == null)
			{
				LookToCamera = Camera.main;
			}
			if (LookToCamera == null)
			{
				return;
			}
			string text = string.Empty;
			if (isUpdateNameFromInfo)
			{
				if (_engine.Info != null && _engine.Info.Id != string.Empty)
				{
					text = Singleton<PetsManager>.Instance.GetPlayerPet(_engine.Info.Id).PetName;
				}
			}
			else
			{
				text = _engine.PetName;
			}
			SetLabelColor();
			if (TextMesh.text != text)
			{
				TextMesh.text = text;
				for (int i = 0; i < labelsFrame.Count; i++)
				{
					labelsFrame[i].GetComponent<TextMesh>().text = TextMesh.text;
				}
			}
			if (!_engine.IsAlive || !_engine.Model.activeInHierarchy)
			{
				LabelObj.SetActiveSafe(false);
				IconObject.SetActiveSafe(false);
				DirectionObj.SetActiveSafe(false);
				return;
			}
			bool isAlive = _engine.IsAlive;
			Vector3 vector = LookToCamera.WorldToScreenPoint(_engine._lookPoint.transform.position);
			isAlive = vector.x > 0f && vector.x < (float)Screen.width && vector.y > 0f && vector.y < (float)Screen.height;
			if (_engine.Enabled && _engine.IsMine)
			{
				LabelObj.SetActiveSafe(isAlive);
				IconObject.SetActiveSafe(isAlive);
				DirectionObj.SetActiveSafe(!isAlive);
			}
			else if (!_engine.Enabled && !_engine.IsMine)
			{
				LabelObj.SetActiveSafe(isAlive);
				IconObject.SetActiveSafe(false);
				DirectionObj.SetActiveSafe(false);
			}
			else if (!_engine.Enabled)
			{
				LabelObj.SetActiveSafe(true);
				IconObject.SetActiveSafe(false);
				DirectionObj.SetActiveSafe(false);
			}
		}

		private void SetLabelColor()
		{
			if (_engine == null || _engine.Owner == null)
			{
				return;
			}
			SetLabelMaterial();
			if (Defs.isMulti)
			{
				if (ConnectSceneNGUIController.regim == ConnectSceneNGUIController.RegimGame.CapturePoints || ConnectSceneNGUIController.regim == ConnectSceneNGUIController.RegimGame.TeamFight || ConnectSceneNGUIController.regim == ConnectSceneNGUIController.RegimGame.FlagCapture)
				{
					NetworkStartTable myNetworkStartTable = WeaponManager.sharedManager.myNetworkStartTable;
					if (myNetworkStartTable == null)
					{
						TextMesh.color = ColorPetEnemy;
					}
					else if (_engine.Owner == myNetworkStartTable)
					{
						TextMesh.color = ColorPetMy;
					}
					else if (myNetworkStartTable != null && _engine.Owner.myCommand == myNetworkStartTable.myCommand)
					{
						TextMesh.color = ColorPetMyTeam;
					}
					else
					{
						TextMesh.color = ColorPetEnemy;
					}
				}
				else if (Defs.isDaterRegim || ConnectSceneNGUIController.regim == ConnectSceneNGUIController.RegimGame.TimeBattle)
				{
					TextMesh.color = ((!(_engine.Owner == WeaponManager.sharedManager.myPlayerMoveC)) ? ColorPetCoop : ColorPetMy);
				}
				else
				{
					TextMesh.color = ((!_engine.IsMine) ? ColorPetEnemy : ColorPetMy);
				}
			}
			else
			{
				TextMesh.color = ColorPetMy;
			}
		}

		private void SetLabelMaterial()
		{
			if (_engine.IsMine)
			{
				if (_meshRenderer.sharedMaterial != MaterialMy)
				{
					_meshRenderer.sharedMaterial = MaterialMy;
				}
			}
			else if (_meshRenderer.sharedMaterial != MaterialOur)
			{
				_meshRenderer.sharedMaterial = MaterialOur;
			}
		}
	}
}
