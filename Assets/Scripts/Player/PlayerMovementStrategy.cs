using UnityEngine;

public class PlayerMovementStrategy : ITankMovementStrategy
{
    private ITankInput input;
    private Transform cameraPivot;
    private float rotationSpeed = 5f;
    private float moveDeadZone = 0.1f;

    public PlayerMovementStrategy(ITankInput input, Transform cameraPivot)
    {
        this.input = input;
        this.cameraPivot = cameraPivot;
    }

    public void Move(TankController tank)
    {
        Vector2 inputDir = input.MoveInput;

        if (inputDir.sqrMagnitude < moveDeadZone * moveDeadZone)
            return;

        float vertical = inputDir.y;
        float horizontal = inputDir.x;

        // Use the camera's forward to set the world-relative "forward" direction
        Vector3 camForward = Vector3.ProjectOnPlane(cameraPivot.forward, Vector3.up).normalized;
        float tankForwardAngle = Mathf.Atan2(camForward.x, camForward.z) * Mathf.Rad2Deg;

        // Rotate the tank based on horizontal input (like tank treads turning)
        tank.transform.Rotate(0f, horizontal * rotationSpeed, 0f);

        // Move the tank forward or backward along its current forward
        Vector3 moveDir = tank.transform.forward * vertical;

        tank.transform.position += moveDir * tank.tankData.speed * Time.deltaTime;
    }
}
