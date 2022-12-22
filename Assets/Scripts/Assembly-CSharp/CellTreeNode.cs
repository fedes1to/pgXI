using System.Collections.Generic;
using UnityEngine;

public class CellTreeNode
{
	public enum ENodeType
	{
		Root,
		Node,
		Leaf
	}

	public int Id;

	public Vector3 Center;

	public Vector3 Size;

	public Vector3 TopLeft;

	public Vector3 BottomRight;

	public ENodeType NodeType;

	public CellTreeNode Parent;

	public List<CellTreeNode> Childs;

	private float maxDistance;

	public CellTreeNode()
	{
	}

	public CellTreeNode(int id, ENodeType nodeType, CellTreeNode parent)
	{
		Id = id;
		NodeType = nodeType;
		Parent = parent;
	}

	public void AddChild(CellTreeNode child)
	{
		if (Childs == null)
		{
			Childs = new List<CellTreeNode>(1);
		}
		Childs.Add(child);
	}

	public void Draw()
	{
	}

	public void GetAllLeafNodes(List<CellTreeNode> leafNodes)
	{
		if (Childs != null)
		{
			foreach (CellTreeNode child in Childs)
			{
				child.GetAllLeafNodes(leafNodes);
			}
			return;
		}
		leafNodes.Add(this);
	}

	public void GetInsideCells(List<int> insideCells, bool yIsUpAxis, Vector3 position)
	{
		if (!IsPointInsideCell(yIsUpAxis, position))
		{
			return;
		}
		insideCells.Add(Id);
		if (Childs == null)
		{
			return;
		}
		foreach (CellTreeNode child in Childs)
		{
			child.GetInsideCells(insideCells, yIsUpAxis, position);
		}
	}

	public void GetNearbyCells(List<int> nearbyCells, bool yIsUpAxis, Vector3 position)
	{
		if (!IsPointNearCell(yIsUpAxis, position))
		{
			return;
		}
		if (NodeType != ENodeType.Leaf)
		{
			foreach (CellTreeNode child in Childs)
			{
				child.GetNearbyCells(nearbyCells, yIsUpAxis, position);
			}
			return;
		}
		nearbyCells.Add(Id);
	}

	public bool IsPointInsideCell(bool yIsUpAxis, Vector3 point)
	{
		if (point.x < TopLeft.x || point.x > BottomRight.x)
		{
			return false;
		}
		if (yIsUpAxis)
		{
			if (point.y >= TopLeft.y && point.y <= BottomRight.y)
			{
				return true;
			}
		}
		else if (point.z >= TopLeft.z && point.z <= BottomRight.z)
		{
			return true;
		}
		return false;
	}

	public bool IsPointNearCell(bool yIsUpAxis, Vector3 point)
	{
		if (maxDistance == 0f)
		{
			maxDistance = (Size.x + Size.y + Size.z) / 2f;
		}
		return (point - Center).sqrMagnitude <= maxDistance * maxDistance;
	}
}
