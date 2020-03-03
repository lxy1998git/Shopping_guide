namespace Gestures
{
    using Microsoft.Kinect;
    public interface IRelativeGestureSegment
    {
        GesturePartialResult CheckGesture(Body body);
    }
}
