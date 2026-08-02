using UnityEngine;
//Estados que tendra el nodo Abierto y bloqueado.
public enum NodeState
{
    Open, Blocked
}
//Esta clase debe de implementar la interfaz IHeapItem para usar la estructura heap
public class Node : IHeapItem<Node>
{
    //Es el estado del nodo a la hora de construir la grid
    public NodeState currentNodeState;
    //La posicion en el mundo del nodo
    public Vector3 worldPosition;
    
    //Tamano de la grid en los ejes X e Y
    int gridX, gridY;

    //Se definene los costes g y h del nodo
    int gCost, hCost;

    //Se obtiene el nodo padre de este nodo
    Node parentNode;

    //Se usa como indice de la estructura heap
    int heapIndex;
    //Devuelve el coste f de este nodo
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
    //Definimos la propiedad del indice del heap 
    public int HeapIndex
    {
        get
        {
            return heapIndex;
        }
        set
        {
            heapIndex = value;
        }
    }
    //Se usa para comparar los costes f de los nodos. Si son iguales, se comparan los costes h.
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
