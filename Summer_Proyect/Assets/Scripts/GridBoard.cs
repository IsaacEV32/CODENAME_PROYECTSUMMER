using System.Collections.Generic;
using UnityEngine;

public class GridBoard : MonoBehaviour
{
    //Layer para los nodos en los que no se puede mover el jugador
    [SerializeField] LayerMask unwalkableNode;
    //Es el tamano de la grid en espacio mundo
    [SerializeField] Vector2 gridWorldSize;
    //Es el radio del nodo que servira para saber que tan grande queremos el nodo
    [SerializeField] float nodeRadius;

    //Array para la grid
    Node[] grid;

    //Diametro del nodo
    float nodeDiameter;
    //Tamanos de la grid en los ejes X e Y
    int gridSizeX, gridSizeY;

    public List<Node> path;

    [SerializeField] bool onlyDrawPathGizmos;

    public int MaxSize
    {
        get 
        {
            return gridSizeX * gridSizeY;
        }
    }
    private void Awake()
    {
        //Calculamos el diametro del nodo
        nodeDiameter = nodeRadius * 2;
        //Calculamos el tamano de la grid en los ejes teniendo en cuenta el diametro del nodo y el tamano en de la grid en el mundo
        gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
        gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);
        //Creamos la grid
        CreateGrid();
    }
    void CreateGrid()
    {
        //Creamos la grid con el tamano en X e Y
        grid = new Node[MaxSize];
        //Conversion del vector 3D del centro de la grid a un vector 2D
        Vector2 gridCenterPosition = transform.position;
        //Conseguimos la esquina izquierda del mundo
        Vector2 worldBottomLeft = gridCenterPosition - Vector2.right * gridWorldSize.x / 2 - Vector2.up * gridWorldSize.y / 2;
        for (int i = 0; i < gridSizeX; i++)
        {
            for (int j = 0; j < gridSizeY; j++)
            {
                //Por cada nodo de la grid obtenemos el punto en el mundo del nodo
                Vector2 worldPoint = worldBottomLeft + Vector2.right * (i * nodeDiameter + nodeRadius) + Vector2.up * (j * nodeDiameter + nodeRadius);
                //Declaramos que esta disponible en el inicio
                NodeState state = NodeState.Open;
                //Si colisiona con algun objeto que sea "no caminable" entonces ese se indica como bloqueado
                if (Physics2D.OverlapCircle(worldPoint, nodeRadius, unwalkableNode))
                {
                    state = NodeState.Blocked;
                }
                //Se crea un nodo en la posicion indicada
                grid[gridSizeX * j + i] = new Node(state, worldPoint, i, j);
            }
        }
    }
    public Node NodeFromWorldPoint(Vector3 worldPosition)
    {
        //Se usan los porcentajes para evitar que nos den excepciones si se hace click fuera de la grid establecida.
        float percentX = ((worldPosition.x + gridWorldSize.x / 2) / gridWorldSize.x);
        float percentY = ((worldPosition.y + gridWorldSize.y / 2) / gridWorldSize.y);
        //Se usa para evitar que nos de un indice invalido si el jugador se encuentra fuera de la grid por alguna razon
        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);

        //Conseguimos las coordenadas x e y del nodo
        //El -1 es importante para evitar problemas de que nos de indice fuera del array al ser valores entre 0 y 1
        int x = Mathf.RoundToInt((gridSizeX - 1) * percentX);
        int y = Mathf.RoundToInt((gridSizeY - 1) * percentY);
        //Devolvemos el nodo resultante
        return grid[gridSizeX * y + x];
    }
    public List<Node> GetNeighbours(Node node)
    {
        List<Node> neighboursList = new List<Node>();
        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0)
                {
                    continue;
                }
                int checkX = node.GetGridX() + x;
                int checkY = node.GetGridY() + y;

                if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY)
                {
                    neighboursList.Add(grid[gridSizeX * checkY + checkX]);
                }
            }
        }
        return neighboursList;
    }
    private void OnDrawGizmos()
    {
        //Coloreamos en rojo el tamano de la grid
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, new Vector2(gridWorldSize.x, gridWorldSize.y));

        if (onlyDrawPathGizmos)
        {
            if (path != null)
            {
                foreach (Node node in path)
                {
                    Gizmos.color = Color.brown;
                    Gizmos.DrawWireCube(node.worldPosition, Vector3.one * (nodeDiameter - 0.1f));
                }
            }
        }
        else
        {
            if (grid != null)
            {
                foreach (Node node in grid)
                {
                    //Comprobamos el estado del nodo y lo coloreamos segun su estado
                    if (node.currentNodeState == NodeState.Blocked)
                    {
                        Gizmos.color = Color.blue;
                    }
                    else
                    {
                        Gizmos.color = Color.green;
                    }
                    if (path != null)
                    {
                        if (path.Contains(node))
                        {
                            Gizmos.color = Color.brown;
                        }
                    }
                    Gizmos.DrawWireCube(node.worldPosition, Vector3.one * (nodeDiameter - 0.1f));
                }
            }
        }
    }
}
