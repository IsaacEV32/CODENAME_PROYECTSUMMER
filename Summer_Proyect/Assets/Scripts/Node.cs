using UnityEngine;
//Estados que tendra el nodo Cerrado, abierto y bloqueado.
public enum NodeState
{
    Open, Blocked
}
public class Node : IHeapItem<Node>
{
    public NodeState currentNodeState;
    public Vector3 worldPosition;

    int gridX, gridY;

    int gCost, hCost;

    Node parentNode;

    int HeapIndex;
    int fCost
    {
        get
        {
            return gCost + hCost;
        }
    }
    //Constructor del nodo
    public Node(NodeState _nodeState, Vector3 _worldPosition, int _gridSizeX, int _gridSizeY)
    {
        currentNodeState = _nodeState;
        worldPosition = _worldPosition;
        gridX = _gridSizeX;
        gridY = _gridSizeY;
    }
    public int heapIndex
    {
        get
        {
            return HeapIndex;
        }
        set
        {
            HeapIndex = value;
        }
    }
    public int CompareTo(Node nodeToCompare)
    {
        int compare = fCost.CompareTo(nodeToCompare.fCost);
        if (compare == 0)
        {
            compare = hCost.CompareTo(nodeToCompare.hCost);
        }
        return -compare;
    }
    #region Getters_Setters
    public int GetgCost()
    {
        return gCost;
    }
    public int GethCost()
    {
        return hCost;
    }
    public int GetfCost()
    {
        return fCost;
    }
    public int GetGridX()
    {
        return gridX;
    }
    public int GetGridY()
    {
        return gridY;
    }
    public Node GetParentNode()
    {
        return parentNode;
    }
    public void SetgCost(int gCost)
    {
        this.gCost = gCost;
    }
    public void SethCost(int hCost)
    {
        this.hCost = hCost;
    }
    public void SetParentNode(Node parentNode)
    {
        this.parentNode = parentNode;
    }
    #endregion
}
