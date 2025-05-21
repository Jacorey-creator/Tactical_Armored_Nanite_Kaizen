using UnityEngine;

public class PlayerMovementStrategy : ITankMovementStrategy
{
    private ITankInput input;
    private float rotationSpeed = 75.20f;
    private float moveDeadZone = 0.1f;

    public PlayerMovementStrategy(ITankInput input)
    {
        this.input = input;
    }

    public void Move(TankController tank)
    {
        // Get input and check for deadzone
        Vector2 inputDir = input.MoveInput;
        if (inputDir.sqrMagnitude < moveDeadZone * moveDeadZone) return;

        float vertical = inputDir.y;
        float horizontal = inputDir.x;

        // Handle rotation separately from movement
        if (Mathf.Abs(horizontal) > 0.1f)
        {
            // Rotate the tank based on horizontal input with proper Time.deltaTime
            tank.transform.Rotate(0f, horizontal * rotationSpeed * Time.deltaTime, 0f);
        }

        // Only apply forward/backward movement if we have vertical input
        if (Mathf.Abs(vertical) > 0.1f)
        {
            // Move the tank forward or backward along its current forward direction
            Vector3 moveDir = tank.transform.forward * vertical;

            // Apply movement using physics if available, otherwise transform
            Rigidbody tankRb = tank.GetComponent<Rigidbody>();
            if (tankRb != null)
            {
                // Use physics-based movement (recommended)
                tankRb.MovePosition(tank.transform.position + moveDir * tank.TankData.speed * Time.deltaTime);
            }
            else
            {
                // Fallback to transform-based movement
                tank.transform.position += moveDir * tank.TankData.speed * Time.deltaTime;
            }
        }
    }
}
