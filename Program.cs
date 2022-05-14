using Ariadne;
using Microsoft.AspNetCore.SignalR;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSignalR();

var app = builder.Build();
app.UseHttpsRedirection();
app.MapHub<RayCastingHub>("/raycasting");
app.Run();

public class RayCastingHub : Hub
{
    private static GameWindow gameWindow = new GameWindow();

    public async Task RayCast(string keyCode)
    {
        var screen = gameWindow.Run(keyCode);
        await Clients.All.SendAsync("ReceiveMessage", screen);
    }
}