using Sandbox.ModAPI;
using VRage.Game.Components;

namespace Lima.OverTheShoulder
{
  [MySessionComponentDescriptor(MyUpdateOrder.AfterSimulation)]
  public class CamOTSSession : MySessionComponentBase
  {
    private CamOTS _cam;
    private FileStorage<CamOTSConfig> _fileHandler = new FileStorage<CamOTSConfig>("CameraOTS.xml");

    public override void LoadData()
    {
      if (MyAPIGateway.Utilities.IsDedicated)
        return;

      _cam = new CamOTS();
      var current = _fileHandler.Load();
      if (current != null)
        _cam.Config = current;

      MyAPIGateway.Utilities.MessageEntered += MessageHandler;
    }

    protected override void UnloadData()
    {
      if (MyAPIGateway.Utilities.IsDedicated)
        return;

      MyAPIGateway.Utilities.MessageEntered -= MessageHandler;
      _cam = null;
    }

    private void MessageHandler(string message, ref bool sendToOthers)
    {
      message = message.ToLower();
      if (message.StartsWith("/otscam "))
      {
        sendToOthers = false;
        var args = message.Split(' ');
        if (args[1] == "hold")
          _cam.Config.Hold = (args[2] != "false");
        else if (args[1] == "collision")
          _cam.Config.Collision = (args[2] != "false");
        else if (args[1] == "enabled" || args[1] == "enable")
          _cam.Config.Enabled = (args[2] != "false");
        else if (args[1] == "zoom")
          _cam.Config.Zoom = (args[2] != "false");
        else if (args[1] == "left")
          _cam.Config.Left = (args[2] != "false");
        else if (args[1] == "keybind")
          _cam.Config.KeyBind = (args[2] != "false");

        _fileHandler.Save(_cam.Config);
      }
    }

    public override void UpdateAfterSimulation()
    {
      if (MyAPIGateway.Utilities.IsDedicated)
        return;

      _cam.Update();
    }
  }
}