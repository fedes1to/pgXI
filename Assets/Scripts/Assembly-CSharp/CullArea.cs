using System.Collections.Generic;
using UnityEngine;

public class CullArea : MonoBehaviour
{
	private const int MAX_NUMBER_OF_ALLOWED_CELLS = 250;

	public const int MAX_NUMBER_OF_SUBDIVISIONS = 3;

	public readonly int FIRST_GROUP_ID = 1;

	public readonly int[] SUBDIVISION_FIRST_LEVEL_ORDER = new int[4] { 0, 1, 1, 1 };

	public readonly int[] SUBDIVISION_SECOND_LEVEL_ORDER = new int[8] { 0, 2, 1, 2, 0, 2, 1, 2 };

	public readonly int[] SUBDIVISION_THIRD_LEVEL_ORDER = new int[12]
	{
		0, 3, 2, 3, 1, 3, 2, 3, 1, 3,
		2, 3
	};

	public Vector2 Center;

	public Vector2 Size = new Vector2(25f, 25f);

	public Vector2[] Subdivisions = new Vector2[3];

	public int NumberOfSubdivisions;

	public bool YIsUpAxis = true;

	public bool RecreateCellHierarchy;

	private int idCounter;

	public int CellCount { get; private set; }

	public CellTree CellTree { get; private set; }

	public Dictionary<int, GameObject> Map { get; private set; }

	private void Awake()
	{
		idCounter = FIRST_GROUP_ID;
		CreateCellHierarchy();
	}

	public void OnDrawGizmos()
	{
		idCounter = FIRST_GROUP_ID;
		if (RecreateCellHierarchy)
		{
			CreateCellHierarchy();
		}
		DrawCells();
	}

	private void CreateCellHierarchy()
	{
		if (!IsCellCountAllowed())
		{
			if (Debug.isDebugBuild)
			{
				Debug.LogError("There are too many cells created by your subdivision options. Maximum allowed number of cells is " + (250 - FIRST_GROUP_ID) + ". Current number of cells is " + CellCount + ".");
				return;
			}
			Application.Quit();
		}
		CellTreeNode cellTreeNode = new CellTreeNode(idCounter++, CellTreeNode.ENodeType.Root, null);
		if (YIsUpAxis)
		{
			Center = new Vector2(base.transform.position.x, base.transform.position.y);
			Size = new Vector2(base.transform.localScale.x, base.transform.localScale.y);
			cellTreeNode.Center = new Vector3(Center.x, Center.y, 0f);
			cellTreeNode.Size = new Vector3(Size.x, Size.y, 0f);
			cellTreeNode.TopLeft = new Vector3(Center.x - Size.x / 2f, Center.y - Size.y / 2f, 0f);
			cellTreeNode.BottomRight = new Vector3(Center.x + Size.x / 2f, Center.y + Size.y / 2f, 0f);
		}
		else
		{
			Center = new Vector2(base.transform.position.x, base.transform.position.z);
			Size = new Vector2(base.transform.localScale.x, base.transform.localScale.z);
			cellTreeNode.Center = new Vector3(Center.x, 0f, Center.y);
			cellTreeNode.Size = new Vector3(Size.x, 0f, Size.y);
			cellTreeNode.TopLeft = new Vector3(Center.x - Size.x / 2f, 0f, Center.y - Size.y / 2f);
			cellTreeNode.BottomRight = new Vector3(Center.x + Size.x / 2f, 0f, Center.y + Size.y / 2f);
		}
		CreateChildCells(cellTreeNode, 1);
		CellTree = new CellTree(cellTreeNode);
		RecreateCellHierarchy = false;
	}

	private void CreateChildCells(CellTreeNode parent, int cellLevelInHierarchy)
	{
		if (cellLevelInHierarchy > NumberOfSubdivisions)
		{
			return;
		}
		int num = (int)Subdivisions[cellLevelInHierarchy - 1].x;
		int num2 = (int)Subdivisions[cellLevelInHierarchy - 1].y;
		float num3 = parent.Center.x - parent.Size.x / 2f;
		float num4 = parent.Size.x / (float)num;
		for (int i = 0; i < num; i++)
		{
			for (int j = 0; j < num2; j++)
			{
				float num5 = num3 + (float)i * num4 + num4 / 2f;
				CellTreeNode cellTreeNode = new CellTreeNode(idCounter++, (NumberOfSubdivisions != cellLevelInHierarchy) ? CellTreeNode.ENodeType.Node : CellTreeNode.ENodeType.Leaf, parent);
				if (YIsUpAxis)
				{
					float num6 = parent.Center.y - parent.Size.y / 2f;
					float num7 = parent.Size.y / (float)num2;
					float num8 = num6 + (float)j * num7 + num7 / 2f;
					cellTreeNode.Center = new Vector3(num5, num8, 0f);
					cellTreeNode.Size = new Vector3(num4, num7, 0f);
					cellTreeNode.TopLeft = new Vector3(num5 - num4 / 2f, num8 - num7 / 2f, 0f);
					cellTreeNode.BottomRight = new Vector3(num5 + num4 / 2f, num8 + num7 / 2f, 0f);
				}
				else
				{
					float num9 = parent.Center.z - parent.Size.z / 2f;
					float num10 = parent.Size.z / (float)num2;
					float num11 = num9 + (float)j * num10 + num10 / 2f;
					cellTreeNode.Center = new Vector3(num5, 0f, num11);
					cellTreeNode.Size = new Vector3(num4, 0f, num10);
					cellTreeNode.TopLeft = new Vector3(num5 - num4 / 2f, 0f, num11 - num10 / 2f);
					cellTreeNode.BottomRight = new Vector3(num5 + num4 / 2f, 0f, num11 + num10 / 2f);
				}
				parent.AddChild(cellTreeNode);
				CreateChildCells(cellTreeNode, cellLevelInHierarchy + 1);
			}
		}
	}

	private void DrawCells()
	{
		if (CellTree != null && CellTree.RootNode != null)
		{
			CellTree.RootNode.Draw();
		}
		else
		{
			RecreateCellHierarchy = true;
		}
	}

	private bool IsCellCountAllowed()
	{
		int num = 1;
		int num2 = 1;
		Vector2[] subdivisions = Subdivisions;
		for (int i = 0; i < subdivisions.Length; i++)
		{
			Vector2 vector = subdivisions[i];
			num *= (int)vector.x;
			num2 *= (int)vector.y;
		}
		CellCount = num * num2;
		return CellCount <= 250 - FIRST_GROUP_ID;
	}

	public List<int> GetActiveCells(Vector3 position)
	{
		List<int> list = new List<int>(0);
		CellTree.RootNode.GetInsideCells(list, YIsUpAxis, position);
		List<int> list2 = new List<int>(0);
		CellTree.RootNode.GetNearbyCells(list2, YIsUpAxis, position);
		foreach (int item in list2)
		{
			if (!list.Contains(item))
			{
				list.Add(item);
			}
		}
		return list;
	}
}
