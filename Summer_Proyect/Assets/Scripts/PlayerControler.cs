using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerControler : MonoBehaviour
{
    //Referencia a la grid
    [SerializeField] GridBoard grid;
    //Referencia del punto final
    [SerializeField] Transform endPoint;
    //Ultimo punto final que se ha guardado
    Vector3 lastPointSaved;
    //Referencia al pathfinding
    [SerializeField] PathfindingPlayer pathfindingPlayer;
    //Velocidad del jugador
    [SerializeField] float speed;

    //Se usa para almacenar la corrutina
    Coroutine moveSmoothly = null;
    void Start()
    {
        endPoint.gameObject.SetActive(false);
    }

    public void OnLeftClick(InputAction.CallbackContext leftClickContext)
    {
        if (leftClickContext.performed)
        {
            //Obtenemos la posicion en la que el cursor esta apuntando al mundo
            Vector2 pointerPosition = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            //Obtenemos cual es el nodo mas cercano a la posicion del cursor
            Node nearNodePosition = grid.NodeFromWorldPoint(pointerPosition);
            //Evitamos calcular el camino y movernos si esta apuntando a uno bloqueado
            if (nearNodePosition.currentNodeState == NodeState.Blocked)
            {
                return;
            }
            //Comprobamos si esta desactivado el punto final para evitar activarlo siempre
            if (!endPoint.gameObject.activeSelf)
            {
                endPoint.gameObject.SetActive(true);
            }
            //Guardamos la posicion del nodo mas cercano como la posicion final
            endPoint.position = nearNodePosition.worldPosition;
            //Si la posicion final es distinta a la del nodo final
            if (endPoint.position != lastPointSaved)
            {
                //Buscamos el camino (Siguiendo el algoritmo A*)
                pathfindingPlayer.StartCoroutineForFindingPath(endPoint);
                //Detenemos la corrutina actual de movimiento si ya se esta ejecutando
                if (moveSmoothly != null)
                {
                    StopCoroutine(moveSmoothly);
                    moveSmoothly = null;
                }
                //Iniciamos la corrutian de movimiento y guardamos la ultima posicion
                moveSmoothly = StartCoroutine(MoveSmoothlyCorroutine());
                lastPointSaved = endPoint.position;
            }
        }
    }
    IEnumerator MoveSmoothlyCorroutine()
    {
        //Por cada nodo del camino
        foreach (Node node in grid.path)
        {
            while (transform.position != node.worldPosition)
            {
                //Nos movemos hacia el nodo del camino
                transform.position = Vector2.MoveTowards(transform.position, node.worldPosition, speed * Time.deltaTime);
                yield return null;
            }
            //Nos ponemos en el centro del camino
            transform.position = node.worldPosition;
        }
        //Desactivamos el punto final si ya ha llegado
        endPoint.gameObject.SetActive(false);
        moveSmoothly = null;
    }
    public Transform GetEndPoint()
    {
        return endPoint;
    }
    public Vector3 GetLastPointSaved()
    {
        return lastPointSaved;
    }
}
