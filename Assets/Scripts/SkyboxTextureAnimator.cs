using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract class Animation {
    protected int frame = 0;
    protected int duration = 0;

    public bool UpdateWrapper(SkyboxTextureAnimator animator) {
        Update(animator);
        frame++;
        return frame == duration;
    }

    protected abstract void Update(SkyboxTextureAnimator animator);

    public abstract int GetCost();
}

class LineAnimation : Animation {
    private int width = Random.Range(15, 20), height = Random.Range(8, 12);
    private int x = Random.Range(12, 100), y = Random.Range(12, 108);

    public LineAnimation() {
        duration = height * 2;
    }

    protected override void Update(SkyboxTextureAnimator animator) {
        if (frame < height) {
            animator.DrawTextLine(x, y + frame, Random.Range(10, width));
        } else {
            animator.WriteRect(x, y + frame - height, width, 1, false);
        }
    }

    public override int GetCost() { return 4; }
}

class RaindropAnimation : Animation {
    private int x = Random.Range(12, 114), y = 12;

    public RaindropAnimation() {
        duration = 110;
    }

    protected override void Update(SkyboxTextureAnimator animator) { 
        animator.Write(x, y + frame - 5, false);
        animator.Write(x, y + frame, true);
    }
    public override int GetCost() { return 1; }
}

class WormAnimation : Animation {
    private int x = Random.Range(12, 144), y = Random.Range(12, 144);
    private List<int> oldx = new List<int>(), oldy = new List<int>();
    private int length = Random.Range(15, 30);
    // +x 0 +y 1 -x 2 -y 3 none 4
    private int direction = Random.Range(0, 4);

    public WormAnimation() {
        duration = Random.Range(80, 140);
    }

    protected override void Update(SkyboxTextureAnimator animator) {
        if (Random.Range(0, 6) == 0) {
            // Rotate randomly left or right. Or don't.
            direction += Random.Range(0, 3) - 1;
            direction %= 4;
        }
        if (x == 12) {
            direction = 1;
        } else if (x == 144) {
            direction = 3;
        }
        if (y == 12) {
            direction = 0;
        } else if (y == 144) {
            direction = 2;
        }
        oldx.Add(x);
        oldy.Add(y);
        if (direction == 0)  x++;
        if (direction == 1)  y++;
        if (direction == 2)  x--;
        if (direction == 3)  y--;
        if (frame < duration - length) {
            animator.On(x, y);
        }
        if (frame - length >= 0) {
            animator.Off(oldx[frame - length], oldy[frame - length]);
        }
    }

    public override int GetCost() { return 1; }
}

public class SkyboxTextureAnimator : MonoBehaviour {
    private const int MARGIN = 12, ANTIMARGIN = 128 - MARGIN, MAX_COST = 300;
    private const float FRAME_TIME = 0.1f;
    private float frameTimer = 0;
    public Texture2D skyboxTextTexture;
    private Color[] pixels;
    private List<Animation> animations = new List<Animation>();

    void Start() {
        pixels = skyboxTextTexture.GetPixels();
        for (int pixel = 0; pixel < 128 * 128; pixel++) {
            pixels[pixel] = Color.black;
        }
        for (int frame = 0; frame < 100; frame++) {
            AnimCycle();
        }
    }

    public void Write(int x, int y, bool active) {
        if (x < MARGIN || x > ANTIMARGIN || y < MARGIN || y > ANTIMARGIN) return;
        pixels[y * 128 + x] = active ? Color.white : Color.black;
    }

    public void WriteRect(int x, int y, int width, int height, bool value) {
        if (x < MARGIN) x = MARGIN;
        if (y < MARGIN) y = MARGIN;
        if (x + width > ANTIMARGIN) width = ANTIMARGIN - x;
        if (y + height > ANTIMARGIN) height = ANTIMARGIN - y;
        if (width <= 0 || height <= 0) return;
        Color color = value ? Color.white : Color.black;
        for (int dx = 0; dx < width; dx++) {
            for (int dy = 0; dy < height; dy++) {
                pixels[(y + dy) * 128 + x + dx] = color;
            }
        }
    }

    private bool Read(int x, int y) {
        return pixels[x * 128 + y].r > 0;
    }

    public void On(int x, int y) {
        Write(x, y, true);
    }

    public void Off(int x, int y) {
        Write(x, y, false);
    }

    public void Randomize(int x, int y) {
        Write(x, y, Random.Range(0, 2) == 0);
    }

    public void Flip(int x, int y) {
        Write(x, y, !Read(x, y));
    }

    public void DrawTextLine(int x, int y, int width) {
        for (int dx = 0; dx < width; dx++) {
            Write(x + dx, y, Random.Range(0, 8) != 0);
        }
    }

    private void AddRandomAnim() {
        switch (Random.Range(0, 5)) {
            case 0:
            animations.Add(new LineAnimation());
            break;
            case 1:
            case 2:
            case 3:
            case 4:
            animations.Add(new WormAnimation());
            break;
        }
    }

    private void AnimCycle() {
        List<Animation> uncompleted = new List<Animation>();
        int totalCost = 0;
        foreach (Animation anim in animations) {
            bool completed = anim.UpdateWrapper(this);
            if (!completed) {
                uncompleted.Add(anim);
                totalCost += anim.GetCost();
            }
        }
        animations = uncompleted;
        if (totalCost < MAX_COST) {
            AddRandomAnim();
        }
    }

    void Update() {
        frameTimer += Time.deltaTime;
        bool frameRun = frameTimer > FRAME_TIME;
        while (frameTimer > FRAME_TIME) {
            frameTimer -= FRAME_TIME;
            AnimCycle();
        }
        if (frameRun) {
            skyboxTextTexture.SetPixels(pixels);
            skyboxTextTexture.Apply();
        }
    }
}
