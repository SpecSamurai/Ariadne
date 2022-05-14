using System.Numerics;

namespace Ariadne;

public class Player
{
    private const float MOVE_SPEED = 0.2f;
    private const float ROTATION_SPEED = 0.05f;

    public Vector2 Position { get; private set; }
    public Vector2 Direction { get; private set; }
    public Vector2 CameraPlane { get; private set; }

    public Player(Vector2 position, Vector2 direction, Vector2 cameraPlane)
    {
        Position = position;
        Direction = direction;
        CameraPlane = cameraPlane;
    }

    public void MoveForward() =>
        Position += Vector2.Multiply(Direction, MOVE_SPEED);

    public void MoveBackward() =>
        Position -= Vector2.Multiply(Direction, MOVE_SPEED);

    public void RotateLeft()
    {
        Direction = new Vector2(
            Direction.X * (float)Math.Cos(ROTATION_SPEED) - Direction.Y * (float)Math.Sin(ROTATION_SPEED),
            Direction.X * (float)Math.Sin(ROTATION_SPEED) + Direction.Y * (float)Math.Cos(ROTATION_SPEED)
        );

        CameraPlane = new Vector2(
            CameraPlane.X * (float)Math.Cos(ROTATION_SPEED) - CameraPlane.Y * (float)Math.Sin(ROTATION_SPEED),
            CameraPlane.X * (float)Math.Sin(ROTATION_SPEED) + CameraPlane.Y * (float)Math.Cos(ROTATION_SPEED)
        );
    }

    public void RotateRight()
    {
        Direction = new Vector2(
            Direction.X * (float)Math.Cos(-ROTATION_SPEED) - Direction.Y * (float)Math.Sin(-ROTATION_SPEED),
            Direction.X * (float)Math.Sin(-ROTATION_SPEED) + Direction.Y * (float)Math.Cos(-ROTATION_SPEED)
        );

        CameraPlane = new Vector2(
            CameraPlane.X * (float)Math.Cos(-ROTATION_SPEED) - CameraPlane.Y * (float)Math.Sin(-ROTATION_SPEED),
            CameraPlane.X * (float)Math.Sin(-ROTATION_SPEED) + CameraPlane.Y * (float)Math.Cos(-ROTATION_SPEED)
        );
    }
}
