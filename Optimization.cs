using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Optimization : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        //Set target frame rate to 60 fps
        Application.targetFrameRate = 600;

        // Disable VSync
        QualitySettings.vSyncCount = 0;

        // Lower quality settings
        QualitySettings.SetQualityLevel(0, true);

        // Adjust fixed timestep
        Time.fixedDeltaTime = 0.02f; // Try adjusting to 0.03f or 0.04f if needed

        //    // Set collision detection to Discrete for all Rigidbodies
        //    foreach (Rigidbody rb in FindObjectsOfType<Rigidbody>())
        //    {
        //        rb.collisionDetectionMode = CollisionDetectionMode.Discrete;
        //    }

        //    // Lower physics iterations
        Physics.defaultSolverIterations = 4; // Default is 6, try lowering to 4 if needed
        Physics.defaultSolverVelocityIterations = 1; // Default is 1, try lowering if needed
    }
}
