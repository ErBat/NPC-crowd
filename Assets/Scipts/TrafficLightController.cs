using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TrafficLightController : MonoBehaviour
{
    public enum LightState { Red, Green }
    public LightState currentLightState = LightState.Red;

    public float redLightDuration = 5f;
    public float greenLightDuration = 5f;

    public List<Renderer> targetRenderers; 

    private Color redColor = Color.red;
    private Color greenColor = Color.green;

    private void Start()
    {
        StartCoroutine(RunTrafficLights());
    }

    private IEnumerator RunTrafficLights()
    {
        while (true)
        {
            yield return ChangeLightState(LightState.Green, greenLightDuration);
            yield return ChangeLightState(LightState.Red, redLightDuration);
        }
    }

    private IEnumerator ChangeLightState(LightState newState, float duration)
    {
        SetLightState(newState);
        yield return new WaitForSeconds(duration);
    }

    private void SetLightState(LightState newState)
    {
        currentLightState = newState;

        Color newColor = (newState == LightState.Green) ? greenColor : redColor;

        UpdateRendererColors(newColor);
    }

    private void UpdateRendererColors(Color color)
    {
        foreach (Renderer renderer in targetRenderers)
        {
            if (renderer != null)
            {
                renderer.material.color = color; 
            }
        }
    }

    public bool IsGreenLight()
    {
        return currentLightState == LightState.Green;
    }
}
