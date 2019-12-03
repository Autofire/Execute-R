using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AngryEyes: MonoBehaviour {
    private const float WAIT_TIME = 1.0f, MOVE_TIME = 0.3f, FUZZ_PERCENT = 0.3f;
    float progress, maxProgress;
    Quaternion oldRot, newRot;

    private float Fuzz(float baseVal) {
        return baseVal * Random.Range(1.0f - FUZZ_PERCENT, 1.0f + FUZZ_PERCENT);
    }

    private void Plan() {
        progress = -Fuzz(WAIT_TIME);
        maxProgress = Fuzz(MOVE_TIME);
        oldRot = gameObject.transform.localRotation;
        newRot = Quaternion.Euler(Random.Range(10, -10), Random.Range(-20, 20), 0);
    }

    public void FocusAhead() {
        oldRot = gameObject.transform.localRotation;
        newRot = Quaternion.Euler(0, 0, 0);
        progress = 0.0f;
        maxProgress = MOVE_TIME;
    }

    void Start() {
        Plan();
    }

    // Update is called once per frame
    void Update() {
        progress += Time.deltaTime;
        if (progress > maxProgress) {
            Plan();
        }
        if (progress > 0.0f) {
            float percent = progress / maxProgress;
            gameObject.transform.localRotation = Quaternion.Lerp(oldRot, newRot, percent);
        } else {
            gameObject.transform.localRotation = oldRot;
        }
    }
}
