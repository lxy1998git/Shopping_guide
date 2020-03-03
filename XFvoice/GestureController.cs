using System;
using System.Collections.Generic;
using Gestures.Segments.swipLeft;
using Gestures.Segments.swipRight;
using Microsoft.Kinect;

namespace Gestures
{
    class GestureController
    {
        /// <summary>
        /// 存放手势状态
        /// </summary>
        private List<Gesture> gestures = new List<Gesture>();

        public GestureController()
        {
            IRelativeGestureSegment[] swipleftSegments = new IRelativeGestureSegment[3];//相对手势段
            swipleftSegments[0] = new SwipLeftSegment1(); //向左滑动段
             swipleftSegments[1] = new SwipLeftSegment2();
            swipleftSegments[2] = new SwipLeftSegment3();

            AddGesture(GestureTypes.SwipeLeft, swipleftSegments);

            IRelativeGestureSegment[] swiprightSegments = new IRelativeGestureSegment[3];
            swiprightSegments[0] = new SwipRightSegment1();
            swiprightSegments[1] = new SwipRightSegment2();
            swiprightSegments[2] = new SwipRightSegment3();

            AddGesture(GestureTypes.SwipeRight, swiprightSegments);
        }
        /// <summary>
        /// 已识别手势，状态
        /// </summary>
        public event EventHandler<GestureEventArgs> GestureRecognized;

        public void UpdateAllGestures(Body body)
        {
            foreach (Gesture g in this.gestures)//
            {
                g.UpdateGesture(body);

            }
        }
        /// <summary>
        /// 添加一帧中的手势状态
        /// </summary>
        /// <param name="type">具体手势状态，左右挥手</param>
        /// <param name="gestureDefinition">识别是否成功</param>
        public void AddGesture(GestureTypes type, IRelativeGestureSegment[] gestureDefinition)
        {
            Gesture gesture = new Gesture(type, gestureDefinition);
            gesture.GestureDetected += gesture_GestureDetected;
            this.gestures.Add(gesture);
        }
        
        void gesture_GestureDetected(object sender, GestureEventArgs e)//
        {
            if (this.GestureRecognized != null)
            {
                this.GestureRecognized(this, e);
            }

            foreach (Gesture g in gestures)
            {
                g.Reset();
            }
        }
    }
}
