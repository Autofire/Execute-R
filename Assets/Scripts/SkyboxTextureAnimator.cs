using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkyboxTextureAnimator : MonoBehaviour {
    private const int MARGIN = 12, ANTIMARGIN = 128 - MARGIN;
    public Texture2D skyboxTextTexture;
    private Color[] pixels;

    void Start() {
        pixels = skyboxTextTexture.GetPixels();
        for (int pixel = 0; pixel < 128 * 128; pixel++) {
            pixels[pixel] = Color.black;
        }
    }

    private void Write(int x, int y, bool active) {
        if (x < MARGIN || x > ANTIMARGIN || y < MARGIN || y > ANTIMARGIN) return;
        pixels[x * 128 + y] = active ? Color.white : Color.black;
    }

    private bool Read(int x, int y) {
        return pixels[x * 128 + y].r > 0;
    }

    private void On(int x, int y) {
        Write(x, y, true);
    }

    private void Off(int x, int y) {
        Write(x, y, false);
    }

    private void Randomize(int x, int y) {
        Write(x, y, Random.Range(0, 2) == 0);
    }

    private void Flip(int x, int y) {
        Write(x, y, !Read(x, y));
    }

    void Update() {
        for (int i = 0; i < 400; i++) {
            int x = Random.Range(MARGIN, ANTIMARGIN), y = Random.Range(MARGIN, ANTIMARGIN);
            Flip(x, y);
        }
        skyboxTextTexture.SetPixels(pixels);
        skyboxTextTexture.Apply();
    }
}
