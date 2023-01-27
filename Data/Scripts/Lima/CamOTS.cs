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
    public bool Zoom = true;

    public CamOTSConfig() { }
  }

  public class CamOTS
  {
    private Vector3D _zoom = Vector3D.Zero;
    private Vector3D _target = Vector3D.Zero;
    private bool _smooth = false;
    private bool _isZoomToggled = false;
    private bool _isWorking = false;
    private bool _isFirstPerson = false;

    public CamOTSConfig Config = new CamOTSConfig();

    public void Update()
    {
      if (_isWorking && !Config.Enabled)
      {
        MyAPIGateway.Session.SetCameraController(VRage.Game.MyCameraControllerEnum.Entity);
        _isWorking = false;
        return;
      }

      var player = MyAPIGateway.Session.Player;
      if (player == null)
        return;

      var isOnFoot = player.Controller?.ControlledEntity is IMyCharacter;
      var spectatorCamera = MyAPIGateway.Session.CameraController as MySpectator;
      CheckCameraInputKey(spectatorCamera != null, isOnFoot, _isFirstPerson);
      _isFirstPerson = MyAPIGateway.Session.CameraController?.IsInFirstPersonView ?? false;
      if (spectatorCamera == null)
        return;

      if (spectatorCamera.SpectatorCameraMovement != MySpectatorCameraMovementEnum.None)
        return;

      if (!(isOnFoot))
        return;

      var isPlacer = player.Character.EquippedTool is IMyBlockPlacerBase;

      var released2ndAction = MyAPIGateway.Input.IsControl(MyStringId.GetOrCompute("ACTIONS"), MyControlsSpace.SECONDARY_TOOL_ACTION, VRage.Input.MyControlStateType.NEW_RELEASED);
      if (!MyAPIGateway.Gui.IsCursorVisible && !Config.Hold && released2ndAction && !(player.Character.EquippedTool is IMyHandDrill) && !isPlacer)
        _isZoomToggled = !_isZoomToggled;

      var zoom = false;
      var pressed2ndAction = MyAPIGateway.Input.IsControl(MyStringId.GetOrCompute("ACTIONS"), MyControlsSpace.SECONDARY_TOOL_ACTION, VRage.Input.MyControlStateType.PRESSED);
      if (!MyAPIGateway.Gui.IsCursorVisible && !isPlacer && ((!Config.Hold && _isZoomToggled) || pressed2ndAction))
        zoom = true;

      var isHand = player.Character.EquippedTool == null;
      var isTool = player.Character.EquippedTool is IMyHandDrill || player.Character.EquippedTool is IMyWelder || player.Character.EquippedTool is IMyAngleGrinder;

      var charHeadMatrix = player.Character.GetHeadMatrix(false);
      var charHeadPos = charHeadMatrix.Translation;
      var offset = (isTool || isHand || isPlacer) ? Vector3D.Zero : charHeadMatrix.Down * 0.2 + charHeadMatrix.Right * 0.2;
      var charShoulderPos = charHeadPos + offset;

      IHitInfo hit;
      if (MyAPIGateway.Physics.CastRay(charShoulderPos, charShoulderPos + charHeadMatrix.Forward * 10 + offset, out hit, CollisionLayers.CollisionLayerWithoutCharacter))
      {
        _target = hit.Position;
      }
      else
        _target = charShoulderPos + charHeadMatrix.Forward * 10;

      var newZoom = zoom ? charHeadMatrix.Forward * 1.1 + charHeadMatrix.Left * 0.15 : Vector3D.Zero;
      _zoom = Config.Zoom ? Vector3D.Lerp(_zoom, newZoom, 0.15) : Vector3D.Zero;

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

    private void CheckCameraInputKey(bool isSpectator, bool onFoot, bool isFirstPerson)
    {
      if (MyAPIGateway.Gui.IsCursorVisible)
        return;
      if (MyAPIGateway.Input.IsControl(MyStringId.GetOrCompute("CHARACTER"), MyControlsSpace.CAMERA_MODE, VRage.Input.MyControlStateType.NEW_PRESSED))
      {
        if (isSpectator)
        {
          MyAPIGateway.Session.SetCameraController(VRage.Game.MyCameraControllerEnum.Entity);
        }
        else if (isFirstPerson && onFoot)
        {
          MyAPIGateway.Session.SetCameraController(VRage.Game.MyCameraControllerEnum.SpectatorFixed);
          _smooth = false;
        }
      }
    }
  }
}