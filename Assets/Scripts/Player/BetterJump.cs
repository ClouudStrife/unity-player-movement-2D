using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BetterJump : MonoBehaviour {
    public float fallMultiplier;
    public float lowJumpMult;

    public Rigidbody2D player_rb;

    void Update() {
        if (player_rb.velocity.y < 0) {
            player_rb.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        }
        else if (player_rb.velocity.y > 0 && !(Input.GetButton("Jump"))) {
            player_rb.velocity += Vector2.up * Physics2D.gravity.y * (lowJumpMult - 1) * Time.deltaTime;
        }
    }
}
