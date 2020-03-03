namespace Gestures
{
    /// <summary>
    /// 手势结果
    /// </summary>
    public enum GesturePartialResult
    {
        Fail,
        Suceed,
        Undetermined
    }
    /// <summary>
    /// 手势种类
    /// </summary>
    public enum GestureTypes
    {
        WaveRight,//右挥
        WaveLeft,

        RightFist,
        LeftfFist,//左挥拳


        None,
        JointdHands,
        Menu,
        SwipeUp,
        SwipeDown,
        SwipeLeft,
        SwipeRight,
        ZoomIn,
        ZoomOut
                 
       // JointdHands,  双手合十
        //WaveRight,  右挥
        //WaveLeft,  左挥
        //SwipeUp,
        //SwipeDown,
        //SwipeLeft,
        //SwipeRight,
        //ZoomIn,
        //ZoomOut
    }
}