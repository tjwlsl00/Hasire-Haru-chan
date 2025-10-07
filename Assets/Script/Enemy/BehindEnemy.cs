using UnityEngine;

public class BehindEnemy : MonoBehaviour
{
    public float moveSpeed = 5f;
    public Vector3 moveDirection = Vector3.forward;
    public bool isMoving = true;

    void Update()
    {
        if (GameManager.currentState == GameState.Playing && isMoving)
        {
            transform.Translate(moveDirection.normalized * moveSpeed * Time.deltaTime, Space.World);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            PlayerMovement playerMovement = other.gameObject.GetComponent<PlayerMovement>();
            if (playerMovement != null)
            {
                playerMovement.GameOver();
                isMoving = false;
            }

            
        }

        if (other.gameObject.CompareTag("GoalWall"))
        {
            isMoving = false;
        }
    }
}
