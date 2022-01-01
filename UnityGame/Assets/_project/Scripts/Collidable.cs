using System.Collections.Generic;
using UnityEngine;

public class Collidable : MonoBehaviour {
    public enum Kind { None, Floor, Wall, MagicMissile, Player, MrV, Goal }
    public Kind kind;
    private void OnCollisionEnter(Collision collision) {
        Collidable[] collidables = collision.collider.GetComponents<Collidable>();
        if (collidables != null && collidables.Length > 0) {
            for (int i = 0; i < collidables.Length; i++) {
                Collidable collidable = collidables[i];
                List<CollidableRules.Rule> rules = CollidableRules.Instance.GetRule(this, collidable);
                if (rules != null) {
                    rules.ForEach(r => r.onCollision.Invoke(new Collidable[] { this, collidable }));
                }
            }
        }
    }
}
