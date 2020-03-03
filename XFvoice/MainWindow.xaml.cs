using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using iFlyDotNet;
using NAudio.Wave;
using Microsoft.DirectX.DirectSound;
using System.Windows.Threading;
using System.Threading;
//using QCloudAPI_SDK;
using System.Text.RegularExpressions;
using tts;
using System.Runtime.InteropServices;
using System.Media;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using NPOI.HSSF.UserModel;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Data;
using Microsoft.Win32;
using System.Windows.Interop;
using Microsoft.Kinect;
using Gestures;
using System.IO.Ports;
using System.Windows.Media.Animation;
using wpf控件测试.ProcessProcotol;
using System.Diagnostics;
using System.Collections.ObjectModel;
using System.Globalization;

namespace XFvoice
{
    public partial class MainWindow : Window
    {
        #region 变量
      
        /// 显示的画面
        /// </summary>
        private DrawingGroup drawingGroup;
        /// <summary>
        /// 彩色帧读取变量 的装置 
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
        /// <summary>
        /// 彩色图像数据
        /// </summary>
        private Byte[] colorPixelData;
        /// <summary>
        /// 骨头
        /// </summary>
        private List<Tuple<JointType, JointType>> bones;
        /// <summary>
        /// 相机空间的夹紧z值为负值。
        /// </summary>
        private const float InferredZPositionClamp = 0.1f;//
        /// <summary>
        /// 画笔颜色
        /// </summary>
        private List<Pen> bodyColors;
        /// <summary>
        /// 绘制Kinect透明背景的宽
        /// </summary>
        private int displayWidth;
        /// <summary>
        /// 绘制Kinect透明背景的高
        /// </summary>
        private int displayHeight;

        /// <summary>
        /// 物体抓取范围
        /// </summary>
        int _LocationRange = 50;
        /// <summary>
        /// 鼠标点击处
        /// </summary>
        Point MousePoint = new Point(0, 0);
        /// <summary>
        /// 鼠标点击处，在位于图像内画的坐标
        /// </summary>
        Point pPoint = new Point(0, 0);
        /// <summary>
        /// 骨骼帧读取变量 的装置 
        /// </summary>
        private KinectSensor kinectsensor = null;
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
        DrawingContext dc;
        /// <summary>
        /// 夹边矩形厚度
        /// </summary>
        private const double ClipBoundsThickness = 10;//夹边矩形厚度
        bool ifopenclose = false;
        int _DPJgoods = 0;
        string dangpianji = null;
        string VoiceText = null;
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
        /// <summary>
        /// 视频路径排序，序号，文件名字，路径
        /// </summary>
        List<Users> filenamex = new List<Users>();
        /// <summary>
        /// 视频路径
        /// </summary>
        List<string> fileList = new List<string>();
        string s = null;
        /// <summary>
        /// 是否选择有网络的智能模式
        /// </summary>
        bool ifhaveinternect = false;
        string re1;
        /// <summary>
        /// 表格数据
        /// </summary>
        public List<Users> lists = new List<Users>();
        public DataTable dt;
        int ret = 0;
        IntPtr session_ID;
        /// <summary>
        /// 用户与导购系统交互数据
        /// </summary>
        String _RECOVER = null;
        bool voice_flag = false;
        bool Once = false;
        public List<int> intlist = new List<int>();
        iFlyISR isr;
        MicVolume micvolume;
        System.ComponentModel.BackgroundWorker bw = new System.ComponentModel.BackgroundWorker(); //执行多线程任务的控件
   
        /// <summary>
        /// 拉伸关节线的厚度
        /// </summary>
        private const double JointThickness = 3;
        private readonly Brush inferredJointBrush = Brushes.Yellow;
        /// <summary>
        ///用于绘制当前跟踪的接头的刷子。  画一根骨头（关节到关节）      
        /// </summary>
        private readonly Brush trackedJointBrush = new SolidColorBrush(Color.FromArgb(255, 68, 192, 68)); //  
        private readonly Pen inferredBonePen = new Pen(Brushes.Gray, 1);
        DispatcherTimer timer = new DispatcherTimer(); // 定时器timer
        int durTime = 5; // 视频播放时长，也就是循环周期
        bool ifpaly = false;
        bool ifReply = false;
        #endregion
        public MainWindow()
        {
            InitializeComponent();

            media_Play(@"E:\在桌面\dao\语音有错了\XFvoice\Resources\dg.mp4");
            backgroundWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(BAIDUPAIbackgroundWorker_DoWork);
            backgroundWorker.RunWorkerCompleted += BAIDUPAIbackgroundWorker_RunWorkerCompleted;
            //可以返回工作进度
            backgroundWorker.WorkerReportsProgress = true;
            backgroundWorker.ProgressChanged += BAIDUPAIbackgroundWorker_ProgressChanged;
            //允许取消
            backgroundWorker.WorkerSupportsCancellation = true;
            
            bw.DoWork += new System.ComponentModel.DoWorkEventHandler(bw_DoWork);//订阅DoWork事件  
            bw.RunWorkerCompleted += bw_RunWorkerCompleted;//订阅报告进程事件  
            bw.WorkerReportsProgress = true;


            voiceInteractionViewBT.Background = Brushes.White;
            VoiceButtonOFF.Visibility = Visibility.Hidden;
            VoiceButtonON.IsEnabled = true;//开始建可见
            VoiceButtonOFF.IsEnabled = false;//暂停建不可见
            micvolume = new MicVolume();
            micvolume.vol_start();
            micvolume.volume_fresh += new EventHandler<MicVolume.DataArrivedEventArgs>(fresh_volume);
            Brush grey = new SolidColorBrush(Color.FromArgb(128, 128, 128, 128));//指示灯
            StateLight.Fill = grey;

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
            //this.kinectDevice.Open();
            this.DataContext = this;
            this.drawingGroup = new DrawingGroup();
            ColorViewer.Source = new DrawingImage(this.drawingGroup);//骨骼
            this.kinectDevice.Open();
            this.coordinateMapper = this.kinectsensor.CoordinateMapper;
            this.Loaded += MainWindow_Loaded;
            this.Closed += MainWindow_Closed;
        }

        #region 语音
        void CloseAVI()
        {
            try
            {
                TimeSpan AviPosition = mediaElement.Position;//当前播放点

                TimeSpan assdj = TimeSpan.FromSeconds(mediaElement.NaturalDuration.TimeSpan.TotalSeconds);//时长
                //ViewMessgeBox.Text += AviPosition.ToString()  + assdj.ToString();
                //if (assadj.ToString() == assdj.ToString())
                if (mediaElement.Clock.IsPaused)
                {
                    mediaElement.Visibility = Visibility.Hidden;
                }
                //  Thread.Sleep(100);
            }
            catch (Exception)
            { }
        }
        ~MainWindow()
        { Environment.Exit(0); }
        void Isr_ISREnd(object sender, EventArgs e)
        {
        }
        void fresh_volume(object sender, MicVolume.DataArrivedEventArgs e)//音量显示
        {
            this.Dispatcher.BeginInvoke(DispatcherPriority.Send, (ThreadStart)delegate ()
            {
                VolumeBar.Value = e.vol;//音量条
            });//改变输入框内容
        }
        void Voice_control(object sender, bool e)
        {

        }

        /// <summary>
        /// 识别内容显示,进行声控
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Asr_DataAvailable(object sender, iFlyISR.DataArrivedEventArgs e)//识别内容语音控制
        {
            try
            {
                string text = e.result;

                //if ((int)text[1] > 127)//"是汉字",不是汉字开头不识别
                //{
                if (text.Contains("你好") || text.Contains("导购猫") || ifReply)
                {
                    ifReply = true;
                    if (text.Contains("你好") || text.Contains("导购猫"))
                    {
                        this.Dispatcher.BeginInvoke(DispatcherPriority.Send, (ThreadStart)delegate ()
                        {
                            textToBubble("你好！智能导购员小宇很高兴为你服务。");

                            text = "";
                            return;
                        }); //Thread.Sleep(100);
                    }
                    else
                    {
                            string rec_result = Texterrorcorrection(text);
                            switch (rec_result)
                            {
                                case "清理界面":
                                    this.Dispatcher.BeginInvoke(DispatcherPriority.Send, (ThreadStart)delegate ()
                                    {
                                       
                                        ViewMessgeBox.Document.Blocks.Clear();
                                        //textToBubble("清理完成！");
                                        
                                    });
                                rec_result = null;
                                break;
                                case "结束导购":
                                    this.Dispatcher.BeginInvoke(DispatcherPriority.Send, (ThreadStart)delegate ()
                                    {
                                      
                                        ViewMessgeBox.Document.Blocks.Clear();
                                        try
                                        {
                                            if (voice_flag)
                                            {
                                                VoiceButtonON.IsEnabled = true;
                                                VoiceButtonOFF.IsEnabled = false;
                                                voice_flag = false;
                                                isr.iatStop();
                                                Brush red = new SolidColorBrush(Color.FromArgb(128, 255, 0, 0));
                                                StateLight.Fill = red;
                                                VoiceButtonON.Visibility = Visibility.Visible;
                                                VoiceButtonOFF.Visibility = Visibility.Hidden;
                                                errorTb.Text = "语音关闭";
                                            }
                                        }
                                        catch (Exception ex)
                                        {

                                            errorTb.Text = ex.Message;
                                        }
                                        ifReply = false;
                                        textToBubble("本次导购结束，谢谢！");
                                     
                                    });
                                rec_result = null;
                                break;
                            case "关闭视频":
                                    this.Dispatcher.BeginInvoke(DispatcherPriority.Send, (ThreadStart)delegate ()
                                    {
                                        mediaElement.Pause();
                                        mediaElement.Stop();
                                        mediaElement.Visibility = Visibility.Hidden;
                                        textToBubble("已为您关闭视频！");
                                        
                                    });
                                rec_result = null;
                                break;
                            default:
                                    break;
                            }
                        if (rec_result !=null)
                        {
                            this.Dispatcher.BeginInvoke(DispatcherPriority.Send, (ThreadStart)delegate ()
                            {
                                System.Media.SystemSounds.Beep.Play();//提示音
                                string mm = rec_result + "!";
                                VoicetextBox.AppendText(mm);
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            { //  MessageBox.Show( ex.Message);
            }
        }

        /// <summary>
        /// 语音文本转换为气泡
        /// </summary>
        /// <param name="vioceString">语音文本</param>
        public void textToBubble(string vioceString)
        {
            try
            {
                RichTextBox MyMessageBox = new RichTextBox();
                MyMessageBox.AppendText(vioceString);
                TextRange msgBoxContent = new TextRange(MyMessageBox.Document.ContentStart, MyMessageBox.Document.ContentEnd);
                if (!msgBoxContent.IsEmpty)
                {
                    Grid gridMessage;
                    String SendMessage = ChatProcess.CheckOut(MyMessageBox);
                    String msgContent = SendMessage.Substring(0, 5);
                    if (msgContent.Equals("#FONT"))
                    {
                        gridMessage = ChatProcess.AppendMessage(MyMessageBox, this.ViewMessgeBox, "智能--导购猫");
                        BlockUIContainer MessageContent = new BlockUIContainer(gridMessage);
                        this.ViewMessgeBox.Document.Blocks.Add(MessageContent);
                        this.ViewMessgeBox.ScrollToEnd();
                        MyMessageBox.Document.Blocks.Clear();
                    }
                }
                // Thread.Sleep(1);
            }
            catch (Exception ex)
            {
                //   string Text = ex.Message;
            }
        }
        /// <summary>
        /// 写入数据保存
        /// </summary>
        /// <param name="Path">保存路径</param>
        /// <param name="Strings">保存内容</param>
        public static void WriteFile(string Path, string Strings)
        {
            try
            {
                if (!System.IO.File.Exists(Path))
                {
                    System.IO.FileStream f = System.IO.File.Create(Path);
                    f.Close();
                    f.Dispose();
                }
                System.IO.StreamWriter f2 = new System.IO.StreamWriter(Path, true, System.Text.Encoding.UTF8);
                f2.WriteLine(Strings);
                f2.Close();
                f2.Dispose();
            }
            catch (Exception)
            {
            }
        }
        /// <summary>
        /// 状态显示，不重要
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void Rec_DataAvailable(object sender, iFlyISR.DataArrivedEventArgs e)//状态显示
        {
            this.Dispatcher.BeginInvoke(DispatcherPriority.Send, (ThreadStart)delegate ()
            {
                errorTb.Text = e.result;
            });
        }

        public void tts_Finished(object sender, iFlyTTS.JinDuEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine(e.AllP.ToString());
        }
        /// <summary>
        /// 语音修改
        /// </summary>
        /// <param name="t">要修改字段</param>
        /// <returns>修改完成字段</returns>
        public string Texterrorcorrection(string t)
        {
            try
            {
                //Match mcshu = null;
                //string rec_result = Demo.GetApi(t, "LexicalCheck");
                //string ptnshu = ("text\":\"(?<shu>.*?)\"\r\n\"text_annotate");
                //Regex regshu = new Regex(ptnshu);
                //mcshu = regshu.Match(rec_result);
                //Thread.Sleep(100);
                //return mcshu.Groups["shu"].Value;
                return t;
            }
            catch (Exception)
            {
                return t;
            }
        }

        /// <summary>
        /// 连接讯飞接口
        /// </summary>
        /// <returns>返回答复字段</returns>
        public string InteractiveXF()
        {
                string wroing = "网络连接错误！";
                try
                {
                    string input = re1;

                    byte[] responseData = null;
                    string text1 = "text=" + Base64.EncodeBase64(input);

                    string ctime = GetTimeStamp();
                    string s1 = "2b8e41a42c094fae930b512fc0852324" + ctime + "eyJzY2VuZSI6Im1haW4iLCAidXNlcmlkIjoiNWExZmU2ZjUifQ==" + text1;
                    string cs = EncryptMD5(s1);
                    string Param = "eyJzY2VuZSI6Im1haW4iLCAidXNlcmlkIjoiNWExZmU2ZjUifQ==";
                    byte[] postData = Encoding.UTF8.GetBytes(text1);//编码，尤其是汉字，事先要看下抓取网页的编码方式  
                    string url = "http://api.xfyun.cn/v1/aiui/v1/text_semantic";//地址  
                    WebClient webClient = new WebClient();
                    WebHeaderCollection whc = new WebHeaderCollection();
                    whc["X-Appid"] = "5a61fd9e";
                    whc["X-CheckSum"] = cs;
                    whc["X-CurTime"] = ctime;
                    whc["X-Param"] = Param;
                    webClient.Headers = whc;
                    webClient.Headers.Add("Content-Type", "application/x-www-form-urlencoded");

                    responseData = webClient.UploadData(url, "POST", postData);//得到返回字符流  

                    string srcString = Encoding.UTF8.GetString(responseData);//解码  
                    string result = srcString;

                    if (result.Contains("ip"))
                    {
                        string re = "ip非法，您的ip是：(?<ip>.*?)\",\"data";
                        Regex regScore = new Regex(re);
                        Match mcScore = regScore.Match(result);
                        string ip = mcScore.Groups["ip"].Value;
                        if (ip.Contains("."))
                        {
                            s = "请将您联系开发者将IP：" + ip + "加入白名单,或者选择简易交流模式";
                            return s;
                        }
                    }
                    string ans = StrDeal(result);
                    string login_configs = "appid=5a61fd9e";
                    string text = ans.Trim();
                    if (string.IsNullOrEmpty(text))
                    {
                        return text = "请输入合成语音的内容";
                    }
                    string filename = "Call.wav";
                    uint audio_len = 0;
                    tts.SynthStatus synth_status = tts.SynthStatus.MSP_TTS_FLAG_STILL_HAVE_DATA;
                    ret = TTSDll.MSPLogin(string.Empty, string.Empty, login_configs);                                       //MSPLogin方法返回失败
                    if (ret != (int)ErrorCode.MSP_SUCCESS)
                    {
                        return s = wroing;
                    }
                    string _params = "ssm=1,ent=sms16k,vcn=xiaoyan,spd=medium,aue=speex-wb;7,vol=x-loud,auf=audio/L16;rate=16000";
                    session_ID = TTSDll.QTTSSessionBegin(_params, ref ret);

                    if (ret != (int)ErrorCode.MSP_SUCCESS)
                    {
                        return s = wroing;
                    }
                    ret = TTSDll.QTTSTextPut(Ptr2Str(session_ID), text, (uint)Encoding.Default.GetByteCount(text), string.Empty);
                    if (ret != (int)ErrorCode.MSP_SUCCESS)
                    {
                        return s = wroing;
                    }
                    MemoryStream memoryStream = new MemoryStream();
                    memoryStream.Write(new byte[44], 0, 44);
                    while (true)
                    {
                        IntPtr source = TTSDll.QTTSAudioGet(Ptr2Str(session_ID), ref audio_len, ref synth_status, ref ret);
                        byte[] array = new byte[(int)audio_len];
                        if (audio_len > 0)
                        {
                            Marshal.Copy(source, array, 0, (int)audio_len);
                        }
                        memoryStream.Write(array, 0, array.Length);
                        Thread.Sleep(100);
                        if (synth_status == tts.SynthStatus.MSP_TTS_FLAG_DATA_END || ret != 0)
                            break;
                    }
                    WAVE_Header wave_Header = getWave_Header((int)memoryStream.Length - 44);
                    byte[] array2 = this.StructToBytes(wave_Header);
                    memoryStream.Position = 0L;
                    memoryStream.Write(array2, 0, array2.Length);
                    memoryStream.Position = 0L;
                    SoundPlayer soundPlayer = new SoundPlayer(memoryStream);
                    soundPlayer.Stop();
                    soundPlayer.Play();
                    if (filename != null)
                    {
                        FileStream fileStream = new FileStream(filename, FileMode.Create, FileAccess.Write);
                        memoryStream.WriteTo(fileStream);
                        memoryStream.Close();
                        fileStream.Close();
                    }
                    ret = TTSDll.QTTSSessionEnd(Ptr2Str(session_ID), "");
                    ret = TTSDll.MSPLogout();//退出登录
                    s = ans;
                }
                catch (Exception)
                {
                    ret = TTSDll.QTTSSessionEnd(Ptr2Str(session_ID), "");
                    ret = TTSDll.MSPLogout();//退出登录
                    return s = wroing;

                }
                return s;
        }

        /// <summary>
        /// 语音播报回答内容
        /// </summary>
        public void MachineAnswer()
        {
            //string appid = "appid=5a1fe6f5";
            //iFlyTTS tts = new iFlyTTS(appid);
            //tts.Finished += tts_Finished;
            //tts.vol = iFlyDotNet.enuVol.x_loud;
            //tts.speed = iFlyDotNet.enuSpeed.medium;
            //tts.speeker = (iFlyDotNet.enuSpeeker)1;
            //tts.MultiSpeek(s, "audio.wav");
            //System.Media.SoundPlayer sp = new System.Media.SoundPlayer("audio.wav");
            //sp.Play();
        }
        /// <summary>
        /// 无需联网判断
        /// </summary>
        /// 
        public void Local_interaction()
        {
            try
            {
                if (Once)
                {
                    string s1 = re1;
                    s = null;
                    if (lists.Count != 0)
                    {
                        foreach (var item in lists)
                        {
                            if (s1.Contains(item.商品名) && s1.Contains(item.商品名))//
                            {
                                s += "关于" + item.商品名 + "  介绍：" + item.简介.ToString();
                                
                                _RECOVER += s;
                                VoicetextBox.Text = "";

                               foreach (var fitem in filenamex)//找视频
                                {
                                    if (item.序号 == fitem.序号)
                                    {
                                        mediaElement.Source = new Uri(fileList[Convert.ToInt32(item.序号) - 1]);
                                        mediaElement.Visibility = Visibility.Visible;
                                        mediaElement.Play(); // 开始播放
                                        ifpaly = true;
                                     
                                        break;
                                    }
                                    else
                                    {
                                      //  s = "  暂无该介绍视频";
                                    }
                                }
                                textToBubble(s);

                            }
                            else if (item.商品名 == lists[lists.Count - 1].商品名 && s == null)
                            {
                                s = "没有找你要的商品！";
                                textToBubble(s);
                                _RECOVER += s;
                                VoicetextBox.Text = "";
                            }
                        }
                    }
                    else
                    {
                        //s = "系统未加载任何数据，请管理员先加载数据！ ";
                        //textToBubble(s);
                        //_RECOVER += s;
                        VoicetextBox.Text = "";
                    }

                    Once = false;
                }
            }
            catch (Exception ex)
            {
                //  errorTb.Text = "提示：请先录入数据";
            }
        }
        /// <summary>
        /// 联网用线程，接收结果
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void bw_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            Thread.Sleep(1000);
            //BackgroundWorker bw = sender as BackgroundWorker;
            e.Result = InteractiveXF();
            if (bw.CancellationPending)
            {
                e.Cancel = true;
            }
        }
        /// <summary>
        /// 显示线程结果
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (Once)
            {
                if (e.Cancelled)
                {
                    // 取消操作。
                    // tbop.Text = "取消了操作。";
                }
                else if (e.Error != null) //过程出现了一个错误。
                {
                    string msg = String.Format("出现了一个错误--> {0}", e.Error.Message);
                    textToBubble(s);
                    _RECOVER += msg;
                    //  errorTb.Text = msg;
                }
                else // 操作正常完成。
                {
                    // errorTb.Text = "就绪";
                    string msg = String.Format("{0}", e.Result);
                    textToBubble(s);
                    _RECOVER += msg;
                    VoicetextBox.Text = "";
                    Once = false;
                }

            }
        }

        /// <summary>
        /// EXCEL表格放入list集合
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        public List<Users> Ggd(DataTable d)
        {
            goodsname.Items.Clear();

            lists.Clear();
            for (int i = 0; i < d.Rows.Count; i++)
            {
                Users u = new Users();
                u.序号 = d.Rows[i][0].ToString();
                u.商品名 = d.Rows[i][1].ToString();
                u.简介 = d.Rows[i][2].ToString();
                lists.Add(u);
                goodsname.Items.Add(u.商品名);
            }

            return lists;
        }
        /// <summary>
        /// 导入表格
        /// </summary>
        /// <param name="file">表格路径</param>
        /// <returns></returns>
        public static DataTable ExcelToTable(string file)
        {
            DataTable dt = new DataTable();
            IWorkbook workbook;
            string fileExt = System.IO.Path.GetExtension(file).ToLower();
            using (FileStream fs = new FileStream(file, FileMode.Open, FileAccess.Read))
            {
                if (fileExt == ".xlsx")
                {
                    workbook = new XSSFWorkbook(fs);
                }
                else if (fileExt == ".xls")
                {
                    workbook = new HSSFWorkbook(fs);
                }
                else { workbook = null; }
                if (workbook == null) { return null; }
                ISheet sheet = workbook.GetSheetAt(0);

                //表头  
                IRow header = sheet.GetRow(sheet.FirstRowNum);
                List<int> columns = new List<int>();
                for (int i = 0; i < header.LastCellNum; i++)
                {
                    object obj = GetValueType(header.GetCell(i));
                    if (obj == null || obj.ToString() == string.Empty)
                    {
                        dt.Columns.Add(new DataColumn("Columns" + i.ToString()));
                    }
                    else
                        dt.Columns.Add(new DataColumn(obj.ToString()));
                    columns.Add(i);
                }
                //数据  
                for (int i = sheet.FirstRowNum + 1; i <= sheet.LastRowNum; i++)
                {
                    DataRow dr = dt.NewRow();
                    bool hasValue = false;
                    foreach (int j in columns)
                    {
                        dr[j] = GetValueType(sheet.GetRow(i).GetCell(j));
                        if (dr[j] != null && dr[j].ToString() != string.Empty)
                        {
                            hasValue = true;
                        }
                    }
                    if (hasValue)
                    {
                        dt.Rows.Add(dr);
                    }
                }
            }
            return dt;
        }
        /// <summary>
        /// 表头值转化
        /// </summary>
        /// <param name="cell"></param>
        /// <returns></returns>
        public static object GetValueType(ICell cell)
        {
            if (cell == null)
                return null;
            switch (cell.CellType)
            {
                case CellType.Blank: //BLANK:  
                    return null;
                case CellType.Boolean: //BOOLEAN:  
                    return cell.BooleanCellValue;
                case CellType.Numeric: //NUMERIC:  
                    return cell.NumericCellValue;
                case CellType.String: //STRING:  
                    return cell.StringCellValue;
                case CellType.Error: //ERROR:  
                    return cell.ErrorCellValue;
                case CellType.Formula: //FORMULA:  
                default:
                    return "=" + cell.CellFormula;
            }
        }
        /// <summary>
        /// 音频文件数据转换为字节结构
        /// </summary>
        /// <param name="structure">音频文件数据</param>
        /// <returns>字节数组</returns>
        private byte[] StructToBytes(object structure)
        {
            int num = Marshal.SizeOf(structure);
            IntPtr intPtr = Marshal.AllocHGlobal(num);
            byte[] result;
            try
            {
                Marshal.StructureToPtr(structure, intPtr, false);
                byte[] array = new byte[num];
                Marshal.Copy(intPtr, array, 0, num);
                result = array;
            }
            finally
            {
                Marshal.FreeHGlobal(intPtr);
            }
            return result;
        }
        /// <summary>
        /// 结构体初始化赋值
        /// </summary>
        /// <param name="data_len"></param>
        /// <returns></returns>
        private WAVE_Header getWave_Header(int data_len)
        {
            return new WAVE_Header
            {
                RIFF_ID = 1179011410,
                File_Size = data_len + 36,
                RIFF_Type = 1163280727,
                FMT_ID = 544501094,
                FMT_Size = 16,
                FMT_Tag = 1,
                FMT_Channel = 1,
                FMT_SamplesPerSec = 16000,
                AvgBytesPerSec = 32000,
                BlockAlign = 2,
                BitsPerSample = 16,
                DATA_ID = 1635017060,
                DATA_Size = data_len
            };
        }
        /// <summary>
        /// 语音音频头
        /// </summary>
        private struct WAVE_Header
        {
            public int RIFF_ID;
            public int File_Size;
            public int RIFF_Type;
            public int FMT_ID;
            public int FMT_Size;
            public short FMT_Tag;
            public ushort FMT_Channel;
            public int FMT_SamplesPerSec;
            public int AvgBytesPerSec;
            public ushort BlockAlign;
            public ushort BitsPerSample;
            public int DATA_ID;
            public int DATA_Size;
        }
        /// 指针转字符串
        /// </summary>
        /// <param name="p">指向非托管代码字符串的指针</param>
        /// <returns>返回指针指向的字符串</returns>
        public static string Ptr2Str(IntPtr p)
        {
            List<byte> lb = new List<byte>();
            try
            {

                while (Marshal.ReadByte(p) != 0)//dll重复
                {
                    lb.Add(Marshal.ReadByte(p));
                    p = p + 1;
                }
                byte[] bs = lb.ToArray();
                return Encoding.Default.GetString(lb.ToArray());
            }
            catch (Exception)
            {
                return Encoding.Default.GetString(lb.ToArray());
            }
        }
        /// <summary>
        /// 比对数据
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public string StrDeal(string s)
        {
            char[] c = { ':' };
            string regular1 = "\"\\s*service\\s*\".*?\",";
            string s1 = Regex.Match(s, regular1).ToString();
            string s3;
            foreach (var item in lists)
            {
                if (s1.Contains("bao"))
                {
                    string regular3 = "\"\\s*answer\\s*\":.*\"";
                    s3 = Regex.Match(s, regular3).ToString();
                    string regular4 = "\"\\s*text\\s*\":\".*?\"";
                    string re = Regex.Match(s3, regular4).ToString();
                    re = re.Split(c)[1];//"你好，又见面了真开心。"
                    re = re.Replace("\"", "");
                    return re;
                }
                else if (s1.Contains("CQ.daogou1"))//物品返回价格
                {
                    string regular3 = "\"normValue\\s*\":\".*?\"";
                    s3 = Regex.Match(s, regular3).ToString();
                    if (s3.Contains(item.商品名))
                    {
                        this.Dispatcher.BeginInvoke(DispatcherPriority.Send, (ThreadStart)delegate ()
                        {
                            mediaElement.Source = new Uri(fileList[Convert.ToInt32(item.序号) - 1]);//////////////线程
                            mediaElement.Play(); // 开始播放
                        });
                        return "关于" + item.商品名 + "价格为：" + item.简介.ToString();

                    }
                }
                else if (s.Contains(item.商品名))//
                {
                    this.Dispatcher.BeginInvoke(DispatcherPriority.Send, (ThreadStart)delegate ()
                    {
                        mediaElement.Source = new Uri(fileList[Convert.ToInt32(item.序号) - 1]);//////////////线程
                        mediaElement.Play(); // 开始播放
                    });
                    return "关于" + item.商品名 + "价格为：" + item.简介.ToString();

                }
                else if (item.商品名 == lists[lists.Count - 1].商品名)
                {

                    return "没有找你要的商品";
                }
            }
            return "请先导入表";
        }
        /// <summary>
        /// 获取数据
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string GetTimeStamp()
        {
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt64(ts.TotalSeconds).ToString();
        }
        /// <summary>
        /// MD5加密
        /// </summary>
        /// <param name="s">待加密的明文</param>
        /// <returns></returns>
        public static string EncryptMD5(string s)
        {
            var md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
            var result = "";
            if (!string.IsNullOrWhiteSpace(s))
            {
                result = BitConverter.ToString(md5.ComputeHash(UnicodeEncoding.UTF8.GetBytes(s.Trim())));
            }
            result = result.Replace("-", "");
            result = result.ToLower();
            return result;
        }
        public sealed class Base64
        {
            /// <summary>
            /// Base64加密
            /// </summary>
            /// <param name="encode">加密采用的编码方式</param>
            /// <param name="source">待加密的明文</param>
            /// <returns></returns>
            public static string EncodeBase64(Encoding encode, string source)
            {
                string enString = "";
                byte[] bytes = encode.GetBytes(source);
                try
                {
                    enString = Convert.ToBase64String(bytes);
                }
                catch
                {
                    enString = source;
                }
                return enString;
            }
            /// <summary>
            /// Base64加密，采用utf8编码方式加密
            /// </summary>
            /// <param name="source">待加密的明文</param>
            /// <returns>加密后的字符串</returns>
            public static string EncodeBase64(string source)
            {
                return EncodeBase64(Encoding.UTF8, source);
            }
        }

        //public void timerEvent(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        TimeSpan assadj = mediaElement.Position;
        //        TimeSpan assdj = TimeSpan.FromSeconds(mediaElement.NaturalDuration.TimeSpan.TotalSeconds);
        //        if (assadj.ToString() == assdj.ToString())
        //        {
        //            mediaElement.Stop();
        //        }
        //    }
        //    catch (Exception)
        //    {
        //    }
        //}


        private void media_Loaded(object sender, RoutedEventArgs e)
        {
            (sender as MediaElement).Play();
        }
        private void media_MediaEnded(object sender, RoutedEventArgs e)
        {
            // MediaElement需要先停止播放才能再开始播放，
            // 否则会停在最后一帧不动
            (sender as MediaElement).Stop();
            (sender as MediaElement).Play();
        }
        private void media_Unloaded(object sender, RoutedEventArgs e)
        {
            (sender as MediaElement).Stop();
        }
        void media_Play(string mediaUrl)
        {
            mediaElement.Source = new Uri(mediaUrl);
            // 交互式控制
            mediaElement.LoadedBehavior = MediaState.Manual;
            // 添加元素加载完成事件 -- 自动开始播放
            mediaElement.Loaded += new RoutedEventHandler(media_Loaded);
            // 添加媒体播放结束事件 -- 重新播放
            mediaElement.MediaEnded += new RoutedEventHandler(media_MediaEnded);
            // 添加元素卸载完成事件 -- 停止播放
            mediaElement.Unloaded += new RoutedEventHandler(media_Unloaded);
            mediaElement.Visibility = Visibility.Visible;
            ifpaly = true; mediaElement.Play();
        }

        #endregion

        #region kinect动作接收
        /// <summary>
        /// 一帧彩色帧率
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void _ColorFrameReader_FrameArrived(object sender, ColorFrameArrivedEventArgs e)//彩色
        {
            // bool goodsReceived = false;
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
                    //  goodsReceived = true;
                }
            }
        }
        /// <summary>
        /// 打开体感器
        /// </summary>
        public void startKinect()
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
            gesturecontroller = new GestureController();//骨骼挥手
            gesturecontroller.GestureRecognized += gesturecontroller_GestureRecognized;
        }
        /// <summary>
        /// 多开源，获取彩色图和骨骼数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void msfreader_MultiSourceFrameArrived(object sender, MultiSourceFrameArrivedEventArgs e)
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


                            if (closestBody != null && closestBody.IsTracked)//是否有骨骼帧
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
        /// <param name="handState">手实时状态</param>
        /// <param name="handPosition">手坐标</param>
        public void DrawRHand(HandState handState, Point handPosition)
        {
            int x = 0;
            int y = 0;
            string getStr = goodsname.Text;
            try
            {
                switch (handState)
                {
                    case HandState.Closed:
                        x = (int)handPosition.X * (int)ColorViewer.Source.Width / (int)ColorViewer.Width;
                        y = (int)handPosition.Y * (int)ColorViewer.Source.Height / (int)ColorViewer.Height;

                        if (Math.Abs(x - MousePoint.X) < _LocationRange && Math.Abs(y - MousePoint.Y) < _LocationRange)
                        {
                            //"顾客拿到指定货物";
                            if (ifopenclose)
                            {
                                VoicetextBox.Text = "选择了：" + getStr + "。";
                            }
                        }
                        else
                        {
                            //"没拿到货物 ";
                        }
                        ifopenclose = false;

                        break;
                    case HandState.Open:

                        ifopenclose = true;
                        break;
                }
            }
            catch (Exception ex)
            {
                // errorTb.Text = ex.Message;
            }

        }
        /// <summary>
        /// 判断挥手手势
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void gesturecontroller_GestureRecognized(object sender, GestureEventArgs e)
        {
            int index = avicb.SelectedIndex;
            try
            {
                // if (mediaElement.Clock.IsPaused)
                if (mediaElement.HasVideo)
                {
                    if (e.GestureType == GestureTypes.SwipeLeft)
                    {
                        if (fileList.Count != 0 && index != 0)//不是第一个视频,右手
                        {
                            mediaElement.Source = new Uri(fileList[index - 1]);
                            mediaElement.Play(); // 开始播放
                            avicb.SelectedIndex--;
                            //textToBubble("接收到您的指令为您播放上一个介绍视频");
                        }
                        else
                        {
                           // textToBubble("请用左手向右挥");
                        }
                    }
                    if (e.GestureType == GestureTypes.SwipeRight)
                    {
                        if (fileList.Count != 0 && index != fileList.Count)//不是最后一个视频,左手
                        {
                            mediaElement.Source = new Uri(fileList[index + 1]);
                            mediaElement.Play(); // 开始播放
                            avicb.SelectedIndex++;
                         //   textToBubble("接收到您的指令为您播放下一个介绍视频");
                        }
                        else
                        {
                          //  textToBubble("请用右手向左挥");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // errorTb.Text = ex.Message ;
            }
        }
        #region 画骨骼,显示彩色图
        /// <summary>
        /// 获取每个骨骼帧率，画上骨架
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void Reader_FrameArrived(object sender, BodyFrameArrivedEventArgs e)
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
                    
                    #region 画骨骼
                    using (dc = this.drawingGroup.Open())
                    {
                       // ObjectRecognition.Text = ("识别到人体");
                        //绘制透明背景以设置渲染大小。
                        dc.DrawImage(colorBitmap, new Rect(0.0, 0.0, this.displayWidth, this.displayHeight));

                        drawGoods(pPoint, dc);

                        int penIndex = 0;
                        //foreach (Body body in this.bodies)
                        //{
                        //    Pen drawPen = this.bodyColors[penIndex++];

                        //    if (body.IsTracked)
                        //    {
                        //        this.DrawClippedEdges(body, dc);

                        //        IReadOnlyDictionary<JointType, Joint> joints = body.Joints;
                        //        Dictionary<JointType, Point> jointPoints = new Dictionary<JointType, Point>();

                        //        foreach (JointType jointType in joints.Keys)
                        //        {
                        //            CameraSpacePoint position = joints[jointType].Position;
                        //            if (position.Z < 0)
                        //            {
                        //                position.Z = InferredZPositionClamp;
                        //            }

                        //            DepthSpacePoint depthSpacePoint = this.coordinateMapper.MapCameraPointToDepthSpace(position);
                        //            jointPoints[jointType] = new Point(depthSpacePoint.X, depthSpacePoint.Y);
                        //        }
                        //        this.DrawBody(joints, jointPoints, dc, drawPen);
                        //        //this.DrawLHand(body.HandLeftState, jointPoints[JointType.HandLeft], dc);//左手圈
                        //        //this.DrawRHand(body.HandRightState, jointPoints[JointType.HandRight], dc);//右手圈
                        //    }
                        //}
                        //防止在渲染区域之外绘制
                        this.drawingGroup.ClipGeometry = new RectangleGeometry(new Rect(0.0, 0.0, this.displayWidth, this.displayHeight));
                    }
                    #endregion
                }
                else
                {
                }
            }
            catch (Exception ex)
            {
                errorTb.Text = "暂无骨骼画像" + ex.Message;
            }
        }
        /// <summary>
        ///画布大小
        /// </summary>
        /// <param name="body">身体</param>
        /// <param name="drawingContext">画入的地方</param>
        public void DrawClippedEdges(Body body, DrawingContext drawingContext)
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
        /// <summary>
        ///  描身体
        /// </summary>
        /// <param name="joints">关节</param>
        /// <param name="jointPoints">关节点位置坐标</param>
        /// <param name="drawingContext">画入的地方</param>
        /// <param name="drawingPen">画笔</param>
        public void DrawBody(IReadOnlyDictionary<JointType, Joint> joints, IDictionary<JointType, Point> jointPoints, DrawingContext drawingContext, Pen drawingPen)
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
        /// <summary>
        /// 画骨头
        /// </summary>
        /// <param name="joints">关节</param>
        /// <param name="jointPoints">关节点位置坐标</param>
        /// <param name="jointType0">（起始）关节点1</param>
        /// <param name="jointType1">（终点）关节点2</param>
        /// <param name="drawingContext">画入的地方</param>
        /// <param name="drawingPen">画笔</param>
        public void DrawBone(IReadOnlyDictionary<JointType, Joint> joints, IDictionary<JointType, Point> jointPoints, JointType jointType0, JointType jointType1, DrawingContext drawingContext, Pen drawingPen)
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
        /// <summary>
        /// 所点击坐标画物体
        /// </summary>
        /// <param name="p">坐标</param>
        /// <param name="drawingContext">画布</param>
        public void drawGoods(Point p, DrawingContext drawingContext)
        {

            drawingContext.DrawEllipse(
              Brushes.Blue,
              null,
             p, 10, 10);
        }

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

        /// <summary>
        /// 接收单片机数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void ReciveData(object sender, SerialDataReceivedEventArgs e)
        {
            this.Dispatcher.BeginInvoke(DispatcherPriority.Send, (ThreadStart)delegate ()
            { ///是否接受完整
               // bool isStop = false;
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
                        //"设备返回不支持的数据！";
                    }
                }
                dangpianji = MsgStr.ToString();

                MsgStr.Clear();
                bool ifTake = false;
                if (dangpianji == "fa" || dangpianji == "7a")//没有物品，顾客拿了上
                {
                    _DPJgoods++;
                    try
                    {
                        avicb.SelectedIndex = 0;
                        mediaElement.Source = new Uri(fileList[0]);
                        mediaElement.Play(); // 开始播放
                       // textToBubble(goodsname.Text + "被顾客拿下货架" + _DPJgoods);
                        //vtb.Text ="顾客拿下货架物品"+goodsname.Text;
                        ifTake = true;
                    }
                    catch (Exception)
                    {
                    }
                }
                else if (dangpianji.Contains("fa7afa"))//有物品，顾客没拿下
                {
                    ifTake = false;
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

            errorTb.Text = stuta;
        }
        #endregion

        #endregion
        
        #region 识别物体
        
        void BAIDUPAIbackgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
           // progressBar.Value = e.ProgressPercentage;
        }
        void BAIDUPAIbackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {

            if (e.Cancelled)
            {
                //MessageBox.Show("error");

            }
            else if (e.Error != null) //过程出现了一个错误。
            {
                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, (ThreadStart)delegate ()
                {
                    errorTb.Text = String.Format("识别出现错误--> {0}", e.Error.Message);
                });
            }
            else
            {
                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, (ThreadStart)delegate ()
                {
                    string isbao = e.Result.ToString();
                    //if(isbao.Contains("包"))
                    //{
                    //    ObjectRecognition.Text=("视线范围检测到：  "+isbao);
                    //}
                    //else
                    //{
                    //    ObjectRecognition.Text = ("视线范围没有检测到包！");
                    //}
                });
            }
        }
        void BAIDUPAIbackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            //BackgroundWorker worker = sender as BackgroundWorker;
            //if (worker.CancellationPending)
            //{
            //    e.Cancel = true;
            //}
            //else
            //{
            //    Thread.Sleep(15000);
            //    e.Result = Choice();
            //}
            Thread.Sleep(10000);//结束数据大概时间
            //在此处设置返回值
            e.Result = Choice();
        }
         void BAIDUPAIbackgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            //在此处接收传递回来的值
            int returnValue = (int)e.Result;
        }
        string Choice()
        {
            string words = "";
            this.Dispatcher.Invoke(DispatcherPriority.Normal, (ThreadStart)(delegate
            {
                if (this.colorBitmap != null)
                {
                    // 创建一个知道如何保存.png文件的png位图编码器
                    BitmapEncoder encoder = new PngBitmapEncoder();

                    // 从可写位图创建帧并添加到编码器
                    encoder.Frames.Add(BitmapFrame.Create(this.colorBitmap));

                    string time = System.DateTime.Now.ToString("hh'-'mm'-'ss", CultureInfo.CurrentUICulture.DateTimeFormat);

                    string myPhotos = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);

                    string path = System.IO.Path.Combine(myPhotos, "导购" + time + ".png");

                    // 将新文件写入磁盘

                    // 文件流可IDisposable
                    FileStream fs;
                    using (fs = new FileStream(path, FileMode.Create))
                    {
                        encoder.Save(fs);
                    }
                    var image = File.ReadAllBytes(path);
                    string API_KEY = "U7qFDaD6ioXIHsNzf1I3ziQn";
                    string SECRET_KEY = "my2r1tMGDbzbISES9LjdPn7AjH7ePYxW";

                    var client = new Baidu.Aip.ImageClassify.ImageClassify(API_KEY, SECRET_KEY);
                    client.Timeout = 60000;
                    var result = client.AdvancedGeneral(image);

                   
                    foreach (var item in result.Last.Last)
                    {
                        string ifHaveBao= item.Last.Last + "";
                        // words = item.Last.Last + "";

                        if (lists == null)
                        {
                        }
                        else
                        {
                            foreach (var listsitem in lists)
                            {
                                if (ifHaveBao == listsitem.商品名)
                                {
                                    goodsname.Text = listsitem.商品名;
                                    words += ifHaveBao + ",";
                                    break;
                                }
                                else if (words.Contains("包"))
                                {
                                    words += ifHaveBao + ",";
                                    break;
                                }
                            }
                        }
                    }
                    // textBox.Text = words;
                }
            }));
            return words;
        }
        #endregion
        
        #region 控件事件
        public void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            startKinect();
            this.bodyFrameReader = this.kinectDevice.BodyFrameSource.OpenReader();
            if (this.bodyFrameReader != null)
            {
                this.bodyFrameReader.FrameArrived += this.Reader_FrameArrived;
            }
        }
        public void MainWindow_Closed(object sender, EventArgs e)
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
        /// 选择视频文件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void SZ_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var openFileDialog = new Microsoft.Win32.OpenFileDialog()
                {
                    Title = "选择视频文件",
                    Filter = "(*.mp4)|*.mp4|(*.avi)|*.avi|(*.wmv)|*.wmv|All files(*.*)|*.*"
                };
                openFileDialog.Multiselect = true;// Multiselect
                var result = openFileDialog.ShowDialog();
                if (result == true)
                {
                    if (openFileDialog.FileNames.Length > 1)
                    {

                  
                        avicb.Items.Clear();
                        filenamex.Clear();
                        for (int i = 0; i < openFileDialog.FileNames.Length; i++)
                        {
                            Users na = new Users();
                                na.序号 = System.IO.Path.GetFileNameWithoutExtension(openFileDialog.FileNames[i]).Split('@').First();
                            na.商品名 = System.IO.Path.GetFileNameWithoutExtension(openFileDialog.FileNames[i]);
                            na.简介 = openFileDialog.FileNames[i];
                            filenamex.Add(na);
                                //fileList.Add(openFileDialog.FileNames[i]);
                                //avicb.Items.Add(System.IO.Path.GetFileNameWithoutExtension(openFileDialog.FileNames[i]));
                        }
                          filenamex.OrderBy(x => x.序号);  //按照sort属性正序排序，   //list.OrderByDescending(x+>x.Name);  //按照Name倒序排列

                        foreach (var item in filenamex)
                        {
                            fileList.Add(item.简介);
                            avicb.Items.Add(item.商品名);
                        }
                        this.avicb.SelectedIndex = 0;
                    }
                    else
                    {
                        avicb.Items.Clear();
                        fileList.Add(openFileDialog.FileName);
                      
                        avicb.Items.Add(System.IO.Path.GetFileNameWithoutExtension(openFileDialog.FileName));
                        //默认选择第一项
                        this.avicb.SelectedIndex = 0;
                    }
                    errorTb.Text = "视频文件加载完成";


                    //mediaElement.Source = new Uri(fileList[0]);
                    //mediaElement.Play(); // 开始播放
                    //mediaElement.Visibility = Visibility.Visible;
                    //ifpaly = true;
                    media_Play(fileList[0]);

                }
            }
            catch (Exception)
            {
                MessageBox.Show("不支持所选取的文件");
            }
        }
        public void vtb_TextChanged(object sender, TextChangedEventArgs e)
        {
        }

        public void window_Loaded(object sender, RoutedEventArgs e)
        {
        }
        /// <summary>
        /// 窗口内选中控件作出反应
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void window_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Tab && avicb.IsFocused)
            {
                avicb.IsDropDownOpen = true;
            }
            if (e.Key == Key.Tab && goodsname.IsFocused)
            {
                goodsname.IsDropDownOpen = true;
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
             //   ObjectRecognition.Text = "由于网络原因，识别较慢，请稍后...";
                MousePoint = e.GetPosition(ColorViewer);//1000.526
                pPoint.X = (int)MousePoint.X * ColorViewer.Source.Width / ColorViewer.Width;
                pPoint.Y = (int)MousePoint.Y * ColorViewer.Source.Height / ColorViewer.Height;
             //   blueE.Visibility = Visibility.Visible;
                goodsname.Visibility = Visibility.Visible;
                this.Dispatcher.BeginInvoke(DispatcherPriority.Background,
                (Action)(() => { Keyboard.Focus(goodsname); })); //Thread.Sleep(100);
                
                if (!backgroundWorker.IsBusy)
                {
                    backgroundWorker.RunWorkerAsync();
                }
                else
                {
                    backgroundWorker.CancelAsync();
                    backgroundWorker.RunWorkerAsync();
                }
               
            }
            catch (Exception ex)
            {
                errorTb.Text = ex.Message;
            }
        }
  
        BackgroundWorker backgroundWorker = new BackgroundWorker();
        private void ViewMessgeBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            //timer.Interval = new TimeSpan(0, 0, 0, durTime); // 设置定时器重复周期
            //timer.Tick += new EventHandler(timerEvent); // 设置定时器事件
            //timer.Start(); // 启动定时器
            //Thread.Sleep(100);
            //MachineAnswer();
        }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                if (voice_flag)//关闭声音获取
                {
                    isr.iatStop();
                }
                micvolume.vol_stop();//停止音量显示
                if (voice_flag)
                {
                    isr.iatStop();
                }//停止语音

                if (_RECOVER != null)//有数据提醒保存
                {
                    MessageBoxResult ifclosemian = MessageBox.Show("是否保存顾客数据至本电脑文本中", "数据保存", MessageBoxButton.YesNoCancel, MessageBoxImage.Question, MessageBoxResult.Yes, MessageBoxOptions.RightAlign);
                    if (MessageBoxResult.Yes == ifclosemian)
                    {
                        var openFileDialog = new Microsoft.Win32.OpenFileDialog()
                        {
                            Title = "选择保存顾客数据的位置文件",
                            Filter = "All files(*.*)|*.*"
                        };
                        var result = openFileDialog.ShowDialog();
                        if (result == true)
                        {
                            WriteFile(openFileDialog.FileName, _RECOVER);
                        }
                    }
                    else if (MessageBoxResult.Cancel == ifclosemian) 
                    {
                        e.Cancel = true;
                    }
                    else
                    {
                    }
                }
            }
            catch (Exception ex)
            {
                errorTb.Text = "无法关闭：" + ex.Message;
            }
        }
        /// <summary>
        /// 导入物品介绍数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void btInput_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var openFileDialog = new Microsoft.Win32.OpenFileDialog()
                {
                    Title = "选择一个EXCEL文件",
                    Filter = "excel files(*.xls)|*.xls|All files(*.*)|*.*"
                };
                var result = openFileDialog.ShowDialog();
                if (result == true)
                {

                    dt = ExcelToTable(openFileDialog.FileName);

                    if (dt.Rows.Count > 0)//try
                    {
                        lists = Ggd(dt);
                        dgExcel.ItemsSource = null;
                        dgExcel.ItemsSource = lists;
                    }
                    errorTb.Text = "表格导入完成";
                }

            }
            catch (Exception ex)
            {
                errorTb.Text = "表格数据第一行不对或不支持所选取的EXCEL版本,建议更换表格或更改文件格式为xls" + ex.Message;
            }
        }
        /// <summary>
        /// 单次识别数量大大于3则显示
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (jiandanRb.IsChecked == true)
                {
                    ifhaveinternect = false;
                }
                else if (zhinengRb.IsChecked == true)
                {
                    ifhaveinternect = true;
                }
                Once = true;
                if (VoicetextBox.Text == "" || VoicetextBox.Text == null)
                {

                }
                else if (VoicetextBox.Text.Length >= 3 && (VoicetextBox.Text.Contains(".") || VoicetextBox.Text.Contains("。") || VoicetextBox.Text.Contains("!")))
                {
                    VoiceText = VoicetextBox.Text;
                    VoicetextBox.Text = "";
                    if (ifhaveinternect)
                    {
                        // MainWin win = new MainWin();
                        bool onoff = true;
                        try
                        {
                            if (voice_flag == true)//关闭语音
                            {
                                onoff = false;
                                voice_flag = false;
                                isr.iatStop();
                            }
                            if (VoiceText != "" || VoiceText != null)
                            {
                                Once = true;

                                this.Dispatcher.BeginInvoke(DispatcherPriority.Send, (ThreadStart)delegate ()
                                {
                                
                                    re1 = VoiceText;
                                    // re1 = Texterrorcorrection(VoicetextBox.Text);
                                    this.bw.RunWorkerAsync();//要补充一个判断，不能同时运行// 
                                }); 
                            }
                        }
                        catch (Exception)
                        {
                        }
                        finally
                        {
                            if (onoff == false)//开语音
                            {
                                voice_flag = true;
                                string c1 = "server_url=dev.voicecloud.cn,appid=54d42c2d ,timeout=10000";
                                string c2 = "sub=iat,ssm=1,auf=audio/L16;rate=16000,aue=speex,ent=sms16k,rst=plain,vad_timeout=30000,vad_speech_tail=500";
                                isr = new iFlyISR(c1, c2);
                                isr.RecRunning += new EventHandler<iFlyISR.DataArrivedEventArgs>(Rec_DataAvailable);
                                isr.DataArrived += new EventHandler<iFlyISR.DataArrivedEventArgs>(Asr_DataAvailable);
                                isr.RecEpStates += new EventHandler<bool>(Voice_control);
                                isr.ISREnd += new EventHandler(Isr_ISREnd);
                                isr.iatStart();
                            }
                          
                            this.Visibility = System.Windows.Visibility.Visible;
                            // win.Close();
                        }
                        errorTb.Text = "就绪";
                    }
                    else//简易
                    {
                        Once = true;
                        re1 = VoiceText;
                        Local_interaction();
                    }
                    vtb.Text = "客户内容: " + VoiceText;
                }
            }
            catch (Exception ex)
            {
                VoicetextBox.Text = "";
                errorTb.Text = ex.Message+"shibie";
            }
        }

        /// <summary>
        /// 语音关闭
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_voiceOFF(object sender, RoutedEventArgs e)
        {
            try
            {
                if (voice_flag)
                {
                    VoiceButtonON.IsEnabled = true;
                    VoiceButtonOFF.IsEnabled = false;
                    voice_flag = false;
                    isr.iatStop();
                    Brush red = new SolidColorBrush(Color.FromArgb(128, 255, 0, 0));
                    StateLight.Fill = red;
                    VoiceButtonON.Visibility = Visibility.Visible;
                    VoiceButtonOFF.Visibility = Visibility.Hidden;
                    errorTb.Text = "语音关闭";
                }
            }
            catch (Exception ex)
            {
              
                errorTb.Text = ex.Message;
            }
        }
        /// <summary>
        /// 语音打开
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void VoiceButtonON_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!voice_flag)
                {
                    VoiceButtonON.IsEnabled = false;
                    VoiceButtonOFF.IsEnabled = true;
                    voice_flag = true;
                    string c1 = "server_url=dev.voicecloud.cn,appid=54d42c2d ,timeout=10000";
                    string c2 = "sub=iat,ssm=1,auf=audio/L16;rate=16000,aue=speex,ent=sms16k,rst=plain,vad_timeout=30000,vad_speech_tail=500";
                    isr = new iFlyISR(c1, c2);
                    isr.RecRunning += new EventHandler<iFlyISR.DataArrivedEventArgs>(Rec_DataAvailable);
                    isr.DataArrived += new EventHandler<iFlyISR.DataArrivedEventArgs>(Asr_DataAvailable);
                    isr.RecEpStates += new EventHandler<bool>(Voice_control);
                    isr.ISREnd += new EventHandler(Isr_ISREnd);
                    isr.iatStart();
                    Brush green = new SolidColorBrush(Color.FromArgb(128, 0, 255, 0));
                    StateLight.Fill = green;
                    VoiceButtonON.Visibility = Visibility.Hidden;
                    VoiceButtonOFF.Visibility = Visibility.Visible;
                    errorTb.Text = "语音以打开";
                }
            }
            catch (Exception ex)
            {
            
                errorTb.Text = ex.Message;
            }
        }
        #endregion

        private void voiceInteractionViewBT_Click(object sender, RoutedEventArgs e)
        {
            ViewMessgeBox.Visibility = Visibility.Visible;


            mainGr.Visibility = Visibility.Visible;
            dataGRID.Visibility = Visibility.Hidden;
            KinectGrid.Visibility = Visibility.Hidden;

            voiceInteractionViewBT.Background = Brushes.White;
            dataInputBt.Background = null;
            KinectImageBt.Background = null;
        }

        private void dataInputBt_Click(object sender, RoutedEventArgs e)
        {

            ViewMessgeBox.Visibility = Visibility.Hidden;

            mainGr.Visibility = Visibility.Hidden;

            KinectGrid.Visibility = Visibility.Hidden;
            dataGRID.Visibility = Visibility.Visible;


            voiceInteractionViewBT.Background = null;
            dataInputBt.Background = Brushes.White;
            KinectImageBt.Background = null;
        }

        private void KinectImageBt_Click(object sender, RoutedEventArgs e)
        {
            ViewMessgeBox.Visibility = Visibility.Hidden;

            mainGr.Visibility = Visibility.Hidden;
            KinectGrid.Visibility = Visibility.Visible;
            dataGRID.Visibility = Visibility.Hidden;

            voiceInteractionViewBT.Background = null;
            dataInputBt.Background = null;
            KinectImageBt.Background = Brushes.White;
            
        }
    }
    public class Users
    {
        public string 序号 { get; set; }  //id
        public string 商品名 { get; set; }  //名字
        public string 简介 { get; set; }   //钱数
    }
}
