using System;

namespace Gestures
{
    public class GestureEventArgs : EventArgs
    {
        /// <summary>
        /// 初始化的新实例 <see cref="GestureEventArgs"/> class.
        /// </summary>
        /// <param name="type">手势类型.</param>
        /// <param name="trackingID">跟踪标识.</param>
        /// <param name="userID">用户ID<param>
        public GestureEventArgs(GestureTypes type, ulong trackingId)
        {
            this.TrackingId = trackingId;
            this.GestureType = type;
        }

        /// <summary>
        /// 获取或设置手势的类型。
        /// </summary>
        /// <value>
        /// 手势的类型。
        /// </value>
        public GestureTypes GestureType
        {
            get;
            set;
        }

        /// <summary>
        ///获取或设置跟踪ID。
        /// </summary>
        /// <value>
        /// 跟踪ID。
        /// </value>
        public ulong TrackingId
        {
            get;
            set;
        }
    }
}