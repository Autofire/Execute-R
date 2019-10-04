using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// One option for doing movement in the game is having enemies capable of performing arbitrary
/// animations and then updating their cell position based on where they are currently positioned.
/// This is nice and simple but can cause problems with enemies overlapping each other on screen
/// and thus not having properly updated grid positions.
public class AnimatedExample : MonoBehaviour {
    GridDweller dweller;
    float timer = 0.0f;
    void Start() {
        dweller = GetComponent<GridDweller>();
    }

    // Update is called once per frame
    void Update() {
        timer += Time.deltaTime;
        // Move the position of our object.
        gameObject.transform.position = 
            Vector3.forward * Mathf.Sin(timer) * 5.0f
            + Vector3.left * Mathf.Cos(timer * 5);
        // Update which cell we are currently in.
        dweller.SyncCellPositionToRealPosition();
    }
}
