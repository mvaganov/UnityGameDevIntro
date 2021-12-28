using NonStandard;
using NonStandard.Character;
using UnityEngine;
using UnityEngine.InputSystem;

public class PointAtMouse : MonoBehaviour {
    public enum LockAxis { None, XAxis, YAxis, ZAxis }
    public LockAxis lockAxis;
    public void UpdateDirection() {
        Ray ray = Global.GetComponent<CharacterCamera>().Camera.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (Physics.Raycast(ray, out RaycastHit hitInfo)) {
            Vector3 p = hitInfo.point;
            switch (lockAxis) {
                case LockAxis.XAxis: p.x = transform.position.x; break;
                case LockAxis.YAxis: p.y = transform.position.y; break;
                case LockAxis.ZAxis: p.z = transform.position.z; break;
            }
            transform.LookAt(p);
        }
    }
}
