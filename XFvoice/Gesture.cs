using System;
using Microsoft.Kinect;

namespace Gestures
{
    class Gesture
    {
        private IRelativeGestureSegment[] gestureParts;//结果，有没有3
        private int currentGesturePart = 0;//当前
        private int pausedFrameCount = 10;//暂停的帧计数
        private int frameCount = 0;//帧数
        private bool paused = false;//暂停
        private GestureTypes type;

        public Gesture(GestureTypes type, IRelativeGestureSegment[] gestureParts)
        {
            this.gestureParts = gestureParts;
            this.type = type;
        }
        /// <summary>
        /// 检测到的手势
        /// </summary>
        public event EventHandler<GestureEventArgs> GestureDetected;
        /// <summary>
        /// 更新手势
        /// </summary>
        /// <param name="body"></param>
        public void UpdateGesture(Body body)
        {
            if (this.paused)
            {
                if (this.frameCount == this.pausedFrameCount)//10
                {
                    this.paused = false;
                }
                this.frameCount++;
            }

            GesturePartialResult result = this.gestureParts[this.currentGesturePart].CheckGesture(body);
            if (result == GesturePartialResult.Suceed)
            {
                if (currentGesturePart + 1 < gestureParts.Length)//结果数3 且为最后轨迹点

                {
                    currentGesturePart++;
                    this.frameCount = 0;
                    this.pausedFrameCount = 10;
                    this.paused = true;
                }
                else
                {
                    if (this.GestureDetected != null)
                    {
                        GestureDetected(this, new GestureEventArgs(this.type, body.TrackingId));
                        Reset();
                    }
                }
            }
            else if (result == GesturePartialResult.Fail || this.frameCount == 50)//不符合手势
            {
                this.currentGesturePart = 0;
                this.frameCount = 0;
                this.pausedFrameCount = 5;
                this.paused = true;
            }
            else//手势模糊
            {
                this.frameCount++;
                this.pausedFrameCount = 5;
                this.paused = true;
            }
        }
        /// <summary>
        /// 重置
        /// </summary>
        public void Reset()
        {
            currentGesturePart = 0;
            frameCount = 0;
            pausedFrameCount = 5;
            paused = true;
        }
    }
}
