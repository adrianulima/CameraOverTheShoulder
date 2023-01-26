using Sandbox.Game;
using Sandbox.ModAPI;
using Sandbox.ModAPI.Weapons;
using VRage;
using VRage.Game.ModAPI;
using VRage.Utils;
using VRageMath;
using CollisionLayers = Sandbox.Engine.Physics.MyPhysics.CollisionLayers;

namespace Lima.OverTheShoulder
{
  public class CamOTSConfig
  {
    public bool Hold = true;
    public bool Collision = true;
    public bool Enabled = true;

    public CamOTSConfig() { }
  }

  public class CamOTS
  {
    private Vector3D _zoom = Vector3D.Zero;
    private Vector3D _target = Vector3D.Zero;
    private bool _smooth = false;
    private bool _isZoomToggled = false;
    private bool _isWorking = false;

    public CamOTSConfig Config = new CamOTSConfig();

    public void Update()
    {
      var spectatorCamera = MyAPIGateway.Session.CameraController as MySpectator;
      if (spectatorCamera == null)
      {
        CheckCameraInputKey(false);
        return;
      }

      if (spectatorCamera.SpectatorCameraMovement != MySpectatorCameraMovementEnum.None)
        return;

      if (_isWorking && !Config.Enabled)
      {
        MyAPIGateway.Session.SetCameraController(VRage.Game.MyCameraControllerEnum.Entity);
        _isWorking = false;
        return;
      }

      var player = MyAPIGateway.Session.Player;
      if (player == null)
        return;

      if (!(player.Controller?.ControlledEntity is IMyCharacter))
        return;

      CheckCameraInputKey(true);

      var released2ndAction = MyAPIGateway.Input.IsControl(MyStringId.GetOrCompute("ACTIONS"), MyControlsSpace.SECONDARY_TOOL_ACTION, VRage.Input.MyControlStateType.NEW_RELEASED);
      if (!MyAPIGateway.Gui.IsCursorVisible && !Config.Hold && released2ndAction && !(player.Character.EquippedTool is IMyHandDrill))
        _isZoomToggled = !_isZoomToggled;

      var zoom = false;
      var pressed2ndAction = MyAPIGateway.Input.IsControl(MyStringId.GetOrCompute("ACTIONS"), MyControlsSpace.SECONDARY_TOOL_ACTION, VRage.Input.MyControlStateType.PRESSED);
      if (!MyAPIGateway.Gui.IsCursorVisible && ((!Config.Hold && _isZoomToggled) || pressed2ndAction))
        zoom = true;

      var charHeadMatrix = player.Character.GetHeadMatrix(false);
      var charHeadPos = charHeadMatrix.Translation;
      var offset = charHeadMatrix.Down * 0.2 + charHeadMatrix.Right * 0.2;
      var charShoulderPos = charHeadPos + offset;

      IHitInfo hit;
      if (MyAPIGateway.Physics.CastRay(charShoulderPos, charShoulderPos + charHeadMatrix.Forward * 10 + offset, out hit, CollisionLayers.CollisionLayerWithoutCharacter))
        _target = hit.Position;
      else
        _target = charShoulderPos + charHeadMatrix.Forward * 10;

      var newZoom = zoom ? charHeadMatrix.Forward * 1.1 + charHeadMatrix.Left * 0.15 : Vector3D.Zero;
      _zoom = Vector3D.Lerp(_zoom, newZoom, 0.15);

      var desiredPos = charHeadPos + charHeadMatrix.Backward * 2 + charHeadMatrix.Right * 0.6 + _zoom;
      Vector3D targetToCam = Vector3D.Normalize(_target - desiredPos);

      var distanceDesiredToHead = Vector3D.Distance(desiredPos, charHeadPos);
      var newCamPos = desiredPos;

      if (Config.Collision || (MyAPIGateway.Multiplayer.MultiplayerActive && !MyAPIGateway.Multiplayer.IsServer && !MyAPIGateway.Session.IsUserAdmin(player.SteamUserId)))
      {
        IHitInfo hitCam;
        if (MyAPIGateway.Physics.CastRay(desiredPos + targetToCam * distanceDesiredToHead, desiredPos - targetToCam * 1, out hitCam, CollisionLayers.CollisionLayerWithoutCharacter))
          newCamPos = hitCam.Position + targetToCam * 0.1;
        if (MyAPIGateway.Physics.CastRay(newCamPos + charHeadMatrix.Left * 1, newCamPos + charHeadMatrix.Right * 1, out hitCam, CollisionLayers.CollisionLayerWithoutCharacter))
          newCamPos = newCamPos + targetToCam * 0.1 - charHeadMatrix.Right * (1 - hitCam.Fraction);

        var distance = Vector3D.Distance(newCamPos, charHeadPos);
        if (distance > distanceDesiredToHead)
          newCamPos = desiredPos;

        if (distance < 0.8f)
          newCamPos = charShoulderPos + charHeadMatrix.Up * 0.2;
      }

      spectatorCamera.Position = _smooth ? newCamPos : Vector3D.Lerp(spectatorCamera.Position, newCamPos, 0.35);
      spectatorCamera.SetTarget(_target, charHeadMatrix.Up);

      _smooth = true;
      _isWorking = true;
    }

    private void CheckCameraInputKey(bool isSpectator)
    {
      if (MyAPIGateway.Gui.IsCursorVisible)
        return;

      if (MyAPIGateway.Input.IsControl(MyStringId.GetOrCompute("CHARACTER"), MyControlsSpace.CAMERA_MODE, VRage.Input.MyControlStateType.NEW_PRESSED))
      {
        var isFirstPerson = MyAPIGateway.Session?.CameraController?.IsInFirstPersonView ?? false;
        if (isSpectator) MyAPIGateway.Session.SetCameraController(VRage.Game.MyCameraControllerEnum.Entity);
        else if (isFirstPerson)
        {
          MyAPIGateway.Session.SetCameraController(VRage.Game.MyCameraControllerEnum.SpectatorFixed);
          _smooth = false;
        }
      }
    }
  }
}