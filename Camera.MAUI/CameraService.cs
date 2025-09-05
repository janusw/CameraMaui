using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Runtime.CompilerServices;

using Debug = System.Diagnostics.Debug;
namespace Camera.MAUI;

public class CameraService : Internals.ObservableBase
{
    public string RawOrientation
    {
        get => _rawOrientation;
        set => SetProperty(ref _rawOrientation, value);
    }
    private string _rawOrientation;

    public System.Numerics.Vector3 Acceleration
    {
        get => _acceleration;
        set => SetProperty(ref _acceleration, value);
    }
    private System.Numerics.Vector3 _acceleration;

    public float DeviceOrientationAngle
    {
        get => _deviceOrientationAngle;
        protected set => SetProperty(ref _deviceOrientationAngle, value);
    }
    private float _deviceOrientationAngle;

    private float ComputeDeviceOrientation()
    {
        float y = Acceleration.Y;
        float x = Acceleration.X;

        //**todo: Android-specific here, so we should... do what?
        // Correct 0° reference: add 90° so 0° matches the phone's native orientation
        double angle = Math.Atan2(-x, y) * (180.0 / Math.PI) + 90.0;
        if (angle < 0) angle += 360;
        if (angle > 360) angle -= 360;

        return (float)angle;
    }

    public CameraService() {}

    public void StopAccelerometer()
    {
        if (Accelerometer.IsSupported)
        {
            Accelerometer.Stop();
            Accelerometer.ReadingChanged -= OnAccelerometerReadingChanged;
        }
    }

    public void StartAccelerometer()
    {
        if (!Accelerometer.IsSupported) return;
        if (Accelerometer.IsMonitoring) return; // already started

        Accelerometer.ReadingChanged += OnAccelerometerReadingChanged;
        Accelerometer.Start(SensorSpeed.UI);
    }

    private uint sample = 0;

    private void OnAccelerometerReadingChanged(object sender, AccelerometerChangedEventArgs e)
    {
        // this sample rate produces updates ~2Hz
        if (sample == 10)
        {
            sample = 0;
            return;
        }

        if (sample == 0)
        {
            Acceleration = e.Reading.Acceleration;
            RawOrientation = e.Reading.Acceleration.ToString();
            Debug.WriteLine($"Device accel: {RawOrientation}");
            //calc the new orientation here so that app-code can subscribe to it only when it changes/databind
            DeviceOrientationAngle = ComputeDeviceOrientation();
        }

        ++sample;
    }

}