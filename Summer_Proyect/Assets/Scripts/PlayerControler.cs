using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerControler : MonoBehaviour
{
    [SerializeField] GridBoard grid;
    [SerializeField] Transform endPoint;
    Vector3 lastPointSaved;
    [SerializeField] PathfindingPlayer pathfindingPlayer;
    [SerializeField] float speed;

    Coroutine moveSmoothly = null;

    bool checkIfPossibleMove;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        endPoint.gameObject.SetActive(false);
    }

    public void OnLeftClick(InputAction.CallbackContext leftClickContext)
    {
        if (leftClickContext.performed)
        {
            Camera camera = Camera.main;
            Vector2 pointerPosition = camera.ScreenToWorldPoint(Mouse.current.position.ReadValue());

            if (!endPoint.gameObject.activeSelf)
            {
                endPoint.gameObject.SetActive(true);
            }
            Node nearNodePosition = grid.NodeFromWorldPoint(pointerPosition);
            endPoint.position = nearNodePosition.worldPosition;
            pathfindingPlayer.StartCoroutineForFindingPath(endPoint);
            if (moveSmoothly != null && endPoint.position != lastPointSaved)
            {
                StopCoroutine(moveSmoothly);
                moveSmoothly = null;
            }
            if (checkIfPossibleMove && endPoint.position != lastPointSaved)
            {
                moveSmoothly = StartCoroutine(MoveSmoothlyCorroutine());
            }
            lastPointSaved = endPoint.position;
        }
    }
    IEnumerator MoveSmoothlyCorroutine()
    {
        foreach (Node node in grid.path)
        {
            while (transform.position != node.worldPosition)
            {
                transform.position = Vector2.MoveTowards(transform.position, node.worldPosition, speed * Time.deltaTime);
                yield return null;
            }
            transform.position = node.worldPosition;
        }
        endPoint.gameObject.SetActive(false);
        moveSmoothly = null;
    }
    public void PlayerCanMoveToTheNode(bool checkIfCanMoveToTheNode)
    {
        checkIfPossibleMove = checkIfCanMoveToTheNode;
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
