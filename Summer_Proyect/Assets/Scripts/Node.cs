using UnityEngine;
//Estados que tendra el nodo Cerrado, abierto y bloqueado.
public enum NodeState
{
    Open, Blocked
}
public class Node
{
    public NodeState currentNodeState;
    public Vector3 worldPosition;
    //Constructor del nodo
    public Node(NodeState _nodeState, Vector3 _worldPosition)
    {
        currentNodeState = _nodeState;
        worldPosition = _worldPosition;
    }
}
