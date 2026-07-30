using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using UnityEngine;

public class PathfindingPlayer : MonoBehaviour
{
    GridBoard grid;
    [SerializeField] Transform seeker, target;
    [SerializeField] PlayerControler playerControler;
    Coroutine loopPath;

    Heap<Node> openSet;
    HashSet<Node> closeSet = new HashSet<Node>();
    private void Awake()
    {
        grid = GetComponent<GridBoard>();
        openSet = new Heap<Node>(grid.MaxSize);
    }
    public void StartCoroutineForFindingPath(Transform _target)
    {
        target = _target;
        if (loopPath != null && playerControler.GetEndPoint().position != playerControler.GetLastPointSaved())
        {
            StopCoroutine(loopPath);
            loopPath = null;
        }
        if (playerControler.GetEndPoint().position != playerControler.GetLastPointSaved())
        {
            loopPath = StartCoroutine(LoopFindingPath());
        }
    }
    IEnumerator LoopFindingPath()
    {
        FindPath(seeker.position, target.position);
        yield return new WaitForSeconds(1);

    }
    void FindPath(Vector2 startPosition, Vector2 endPosition)
    {
        Stopwatch sw = new Stopwatch();
        sw.Start();

        Node startNode = grid.NodeFromWorldPoint(startPosition);
        Node endNode = grid.NodeFromWorldPoint(endPosition);
        if (endNode.currentNodeState != NodeState.Blocked)
        {
            openSet.Clear();
            closeSet.Clear();
            openSet.Add(startNode);

            for (int i = 0; openSet.Count > 0; i++)
            {
                Node currentNode = openSet.RemoveFirstItem();
                closeSet.Add(currentNode);
                if (currentNode == endNode)
                {
                    sw.Stop();
                    print("Path found: " + sw.ElapsedMilliseconds + "ms");
                    RetracePath(startNode, endNode);
                    playerControler.PlayerCanMoveToTheNode(true);
                    break;
                }
                foreach (Node neighbour in grid.GetNeighbours(currentNode))
                {
                    if (neighbour.currentNodeState == NodeState.Blocked || closeSet.Contains(neighbour))
                    {
                        continue;
                    }
                    int newMovementCostToNeighbour = currentNode.GetgCost() + GetDistance(currentNode, neighbour);
                    if (newMovementCostToNeighbour < neighbour.GetgCost() || !openSet.Contains(neighbour))
                    {
                        neighbour.SetgCost(newMovementCostToNeighbour);
                        neighbour.SethCost(GetDistance(neighbour, endNode));
                        neighbour.SetParentNode(currentNode);

                        if (!openSet.Contains(neighbour))
                        {
                            openSet.Add(neighbour);
                        }
                    }
                }
            }
        }
        else
        {
            playerControler.GetEndPoint().gameObject.SetActive(false);
            playerControler.PlayerCanMoveToTheNode(false);
        }

        loopPath = null;
    }
    void RetracePath(Node startNode, Node endNode)
    {
        List<Node> path = new List<Node>();
        Node currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.GetParentNode();
        }
        path.Add(currentNode);
        path.Reverse();

        grid.path = path;

    }
    int GetDistance(Node nodeA, Node nodeB)
    {
        int distX = Mathf.Abs(nodeA.GetGridX() - nodeB.GetGridX());
        int distY = Mathf.Abs(nodeA.GetGridY() - nodeB.GetGridY());

        if (distX > distY)
        {
            return 14 * distY + 10 * (distX - distY);
        }
        else
        {
            return 14 * distX + 10 * (distY - distX);
        }
    }
}
