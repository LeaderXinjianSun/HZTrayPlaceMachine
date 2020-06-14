using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BingLibrary.hjb.Metro;
using System.IO;
using HalconDotNet;
using Newtonsoft.Json;
using BingLibrary.HVision;
using BingLibrary.hjb.file;
using ViewROI;
using System.Diagnostics;
using System.Collections.ObjectModel;

namespace HZTrayPlaceMachine.ViewModels
{
    class MainWindowViewModel : NotificationObject
    {
        #region 属性绑定
        private string windowTitle;

        public string WindowTitle
        {
            get { return windowTitle; }
            set
            {
                windowTitle = value;
                this.RaisePropertyChanged("WindowTitle");
            }
        }
        private string loginMenuItemHeader;

        public string LoginMenuItemHeader
        {
            get { return loginMenuItemHeader; }
            set
            {
                loginMenuItemHeader = value;
                this.RaisePropertyChanged("LoginMenuItemHeader");
            }
        }
        private bool isLogin;

        public bool IsLogin
        {
            get { return isLogin; }
            set
            {
                isLogin = value;
                this.RaisePropertyChanged("IsLogin");
            }
        }
        private string halconWindowVisibility;

        public string HalconWindowVisibility
        {
            get { return halconWindowVisibility; }
            set
            {
                halconWindowVisibility = value;
                this.RaisePropertyChanged("HalconWindowVisibility");
            }
        }
        private string messageStr;

        public string MessageStr
        {
            get { return messageStr; }
            set
            {
                messageStr = value;
                this.RaisePropertyChanged("MessageStr");
            }
        }
        private long cycle;

        public long Cycle
        {
            get { return cycle; }
            set
            {
                cycle = value;
                this.RaisePropertyChanged("Cycle");
            }
        }
        private bool satusCamera;

        public bool StatusCamera
        {
            get { return satusCamera; }
            set
            {
                satusCamera = value;
                this.RaisePropertyChanged("StatusCamera");
            }
        }
        private bool satusRobot;

        public bool StatusRobot
        {
            get { return satusRobot; }
            set
            {
                satusRobot = value;
                this.RaisePropertyChanged("StatusRobot");
            }
        }
        
        private HImage cameraIamge;

        public HImage CameraIamge
        {
            get { return cameraIamge; }
            set
            {
                cameraIamge = value;
                this.RaisePropertyChanged("CameraIamge");
            }
        }
        private bool statusPLC;

        public bool StatusPLC
        {
            get { return statusPLC; }
            set
            {
                statusPLC = value;
                this.RaisePropertyChanged("StatusPLC");
            }
        }

        private HObject cameraAppendHObject;

        public HObject CameraAppendHObject
        {
            get { return cameraAppendHObject; }
            set
            {
                cameraAppendHObject = value;
                this.RaisePropertyChanged("CameraAppendHObject");
            }
        }
        private Tuple<string, object> cameraGCStyle;

        public Tuple<string, object> CameraGCStyle
        {
            get { return cameraGCStyle; }
            set
            {
                cameraGCStyle = value;
                this.RaisePropertyChanged("CameraGCStyle");
            }
        }
        private double cameraX;

        public double CameraX
        {
            get { return cameraX; }
            set
            {
                cameraX = value;
                this.RaisePropertyChanged("CameraX");
            }
        }
        private double cameraY;

        public double CameraY
        {
            get { return cameraY; }
            set
            {
                cameraY = value;
                this.RaisePropertyChanged("CameraY");
            }
        }
        private double cameraU;

        public double CameraU
        {
            get { return cameraU; }
            set
            {
                cameraU = value;
                this.RaisePropertyChanged("CameraU");
            }
        }
        
        private bool onlyImage;

        public bool OnlyImage
        {
            get { return onlyImage; }
            set
            {
                onlyImage = value;
                this.RaisePropertyChanged("OnlyImage");
            }
        }
        private ObservableCollection<Point> points;

        public ObservableCollection<Point> Points
        {
            get { return points; }
            set
            {
                points = value;
                this.RaisePropertyChanged("Points");
            }
        }
        private string homePageVisibility;

        public string HomePageVisibility
        {
            get { return homePageVisibility; }
            set
            {
                homePageVisibility = value;
                this.RaisePropertyChanged("HomePageVisibility");
            }
        }
        private string pointsPageVisibility;

        public string PointsPageVisibility
        {
            get { return pointsPageVisibility; }
            set
            {
                pointsPageVisibility = value;
                this.RaisePropertyChanged("PointsPageVisibility");
            }
        }

        #endregion
        #region 方法绑定
        public DelegateCommand AppLoadedEventCommand { get; set; }
        public DelegateCommand AppClosedEventCommand { get; set; }
        public DelegateCommand<object> MenuActionCommand { get; set; }
        public DelegateCommand LoginCommand { get; set; }
        public DelegateCommand GrabCommand { get; set; }
        public DelegateCommand<object> SelectIndexCommand { get; set; }
        public DelegateCommand SaveCommand { get; set; }
        public DelegateCommand ReadImageCommand { get; set; }
        public DelegateCommand ShapeModelCommand { get; set; }
        public DelegateCommand LineCommand { get; set; }
        public DelegateCommand RegionCommand { get; set; }
        public DelegateCommand RecognizeCommand { get; set; }
        public DelegateCommand CalibCommand { get; set; }
        public DelegateCommand UpdatePointsCommand { get; set; }
        #endregion
        #region 变量
        Metro metro = new Metro();
        int SelectIndexValue;
        //Point CameraP1, CameraP2, CameraP3, CameraP4;
        //Point TargetP1, TargetP2, TargetP3, TargetP4;
        CameraOperate cameraOperate = new CameraOperate();
        private string iniParameterPath = Path.Combine(System.Environment.CurrentDirectory, "Parameter.ini");
        DXH.Modbus.DXHModbusTCP ModbusTCP_Client;
        DXH.Robot.DXHYAMAHALink Robot_1_Link;
        bool HasStartCalib = false;
        int[] Robot_1_In = new int[24];
        int[] Robot_1_Out = new int[24];
        bool isUpdatePoint = false;
        #endregion
        #region 构造函数
        public MainWindowViewModel()
        {
            AppLoadedEventCommand = new DelegateCommand(new Action(this.AppLoadedEventCommandExecute));
            AppClosedEventCommand = new DelegateCommand(new Action(this.AppClosedEventCommandExecute));
            MenuActionCommand = new DelegateCommand<object>(new Action<object>(this.MenuActionCommandExecute));
            LoginCommand = new DelegateCommand(new Action(this.LoginCommandExecute));
            GrabCommand = new DelegateCommand(new Action(this.GrabCommandExecute));
            SelectIndexCommand = new DelegateCommand<object>(new Action<object>(this.SelectIndexCommandExecute));
            SaveCommand = new DelegateCommand(new Action(this.SaveCommandExecute));
            ReadImageCommand = new DelegateCommand(new Action(this.ReadImageCommandExecute));
            ShapeModelCommand = new DelegateCommand(new Action(this.ShapeModelCommandExecute));
            LineCommand = new DelegateCommand(new Action(this.LineCommandExecute));
            RegionCommand = new DelegateCommand(new Action(this.RegionCommandExecute));
            RecognizeCommand = new DelegateCommand(new Action(this.RecognizeCommandExecute));
            CalibCommand = new DelegateCommand(new Action(this.CalibCommandExecute));
            UpdatePointsCommand = new DelegateCommand(new Action(this.UpdatePointsCommandExecute));
            Init();
        }
        #endregion
        #region 方法绑定函数
        private void AppLoadedEventCommandExecute()
        {
            UIRun();
            Task.Run(()=> {
                string CameraName = Inifile.INIGetStringValue(iniParameterPath, "Camera", "Name", "CAM1");
                if (cameraOperate.OpenCamera(CameraName, "GigEVision"))
                {
                    AddMessage("Camera Open Success!");
                    int ExposureValue = int.Parse(Inifile.INIGetStringValue(iniParameterPath, "Camera", "ExposureValue", "3500"));
                    cameraOperate.SetExpose(ExposureValue);
                    if (cameraOperate.GrabImage(0))
                    {
                        AddMessage("Camera拍照成功");
                        CameraIamge = cameraOperate.CurrentImage;
                    }
                }
                else
                {
                    AddMessage("Camera Open Fail!");
                }
            });

            ModbusTCP_Client.StartConnect();
            StartReadPLC();
            Robot_1_Link.StartTCPConnect();
            GetRobot_1_Status();
        }
        private void AppClosedEventCommandExecute()
        {
            cameraOperate.CloseCamera();
            try
            {
                ModbusTCP_Client.Close();
                Robot_1_Link.Close();
            }
            catch 
            {

            }
        }
        private void MenuActionCommandExecute(object p)
        {
            switch (p.ToString())
            {
                case "1":
                    HomePageVisibility = "Collapsed";
                    PointsPageVisibility = "Visible";
                    break;
                case "0":
                default:
                    HomePageVisibility = "Visible";
                    PointsPageVisibility = "Collapsed";
                    break;
            }
        }
        private async void LoginCommandExecute()
        {
            if (IsLogin)
            {
                IsLogin = false;
                LoginMenuItemHeader = "登录";
                AddMessage("已登出");
            }
            else
            {
                metro.ChangeAccent("Light.Red");
                HalconWindowVisibility = "Collapsed";
                //var r = await metro.ShowLoginOnlyPassword("请登录");
                string r = await metro.ShowLoginOnlyPassword("密码");
                if (r == GetPassWord())
                {
                    IsLogin = true;
                    LoginMenuItemHeader = "登出";
                }
                else
                {
                    AddMessage("密码错误");
                }
                HalconWindowVisibility = "Visible";
                metro.ChangeAccent("Light.Blue");
            }
        }
        private void SelectIndexCommandExecute(object p)
        {
            switch (p.ToString())
            {
                case "0":
                    SelectIndexValue = 0;
                    CameraX = Points[0].X;
                    CameraY = Points[0].Y;
                    CameraU = Points[0].U;
                    AddMessage("选择1号产品参数");
                    break;
                case "1":
                    SelectIndexValue = 1;
                    CameraX = Points[1].X;
                    CameraY = Points[1].Y;
                    CameraU = Points[1].U;
                    AddMessage("选择2号产品参数");
                    break;
                case "2":
                    SelectIndexValue = 2;
                    CameraX = Points[2].X;
                    CameraY = Points[2].Y;
                    CameraU = Points[2].U;          
                    AddMessage("选择3号产品参数");
                    break;
                case "3":
                    SelectIndexValue = 3;
                    CameraX = Points[3].X;
                    CameraY = Points[3].Y;
                    CameraU = Points[3].U;
                    AddMessage("选择4号产品参数");
                    break;
                default:
                    break;
            }
        }
        private void SaveCommandExecute()
        {
            string path = "";
            switch (SelectIndexValue)
            {
                case 0:
                    path = Path.Combine(System.Environment.CurrentDirectory, @"Camera\1");
                    break;
                case 1:
                    path = Path.Combine(System.Environment.CurrentDirectory, @"Camera\2");
                    break;
                case 2:
                    path = Path.Combine(System.Environment.CurrentDirectory, @"Camera\3");
                    break;
                case 3:
                    path = Path.Combine(System.Environment.CurrentDirectory, @"Camera\4");
                    break;
                default:
                    break;
            }
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            WriteToJson(Points, Path.Combine(System.Environment.CurrentDirectory, @"Camera", "Points.json"));


        }
        private void GrabCommandExecute()
        {
            try
            {
                cameraOperate.GrabImageVoid(0);
                CameraIamge = cameraOperate.CurrentImage;
            }
            catch (Exception ex)
            {
                AddMessage(ex.Message);
            }
        }
        private void ReadImageCommandExecute()
        {
            System.Windows.Forms.OpenFileDialog ofd = new System.Windows.Forms.OpenFileDialog();
            ofd.Filter = "Image文件(*.bmp;*.jpg)|*.bmp;*.jpg|所有文件|*.*";
            ofd.ValidateNames = true;
            ofd.CheckPathExists = true;
            ofd.CheckFileExists = true;
            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string strFileName = ofd.FileName;
                HObject image;
                HOperatorSet.ReadImage(out image, strFileName);
                CameraIamge = new HImage(image);
            }
        }
        private async void ShapeModelCommandExecute()
        {
            metro.ChangeAccent("Light.Red");
            HalconWindowVisibility = "Collapsed";
            bool r = await metro.ShowConfirm("确认","确认要重新画模板吗？");
            if (r)
            {
                string path = "";
                switch (SelectIndexValue)
                {
                    case 0:
                        path = Path.Combine(System.Environment.CurrentDirectory, @"Camera\1");
                        break;
                    case 1:
                        path = Path.Combine(System.Environment.CurrentDirectory, @"Camera\2");
                        break;
                    case 2:
                        path = Path.Combine(System.Environment.CurrentDirectory, @"Camera\3");
                        break;
                    case 3:
                        path = Path.Combine(System.Environment.CurrentDirectory, @"Camera\4");
                        break;
                    default:
                        break;
                }
                metro.ChangeAccent("Light.Blue");
                HalconWindowVisibility = "Visible";
                CameraAppendHObject = null;
                CameraGCStyle = new Tuple<string, object>("Color", "green");
                ROI roi = Global.CameraImageViewer.DrawROI(ROI.ROI_TYPE_REGION);
                HObject ReduceDomainImage;
                HOperatorSet.ReduceDomain(CameraIamge, roi.getRegion(), out ReduceDomainImage);
                HObject modelImages, modelRegions;
                HOperatorSet.InspectShapeModel(ReduceDomainImage, out modelImages, out modelRegions, 7, 30);
                HObject objectSelected;
                HOperatorSet.SelectObj(modelRegions, out objectSelected, 1);
                CameraAppendHObject = objectSelected;
                HOperatorSet.WriteRegion(objectSelected, Path.Combine(path, "ModelRegion.hobj"));
                HTuple ModelID;
                HOperatorSet.CreateShapeModel(ReduceDomainImage, 7, (new HTuple(-45)).TupleRad(), (new HTuple(90)).TupleRad(), (new HTuple(0.1)).TupleRad(), "no_pregeneration", "use_polarity", 30, 10, out ModelID);
                HOperatorSet.WriteShapeModel(ModelID, Path.Combine(path, "ShapeModel.shm"));
                CameraIamge.WriteImage("bmp", 0, Path.Combine(path, "ModelImage.bmp"));
                AddMessage("创建模板完成");
            }
            else
            {
                metro.ChangeAccent("Light.Blue");
                HalconWindowVisibility = "Visible";
            }
        }
        private async void LineCommandExecute()
        {
            metro.ChangeAccent("Light.Red");
            HalconWindowVisibility = "Collapsed";
            bool r = await metro.ShowConfirm("确认", "确认要重新画直线吗？");
            if (r)
            {
                string path = "";
                switch (SelectIndexValue)
                {
                    case 0:
                        path = Path.Combine(System.Environment.CurrentDirectory, @"Camera\1");
                        break;
                    case 1:
                        path = Path.Combine(System.Environment.CurrentDirectory, @"Camera\2");
                        break;
                    case 2:
                        path = Path.Combine(System.Environment.CurrentDirectory, @"Camera\3");
                        break;
                    case 3:
                        path = Path.Combine(System.Environment.CurrentDirectory, @"Camera\4");
                        break;
                    default:
                        break;
                }
                metro.ChangeAccent("Light.Blue");
                HalconWindowVisibility = "Visible";
                CameraAppendHObject = null;
                CameraGCStyle = new Tuple<string, object>("Color", "red");
                ROI roi = Global.CameraImageViewer.DrawROI(ROI.ROI_TYPE_RECTANGLE2);
                CameraAppendHObject = roi.getRegion();
                HOperatorSet.WriteRegion(roi.getRegion(), Path.Combine(path, "Line.hobj"));
                AddMessage("画直线完成");

            }
            else
            {
                metro.ChangeAccent("Light.Blue");
                HalconWindowVisibility = "Visible";
            }
        }
        private async void RegionCommandExecute()
        {
            metro.ChangeAccent("Light.Red");
            HalconWindowVisibility = "Collapsed";
            bool r = await metro.ShowConfirm("确认", "确认要重新画区域吗？");
            if (r)
            {
                string path = "";
                switch (SelectIndexValue)
                {
                    case 0:
                        path = Path.Combine(System.Environment.CurrentDirectory, @"Camera\1");
                        break;
                    case 1:
                        path = Path.Combine(System.Environment.CurrentDirectory, @"Camera\2");
                        break;
                    case 2:
                        path = Path.Combine(System.Environment.CurrentDirectory, @"Camera\3");
                        break;
                    case 3:
                        path = Path.Combine(System.Environment.CurrentDirectory, @"Camera\4");
                        break;
                    default:
                        break;
                }
                metro.ChangeAccent("Light.Blue");
                HalconWindowVisibility = "Visible";
                CameraAppendHObject = null;
                CameraGCStyle = new Tuple<string, object>("Color", "red");
                ROI roi = Global.CameraImageViewer.DrawROI(ROI.ROI_TYPE_RECTANGLE1);
                CameraAppendHObject = roi.getRegion();
                HOperatorSet.WriteRegion(roi.getRegion(), Path.Combine(path, "Region.hobj"));
                AddMessage("画区域完成");

            }
            else
            {
                metro.ChangeAccent("Light.Blue");
                HalconWindowVisibility = "Visible";
            }
        }
        private void RecognizeCommandExecute()
        {
            var calcrst = RecognizeOperete(SelectIndexValue,CameraIamge);
            AddMessage(calcrst.Item1[0].ToString("F2") + "," + calcrst.Item1[1].ToString("F2") + "," + calcrst.Item1[2].ToString("F2"));
        }
        private async void CalibCommandExecute()
        {
            metro.ChangeAccent("Light.Red");
            HalconWindowVisibility = "Collapsed";
            bool r = await metro.ShowConfirm("确认", "确认要重新标定吗？");
            if (r)
            {
                metro.ChangeAccent("Light.Blue");
                HalconWindowVisibility = "Visible";
                if (!HasStartCalib)
                {
                    HasStartCalib = true;
                    try
                    {
                        AddMessage("开始标定");
                        string path = "";
                        switch (SelectIndexValue)
                        {
                            case 0:
                                path = Path.Combine(System.Environment.CurrentDirectory, @"Camera\1");
                                break;
                            case 1:
                                path = Path.Combine(System.Environment.CurrentDirectory, @"Camera\2");
                                break;
                            case 2:
                                path = Path.Combine(System.Environment.CurrentDirectory, @"Camera\3");
                                break;
                            case 3:
                                path = Path.Combine(System.Environment.CurrentDirectory, @"Camera\4");
                                break;
                            default:
                                break;
                        }

                        DXH.Net.DXHTCPClient Calib_Link = new DXH.Net.DXHTCPClient();
                        string Calib_IPAddress = Inifile.INIGetStringValue(iniParameterPath, "System", "CalibIP", "192.168.1.13");
                        int Calib_IPPort = int.Parse(Inifile.INIGetStringValue(iniParameterPath, "System", "CalibPORT", "8000"));
                        Calib_Link.RemoteIPAddress = Calib_IPAddress;
                        Calib_Link.RemoteIPPort = Calib_IPPort;
                        if (!OnlyImage)
                        {

                            if (Calib_Link.ConnectState != "Connected")
                                Calib_Link.StartTCPConnect();

                            if (Calib_Link.ConnectState != "Connected")
                            {
                                AddMessage("标定程序未连接.");
                                await Task.Delay(1000);
                            }
                            if (Calib_Link.ConnectState != "Connected")
                            {
                                AddMessage("标定程序未连接..");
                                await Task.Delay(1000);
                            }
                            if (Calib_Link.ConnectState != "Connected")
                            {
                                AddMessage("标定程序未连接...");
                                await Task.Delay(1000);
                            }
                            if (Calib_Link.ConnectState != "Connected")
                            {
                                AddMessage("标定程序未连接....");
                                await Task.Delay(1000);
                            }
                            if (Calib_Link.ConnectState != "Connected")
                            {
                                AddMessage("标定程序未连接.....");
                                AddMessage("标定流程退出");
                                HasStartCalib = false;
                                return;
                            }

                            AddMessage("标定程序连接成功");


                            string str = Calib_Link.TCPSend("START\r\n");
                            AddMessage(str);
                            if (str != "OK\r\n")
                            {
                                AddMessage("连接机械手失败!");
                                HasStartCalib = false;
                                return;
                            }
                        }
                        

                        int[][] diff = new int[9][] {
                    new int[] { 0,0,0},
                    new int[] { -10,-10,0},
                    new int[] { 0,-10,0},
                    new int[] { 10,-10,0},
                    new int[] { 10,0,0},
                    new int[] { 10,10,0},
                    new int[] { 0,10,0},
                    new int[] { -10,10,0},
                    new int[] { -10,0,0}
                };
                        double[][] diff_r = new double[3][] {
                    new double[] { 0,0,0},
                    new double[] { 0,0,15},
                    new double[] { 0,0, -15 }
                };
                        if (!OnlyImage)
                        {
                            for (int i = 0; i < 9; i++)
                            {
                                await Task.Run(() => {
                                    string mMotionStr = Calib_Link.TCPSend(diff[i][0] + "," + diff[i][1] + "," + diff[i][2] + "\r\n", true, 3000);
                                    AddMessage(diff[i][0] + "," + diff[i][1] + "," + diff[i][2]);
                                    AddMessage(mMotionStr);
                                });
                                await Task.Delay(3000);

                                cameraOperate.GrabImageVoid(0);
                                CameraIamge = cameraOperate.CurrentImage;

                                if (!Directory.Exists(Path.Combine(path, "Calib")))
                                {
                                    Directory.CreateDirectory(Path.Combine(path, "Calib"));
                                }
                                cameraOperate.SaveImage("bmp", Path.Combine(path, "Calib", (i + 1).ToString() + ".bmp"));



                            }
                        }

                        double[][] Array1 = new double[9][];
                        for (int i = 0; i < 9; i++)
                        {
                            try
                            {
                                HObject img;
                                HOperatorSet.ReadImage(out img, Path.Combine(path, "Calib", (i + 1).ToString() + ".bmp"));
                                HTuple ModelID, row, column, angle, score;
                                HOperatorSet.ReadShapeModel(Path.Combine(path, "ShapeModel.shm"), out ModelID);
                                HOperatorSet.FindShapeModel(img, ModelID, (new HTuple(-45)).TupleRad(), (new HTuple(90)).TupleRad(), 0.5, 1, 0, "least_squares", 0, 0.9, out row, out column, out angle, out score);

                                Array1[i] = new double[4] { row.D, column.D, Points[0].X + diff[i][0], Points[0].Y + diff[i][1] };
                            }
                            catch (Exception ex)
                            {
                                Array1[i] = new double[4] { 0, 0, Points[0].X + diff[i][0], Points[0].Y + diff[i][1] };
                                AddMessage(ex.Message);
                            }
                        }
                        HTuple homMat2D = new HTuple();
                        try
                        {
                            HOperatorSet.VectorToHomMat2d(new HTuple(Array1[0][0]).TupleConcat(Array1[1][0]).TupleConcat(Array1[2][0]).TupleConcat(Array1[3][0]).TupleConcat(Array1[4][0]).TupleConcat(Array1[5][0]).TupleConcat(Array1[6][0]).TupleConcat(Array1[7][0]).TupleConcat(Array1[8][0]),
        new HTuple(Array1[0][1]).TupleConcat(Array1[1][1]).TupleConcat(Array1[2][1]).TupleConcat(Array1[3][1]).TupleConcat(Array1[4][1]).TupleConcat(Array1[5][1]).TupleConcat(Array1[6][1]).TupleConcat(Array1[7][1]).TupleConcat(Array1[8][1]),
        new HTuple(Array1[0][2]).TupleConcat(Array1[1][2]).TupleConcat(Array1[2][2]).TupleConcat(Array1[3][2]).TupleConcat(Array1[4][2]).TupleConcat(Array1[5][2]).TupleConcat(Array1[6][2]).TupleConcat(Array1[7][2]).TupleConcat(Array1[8][2]),
        new HTuple(Array1[0][3]).TupleConcat(Array1[1][3]).TupleConcat(Array1[2][3]).TupleConcat(Array1[3][3]).TupleConcat(Array1[4][3]).TupleConcat(Array1[5][3]).TupleConcat(Array1[6][3]).TupleConcat(Array1[7][3]).TupleConcat(Array1[8][3])
        , out homMat2D);
                        }
                        catch (Exception ex)
                        {
                            AddMessage(ex.Message);
                        }

                        //旋转标定拍照
                        if (!OnlyImage)
                        {
                            for (int i = 0; i < 3; i++)
                            {
                                await Task.Run(() => {
                                    string mMotionStr = Calib_Link.TCPSend(diff_r[i][0] + "," + diff_r[i][1] + "," + diff_r[i][2] + "\r\n", true, 3000);
                                    AddMessage(diff_r[i][0] + "," + diff_r[i][1] + "," + diff_r[i][2]);
                                    AddMessage(mMotionStr);
                                });
                                await Task.Delay(3000);

                                cameraOperate.GrabImageVoid(0);
                                CameraIamge = cameraOperate.CurrentImage;

                                if (!Directory.Exists(Path.Combine(path, "Calib")))
                                {
                                    Directory.CreateDirectory(Path.Combine(path, "Calib"));
                                }
                                cameraOperate.SaveImage("bmp", Path.Combine(path, "Calib", (i + 1 + 9).ToString() + ".bmp"));
                            }
                            Calib_Link.TCPSend("FINISH\r\n", false, 3000);
                        }

                        double[][] Array2 = new double[3][];
                        for (int i = 0; i < 3; i++)
                        {
                            try
                            {
                                HObject img;
                                HOperatorSet.ReadImage(out img, Path.Combine(path, "Calib", (i + 1 + 9).ToString() + ".bmp"));
                                HTuple ModelID, row, column, angle, score;
                                HOperatorSet.ReadShapeModel(Path.Combine(path, "ShapeModel.shm"), out ModelID);
                                HOperatorSet.FindShapeModel(img, ModelID, (new HTuple(-45)).TupleRad(), (new HTuple(90)).TupleRad(), 0.5, 1, 0, "least_squares", 0, 0.9, out row, out column, out angle, out score);
                                Array2[i] = new double[2] { row.D, column.D };
                            }
                            catch (Exception ex)
                            {
                                Array2[i] = new double[2] { 0, 0 };
                                AddMessage(ex.Message);
                            }
                        }
                        double[] circleCenter = rotateCenter(Array2[0][0], Array2[0][1], Array2[1][0], Array2[1][1], Array2[2][0], Array2[2][1]);

                        try
                        {
                            HTuple qx0, qy0;
                            HOperatorSet.AffineTransPoint2d(homMat2D, circleCenter[0], circleCenter[1], out qx0, out qy0);
                            double delta_x = Points[0].X - qx0;
                            double delta_y = Points[0].Y - qy0;
                            AddMessage(delta_x.ToString() + " , " + delta_y.ToString());
                            HOperatorSet.VectorToHomMat2d(new HTuple(Array1[0][0]).TupleConcat(Array1[1][0]).TupleConcat(Array1[2][0]).TupleConcat(Array1[3][0]).TupleConcat(Array1[4][0]).TupleConcat(Array1[5][0]).TupleConcat(Array1[6][0]).TupleConcat(Array1[7][0]).TupleConcat(Array1[8][0]),
                                new HTuple(Array1[0][1]).TupleConcat(Array1[1][1]).TupleConcat(Array1[2][1]).TupleConcat(Array1[3][1]).TupleConcat(Array1[4][1]).TupleConcat(Array1[5][1]).TupleConcat(Array1[6][1]).TupleConcat(Array1[7][1]).TupleConcat(Array1[8][1]),
                                new HTuple(Array1[0][2] + delta_x).TupleConcat(Array1[1][2] + delta_x).TupleConcat(Array1[2][2] + delta_x).TupleConcat(Array1[3][2] + delta_x).TupleConcat(Array1[4][2] + delta_x).TupleConcat(Array1[5][2] + delta_x).TupleConcat(Array1[6][2] + delta_x).TupleConcat(Array1[7][2] + delta_x).TupleConcat(Array1[8][2] + delta_x),
                                new HTuple(Array1[0][3] + delta_y).TupleConcat(Array1[1][3] + delta_y).TupleConcat(Array1[2][3] + delta_y).TupleConcat(Array1[3][3] + delta_y).TupleConcat(Array1[4][3] + delta_y).TupleConcat(Array1[5][3] + delta_y).TupleConcat(Array1[6][3] + delta_y).TupleConcat(Array1[7][3] + delta_y).TupleConcat(Array1[8][3] + delta_y)
                                , out homMat2D);
                            HOperatorSet.WriteTuple(homMat2D, Path.Combine(path, "homMat2D.tup"));
                            AddMessage("保存标定文件成功");
                        }
                        catch (Exception ex)
                        {
                            AddMessage(ex.Message);
                        }

                        HOperatorSet.SetColor(Global.CameraImageViewer.viewController.viewPort.HalconWindow, "green");
                        for (int i = 0; i < 9; i++)
                        {
                            HOperatorSet.DispCross(Global.CameraImageViewer.viewController.viewPort.HalconWindow, Array1[i][0], Array1[i][1], 120, 0);
                        }
                        for (int i = 0; i < 3; i++)
                        {
                            HOperatorSet.DispCross(Global.CameraImageViewer.viewController.viewPort.HalconWindow, Array2[i][0], Array2[i][1], 120, 0);
                        }

                        AddMessage("标定结束");
                        HasStartCalib = false;
                        Calib_Link.Close();
                    }
                    catch (Exception ex)
                    {
                        AddMessage(ex.Message);
                        HasStartCalib = false;
                    }
                }
            }
            else
            {
                metro.ChangeAccent("Light.Blue");
                HalconWindowVisibility = "Visible";
            }      
        }
        private void UpdatePointsCommandExecute()
        {
            if (!isUpdatePoint)
            {
                isUpdatePoint = true;
            }
            //Points[5].X = DateTime.Now.Second;
        }
        #endregion
        #region 自定义函数
        private void Init()
        {
            
            string YAMAHA_IPAddress = Inifile.INIGetStringValue(iniParameterPath, "System", "YAMAHAIP", "192.168.1.13");
            

            Robot_1_Link = new DXH.Robot.DXHYAMAHALink(YAMAHA_IPAddress);
            Robot_1_Link.ConnectStateChanged += Robot_1_Link_ConnectStateChanged;

            ModbusTCP_Client = new DXH.Modbus.DXHModbusTCP();
            ModbusTCP_Client.RemoteIPAddress = Inifile.INIGetStringValue(iniParameterPath, "System", "MODBUSIP", "192.168.1.13");
            ModbusTCP_Client.RemoteIPPort = 502;
            ModbusTCP_Client.ModbusStateChanged += ModbusTCP_Client_ModbusStateChanged;
            ModbusTCP_Client.ConnectStateChanged += ModbusTCP_Client_ConnectStateChanged;

            WindowTitle = "HZTrayPlaceMachine20200614";
            IsLogin = false;
            LoginMenuItemHeader = "登录";
            MessageStr = "";
            OnlyImage = true;
            HomePageVisibility = "Visible";
            PointsPageVisibility = "Collapsed";
            try
            {
                using (StreamReader reader = new StreamReader(Path.Combine(System.Environment.CurrentDirectory, @"Camera", "Points.json")))
                {
                    string json = reader.ReadToEnd();
                    Points = JsonConvert.DeserializeObject<ObservableCollection<Point>>(json);
                }
            }
            catch (Exception ex)
            {
                Points = new ObservableCollection<Point>();
                for (int i = 0; i < 16; i++)
                {
                    Points.Add(new Point());
                }
                AddMessage(ex.Message);
            }
            
            SelectIndexValue = 0;
            CameraX = Points[0].X;
            CameraY = Points[0].Y;
            CameraU = Points[0].U;  
        }
        private string GetPassWord()
        {
            int day = System.DateTime.Now.Day;
            int month = System.DateTime.Now.Month;
            string ss = (day + month).ToString();
            string passwordstr = "";
            for (int i = 0; i < 4 - ss.Length; i++)
            {
                passwordstr += "0";
            }
            passwordstr += ss;
            return passwordstr;
        }
        private void AddMessage(string str)
        {
            string[] s = MessageStr.Split('\n');
            if (s.Length > 1000)
            {
                MessageStr = "";
            }
            if (MessageStr != "")
            {
                MessageStr += "\n";
            }
            RunLog(str);
            MessageStr += System.DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + " " + str;
        }
        void RunLog(string str)
        {
            try
            {
                string tempSaveFilee5 = System.AppDomain.CurrentDomain.BaseDirectory + @"RunLog";
                DateTime dtim = DateTime.Now;
                string DateNow = dtim.ToString("yyyy/MM/dd");
                string TimeNow = dtim.ToString("HH:mm:ss");

                if (!Directory.Exists(tempSaveFilee5))
                {
                    Directory.CreateDirectory(tempSaveFilee5);  //创建目录 
                }

                if (File.Exists(tempSaveFilee5 + "\\" + DateNow.Replace("/", "") + ".txt"))
                {
                    //第一种方法：
                    FileStream fs = new FileStream(tempSaveFilee5 + "\\" + DateNow.Replace("/", "") + ".txt", FileMode.Append);
                    StreamWriter sw = new StreamWriter(fs);
                    sw.WriteLine("TTIME：" + TimeNow + " 执行事件：" + str);
                    sw.Dispose();
                    fs.Dispose();
                    sw.Close();
                    fs.Close();
                }
                else
                {
                    //不存在就新建一个文本文件,并写入一些内容 
                    StreamWriter sw;
                    sw = File.CreateText(tempSaveFilee5 + "\\" + DateNow.Replace("/", "") + ".txt");
                    sw.WriteLine("TTIME：" + TimeNow + " 执行事件：" + str);
                    sw.Dispose();
                    sw.Close();
                }
            }
            catch { }
        }
        private void WriteToJson(object p, string path)
        {
            try
            {
                using (FileStream fs = File.Open(path, FileMode.Create))
                using (StreamWriter sw = new StreamWriter(fs))
                using (JsonWriter jw = new JsonTextWriter(sw))
                {
                    jw.Formatting = Formatting.Indented;
                    JsonSerializer serializer = new JsonSerializer();
                    serializer.Serialize(jw, p);
                }
            }
            catch (Exception ex)
            {
                AddMessage(ex.Message);
            }
        }
        private async void UIRun()
        {
            while (true)
            {
                await Task.Delay(200);
                StatusCamera = cameraOperate.Connected;
            }
        }
        private Tuple<double[], double[], double[], bool> RecognizeOperete(int index,HImage image)
        {
            string path = "";
            Point camerap, targetp1, targetp2, targetp3;
            switch (index)
            {
                case 1:
                    path = Path.Combine(System.Environment.CurrentDirectory, @"Camera\2");
                    camerap = Points[1];
                    targetp1 = Points[4]; targetp2 = Points[5]; targetp3 = Points[6];
                    break;
                case 2:
                    path = Path.Combine(System.Environment.CurrentDirectory, @"Camera\3");
                    camerap = Points[2];
                    targetp1 = Points[7]; targetp2 = Points[8]; targetp3 = Points[9];
                    break;
                case 3:
                    path = Path.Combine(System.Environment.CurrentDirectory, @"Camera\4");
                    camerap = Points[3];
                    targetp1 = Points[10]; targetp2 = Points[11]; targetp3 = Points[12];
                    break;
                case 0:
                default:
                    path = Path.Combine(System.Environment.CurrentDirectory, @"Camera\1");
                    camerap = Points[0];
                    targetp1 = Points[13]; targetp2 = Points[14]; targetp3 = Points[15];
                    break;
            }
            try
            {
                #region 找模板
                HObject ModelImage;
                HOperatorSet.ReadImage(out ModelImage, Path.Combine(path, "ModelImage.bmp"));
                HTuple ModelID, row, column, angle, score, row1, column1, angle1, score1;
                HOperatorSet.ReadShapeModel(Path.Combine(path, "ShapeModel.shm"), out ModelID);
                HObject ImageRegion;
                HOperatorSet.ReadRegion(out ImageRegion, Path.Combine(path, "Region.hobj"));
                HObject ImageReduced;
                HOperatorSet.ReduceDomain(ModelImage, ImageRegion, out ImageReduced);
                HOperatorSet.FindShapeModel(ImageReduced, ModelID, (new HTuple(-45)).TupleRad(), (new HTuple(90)).TupleRad(), 0.5, 1, 0, "least_squares", 0, 0.9, out row, out column, out angle, out score);
                HOperatorSet.ReduceDomain(image, ImageRegion, out ImageReduced);
                HOperatorSet.FindShapeModel(ImageReduced, ModelID, (new HTuple(-45)).TupleRad(), (new HTuple(90)).TupleRad(), 0.5, 1, 0, "least_squares", 0, 0.9, out row1, out column1, out angle1, out score1);
                HTuple homMat2D;
                HOperatorSet.VectorAngleToRigid(row, column, angle, row1, column1, angle1, out homMat2D);
                HObject modelRegion;
                HOperatorSet.ReadRegion(out modelRegion, Path.Combine(path, "ModelRegion.hobj"));
                HObject regionAffineTrans;
                HOperatorSet.AffineTransRegion(modelRegion, out regionAffineTrans, homMat2D, "nearest_neighbor");
                CameraGCStyle = new Tuple<string, object>("Color", "green");
                CameraAppendHObject = null;
                CameraAppendHObject = regionAffineTrans;
                AddMessage("找到模板: Row:" + row1.D.ToString("F0") + " Column:" + column1.D.ToString("F0") + " Angle:" + angle1.TupleDeg().D.ToString("F2") + " Score:" + score1.D.ToString("F1"));
                #endregion
                #region 确认角度
                HObject lineRegion;
                HOperatorSet.ReadRegion(out lineRegion, Path.Combine(path, "Line.hobj"));
                HObject imageReduced1;
                HOperatorSet.ReduceDomain(ModelImage, lineRegion, out imageReduced1);
                HObject edges1;
                HOperatorSet.EdgesSubPix(imageReduced1, out edges1, "canny", 1, 20, 40);
                HObject contoursSplit1;
                HOperatorSet.SegmentContoursXld(edges1, out contoursSplit1, "lines_circles", 5, 4, 2);
                HObject selectedContours1;
                HOperatorSet.SelectContoursXld(contoursSplit1, out selectedContours1, "contour_length", 15, 500, -0.5, 0.5);
                HObject unionContours1;
                HOperatorSet.UnionAdjacentContoursXld(selectedContours1, out unionContours1, 10, 1, "attr_keep");
                HTuple rowBegin1, colBegin1, rowEnd1, colEnd1, nr1, nc1, dist1;
                HOperatorSet.FitLineContourXld(unionContours1, "tukey", -1, 0, 5, 2, out rowBegin1, out colBegin1, out rowEnd1, out colEnd1, out nr1, out nc1, out dist1);
                HObject regionLine;
                HOperatorSet.GenRegionLine(out regionLine, rowBegin1, colBegin1, rowEnd1, colEnd1);
                var _index = FindMaxLine(regionLine);
                double lineAngle1 = Math.Atan2((nc1.DArr[_index]), (nr1.DArr[_index])) * 180 / Math.PI - 90;

                HObject regionLineAffineTrans;
                HOperatorSet.AffineTransRegion(lineRegion, out regionLineAffineTrans, homMat2D, "nearest_neighbor");
                HOperatorSet.ReduceDomain(image, regionLineAffineTrans, out imageReduced1);
                HOperatorSet.EdgesSubPix(imageReduced1, out edges1, "canny", 1, 20, 40);
                HOperatorSet.SegmentContoursXld(edges1, out contoursSplit1, "lines_circles", 5, 4, 2);
                HOperatorSet.SelectContoursXld(contoursSplit1, out selectedContours1, "contour_length", 15, 500, -0.5, 0.5);
                HOperatorSet.UnionAdjacentContoursXld(selectedContours1, out unionContours1, 10, 1, "attr_keep");
                HOperatorSet.FitLineContourXld(unionContours1, "tukey", -1, 0, 5, 2, out rowBegin1, out colBegin1, out rowEnd1, out colEnd1, out nr1, out nc1, out dist1);
                HOperatorSet.GenRegionLine(out regionLine, rowBegin1, colBegin1, rowEnd1, colEnd1);
                CameraAppendHObject = regionLine;
                _index = FindMaxLine(regionLine);
                double lineAngle2 = Math.Atan2((nc1.DArr[_index]), (nr1.DArr[_index])) * 180 / Math.PI - 90;
                #endregion
                #region 坐标换算
                HOperatorSet.ReadTuple(Path.Combine(path, "homMat2D.tup"), out homMat2D);
                HTuple CamImage_x, CamImage_y;
                HOperatorSet.AffineTransPoint2d(homMat2D, row, column, out CamImage_x, out CamImage_y);
                HTuple CamImage_x1, CamImage_y1;
                HOperatorSet.AffineTransPoint2d(homMat2D, row1, column1, out CamImage_x1, out CamImage_y1);
                HTuple T2;
                HOperatorSet.VectorAngleToRigid(CamImage_x1, CamImage_y1, new HTuple(lineAngle2 * -1).TupleRad(), CamImage_x, CamImage_y, new HTuple(lineAngle1 * -1).TupleRad(), out T2);//T2是新料移动到模板料位置的变换
                HTuple T1;
                HOperatorSet.VectorAngleToRigid(camerap.X, camerap.Y, new HTuple(camerap.U).TupleRad(), targetp1.X, targetp1.Y, new HTuple(targetp1.U).TupleRad(), out T1);//T1是拍照位置移动到贴合位置的变换
                HTuple CamRobot_x1, CamRobot_y1;
                HOperatorSet.AffineTransPoint2d(T2, camerap.X, camerap.Y, out CamRobot_x1, out CamRobot_y1);//移动到新料与模板料重合
                HTuple FitRobot_x1, FitRobot_y1;
                HOperatorSet.AffineTransPoint2d(T1, CamRobot_x1, CamRobot_y1, out FitRobot_x1, out FitRobot_y1);//移动到贴合位置
                double[] resultP1 = new double[3] { FitRobot_x1.D - targetp1.X, FitRobot_y1.D - targetp1.Y, (lineAngle1 - lineAngle2) * -1 };
                HOperatorSet.VectorAngleToRigid(camerap.X, camerap.Y, new HTuple(camerap.U).TupleRad(), targetp2.X, targetp2.Y, new HTuple(targetp2.U).TupleRad(), out T1);
                HOperatorSet.AffineTransPoint2d(T1, CamRobot_x1, CamRobot_y1, out FitRobot_x1, out FitRobot_y1);
                double[] resultP2 = new double[3] { FitRobot_x1.D - targetp2.X, FitRobot_y1.D - targetp2.Y, (lineAngle1 - lineAngle2) * -1 };
                HOperatorSet.VectorAngleToRigid(camerap.X, camerap.Y, new HTuple(camerap.U).TupleRad(), targetp3.X, targetp3.Y, new HTuple(targetp3.U).TupleRad(), out T1);
                HOperatorSet.AffineTransPoint2d(T1, CamRobot_x1, CamRobot_y1, out FitRobot_x1, out FitRobot_y1);
                double[] resultP3 = new double[3] { FitRobot_x1.D - targetp3.X, FitRobot_y1.D - targetp3.Y, (lineAngle1 - lineAngle2) * -1 };
                #endregion
                #region 范围
                bool result = true;
                if (Math.Abs(resultP1[0]) > 10 || Math.Abs(resultP1[1]) > 10 || Math.Abs(resultP2[0]) > 10 || Math.Abs(resultP2[1]) > 10 || Math.Abs(resultP3[0]) > 10 || Math.Abs(resultP3[1]) > 10 || Math.Abs(lineAngle1 - lineAngle2) > 15)
                {
                    result = false;
                }
                #endregion
                return new Tuple<double[], double[], double[], bool>(resultP1, resultP2, resultP3, result);
            }
            catch (Exception ex)
            {
                AddMessage(ex.Message);
                return new Tuple<double[], double[], double[], bool>(new double[3] { 0, 0, 0 }, new double[3] { 0, 0, 0 }, new double[3] { 0, 0, 0 }, false);
            }

        }
        private int FindMaxLine(HObject LineRegion)
        {
            HTuple area, row, column;
            HOperatorSet.AreaCenter(LineRegion, out area, out row, out column);
            HTuple max;
            HOperatorSet.TupleMax(area, out max);

            for (int i = 0; i < area.IArr.Length; i++)
            {
                if (area.IArr[i] == max)
                {
                    return i;
                }
            }
            return 0;
        }
        private double[] rotateCenter(double x1, double y1, double x2, double y2, double x3, double y3)
        {
            double a, b, c, d, e, f;
            a = 2 * (x2 - x1);
            b = 2 * (y2 - y1);
            c = x2 * x2 + y2 * y2 - x1 * x1 - y1 * y1;
            d = 2 * (x3 - x2);
            e = 2 * (y3 - y2);
            f = x3 * x3 + y3 * y3 - x2 * x2 - y2 * y2;

            double x = (b * f - e * c) / (b * d - e * a);
            double y = (d * c - a * f) / (b * d - e * a);
            double[] xy = new double[2];
            xy[0] = x;
            xy[1] = y;
            return xy;
        }
        
        private void ModbusTCP_Client_ConnectStateChanged(object sender, string e)
        {
            AddMessage("PLC网络连接：" + e);
            StatusPLC = e == "Connected";
        }
        private void ModbusTCP_Client_ModbusStateChanged(object sender, bool e)
        {
            //StatusPLC = e;
        }
        private void Robot_1_Link_ConnectStateChanged(object sender, string e)
        {
            AddMessage("机械手互刷连接：" + e.ToString());
            StatusRobot = e == "Connected";
        }
        bool mStartReadPLCStatus = false;
        private async void StartReadPLC()
        {
            if (!mStartReadPLCStatus)
                mStartReadPLCStatus = true;
            else
                return;
            await Task.Delay(500);

            Task Task_StartReadPLC = Task.Run(() =>
            {
                while (mStartReadPLCStatus)
                {
                    try
                    {
                        System.Threading.Thread.Sleep(10);
                        int[] mPLC_Out = ModbusTCP_Client.ModbusRead(1, 1, 200, 24);

                        if (mPLC_Out == null)
                            continue;

                        Robot_1_In = mPLC_Out;
                        ModbusTCP_Client.ModbusWrite(1, 15, 230, Robot_1_Out);//写给PLC机械手输出信号
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("StartReadPLC:" + ex.Message);
                    }
                }
            });
            await Task_StartReadPLC;
        }
        bool HasStartGetRobot_1_Stauts = false;
        public async void GetRobot_1_Status()
        {
            Stopwatch sw = new Stopwatch();
            if (HasStartGetRobot_1_Stauts == false)
                HasStartGetRobot_1_Stauts = true;
            else
                return;
            await Task.Delay(1000);

            Task Task_GetRobotStatus = Task.Run(() =>
            {
                while (HasStartGetRobot_1_Stauts)
                {
                    sw.Restart();
                    try
                    {
                        #region 互刷
                        int[] mStatus = Robot_1_Link.RobotReadM(100, 24);
                        if (mStatus != null)
                            Robot_1_Out = mStatus;
                        Robot_1_Link.RobotWriteM(10, Robot_1_In);
                        Robot_1_Link.RobotWriteM(0, new int[] { 1 });
                        #endregion
                        #region 视觉
                        int[] CCDCmd = Robot_1_Link.RobotReadM(200, 4);
                        if (CCDCmd != null)
                        {
                            if (CCDCmd[0] == 1)
                            {
                                AddMessage("位置1触发");
                                cameraOperate.GrabImageVoid(0);
                                CameraIamge = cameraOperate.CurrentImage;
                                var calcrst = RecognizeOperete(0, CameraIamge);
                                if (calcrst.Item4)
                                {
                                    Robot_1_Link.RobotWriteM(210, new int[] { 1 });
                                    Robot_1_Link.RobotWriteP(155, calcrst.Item1[0], calcrst.Item1[1], 0, calcrst.Item1[2], 2);
                                    Robot_1_Link.RobotWriteP(156, calcrst.Item2[0], calcrst.Item2[1], 0, calcrst.Item2[2], 2);
                                    Robot_1_Link.RobotWriteP(157, calcrst.Item3[0], calcrst.Item3[1], 0, calcrst.Item3[2], 2);
                                }
                                else
                                {
                                    Robot_1_Link.RobotWriteM(211, new int[] { 1 });
                                    Robot_1_Link.RobotWriteP(155, 0, 0, 0, 0, 2);
                                    Robot_1_Link.RobotWriteP(156, 0, 0, 0, 0, 2);
                                    Robot_1_Link.RobotWriteP(157, 0, 0, 0, 0, 2);
                                }
                                Robot_1_Link.RobotWriteM(200, new int[] { 0 });
                            }
                            if (CCDCmd[1] == 1)
                            {
                                AddMessage("位置2触发");
                                cameraOperate.GrabImageVoid(0);
                                CameraIamge = cameraOperate.CurrentImage;
                                var calcrst = RecognizeOperete(1, CameraIamge);
                                if (calcrst.Item4)
                                {
                                    Robot_1_Link.RobotWriteM(210, new int[] { 1 });
                                    Robot_1_Link.RobotWriteP(159, calcrst.Item1[0], calcrst.Item1[1], 0, calcrst.Item1[2], 2);
                                    Robot_1_Link.RobotWriteP(160, calcrst.Item2[0], calcrst.Item2[1], 0, calcrst.Item2[2], 2);
                                    Robot_1_Link.RobotWriteP(161, calcrst.Item3[0], calcrst.Item3[1], 0, calcrst.Item3[2], 2);
                                }
                                else
                                {
                                    Robot_1_Link.RobotWriteM(211, new int[] { 1 });
                                    Robot_1_Link.RobotWriteP(159, 0, 0, 0, 0, 2);
                                    Robot_1_Link.RobotWriteP(160, 0, 0, 0, 0, 2);
                                    Robot_1_Link.RobotWriteP(161, 0, 0, 0, 0, 2);
                                }
                                Robot_1_Link.RobotWriteM(201, new int[] { 0 });
                            }
                            if (CCDCmd[2] == 1)
                            {
                                AddMessage("位置3触发");
                                cameraOperate.GrabImageVoid(0);
                                CameraIamge = cameraOperate.CurrentImage;
                                var calcrst = RecognizeOperete(2, CameraIamge);
                                if (calcrst.Item4)
                                {
                                    Robot_1_Link.RobotWriteM(210, new int[] { 1 });
                                    Robot_1_Link.RobotWriteP(162, calcrst.Item1[0], calcrst.Item1[1], 0, calcrst.Item1[2], 2);
                                    Robot_1_Link.RobotWriteP(163, calcrst.Item2[0], calcrst.Item2[1], 0, calcrst.Item2[2], 2);
                                    Robot_1_Link.RobotWriteP(164, calcrst.Item3[0], calcrst.Item3[1], 0, calcrst.Item3[2], 2);
                                }
                                else
                                {
                                    Robot_1_Link.RobotWriteM(211, new int[] { 1 });
                                    Robot_1_Link.RobotWriteP(162, 0, 0, 0, 0, 2);
                                    Robot_1_Link.RobotWriteP(163, 0, 0, 0, 0, 2);
                                    Robot_1_Link.RobotWriteP(164, 0, 0, 0, 0, 2);
                                }
                                Robot_1_Link.RobotWriteM(202, new int[] { 0 });
                            }
                            if (CCDCmd[3] == 1)
                            {
                                AddMessage("位置4触发");
                                cameraOperate.GrabImageVoid(0);
                                CameraIamge = cameraOperate.CurrentImage;
                                var calcrst = RecognizeOperete(3, CameraIamge);
                                if (calcrst.Item4)
                                {
                                    Robot_1_Link.RobotWriteM(210, new int[] { 1 });
                                    Robot_1_Link.RobotWriteP(166, calcrst.Item1[0], calcrst.Item1[1], 0, calcrst.Item1[2], 2);
                                    Robot_1_Link.RobotWriteP(167, calcrst.Item2[0], calcrst.Item2[1], 0, calcrst.Item2[2], 2);
                                    Robot_1_Link.RobotWriteP(168, calcrst.Item3[0], calcrst.Item3[1], 0, calcrst.Item3[2], 2);
                                }
                                else
                                {
                                    Robot_1_Link.RobotWriteM(211, new int[] { 1 });
                                    Robot_1_Link.RobotWriteP(168, 0, 0, 0, 0, 2);
                                    Robot_1_Link.RobotWriteP(167, 0, 0, 0, 0, 2);
                                    Robot_1_Link.RobotWriteP(168, 0, 0, 0, 0, 2);
                                }
                                Robot_1_Link.RobotWriteM(203, new int[] { 0 });
                            }
                        }
                        #endregion
                        #region 点位
                        if (isUpdatePoint)
                        {
                            //拍照位
                            for (int i = 0; i < 4; i++)
                            {
                                double[] p = Robot_1_Link.RobotReadP(31 + i);
                                Points[i].X = p[0];
                                Points[i].Y = p[1];
                                Points[i].U = p[3];
                                //System.Threading.Thread.Sleep(50);
                            }
                            //目标位
                            for (int i = 0; i < 4; i++)
                            {
                                for (int j = 0; j < 3; j++)
                                {
                                    double[] p = Robot_1_Link.RobotReadP(139 + j + 4 * i);
                                    Points[i * 3 + j].X = p[0];
                                    Points[i * 3 + j].Y = p[1];
                                    Points[i * 3 + j].U = p[3];
                                    //System.Threading.Thread.Sleep(50);
                                }
                            }
                            isUpdatePoint = false;
                            WriteToJson(Points, Path.Combine(System.Environment.CurrentDirectory, @"Camera", "Points.json"));
                            switch (SelectIndexValue)
                            {
                                case 0:
                                    CameraX = Points[0].X;
                                    CameraY = Points[0].Y;
                                    CameraU = Points[0].U;
                                    break;
                                case 1:
                                    SelectIndexValue = 1;
                                    CameraX = Points[1].X;
                                    CameraY = Points[1].Y;
                                    CameraU = Points[1].U;
                                    break;
                                case 2:
                                    SelectIndexValue = 2;
                                    CameraX = Points[2].X;
                                    CameraY = Points[2].Y;
                                    CameraU = Points[2].U;
                                    break;
                                case 3:
                                    SelectIndexValue = 3;
                                    CameraX = Points[3].X;
                                    CameraY = Points[3].Y;
                                    CameraU = Points[3].U;
                                    break;
                                default:
                                    break;
                            }
                            AddMessage("点位更新完成");
                        }


                        //139
                        //155
                        #endregion
                    }
                    catch(Exception ex) { AddMessage(ex.Message); isUpdatePoint = false; }
                    System.Threading.Thread.Sleep(100);
                    Cycle = sw.ElapsedMilliseconds;
                }
            });
            await Task_GetRobotStatus;
        }
        #endregion
    }

}
