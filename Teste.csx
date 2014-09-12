using System.IO;
using Microsoft.AspNet.SignalR.Hubs;

var watcher = new FileSystemWatcher();

watcher.Path = Environment.CurrentDirectory;
Console.WriteLine("Listening on path: {0}",watcher.Path);
watcher.Filter = "*.css";
watcher.Changed += new FileSystemEventHandler((source, e) => 
{
	Console.WriteLine("File: " +  e.FullPath + " " + e.ChangeType);
	var hubContente = GlobalHost.ConnectionManager.GetHubContext<MyHub>();
	var hb = hubContente.Clients.All as IClientProxy;
	hb.Invoke("addMessage", e.Name);
});
watcher.EnableRaisingEvents = true;

var signalR = Require<SignalR>();
signalR.CreateServer("http://localhost:8080");
Console.WriteLine("server created");
Console.ReadLine();

public class MyHub : Hub
{
    public void Send(string message)
    {
        Console.WriteLine("Responding to message: {0}", message);
        var strings = message.Split(new[]{' '});
        foreach (var s in strings) 
        	Clients.SendToAll("addMessage", s);
        Clients.SendToCaller("addMessage", "Hello caller! Thanks for sending " + message);  
    }
}