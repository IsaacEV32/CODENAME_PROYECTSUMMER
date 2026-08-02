using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class PathfindingPlayer : MonoBehaviour
{
    //Referencia a la grid, que se debera de anadir junto a este objeto
    GridBoard grid;
    //Se anaden los puntos de inicio y final
    [SerializeField] Transform seeker, target;
    //Referencia al jugador
    [SerializeField] PlayerControler playerControler;
    //Corrutina para el loop de encontrar camino
    Coroutine loopPath;

    //El Heap con los nodos que estan disponibles
    Heap<Node> openSet;
    //Es el set con los nodos que ya no estan disponibles
    HashSet<Node> closeSet = new HashSet<Node>();
    private void Awake()
    {
        //Inicializamos la grid y el heap los nodos que estan disponibles
        grid = GetComponent<GridBoard>();
        openSet = new Heap<Node>(grid.MaxSize);
    }
    //Funcion para iniciar la corrutina de encontrar camino
    public void StartCoroutineForFindingPath(Transform _target)
    {
        target = _target;
        //Si se ha iniciado la corrutina
        if (loopPath != null && playerControler.GetEndPoint().position != playerControler.GetLastPointSaved())
        {
            //Se detiene la corrutina
            StopCoroutine(loopPath);
            loopPath = null;
        }
        if (playerControler.GetEndPoint().position != playerControler.GetLastPointSaved())
        {
            //Se inicia la corrutina
            loopPath = StartCoroutine(LoopFindingPath());
        }
    }
    IEnumerator LoopFindingPath()
    {
        FindPath(seeker.position, target.position);
        yield return null;

    }
    //Funcion para encontrar el camino
    void FindPath(Vector2 startPosition, Vector2 endPosition)
    {
        //Variable para poder medir el tiempo pasado con precision
        Stopwatch sw = new Stopwatch();
        //Se empieza a medir aqui
        sw.Start();

        //Se crean los nodos de inicio obteniendo el nodo mas cercano de su posicion
        Node startNode = grid.NodeFromWorldPoint(startPosition);
        //Se crean los nodos final obteniendo el nodo mas cercano de su posicion
        Node endNode = grid.NodeFromWorldPoint(endPosition);

        if (endNode.currentNodeState != NodeState.Blocked)
        {
            //Se limpian los sets abiertos y cerrados
            openSet.Clear();
            closeSet.Clear();
            //Se anade el nodo de inicio como abierto
            openSet.Add(startNode);
            //Se busca dentro de los nodos abiertos
            for (int i = 0; openSet.Count > 0; i++)
            {
                //Se quita el primer nodo en el set de nodos abiertos y obtenemos el nodo actual
                Node currentNode = openSet.RemoveFirstItem();
                //Se anade el nodo actual en el set de nodos cerrados
                closeSet.Add(currentNode);
                //Si el nodo actual es el final
                if (currentNode == endNode)
                {
                    //Paramos de contar cuanto tiempo a ha tardado
                    sw.Stop();
                    print("Path found: " + sw.ElapsedMilliseconds + "ms");
                    //Recreamos el camino con el nodo de inicio y el nodo final
                    RetracePath(startNode, endNode);
                    //Activamos el permiso para que el jugador pueda moverse
                    playerControler.PlayerCanMoveToTheNode(true);
                    break;
                }
                //Por cada nodo vecino del nodo actual
                foreach (Node neighbour in grid.GetNeighbours(currentNode))
                {
                    //Si el vecino esta bloqueado o se encuentra dentro del set de nodos cerrados, se ignora a ese vecino
                    if (neighbour.currentNodeState == NodeState.Blocked || closeSet.Contains(neighbour))
                    {
                        continue;
                    }
                    //Obtenemos el nuevo coste en base al coste g del vecino y se le suma la distancia entre el nodo actual y el vecino
                    int newMovementCostToNeighbour = currentNode.GetgCost() + GetDistance(currentNode, neighbour);
                    //Si este nuevo coste es menor que el coste g del vecino o si el set de nodos abiertos no contiene el nodo vecino
                    if (newMovementCostToNeighbour < neighbour.GetgCost() || !openSet.Contains(neighbour))
                    {
                        //Se setean los costes g y h del vecino
                        neighbour.SetgCost(newMovementCostToNeighbour);
                        neighbour.SethCost(GetDistance(neighbour, endNode));
                        //Se añade como padre de este vecino el nodo actual
                        neighbour.SetParentNode(currentNode);
                        //SI el set de nodos no contiene al vecino, se anade 
                        if (!openSet.Contains(neighbour))
                        {
                            openSet.Add(neighbour);
                        }
                        //Si no se actualiza ese nodo en el nodo de sets vecinos
                        else
                        {
                            openSet.UpdateItem(neighbour);
                        }
                    }
                }
            }
        }
        //Si no se desactica el punto final camino y no se le permite mover al jugador
        else
        {
            playerControler.GetEndPoint().gameObject.SetActive(false);
            playerControler.PlayerCanMoveToTheNode(false);
        }
        //Se termina la busqueda del camino
        loopPath = null;
    }
    //Se rehace el camino de nodos
    void RetracePath(Node startNode, Node endNode)
    {
        //Lista para guardar el camino 
        List<Node> path = new List<Node>();
        //Guardamos el ultimo nodo como el actual
        Node currentNode = endNode;
        
        //Mientras el nodo actual no sea el nodo de inicio
        while (currentNode != startNode)
        {
            //Se anade el nodo actual al camino
            path.Add(currentNode);
            //El nodo actual ahora es el padre del nodo actual
            currentNode = currentNode.GetParentNode();
        }
        //Se anade el ultimo nodo al camino
        path.Add(currentNode);
        
        //Se le da la vuelta
        path.Reverse();

        //Anadimos el camino a la grid
        grid.path = path;

    }
    //Se usa para medir la distancia
    int GetDistance(Node nodeA, Node nodeB)
    {
        //Se obtiene la distancia en x e y entre el nodo inicial y el nodo final
        int distX = Mathf.Abs(nodeA.GetGridX() - nodeB.GetGridX());
        int distY = Mathf.Abs(nodeA.GetGridY() - nodeB.GetGridY());

        //Si la distancia en el eje X es mayor a la del Y
        if (distX > distY)
        {
            //Nota: 14 y 10 son valores personalmente elegidos por conveniencia.
            //Podrian ser otros pero por facilitar los calculos y por estandar se han elegido estos
            return 14 * distY + 10 * (distX - distY);
        }
        else
        {
            //Se invierte las distancias en el eje x y en el eje y
            return 14 * distX + 10 * (distY - distX);
        }
    }
}
