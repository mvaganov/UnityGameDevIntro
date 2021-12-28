using UnityEngine;
using UnityEngine.Events;

public class CollidableLogic : MonoBehaviour {
    public Object argument;
    public void ActivateArgument(Collidable[] collidables) {
        switch (argument) {
            case GameObject go: go.SetActive(true); break;
            case MonoBehaviour m: m.enabled = true; break;
        }
    }
    public void DeactivateArgument(Collidable[] collidables) {
        switch (argument) {
            case GameObject go: go.SetActive(false); break;
            case MonoBehaviour m: m.enabled = false; break;
        }
    }
}