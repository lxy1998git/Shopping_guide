using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Kinect;
using Gestures;


namespace Gestures.Segments.swipLeft
{
    /// <summary>
    /// Z轴：右手在右肘的前方
    /// Y轴：右手的高度介于肩部和臀部之间
    /// X轴：右手在右肩的右侧
    /// </summary>
    /// 右手
    ///右肩
    ///右髋关节
    ///右肘
    public class SwipLeftSegment1 : IRelativeGestureSegment
    {
        /// <summary>
        /// 手势是否满足挥手状态
        /// </summary>
        /// <param name="body"></param>
        /// <returns></returns>
        public GesturePartialResult CheckGesture(Body body)
        {
            Joint rightHand = body.Joints[JointType.HandRight];//右手
            Joint rightShoulder = body.Joints[JointType.ShoulderRight];//右肩
            Joint rightHip = body.Joints[JointType.HipRight];//右髋关节  右股
            Joint rightElbow = body.Joints[JointType.ElbowRight];//右肘

            //Z轴
            if (rightHand.Position.Z < rightElbow.Position.Z)    /// Z轴：右手在右肘的前方
            {//Y
                if (rightHand.Position.Y > rightElbow.Position.Y)    /// Y轴：右手的高度介于肩部和臀部之间
                { //X
                    if (rightHand.Position.X > rightShoulder.Position.X)    /// X轴：右手在右肩的右侧
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
    /// Z:右手位于右肩前
    /// Y：肩部与臀部之间
    /// X：两肩之间
    /// </summary>
    public class SwipLeftSegment2 : IRelativeGestureSegment
    {
        public GesturePartialResult CheckGesture(Body body)
        {
            Joint rightHand = body.Joints[JointType.HandRight];
            Joint rightShoulder = body.Joints[JointType.ShoulderRight];
            Joint rightHip = body.Joints[JointType.HipRight];
            Joint rightElbow = body.Joints[JointType.ElbowRight];
            Joint leftShoulder = body.Joints[JointType.ShoulderLeft];
            //Z轴
            if (rightHand.Position.Z < rightElbow.Position.Z) /// Z轴：右手在右肘的前方
            {
                //Y
                if (rightHand.Position.Y > rightElbow.Position.Y) /// Y轴：右手的高度介于肩部和臀部之间
                {
                    //X   // X轴：右手在右肩的左侧                                  // X轴：右手在左肩的右侧
                    if (rightHand.Position.X < rightShoulder.Position.X && rightHand.Position.X > leftShoulder.Position.X)//两肩之间
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
    /// Z:右手位于右肩前
    /// Y：肩部与臀部之间
    /// X：左肩右侧
    /// </summary>
    public class SwipLeftSegment3 : IRelativeGestureSegment
    {
        public GesturePartialResult CheckGesture(Body body)
        {
            Joint rightHand = body.Joints[JointType.HandRight];
            Joint rightShoulder = body.Joints[JointType.ShoulderRight];
            Joint rightHip = body.Joints[JointType.HipRight];
            Joint rightElbow = body.Joints[JointType.ElbowRight];
            Joint leftShoulder = body.Joints[JointType.ShoulderLeft];
            Joint midHip = body.Joints[JointType.SpineBase];
            //Z轴
            if (rightHand.Position.Z < rightElbow.Position.Z)// Z轴：右手在右肘的前方
            {
                //Y
                if (rightHand.Position.Y > rightElbow.Position.Y)// Y轴：右手的高度介于肩部和臀部之间
                {
                    //X
                    if (rightHand.Position.X > leftShoulder.Position.X)// X轴：右手在左肩的右侧
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
