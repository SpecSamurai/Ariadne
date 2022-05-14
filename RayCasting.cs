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
        int mapX = (int)(player.Position.X);
        int mapY = (int)(player.Position.Y);

        var cameraX = 2 * screenX / (float)screenWidth - 1;
        var ray = new Vector2(
            player.Direction.X + player.CameraPlane.X * cameraX,
            player.Direction.Y + player.CameraPlane.Y * cameraX
        );

        float sideDistanceX;
        float sideDistanceY;

        float deltaDistanceX = (ray.X == 0) ? 1e30f : Math.Abs(1 / ray.X);
        float deltaDistanceY = (ray.Y == 0) ? 1e30f : Math.Abs(1 / ray.Y);

        int stepX;
        int stepY;

        if (ray.X < 0)
        {
            stepX = -GridSize;
            sideDistanceX = (player.Position.X - mapX) * deltaDistanceX;
        }
        else
        {
            stepX = GridSize;
            sideDistanceX = (mapX + 1.0f - player.Position.X) * deltaDistanceX;
        }
        if (ray.Y < 0)
        {
            stepY = -GridSize;
            sideDistanceY = (player.Position.Y - mapY) * deltaDistanceY;
        }
        else
        {
            stepY = GridSize;
            sideDistanceY = (mapY + 1.0f - player.Position.Y) * deltaDistanceY;
        }

        int hit = 0;
        var side = WallSide.Invalid;
        while (hit == 0)
        {
            if (sideDistanceX < sideDistanceY)
            {
                sideDistanceX += deltaDistanceX;
                mapX += stepX;
                side = WallSide.X;
            }
            else
            {
                sideDistanceY += deltaDistanceY;
                mapY += stepY;
                side = WallSide.Y;
            }

            if (map[mapX, mapY] > 0) hit = 1;
        }

        var perpendicularRayDistance = side == WallSide.X
            ? sideDistanceX - deltaDistanceX
            : sideDistanceY - deltaDistanceY;

        return new HitWall(perpendicularRayDistance, mapX, mapY);
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
