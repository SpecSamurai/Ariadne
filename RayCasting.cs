using System.Buffers;
using System.Numerics;

namespace Ariadne;

public record HitWall(float WallDistance, int MapX, int MapY);
public record ScreenStripe(int Start, int End);

public class RayCasting
{
    public const int GridSize = 1;
    public const char WALL = '#';
    public const char EMPTY_SPACE = '.';

    private readonly int[,] map;
    private readonly int screenWidth;
    private readonly int screenHeight;

    public RayCasting(int[,] map, int screenWidth, int screenHeight)
    {
        this.map = map;
        this.screenWidth = screenWidth;
        this.screenHeight = screenHeight;
    }

    public char[][] Cast(Player player)
    {
        var screen = new char[screenHeight][];
        for (int h = 0; h < screen.Length; h++)
        {
            screen[h] = Enumerable.Repeat(EMPTY_SPACE, screenWidth).ToArray();
        }

        for (int screenX = 0; screenX < screenWidth; screenX++)
        {
            var wall = CalculateHitWall(player, screenX);
            var (start, end) = CalculateScreenStripe(wall);

            if (map[wall.MapX, wall.MapY] > 0)
            {
                for (int y = start; y < end; y++)
                {
                    screen[y][screenX] = WALL;
                }
            }
        }

        return screen;
    }

    public HitWall CalculateHitWall(Player player, int screenX)
    {
        var cameraX = 2 * screenX / (float)screenWidth - 1;
        var rayDirection = new Vector2(
            player.Direction.X + player.CameraPlane.X * cameraX,
            player.Direction.Y + player.CameraPlane.Y * cameraX
        );

        (int X, int Y) mapPosition = ((int)(player.Position.X), (int)(player.Position.Y));
        (float X, float Y) deltaDistance = (
            (rayDirection.X == 0) ? float.MaxValue : Math.Abs(1 / rayDirection.X),
            (rayDirection.Y == 0) ? float.MaxValue : Math.Abs(1 / rayDirection.Y)
        );

        (float X, float Y) sideDistance = (
            rayDirection.X < 0
                ? (player.Position.X - mapPosition.X) * deltaDistance.X
                : (mapPosition.X + 1.0f - player.Position.X) * deltaDistance.X,
            rayDirection.Y < 0
                ? (player.Position.Y - mapPosition.Y) * deltaDistance.Y
                : (mapPosition.Y + 1.0f - player.Position.Y) * deltaDistance.Y
        );

        (int X, int Y) step = (
            rayDirection.X < 0 ? -GridSize : GridSize,
            rayDirection.Y < 0 ? -GridSize : GridSize
        );

        var hit = false;
        var hitSideOfTheWall = WallSide.Invalid;
        while (hit is false)
        {
            if (sideDistance.X < sideDistance.Y)
            {
                sideDistance.X += deltaDistance.X;
                mapPosition.X += step.X;
                hitSideOfTheWall = WallSide.X;
            }
            else
            {
                sideDistance.Y += deltaDistance.Y;
                mapPosition.Y += step.Y;
                hitSideOfTheWall = WallSide.Y;
            }

            if (map[mapPosition.X, mapPosition.Y] > 0) hit = true;
        }

        var perpendicularRayDistance = hitSideOfTheWall == WallSide.X
            ? sideDistance.X - deltaDistance.X
            : sideDistance.Y - deltaDistance.Y;

        return new HitWall(perpendicularRayDistance, mapPosition.X, mapPosition.Y);
    }

    public ScreenStripe CalculateScreenStripe(HitWall perpendicularWall)
    {
        int lineHeight = (int)(screenHeight / perpendicularWall.WallDistance);

        int drawStart = -lineHeight / 2 + screenHeight / 2;
        if (drawStart < 0) drawStart = 0;
        int drawEnd = lineHeight / 2 + screenHeight / 2;
        if (drawEnd >= screenHeight) drawEnd = screenHeight - 1;

        return new ScreenStripe(drawStart, drawEnd);
    }
}
