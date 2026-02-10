using System;
using System.Globalization;
using UnityEngine;
using UnityEngine.Events;
public class DayCycle : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [Header("Day Cycle Settings")]
    [Tooltip("Duration of a full day in seconds")]
    [SerializeField] private float dayTime;
    private float sunRiseAngle = 10f;
    private float sunSetAngle = 170f;
    private bool isDayEnded = false;
    private float rotationSpeed;
    private float currRotation;

    public event Action OnDayEnded;
    public Light directionLight;
    public Gradient lightColor;
    public AnimationCurve intensity;
    void Start()
    {
         rotationSpeed = (sunSetAngle-sunRiseAngle)/dayTime;
        directionLight = GetComponent<Light>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isDayEnded) return;
        currRotation += Time.deltaTime * rotationSpeed;
        transform.Rotate(Vector3.right * rotationSpeed);
        directionLight.transform.localRotation = Quaternion.Euler(currRotation, -30, 0);
        float dayProgress = (currRotation-sunRiseAngle)/(sunSetAngle-sunRiseAngle);
        directionLight.color = lightColor.Evaluate(dayProgress);
        directionLight.intensity = intensity.Evaluate(dayProgress);
        if(currRotation >= sunSetAngle)
        {
            EndDay();
        }
    }

    private void EndDay()
    {
        isDayEnded = true;
        currRotation = sunSetAngle;
        OnDayEnded?.Invoke();
    }

    public void StartNewDay()
    {
        isDayEnded = false;
        currRotation = sunRiseAngle;
        directionLight.transform.localRotation = Quaternion.Euler(currRotation, -30f, 0f);
    }

    public bool GetDayStatus()
    {
        return isDayEnded;
    }
    public float GetDayProgress()
    {
        return (currRotation-sunRiseAngle)/(sunSetAngle-sunRiseAngle);
    }
}
