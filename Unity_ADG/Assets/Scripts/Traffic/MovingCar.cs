public class MovingCar : MovingObject
{
    CarDetector carDetector;

    TrafficSystem trafficSystem;

    protected override void VirtualOnEnable()
    {
        trafficSystem = FindObjectOfType<TrafficSystem>();
        carDetector = GetComponentInChildren<CarDetector>();
        if (carDetector)
        {
            carDetector.onDetecting += DetectingCar;
        }
    }
    protected override void VirtualStart()
    {
        if (connectPath)
        {
            trafficSystem?.InjectVehicle(this);
        }
    }
    protected override void VirtualOnDestroy()
    {
        trafficSystem?.EjectVehicle(this);
    }

    void DetectingCar(bool state)
    {
        if (state)
        {
            movingSpeed = 0;
        }
        else
        {
            movingSpeed = c_MoveSpeed;
        }
    }
}
