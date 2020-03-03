using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Kinect;
/// <summary>
/// //左手右挥
/// </summary>
namespace Gestures.Segments.swipRight
{
    /// <summary>
    /// Z轴：左手在左肘的前方
    /// Y轴：左手的高度介于肩部和臀部之间
    /// X轴：左手在左肩的左侧
    /// </summary>
    public class SwipRightSegment1 : IRelativeGestureSegment
    {
        public GesturePartialResult CheckGesture(Body body)
        {
            Joint leftHand = body.Joints[JointType.HandLeft];
            Joint leftShoulder = body.Joints[JointType.ShoulderLeft];
            Joint leftHip = body.Joints[JointType.HipLeft];
            Joint leftElbow = body.Joints[JointType.ElbowLeft];

            //Z轴
            if (leftHand.Position.Z < leftElbow.Position.Z)
            {
                //Y
                if (leftHand.Position.Y > leftElbow.Position.Y)
                {
                    //X
                    if (leftHand.Position.X < leftShoulder.Position.X)
                    {
                        return GesturePartialResult.Suceed;
                    }
                    return GesturePartialResult.Undetermined;
                }
                return GesturePartialResult.Fail;
            }
            return GesturePartialResult.Fail;

        }
    }

    /// <summary>
    /// Z:左手位于左肩前
    /// Y：肩部与臀部之间
    /// X：两肩之间
    /// </summary>
    public class SwipRightSegment2 : IRelativeGestureSegment
    {
        public GesturePartialResult CheckGesture(Body body)
        {
            Joint leftHand = body.Joints[JointType.HandLeft];
            Joint leftShoulder = body.Joints[JointType.ShoulderLeft];
            Joint leftHip = body.Joints[JointType.HipLeft];
            Joint leftElbow = body.Joints[JointType.ElbowLeft];
            Joint rightShoulder = body.Joints[JointType.ShoulderRight];
            //Z轴
            if (leftHand.Position.Z < leftElbow.Position.Z)
            {
                //Y
                if (leftHand.Position.Y > leftElbow.Position.Y)
                {
                    //X
                    if (leftHand.Position.X > leftShoulder.Position.X && leftHand.Position.X < rightShoulder.Position.X)
                    {
                        return GesturePartialResult.Suceed;
                    }
                    return GesturePartialResult.Undetermined;
                }
                return GesturePartialResult.Fail;
            }
            return GesturePartialResult.Fail;
        }
    }

    /// <summary>
    /// Z:左手位于左肩前
    /// Y：肩部与臀部之间
    /// X：两肩之间
    /// </summary>
    public class SwipRightSegment3 : IRelativeGestureSegment
    {
        public GesturePartialResult CheckGesture(Body body)
        {
            Joint leftHand = body.Joints[JointType.HandLeft];
            Joint leftShoulder = body.Joints[JointType.ShoulderLeft];
            Joint leftHip = body.Joints[JointType.HipLeft];
            Joint leftElbow = body.Joints[JointType.ElbowLeft];
            Joint rightShoulder = body.Joints[JointType.ShoulderRight];
            Joint midHip = body.Joints[JointType.SpineBase];
            //Z轴
            if (leftHand.Position.Z < leftElbow.Position.Z)
            {
                //Y
                if (leftHand.Position.Y > leftElbow.Position.Y)
                {
                    //X
                    if (leftHand.Position.X < rightShoulder.Position.X)
                    {
                        return GesturePartialResult.Suceed;
                    }
                    return GesturePartialResult.Undetermined;
                }
                return GesturePartialResult.Fail;
            }
            return GesturePartialResult.Fail;
        }
    }
}
