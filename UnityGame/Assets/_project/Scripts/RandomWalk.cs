using NonStandard;
using NonStandard.Character;
using UnityEngine;

public class RandomWalk : MonoBehaviour {
    CharacterMove cm;
    long whenToChangeMoveTarget = 0;
    public float moveAttentionSpan = 3;
    public float randomWalkTurnArc = 180;
    void Start() {
        cm = GetComponent<CharacterMove>();
        SelectNextMoveTarget();
    }
    void SelectNextMoveTarget() {
        float angleTurn = Random.Range(-randomWalkTurnArc / 2, randomWalkTurnArc / 2);
        float horizonAdjust = Random.Range(-45, 0);
        Quaternion gaze = transform.rotation;
        gaze *= Quaternion.AngleAxis(horizonAdjust, transform.right);
        gaze *= Quaternion.AngleAxis(angleTurn, Vector3.up);
        Vector3 dir = gaze * Vector3.forward;
        Lines.Make("gaze").Arrow(cm.head.position, cm.head.position + dir, Color.cyan);
        Vector3 p = transform.position;
        if (Physics.Raycast(cm.head.position, dir, out RaycastHit hitInfo)) {
            p = hitInfo.point;
            transform.Rotate(Vector3.up, angleTurn);
        }
        cm.move.automaticMovement.SetAutoMovePosition(p, 1);
        whenToChangeMoveTarget = System.Environment.TickCount + (long)(moveAttentionSpan * 1000);
    }
    void Update() {
        if ((cm.move.automaticMovement.arrived && cm.move.IsStableOnGround) || System.Environment.TickCount > whenToChangeMoveTarget) {
            SelectNextMoveTarget();
        }
    }
}
