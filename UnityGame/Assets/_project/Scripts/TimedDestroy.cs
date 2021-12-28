using UnityEngine;

public class TimedDestroy : MonoBehaviour {
    public float lifetime = 3;
    void Start() {
        Destroy(gameObject, lifetime);
    }
}
