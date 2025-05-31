using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineController : MonoBehaviour
{
    [SerializeField] private Texture[] textures;
    [SerializeField] private float fps = 30f;

    private LineRenderer lineRenderer;
    private Transform target;
    private int animationStep = 0;
    private float fpsCounter;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    public void AssignTarget(Vector3 position, Transform newTarget)
    {
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, position);
        lineRenderer.SetPosition(1, newTarget.position);
        target = newTarget;
    }

    private void Update()
    {
        if (target != null)
        {
            lineRenderer.SetPosition(1, target.position);
        }

        fpsCounter += Time.deltaTime;
        if (fpsCounter >= 1f / fps)
        {
            animationStep++;
            if (animationStep == textures.Length)
            {
                animationStep = 0;
            }
            lineRenderer.material.SetTexture("_MainTex", textures[animationStep]);

            fpsCounter = 0f;
        }
    }
}
