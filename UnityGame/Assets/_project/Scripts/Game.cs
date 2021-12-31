using UnityEngine;
using MrV;

public class Game : MonoBehaviour {
    Map2d map;
    public Vector3 scale = Vector3.one * 4;
    [ContextMenuItem("add rules", nameof(AddRules))]
    public GameObject wall, floor, brokenWall, playerCharacter, missileShooter, actionButton, controlsMessage, messageAboutShooting, youWinMessage, mapViewer;
    public NonStandard.Inputs.UserInput userInput;
    public Material travelled;
    void Start() {
        AddRules();
        string m = MazeGen.CreateMaze(new Coord(99, 51), Coord.One, 123);
        map = new Map2d(m);
        CreateMapWithPrefabs();
    }
    public void AddRules() {
        CollidableRules r = CollidableRules.Instance;
        r.AddIfMissingNamed(new CollidableRules.Rule("touching the goal object ends the game", Collidable.Kind.Player, Collidable.Kind.Goal,
            new NonStandard.EventBind(this, nameof(YouWin))));
        r.AddIfMissingNamed(new CollidableRules.Rule("maze traveller marks floor", Collidable.Kind.Player, Collidable.Kind.Floor,
            new NonStandard.EventBind(this, nameof(MarkFloor))));
        r.AddIfMissingNamed(new CollidableRules.Rule("MrV makes the player a wizard", Collidable.Kind.Player, Collidable.Kind.MrV,
            new NonStandard.EventBind(this, nameof(MakeWizard))));
        r.AddIfMissingNamed(new CollidableRules.Rule("missile destroys walls", Collidable.Kind.MagicMissile, Collidable.Kind.Wall,
            new NonStandard.EventBind(r, nameof(r.DestroyBoth))));
        userInput.AddBinding(new NonStandard.Inputs.InputControlBinding("swap to map view", "Player/MapView", NonStandard.Inputs.ControlType.Button,
            new NonStandard.EventBind(this, nameof(this.SwapCharacterAndMapView)), new string[] { "<Keyboard>/m" }));
        userInput.AddBinding(new NonStandard.Inputs.InputControlBinding("show controls information", "Player/ShowControls", NonStandard.Inputs.ControlType.Button,
            new NonStandard.EventBind(this, nameof(this.ToggleControlsMessage)), new string[] { "<Keyboard>/h" }));
    }
    private void Reset() {
        AddRules();
    }
    public void YouWin(Collidable[] collidables) {
        youWinMessage.SetActive(true);
    }
    public void MarkFloor(Collidable[] collidables) {
        collidables[1].GetComponent<Renderer>().material = travelled;
    }
    public void MakeWizard(Collidable[] collidables) {
        if (missileShooter.activeInHierarchy) { return; }
        missileShooter.SetActive(true);
        messageAboutShooting.SetActive(true);
        NonStandard.Utility.Event buttonEvent = actionButton.GetComponent<NonStandard.Utility.Event>();
        buttonEvent.Set(this, nameof(ShootMagicMissile));
        NonStandard.Ui.Cooldown cd = actionButton.GetComponent <NonStandard.Ui.Cooldown>();
        cd.cooldown = .5f;
    }
    public void ShootMagicMissile() {
        if (!NonStandard.Inputs.UserInput.IsMouseOverUIObject() && Cursor.lockState != CursorLockMode.Locked) {
            PointAtMouse.UpdateDirection(playerCharacter.transform, PointAtMouse.LockAxis.YAxis);
            PointAtMouse.UpdateDirection(missileShooter.transform);
        } else {
            missileShooter.transform.localRotation = Quaternion.identity;
        }
        missileShooter.GetComponent<NonStandard.Utility.Event>().Invoke();
    }

    void CreateMapWithPrefabs() {
        Coord c = Coord.Zero;
        for(c.Y = 0; c.Y < map.Height; ++c.Y) {
            for(c.X = 0; c.X < map.Width; ++c.X) {
                char tileCh = map[c];
                GameObject tileGo = null;
                switch (tileCh) {
                    case ' ': case '.': tileGo = floor; break;
                    case ',': tileGo = brokenWall; break;
                    default: tileGo = wall; break;
                }
                GameObject go = Instantiate(tileGo);
                Vector3 p = new Vector3(c.X, 0, -c.Y);
                p.Scale(scale);
                go.transform.SetParent(transform);
                go.transform.localPosition = p;
            }
        }
    }
    public void SwapCharacterAndMapView(UnityEngine.InputSystem.InputAction.CallbackContext context) {
        if (context.phase != UnityEngine.InputSystem.InputActionPhase.Performed) { return; }
        NonStandard.Character.UserController user = NonStandard.Character.UserController.GetUserCharacterController();
        if (user.Target.gameObject == playerCharacter) {
            mapViewer.transform.position = user.GetCameraTarget().position;
            mapViewer.SetActive(true);
            mapViewer.GetComponent<NonStandard.Character.CharacterRoot>().TakeControlOfUserInterface();
            return;
        }
        if (user.Target.gameObject == mapViewer) {
            playerCharacter.GetComponent<NonStandard.Character.CharacterRoot>().TakeControlOfUserInterface();
            mapViewer.SetActive(false);
            return;
        }
        Debug.LogWarning("User is not controlling "+ playerCharacter+" or "+ mapViewer+"?");
    }
    public void ToggleControlsMessage(UnityEngine.InputSystem.InputAction.CallbackContext context) {
        controlsMessage.SetActive(!controlsMessage.activeInHierarchy);
    }
}
