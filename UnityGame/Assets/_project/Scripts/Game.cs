using UnityEngine;
using MrV;

public class Game : MonoBehaviour {
    Map2d map;
    public Vector3 scale = Vector3.one * 4;
    [ContextMenuItem("add rules", nameof(AddRules))]
    public GameObject wall, floor, brokenWall, missileShooter, messageAboutShooting, youWinMessage;
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
        if (!missileShooter.activeInHierarchy) {
            missileShooter.SetActive(true);
            messageAboutShooting.SetActive(true);
        }
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
}
