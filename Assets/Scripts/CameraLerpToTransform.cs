using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraLerpToTransform : MonoBehaviour {

    // public fields let you specify the target the camera will track
    // tracking speed and the camera's min & max bounds.
    public Transform camTarget;
    public float trackingSpeed;
    public float minX;
    public float minY;
    public float maxX;
    public float maxY;

    // FixedUpdate() is called with every fixed-framerate frame — it’s the best method
    // when dealing with a Rigidbody component of any kind.The result is the camera will
    // track the player, which will have a RigidBody2D attached to it.
    void FixedUpdate()
    {
        // This null check ensures that a valid Transform component was assigned to the
        // camTarget field on the script in the editor.
        if (camTarget != null) {

            // Vector2.Lerp() performs smooth linear transition between two vectors by the third
            // parameter's value(a value between 0 and 1). Normally, you’d usually use this to
            // move between positions over time, but here you actually slow tracking as you near
            // the camera's target(Soy Boy). The rest of the Lerp method ensures that the position
            // calculated is clamped (constrained)to the MinX, MinY, MaxX, and MaxY points.
            var newPos = Vector2.Lerp(transform.position, camTarget.position, Time.deltaTime * trackingSpeed);
            //note z=-10f, just means the camera is always 10 units away from the 2D game
            var camPosition = new Vector3(newPos.x, newPos.y, -10f);
            var v3 = camPosition;
            var clampX = Mathf.Clamp(v3.x, minX, maxX);
            var clampY = Mathf.Clamp(v3.y, minY, maxY);
            transform.position = new Vector3(clampX, clampY, -10f); //this line actually moves the cam
                                                                    //incrementally towards its goal
        }
    }
}

