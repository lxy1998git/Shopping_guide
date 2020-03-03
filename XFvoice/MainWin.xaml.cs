using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Kinect;
using Gestures;
using System.Runtime.InteropServices;
using System.Windows.Threading;
using System.IO.Ports;
using System.Threading;

namespace XFvoice
{
    public partial class MainWindow : Window
    {
        #region 变量
        private DrawingGroup drawingGroup;
        /// <summary>
        /// 彩色帧读取变量  
        /// </summary>
        private KinectSensor kinectDevice = null;
        /// <summary>
        /// 彩色帧描述
        /// </summary>                                      
        private ColorFrameReader colorFrameReader = null;

        /// <summary>
        /// 位图对象 
        /// </summary>
        private FrameDescription colorFrameDescription = null;
        /// <summary>
        /// 存放一帧彩色图像的矩形框
        /// </summary>
        private WriteableBitmap colorBitmap = null;//
        /// <summary>
        /// /步长，一维彩色像素数组转化为矩形框的步长。  
        /// </summary>
        private Int32Rect colorBitmapRect;
        /// <summary>
        /// 存放一帧彩色图像像素  
        /// </summary>
        private int colorBitmapStride;//

        private Byte[] colorPixelData;

        private List<Tuple<JointType, JointType>> bones;
        /// <summary>
        /// 相机空间的夹紧z值为负值。
        /// </summary>
        private const float InferredZPositionClamp = 0.1f;//
        private List<Pen> bodyColors;
        private int displayWidth;
        private int displayHeight;
        int bn = 1;
        /// <summary>
        /// 物体抓取范围
        /// </summary>
        int _LocationRange = 50;
        Point MousePoint = new Point(0, 0);
        Point pPoint = new Point(0, 0);
        private KinectSensor kinectsensor;
        private byte[] colorframedata;
        private WriteableBitmap colormap;
        private MultiSourceFrameReader msfreader;
        private FrameDescription cfd;
        private Int32Rect crect;
        private int stride;
        private Body[] bodies;
        private GestureController gesturecontroller;
        private CoordinateMapper coordinateMapper = null;
        private Point endPosition;
        private BodyFrameReader bodyFrameReader = null;

        bool ifopenclose = false;
        [Flags]
        enum MouseEventFlag : uint //设置鼠标动作的键值
        {
            Move = 0x0001,               //发生移动
            LeftDown = 0x0002,           //鼠标按下左键
            LeftUp = 0x0004,             //鼠标松开左键
            RightDown = 0x0008,          //鼠标按下右键
            RightUp = 0x0010,            //鼠标松开右键
            MiddleDown = 0x0020,         //鼠标按下中键
            MiddleUp = 0x0040,           //鼠标松开中键
            XDown = 0x0080,
            XUp = 0x0100,
            Wheel = 0x0800,              //鼠标轮被移动
            VirtualDesk = 0x4000,        //虚拟桌面
            Absolute = 0x8000
        }
        /// <summary>
        /// 设置鼠标位置
        /// </summary>
        /// <param name="X"></param>
        /// <param name="Y"></param>
        /// <returns></returns>
        [DllImport("user32.dll")]
        static extern bool SetCursorPos(int X, int Y);
        /// <summary>
        /// 鼠标事件
        /// </summary>
        /// <param name="flags"></param>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        /// <param name="data"></param>
        /// <param name="extraInfo"></param>
        [DllImport("user32.dll")]
        static extern void mouse_event(MouseEventFlag flags, int dx, int dy, int data, UIntPtr extraInfo);
        #endregion
        public MainWindow()
        {
            InitializeComponent();
            danpianjijiazai();//单片机
            this.kinectsensor = KinectSensor.GetDefault();
            //获取默认的连接的体感器  
            this.kinectDevice = KinectSensor.GetDefault();
            //彩色帧读取变量初始化  
            this.colorFrameReader = this.kinectDevice.ColorFrameSource.OpenReader();
            //彩色帧描述，宽度和高度，注意括号内的入参，代表这一帧彩色图像像素的格式  
            this.colorFrameDescription = this.kinectDevice.ColorFrameSource.CreateFrameDescription(ColorImageFormat.Bgra);
            //触发彩色帧事件  
            this.colorFrameReader.FrameArrived += _ColorFrameReader_FrameArrived;
            //存放彩色图像的字节数组的长度=帧宽度*帧高度*每个像素占用的字节数4  
            this.colorPixelData = new Byte[this.colorFrameDescription.LengthInPixels * 4];
            //位图初始化，宽度，高度，96.0表示分辨率，像素格式  
            this.colorBitmap = new WriteableBitmap(this.colorFrameDescription.Width,
                                                this.colorFrameDescription.Height, 96.0, 96.0, PixelFormats.Bgra32, null);
            //存放图像像素的矩形框，起点为（0，0），宽度和高度  
            this.colorBitmapRect = new Int32Rect(0, 0, this.colorBitmap.PixelWidth, this.colorBitmap.PixelHeight);
            //一个像素占用4个自己，分别是B、G、R、A的格式  
            this.colorBitmapStride = this.colorFrameDescription.Width * 4;
            //获取坐标映射器
            this.coordinateMapper = this.kinectDevice.CoordinateMapper;
            // 获取深度（显示）范围
            FrameDescription frameDescription = this.kinectDevice.DepthFrameSource.FrameDescription;
            // 获取关节空间的大小
            this.displayWidth = frameDescription.Width;
            this.displayHeight = frameDescription.Height;
            // 打开阅读器的身体框架
            this.bodyFrameReader = this.kinectDevice.BodyFrameSource.OpenReader();
            // 两关节骨之间的线  
            this.bones = new List<Tuple<JointType, JointType>>();
            #region 25关节
            // 人体躯干
            this.bones.Add(new Tuple<JointType, JointType>(JointType.Head, JointType.Neck));
            this.bones.Add(new Tuple<JointType, JointType>(JointType.Neck, JointType.SpineShoulder));
            this.bones.Add(new Tuple<JointType, JointType>(JointType.SpineShoulder, JointType.SpineMid));
            this.bones.Add(new Tuple<JointType, JointType>(JointType.SpineMid, JointType.SpineBase));
            this.bones.Add(new Tuple<JointType, JointType>(JointType.SpineShoulder, JointType.ShoulderRight));
            this.bones.Add(new Tuple<JointType, JointType>(JointType.SpineShoulder, JointType.ShoulderLeft));
            this.bones.Add(new Tuple<JointType, JointType>(JointType.SpineBase, JointType.HipRight));
            this.bones.Add(new Tuple<JointType, JointType>(JointType.SpineBase, JointType.HipLeft));

            // 右臂
            this.bones.Add(new Tuple<JointType, JointType>(JointType.ShoulderRight, JointType.ElbowRight));
            this.bones.Add(new Tuple<JointType, JointType>(JointType.ElbowRight, JointType.WristRight));
            this.bones.Add(new Tuple<JointType, JointType>(JointType.WristRight, JointType.HandRight));
            this.bones.Add(new Tuple<JointType, JointType>(JointType.HandRight, JointType.HandTipRight));
            this.bones.Add(new Tuple<JointType, JointType>(JointType.WristRight, JointType.ThumbRight));

            // 左臂 
            this.bones.Add(new Tuple<JointType, JointType>(JointType.ShoulderLeft, JointType.ElbowLeft));
            this.bones.Add(new Tuple<JointType, JointType>(JointType.ElbowLeft, JointType.WristLeft));
            this.bones.Add(new Tuple<JointType, JointType>(JointType.WristLeft, JointType.HandLeft));
            this.bones.Add(new Tuple<JointType, JointType>(JointType.HandLeft, JointType.HandTipLeft));
            this.bones.Add(new Tuple<JointType, JointType>(JointType.WristLeft, JointType.ThumbLeft));

            //右腿
            this.bones.Add(new Tuple<JointType, JointType>(JointType.HipRight, JointType.KneeRight));
            this.bones.Add(new Tuple<JointType, JointType>(JointType.KneeRight, JointType.AnkleRight));
            this.bones.Add(new Tuple<JointType, JointType>(JointType.AnkleRight, JointType.FootRight));

            // 左腿
            this.bones.Add(new Tuple<JointType, JointType>(JointType.HipLeft, JointType.KneeLeft));
            this.bones.Add(new Tuple<JointType, JointType>(JointType.KneeLeft, JointType.AnkleLeft));
            this.bones.Add(new Tuple<JointType, JointType>(JointType.AnkleLeft, JointType.FootLeft));
            #endregion
            this.bodyColors = new List<Pen>();
            this.bodyColors.Add(new Pen(Brushes.Red, 6));
            this.bodyColors.Add(new Pen(Brushes.Orange, 6));
            this.bodyColors.Add(new Pen(Brushes.Green, 6));
            this.bodyColors.Add(new Pen(Brushes.Blue, 6));
            this.bodyColors.Add(new Pen(Brushes.Indigo, 6));
            this.bodyColors.Add(new Pen(Brushes.Violet, 6));
            this.kinectDevice.Open();
            this.DataContext = this;
            this.drawingGroup = new DrawingGroup();
            ColorViewer.Source = new DrawingImage(this.drawingGroup);//骨骼
            this.kinectDevice.Open();
            this.coordinateMapper = this.kinectsensor.CoordinateMapper;
            this.Loaded += MainWindow_Loaded;
            this.Closed += MainWindow_Closed;
        }
        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            startKinect();
            this.bodyFrameReader = this.kinectDevice.BodyFrameSource.OpenReader();
            if (this.bodyFrameReader != null)
            {
                this.bodyFrameReader.FrameArrived += this.Reader_FrameArrived;
            }
        }
        private void _ColorFrameReader_FrameArrived(object sender, ColorFrameArrivedEventArgs e)//彩色
        {
            bool goodsReceived = false;
            //获取一帧彩色图像  
            using (ColorFrame colorFrame = e.FrameReference.AcquireFrame())
            {
                //如果获取成功  
                if (colorFrame != null)
                {
                    //将彩色帧数据存放在字节数组里面，注意格式  
                    colorFrame.CopyConvertedFrameDataToArray(this.colorPixelData, ColorImageFormat.Bgra);      //把一帧彩色图像数据拷贝到字节数组中  
                    //将像素写入位图，需要什么我们就补充什么  
                    this.colorBitmap.WritePixels(this.colorBitmapRect, this.colorPixelData, this.colorBitmapStride, 0);
                    goodsReceived = true;
                }
            }
        }
        private void startKinect()
        {
            kinectsensor = KinectSensor.GetDefault();
            msfreader = kinectsensor.OpenMultiSourceFrameReader(FrameSourceTypes.Color | FrameSourceTypes.Body);
            cfd = kinectsensor.ColorFrameSource.CreateFrameDescription(ColorImageFormat.Bgra);

            colorframedata = new byte[cfd.LengthInPixels * cfd.BytesPerPixel];
            colormap = new WriteableBitmap(cfd.Width, cfd.Height, 96, 96, PixelFormats.Bgra32, null);
            crect = new Int32Rect(0, 0, cfd.Width, cfd.Height);
            stride = (int)(cfd.Width * cfd.BytesPerPixel);

            msfreader.MultiSourceFrameArrived += msfreader_MultiSourceFrameArrived;
            try
            {
                kinectsensor.Open();
            }
            catch (System.IO.IOException)
            {
            }
            gesturecontroller = new GestureController();
            gesturecontroller.GestureRecognized += gesturecontroller_GestureRecognized;
        }
        /// <summary>
        /// 多开源，获取彩色图和骨骼数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void msfreader_MultiSourceFrameArrived(object sender, MultiSourceFrameArrivedEventArgs e)
        {
            MultiSourceFrame msframe = e.FrameReference.AcquireFrame();
            if (msframe != null)
            {
                using (ColorFrame cframe = msframe.ColorFrameReference.AcquireFrame())
                {
                    if (cframe != null)
                    {
                        cframe.CopyConvertedFrameDataToArray(colorframedata, ColorImageFormat.Bgra);
                        colormap.WritePixels(crect, colorframedata, stride, 0);
                    }

                    using (BodyFrame bframe = msframe.BodyFrameReference.AcquireFrame())
                    {
                        if (bframe != null)
                        {
                            bodies = new Body[bframe.BodyCount];
                            bframe.GetAndRefreshBodyData(bodies);

                            Body closestBody = (from s in bodies where s.IsTracked && s.Joints[JointType.SpineMid].TrackingState == TrackingState.Tracked select s)
                                .OrderBy(s => s.Joints[JointType.SpineMid].Position.Z).FirstOrDefault();


                            if (closestBody != null && closestBody.IsTracked)
                            {
                                gesturecontroller.UpdateAllGestures(closestBody);
                            }
                            else
                            {
                            }
                            foreach (Body body1 in this.bodies)
                            {
                                if (body1.IsTracked)
                                {
                                    IReadOnlyDictionary<JointType, Joint> joints = body1.Joints;
                                    // 将关节点转换为深度（显示）空间
                                    Dictionary<JointType, Point> jointPoints = new Dictionary<JointType, Point>();

                                    foreach (JointType jointType in joints.Keys)
                                    {
                                        // 有时推断的关节的深度（z）可以表示为负值。
                                        //钳位到0.1F，以防止协调器映射器返回（无穷大，无穷大）
                                        CameraSpacePoint position = joints[jointType].Position;
                                        if (position.Z < 0)
                                        {
                                            position.Z = 0.1f;
                                        }
                                        DepthSpacePoint depthSpacePoint = this.coordinateMapper.MapCameraPointToDepthSpace(position);
                                        jointPoints[jointType] = new Point(depthSpacePoint.X, depthSpacePoint.Y);
                                    }
                                    int x = Convert.ToInt32(jointPoints[JointType.HandRight].X);//坐标3
                                    int y = Convert.ToInt32(jointPoints[JointType.HandRight].Y);//2
                                    DrawRHand(body1.HandRightState, jointPoints[JointType.HandRight]);
                                    DrawRHand(body1.HandLeftState, jointPoints[JointType.HandLeft]);
                                }
                            }
                        }
                    }
                }
            }
        }
        /// <summary>
        /// 判断手手势
        /// </summary>
        /// <param name="handState"></param>
        /// <param name="handPosition"></param>
        private void DrawRHand(HandState handState, Point handPosition)
        {
            //dnsv();
            int x = 0;
            int y = 0;
            try
            {
                switch (handState)
                {
                    case HandState.Closed:
                        x = (int)handPosition.X * (int)ColorViewer.Source.Width / (int)ColorViewer.Width;
                        y = (int)handPosition.Y * (int)ColorViewer.Source.Height / (int)ColorViewer.Height;
                        zuobiao.AppendText("----------x=" + x.ToString() + "y=" + y.ToString());
                        if (Math.Abs(x - MousePoint.X) < _LocationRange && Math.Abs(y - MousePoint.Y) < _LocationRange)
                        {
                            textshou.Text = "顾客拿到指定货物";
                            if (ifopenclose)
                            {
                                SendMessage("导购——人机交流", goodsname.Text + "111");
                            }
                        }
                        else
                        {
                            textshou.Text = "没拿到货物 ";
                        }
                        ifopenclose = false;
                        //mouse_event(MouseEventFlag.LeftUp, 0, 0, 1, UIntPtr.Zero);//单击   2双击按下左键
                        break;
                    case HandState.Open:
                        //  textshou.Text = "张开手掌";
                        ifopenclose = true;
                        // mouse_event(MouseEventFlag.LeftUp, 0, 0, 0, UIntPtr.Zero);//放开左键
                        break;
                }
            }
            catch (Exception ex)
            {
                textshou.Text = "没拿到货物";
                errorTb.Text = ex.Message;
            }

        }
        /// <summary>
        /// 判断挥手手势
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void gesturecontroller_GestureRecognized(object sender, GestureEventArgs e)
        {
            try
            {
                if (e.GestureType == GestureTypes.SwipeLeft)
                {
                    //   System.Windows.Forms.SendKeys.SendWait("{Left}");
                    ui.Text = "右手左滑";
                    SendMessage("导购——人机交流", "011");

                    // mouse_event(MouseEventFlag.Wheel, 0, 0, 120, UIntPtr.Zero);//上滚
                }
                if (e.GestureType == GestureTypes.SwipeRight)
                {
                    //System.Windows.Forms.SendKeys.SendWait("{Right}");

                    ui.Text = "左手右划";
                    SendMessage("导购——人机交流", "010");
                    //mouse_event(MouseEventFlag.Wheel, 0, 0, -120, UIntPtr.Zero);//下滚
                }
                #region 后期
                else if (e.GestureType == GestureTypes.JointdHands)
                {
                    //  System.Windows.Forms.SendKeys.SendWait("{Right}");
                    textshou.Text = "Jointd Hands";
                }
                else if (e.GestureType == GestureTypes.WaveLeft)
                {
                    //System.Windows.Forms.SendKeys.SendWait("{Right}");
                    textshou.Text = "WaveLef";
                }
                else if (e.GestureType == GestureTypes.WaveRight)
                {
                    // System.Windows.Forms.SendKeys.SendWait("{Right}");
                    textshou.Text = "WaveRight";
                }
                else if (e.GestureType == GestureTypes.JointdHands)
                {
                    // System.Windows.Forms.SendKeys.SendWait("{Right}");
                    textshou.Text = "Jointd Hands";
                }
                else if (e.GestureType == GestureTypes.ZoomIn)
                {
                    //  System.Windows.Forms.SendKeys.SendWait("{Right}");
                    textshou.Text = "ZoomIn";
                }
                else if (e.GestureType == GestureTypes.ZoomOut)
                {
                    // System.Windows.Forms.SendKeys.SendWait("{Right}");
                    textshou.Text = "Zoomout";
                }
                #endregion
            }
            catch (Exception ex)
            {
                errorTb.Text = ex.Message;
            }

        }
        #region 画骨骼,显示彩色图
        DrawingContext dc;
        private void Reader_FrameArrived(object sender, BodyFrameArrivedEventArgs e)
        {
            try
            {
                bool dataReceived = false;

                using (BodyFrame bodyFrame = e.FrameReference.AcquireFrame())
                {
                    if (bodyFrame != null)
                    {
                        if (this.bodies == null)
                        {
                            this.bodies = new Body[bodyFrame.BodyCount];
                        }
                        bodyFrame.GetAndRefreshBodyData(this.bodies);
                        dataReceived = true;
                    }
                }
                if (dataReceived)
                {
                    using (dc = this.drawingGroup.Open())
                    {
                        //绘制透明背景以设置渲染大小。
                        // dc.DrawRectangle(Brushes.Coral, null, new Rect(0.0, 0.0, this.displayWidth, this.displayHeight));
                        dc.DrawImage(colorBitmap, new Rect(0.0, 0.0, this.displayWidth, this.displayHeight));

                        drawGoods(pPoint, dc);

                        int penIndex = 0;
                        foreach (Body body in this.bodies)
                        {
                            Pen drawPen = this.bodyColors[penIndex++];

                            if (body.IsTracked)
                            {
                                this.DrawClippedEdges(body, dc);

                                IReadOnlyDictionary<JointType, Joint> joints = body.Joints;
                                Dictionary<JointType, Point> jointPoints = new Dictionary<JointType, Point>();

                                foreach (JointType jointType in joints.Keys)
                                {
                                    CameraSpacePoint position = joints[jointType].Position;
                                    if (position.Z < 0)
                                    {
                                        position.Z = InferredZPositionClamp;
                                    }

                                    DepthSpacePoint depthSpacePoint = this.coordinateMapper.MapCameraPointToDepthSpace(position);
                                    jointPoints[jointType] = new Point(depthSpacePoint.X, depthSpacePoint.Y);
                                }
                                this.DrawBody(joints, jointPoints, dc, drawPen);
                                //this.DrawLHand(body.HandLeftState, jointPoints[JointType.HandLeft], dc);//左手圈
                                //this.DrawRHand(body.HandRightState, jointPoints[JointType.HandRight], dc);//右手圈
                            }
                        }
                        // prevent drawing outside of our render area
                        this.drawingGroup.ClipGeometry = new RectangleGeometry(new Rect(0.0, 0.0, this.displayWidth, this.displayHeight));
                    }
                }
                else
                {
                }
            }
            catch (Exception)
            {
            }
        }
        private const double ClipBoundsThickness = 10;//夹边矩形厚度
        private void DrawClippedEdges(Body body, DrawingContext drawingContext)
        {
            FrameEdges clippedEdges = body.ClippedEdges;

            if (clippedEdges.HasFlag(FrameEdges.Bottom))
            {
                drawingContext.DrawRectangle(
                    Brushes.Red,
                    null,
                    new Rect(0, this.displayHeight - ClipBoundsThickness, this.displayWidth, ClipBoundsThickness));
            }

            if (clippedEdges.HasFlag(FrameEdges.Top))
            {
                drawingContext.DrawRectangle(
                    Brushes.Red,
                    null,
                    new Rect(0, 0, this.displayWidth, ClipBoundsThickness));
            }

            if (clippedEdges.HasFlag(FrameEdges.Left))
            {
                drawingContext.DrawRectangle(
                    Brushes.Red,
                    null,
                    new Rect(0, 0, ClipBoundsThickness, this.displayHeight));
            }

            if (clippedEdges.HasFlag(FrameEdges.Right))
            {
                drawingContext.DrawRectangle(
                    Brushes.Red,
                    null,
                    new Rect(this.displayWidth - ClipBoundsThickness, 0, ClipBoundsThickness, this.displayHeight));
            }
        }
        private void DrawBody(IReadOnlyDictionary<JointType, Joint> joints, IDictionary<JointType, Point> jointPoints, DrawingContext drawingContext, Pen drawingPen)
        {
            // Draw the bones
            foreach (var bone in this.bones)
            {
                this.DrawBone(joints, jointPoints, bone.Item1, bone.Item2, drawingContext, drawingPen);
            }

            // Draw the joints
            foreach (JointType jointType in joints.Keys)
            {
                Brush drawBrush = null;

                TrackingState trackingState = joints[jointType].TrackingState;

                if (trackingState == TrackingState.Tracked)
                {
                    drawBrush = this.trackedJointBrush;
                }
                else if (trackingState == TrackingState.Inferred)
                {
                    drawBrush = this.inferredJointBrush;
                }

                if (drawBrush != null)
                {
                    drawingContext.DrawEllipse(drawBrush, null, jointPoints[jointType], JointThickness, JointThickness);
                }
            }
        }
        private const double JointThickness = 3;//拉伸关节线的厚度
        private readonly Brush inferredJointBrush = Brushes.Yellow;
        /// 画一根骨头（关节到关节）      
        private readonly Brush trackedJointBrush = new SolidColorBrush(Color.FromArgb(255, 68, 192, 68)); //用于绘制当前跟踪的接头的刷子。   
        private readonly Pen inferredBonePen = new Pen(Brushes.Gray, 1);
        private void DrawBone(IReadOnlyDictionary<JointType, Joint> joints, IDictionary<JointType, Point> jointPoints, JointType jointType0, JointType jointType1, DrawingContext drawingContext, Pen drawingPen)
        {
            Joint joint0 = joints[jointType0];
            Joint joint1 = joints[jointType1];

            // If we can't find either of these joints, exit
            if (joint0.TrackingState == TrackingState.NotTracked ||
                joint1.TrackingState == TrackingState.NotTracked)
            {
                return;
            }
            // We assume all drawn bones are inferred unless BOTH joints are tracked
            Pen drawPen = this.inferredBonePen;
            if ((joint0.TrackingState == TrackingState.Tracked) && (joint1.TrackingState == TrackingState.Tracked))
            {
                drawPen = drawingPen;
            }

            drawingContext.DrawLine(drawPen, jointPoints[jointType0], jointPoints[jointType1]);
        }
        #endregion
        void MainWindow_Closed(object sender, EventArgs e)
        {
            if (msfreader != null)
            {
                msfreader.Dispose();
                msfreader = null;
            }
            if (kinectsensor != null)
            {
                if (kinectsensor.IsOpen)
                {
                    kinectsensor.Close();
                }
                kinectsensor = null;
            }
        }
        /// <summary>
        /// 鼠标点击坐标
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ColorViewer_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                MousePoint = e.GetPosition(ColorViewer);//1000.526
                pPoint.X = (int)MousePoint.X * ColorViewer.Source.Width / ColorViewer.Width;
                pPoint.Y = (int)MousePoint.Y * ColorViewer.Source.Height / ColorViewer.Height;
                blueE.Visibility = Visibility.Visible;
                goodsname.Visibility = Visibility.Visible;

            }
            catch (Exception ex)
            {
                errorTb.Text = ex.Message;
            }
        }
        /// <summary>
        /// 所点击坐标画物体
        /// </summary>
        /// <param name="p">坐标</param>
        /// <param name="drawingContext">画布</param>
        void drawGoods(Point p, DrawingContext drawingContext)
        {
            drawingContext.DrawEllipse(
              Brushes.Blue,
              null,
             p, 10, 10);
        }
        #region 间程通信
        public const int WM_COPYDATA = 0x004A;

        [DllImport("User32.dll", EntryPoint = "FindWindow")]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        /// <summary>
        /// 定义用户要传递的消息的数据
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct CopyDataStruct
        {
            public IntPtr dwData;
            public int cbData;//字符串长度
            [MarshalAs(UnmanagedType.LPStr)]
            public string lpData;//字符串
        }

        [DllImport("User32.dll", EntryPoint = "SendMessage")]
        private static extern int SendMessage(
        IntPtr hWnd,                   //目标窗体句柄
        int Msg,                       //WM_COPYDATA
        int wParam,                //自定义数值
        ref CopyDataStruct lParam             //结构体
        );

        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="windowName">window的title，建议加上GUID，不会重复</param>
        /// <param name="strMsg">要发送的字符串</param>
        public static void SendMessage(string windowName, string strMsg)
        {
            if (strMsg == null) return;
            IntPtr hwnd = FindWindow(null, windowName);
            if (hwnd != IntPtr.Zero)
            {
                CopyDataStruct cds;
                cds.dwData = IntPtr.Zero;
                cds.lpData = strMsg;
                //注意：长度为字节数
                cds.cbData = System.Text.Encoding.Default.GetBytes(strMsg).Length + 1;
                // 消息来源窗体
                int fromWindowHandler = 0;
                SendMessage(hwnd, WM_COPYDATA, fromWindowHandler, ref cds);
            }
        }

        #endregion

        #region 单片机
        private SerialPort port = new SerialPort();
        /// <summary>
        /// 接受消息
        /// </summary>
        private StringBuilder MsgStr = new StringBuilder();
        /// <summary>
        /// 确认连接
        /// </summary>
        private void danpianjijiazai()
        {
            try
            {
                int i = 1;
                ///检测串口是否开着
                while (!port.IsOpen)
                {
                    try
                    {
                        ///设置串口名称
                        port.PortName = "COM" + i;
                        ///设置波特率
                        port.BaudRate = 4800;
                        ///数据位
                        port.DataBits = 8;
                        ///设置无协议
                        port.Handshake = Handshake.None;
                        ///设置读取不超时
                        port.ReadTimeout = -1;
                        ///设置停止位
                        port.StopBits = StopBits.One;
                        port.DtrEnable = true;
                        ///无校验
                        port.Parity = Parity.None;
                        ///设置数据接收事件
                        port.DataReceived += ReciveData;
                        ///打开串口
                        port.Open();
                        UpdateStutas();
                        errorTb.Text = "串口连成功";
                    }
                    catch (Exception)
                    {
                        i++;
                        if (i == 10)
                        {
                            break;
                        }
                    }
                }
                //else
                //{
                //    port.Close();
                //    port.DataReceived -= ReciveData;

                //}
            }
            catch (Exception ex)
            {
                errorTb.Text = ex.Message;
            }
            finally
            {
                UpdateStutas();
            }
        }
        /// <summary>
        /// 数据接收事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        int _DPJgoods = 0;
        string dangpianji = null;
        public void ReciveData(object sender, SerialDataReceivedEventArgs e)
        {
            this.Dispatcher.BeginInvoke(DispatcherPriority.Send, (ThreadStart)delegate ()
            { ///是否接受完整
                bool isStop = false;
                ///数据长度
                int count = port.BytesToRead;
                string msg = null;

                byte[] buff = new byte[count];
                ///接收数据
                port.Read(buff, 0, count);
                ///将比特数据转换为字符串
                foreach (byte buf in buff)
                {
                    try
                    {
                        msg = Convert.ToString(buf, 16);
                        MsgStr.Append(msg);
                    }
                    catch (Exception)
                    {
                        //label9.Text = "设备返回不支持的数据！";
                    }
                }
                dangpianji = MsgStr.ToString();
                MsgStr.Clear();
                bool ifna = false;
                if (dangpianji.Contains("7a"))//没有物品，顾客拿了上
                {
                    _DPJgoods++;
                    try
                    {
                        SendMessage("导购——人机交流", "110");
                        ifna = true;
                    }
                    catch (Exception)
                    {
                    }
                }
                else if (dangpianji.Contains("fa"))//有物品，顾客没拿下
                {
                    SendMessage("导购——人机交流", "000");
                    ifna = false;
                }
            });
        }
        /// <summary>
        /// 获取串口状态
        /// </summary>
        /// <returns></returns>
        private void UpdateStutas()
        {
            if (!port.IsOpen)
                return;
            string stuta = "单片机端口" + port.PortName + ": " + port.IsOpen.ToString() + "连接成功";
            //+ " 数据设置状态 DSR:" + port.DsrHolding + "端口的载波检测行的状态 CDH:" + port.CDHolding +
            //"“可以发送”行的状态 CTS:" + port.CtsHolding;
            errorTb.Text = stuta;
        }
        #endregion
    }
}