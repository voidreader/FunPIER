using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class CameraControl : MonoBehaviour {

    public Material mat; // Needed for setting camera material.
    [HideInInspector]
    public Gradient bottom, top; // They comes from game manager.
    Color overrideBottom, overrideTop; // Override colors
    AnimationCurve overrideMultiplierControlCurve; // To control override colors by animation curves.
    [HideInInspector]
    public float overrideMultiplier; // It is equal to normalized bonusBreakingTime.
    GameManager gm; // GameManager reference.
    void Start()
    {
        gm = GameObject.Find("GameManager").GetComponent<GameManager>(); // Gets game manager reference from the game manager game object in game scene.
        overrideMultiplierControlCurve = gm.overrideMultiplierControlCurve; // Get the curve from game manager. This curve can be set from game manager.
    }
    void OnPreRender()
    {
        overrideBottom = gm.overrideBottom; // Current override bottom color, it comes from game manager.
        overrideTop = gm.overrideTop; // Current override top color, it comes from game manager.
        GL.PushMatrix(); // Pushes new matrix
        mat.SetPass(0); // Sets this material for camera. 0 means, first sub shader pass.
        GL.LoadOrtho(); // Load orthographic view.
        GL.Begin(GL.QUADS); // Starting of drawing a quad.
        GL.Color((1- overrideMultiplier * overrideMultiplierControlCurve.Evaluate(overrideMultiplier)) *bottom.Evaluate(gm.gameStateSlider.value) + overrideBottom * overrideMultiplier * overrideMultiplierControlCurve.Evaluate(overrideMultiplier)); // Sets vertex color of under two vertexes those are located in under two lines.
        GL.Vertex3(0, 0, 1); // Bottom left of the screen.
        GL.Vertex3(1, 0, 1); // Bottom right of the screen.
        GL.Color((1 - overrideMultiplier * overrideMultiplierControlCurve.Evaluate(overrideMultiplier)) * top.Evaluate(gm.gameStateSlider.value) + overrideTop * overrideMultiplier * overrideMultiplierControlCurve.Evaluate(overrideMultiplier)); // Sets vertex color of under two vertexes those are located in under two lines.
        GL.Vertex3(1, 1, 1); // Top right of the screen.
        GL.Vertex3(0, 1, 1); // Top left of the screen.
        GL.End(); // End of the drawing.
        GL.PopMatrix(); // Pops the matrix.
    }
}