using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Room : MonoBehaviour
{
    [Header("Door")]
    [SerializeField] GameObject topDoor;
    [SerializeField] GameObject bottomDoor;
    [SerializeField] GameObject leftDoor;
    [SerializeField] GameObject rightDoor;

    [Header("Wall")]
    [SerializeField] GameObject topWallDoor;
    [SerializeField] GameObject bottomWallDoor;
    [SerializeField] GameObject leftWallDoor;
    [SerializeField] GameObject rightWallDoor;

    [Header("Wall path")]
    [SerializeField] GameObject topPath;
    [SerializeField] GameObject bottomPath;
    [SerializeField] GameObject leftPath;
    [SerializeField] GameObject rightPath;

    [Header("Closed Door")]
    [SerializeField] GameObject closedTop;
    [SerializeField] GameObject closedBottom;
    [SerializeField] GameObject closedLeft;
    [SerializeField] GameObject closedRight;

    public Vector2Int RoomIndex { get; set; }
    public bool IsStairRoom { get; set; } = false;
    public bool IsBossRoom { get; set; } = false;

    private bool bossRoomLocked = false;

    public void OpenDoor(Vector2Int direction)
    {
        if (direction == Vector2Int.up)
        {
            topDoor.SetActive(true);
            topPath.SetActive(true);
            //topWallDoor.SetActive(false);
            topWallDoor.GetComponent<TilemapCollider2D>().enabled = false;
        }

        if (direction == Vector2Int.down)
        {
            bottomDoor.SetActive(true);
            bottomPath.SetActive(true);
            //bottomWallDoor.SetActive(false);
            bottomWallDoor.GetComponent<TilemapCollider2D>().enabled = false;
        }

        if (direction == Vector2Int.left)
        {
            leftDoor.SetActive(true);
            leftPath.SetActive(true);
            //leftWallDoor.SetActive(false);
            leftWallDoor.GetComponent<TilemapCollider2D>().enabled = false;
        }

        if (direction == Vector2Int.right)
        {
            rightDoor.SetActive(true);
            rightPath.SetActive(true);
            //rightWallDoor.SetActive(false);
            rightWallDoor.GetComponent<TilemapCollider2D>().enabled = false;
        }

        Physics2D.SyncTransforms();
    }

    public void CloseAllDoors()
    {
        Debug.Log("Closing all doors in room: " + RoomIndex);

        if (topDoor.activeSelf && closedTop != null)
            closedTop.SetActive(true);

        if (bottomDoor.activeSelf && closedBottom != null)
            closedBottom.SetActive(true);

        if (leftDoor.activeSelf && closedLeft != null)
            closedLeft.SetActive(true);

        if (rightDoor.activeSelf && closedRight != null)
            closedRight.SetActive(true);
    }

    public void OpenAllDoors()
    {
        if (closedTop != null) closedTop.SetActive(false);
        if (closedBottom != null) closedBottom.SetActive(false);
        if (closedLeft != null) closedLeft.SetActive(false);
        if (closedRight != null) closedRight.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (IsBossRoom && collision.CompareTag("Player") && !bossRoomLocked)
        {
            CloseAllDoors();
            bossRoomLocked = true;
        }
    }
}
