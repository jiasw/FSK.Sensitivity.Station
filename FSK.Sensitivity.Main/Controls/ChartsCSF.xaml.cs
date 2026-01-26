using FSK.Sensitivity.Core.Model;
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

namespace FSK.Sensitivity.Main.Controls
{
    /// <summary>
    /// ChartsCSF.xaml 的交互逻辑
    /// </summary>
    public partial class ChartsCSF : UserControl
    {

        public EventHandler RenderDataEvent;


        public class ChartsLine
        {

            public string Code { get; set; }

            /// <summary>
            /// 折线图
            /// </summary>
            public Polyline Polyline { get; set; }

            /// <summary>
            /// 起始点
            /// </summary>
            public Ellipse Ellipse { get; set; }

            /// <summary>
            /// 数据点集合
            /// </summary>
            public List<CSFPointModel> Points { get; set; }

            //public void ClearLine()
            //{
            //    Polyline.Points.Clear();
            //    Ellipse.Visibility = Visibility.Hidden;
            //}
            ///// <summary>
            ///// 渲染图标
            ///// </summary>
            //public void RenderLine()
            //{
            //    Polyline.Points.Clear();
            //    Ellipse.Visibility = Visibility.Hidden;
            //    if (this.Points.Count > 0)
            //    {
            //        this.StackPanel.Visibility = Visibility.Visible;
            //    }
            //    if (this.Points.Count == 1)
            //    {
            //        this.Ellipse.Visibility = Visibility.Visible;
            //    }
            //}

            public StackPanel StackPanel { get; set; }

        }

        private SolidColorBrush yellowBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFAA00"));
        private SolidColorBrush greenBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#16CE72"));
        private SolidColorBrush redBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF0900"));
        private SolidColorBrush blueBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2394EB"));
        private SolidColorBrush gridBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#DDDDDD"));

        private double _leftpadding = 30;//控件左侧内边距
        private double _rightpadding = 10;//控件右侧内边距
        private double _bottompadding = 30;//控件底部内边距
        private double _toppadding = 10;//控件上部内边距
        private double _width = 1000;//控件总宽度
        private double _height = 800;//控件总高度
        private double _axisWidth = 1;//坐标轴宽度

        #region 图表数据线
        //0.4米
        public ChartsLine microspurLine = new ChartsLine() { Code = "Microspur", };
        //0.8米
        public ChartsLine shortRangeLine = new ChartsLine() { Code = "ShortRange" };
        //1.5米
        public ChartsLine midrangeLine = new ChartsLine() { Code = "Midrange" };
        //5米
        public ChartsLine longRangeLine = new ChartsLine() { Code = "LongRange" };



        #endregion

        private List<ChartsLabel> xLabelsList = new List<ChartsLabel>() {
            new ChartsLabel() { Label = "0.06\n1.8", Location=-1 },
            new ChartsLabel() { Label = "0.1\n 3", Location=-1 },
            new ChartsLabel() { Label = "0.2\n 6", Location=-1 },
            new ChartsLabel() { Label = "0.4\n 12", Location=-1 },
            new ChartsLabel() { Label = "0.6\n 18", Location=-1 },
            new ChartsLabel() { Label = "0.8\n 24", Location=-1 }
        };

        private List<ChartsLabel> yLabelsList = new List<ChartsLabel>() {
            new ChartsLabel() { Label = "1", Location=-1 },
            new ChartsLabel() { Label = "6", Location=-1 },
            new ChartsLabel() { Label = "9", Location=-1 },
            new ChartsLabel() { Label = "12", Location=-1 },
            new ChartsLabel() { Label = "20", Location=-1 },
            new ChartsLabel() { Label = "30", Location=-1 },
            new ChartsLabel() { Label = "45", Location=-1 },
            new ChartsLabel() { Label = "66", Location=-1 },
            new ChartsLabel() { Label = "100", Location=-1 }
        };

        private string _chartsTitle;
        public string ChartsTitle
        {
            get { return _chartsTitle; }
            set { _chartsTitle = value; }
        }

        public static readonly DependencyProperty ChartsTitleProperty =
            DependencyProperty.Register("ChartsTitle", typeof(string), typeof(ChartsCSF), new PropertyMetadata(string.Empty));
        ///图表数据点集合    


        public List<CSFPointModel> MicrospurPoints
        {
            get { return (List<CSFPointModel>)GetValue(MicrospurPointsProperty); }
            set { SetValue(MicrospurPointsProperty, value); }
        }

        public static readonly DependencyProperty MicrospurPointsProperty =
            DependencyProperty.Register("MicrospurPoints", typeof(List<CSFPointModel>), typeof(ChartsCSF), new PropertyMetadata(new List<CSFPointModel>(), OnMicrospurPointsChanged));

        private static void OnMicrospurPointsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ChartsCSF control )
            {
                
                var chart = (ChartsCSF)d;
                chart.microspurLine.Points = (List<CSFPointModel>)e.NewValue;
                chart.BindMicrospurData();
            }
        }

        /// <summary>
        /// 0.8米数据
        /// </summary>
        public List<CSFPointModel> ShortRangePoints
        {
            get { return (List<CSFPointModel>)GetValue(ShortRangePointsProperty); }
            set { SetValue(ShortRangePointsProperty, value); }
        }

        public static readonly DependencyProperty ShortRangePointsProperty =
            DependencyProperty.Register("ShortRangePoints", typeof(List<CSFPointModel>), typeof(ChartsCSF), new PropertyMetadata(new List<CSFPointModel>(), OnShortRangePointsChanged));

        private static void OnShortRangePointsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ChartsCSF control )
            {
                var chart = (ChartsCSF)d;
                chart.shortRangeLine.Points = (List<CSFPointModel>)e.NewValue;
                chart.BindShortRangeData();
            }
        }
        /// <summary>
        /// 1.5米数据
        /// </summary>
        public List<CSFPointModel> MidrangePoints
        {
            get { return (List<CSFPointModel>)GetValue(MidrangePointsProperty); }
            set { SetValue(MidrangePointsProperty, value); }
        }

        public static readonly DependencyProperty MidrangePointsProperty =
            DependencyProperty.Register("MidrangePoints", typeof(List<CSFPointModel>), typeof(ChartsCSF), new PropertyMetadata(new List<CSFPointModel>(), OnMidrangePointsChanged));

        private static void OnMidrangePointsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ChartsCSF control )
            {
                var chart = (ChartsCSF)d;
                chart.midrangeLine.Points = (List<CSFPointModel>)e.NewValue;
                chart.BindMidrangeData();
            }
        }
        /// <summary>
        /// 5米数据
        /// </summary>
        public List<CSFPointModel> LongRangePoints
        {
            get { return (List<CSFPointModel>)GetValue(LongRangePointsProperty); }
            set { SetValue(LongRangePointsProperty, value); }
        }

        public static readonly DependencyProperty LongRangePointsProperty =
            DependencyProperty.Register("LongRangePoints", typeof(List<CSFPointModel>), typeof(ChartsCSF), new PropertyMetadata(new List<CSFPointModel>(), OnLongRangePointsChanged));

        private static void OnLongRangePointsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ChartsCSF control )
            {
                var chart = (ChartsCSF)d;
                chart.longRangeLine.Points = (List<CSFPointModel>)e.NewValue;
                chart.BindLongRangeData();
            }
        }

        private bool _isLoaded=false;




        public ChartsCSF()
        {
            InitializeComponent();
            InitChart();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            _isLoaded = true;
            SetChartDisPlay();
            BindData();
        }
        private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (!_isLoaded)
            {
                SetChartDisPlay();
                BindData();
            }
        }



        private void InitChart()
        {
            microspurLine = new ChartsLine() { Code = "Microspur", Polyline = new Polyline() { Stroke = yellowBrush, StrokeThickness = 2 }, Ellipse = new Ellipse() { Fill = yellowBrush, Width = 4, Height = 4 }, Points = new List<CSFPointModel>(), StackPanel = sp1 };
            shortRangeLine = new ChartsLine() { Code = "ShortRange", Polyline = new Polyline() { Stroke = greenBrush, StrokeThickness = 2, StrokeDashArray = new DoubleCollection(new double[] { 2, 2 }) }, Ellipse = new Ellipse() { Fill = greenBrush, Width = 4, Height = 4 }, Points = new List<CSFPointModel>(), StackPanel = sp2 };
            midrangeLine = new ChartsLine() { Code = "Midrange", Polyline = new Polyline() { Stroke = redBrush, StrokeThickness = 2, StrokeDashArray = new DoubleCollection(new double[] { 2, 2 }) }, Ellipse = new Ellipse() { Fill = redBrush, Width = 4, Height = 4 }, Points = new List<CSFPointModel>(), StackPanel = sp3 };
            longRangeLine = new ChartsLine() { Code = "LongRange", Polyline = new Polyline() { Stroke = blueBrush, StrokeThickness = 2, StrokeDashArray = new DoubleCollection(new double[] { 2, 2 }) }, Ellipse = new Ellipse() { Fill = blueBrush, Width = 4, Height = 4 }, Points = new List<CSFPointModel>(), StackPanel = sp4 };

            microspurLine.StackPanel.Visibility = Visibility.Collapsed;
            shortRangeLine.StackPanel.Visibility = Visibility.Collapsed;
            midrangeLine.StackPanel.Visibility = Visibility.Collapsed;
            longRangeLine.StackPanel.Visibility = Visibility.Collapsed;


        }

        private void SetChartDisPlay()
        {
            // 多重保险：确保控件已加载且有了实际尺寸
            if (myCanvas.ActualHeight<=0 || myCanvas.ActualWidth <= 0) return;

            _width = myCanvas.ActualWidth - _leftpadding - _rightpadding;
            _height = myCanvas.ActualHeight - _bottompadding - _toppadding;
            myCanvas.Children.Clear();
            DrawAxes();
            myCanvas.Children.Add(microspurLine.Polyline);
            myCanvas.Children.Add(shortRangeLine.Polyline);
            myCanvas.Children.Add(midrangeLine.Polyline);
            myCanvas.Children.Add(longRangeLine.Polyline);
            myCanvas.Children.Add(microspurLine.Ellipse);
            myCanvas.Children.Add(shortRangeLine.Ellipse);
            myCanvas.Children.Add(midrangeLine.Ellipse);
            myCanvas.Children.Add(longRangeLine.Ellipse);
        }


        public void BindData()
        {
            
            BindMicrospurData();
            BindShortRangeData();
            BindMidrangeData();
            BindLongRangeData();
        }

        public void BindMicrospurData()
        {
            RenderLine(microspurLine);
        }
public void BindShortRangeData()
        {
            RenderLine(shortRangeLine);
        }
        public void BindMidrangeData()
        {
            RenderLine(midrangeLine);
        }

        public void BindLongRangeData()
        {
            RenderLine(longRangeLine);
        }


        public double GetLoactionXByIndex(int xindex)
        {
            if (xindex < 0 || xindex >= xLabelsList.Count)
            {
                return 0;
            }
            return xLabelsList[xindex].Location;
        }

        public double GetLoactionYByIndex(int yindex)
        {
            if (yindex < 0 || yindex >= yLabelsList.Count)
            {
                return 0;
            }
            return yLabelsList[yindex].Location;
        }


        public void RenderLine(ChartsLine chartsLine)
        {
            // 多重保险：确保控件已加载且有了实际尺寸
            if (!_isLoaded || myCanvas.ActualWidth <= 0)
            {
                SetChartDisPlay();
            }
            chartsLine.Polyline.Points.Clear();
            chartsLine.Ellipse.Visibility = Visibility.Collapsed;
            chartsLine.StackPanel.Visibility = Visibility.Collapsed;
            var points = chartsLine.Points;
            if (points != null && points.Count > 0)
            {
                chartsLine.StackPanel.Visibility = Visibility.Visible;
                if (points.Count == 1)
                {
                    Ellipse ellipse = chartsLine.Ellipse;
                    chartsLine.Ellipse.Visibility = Visibility.Visible;
                    double x = GetLoactionXByIndex(points[0].XIndex);
                    double y = GetLoactionYByIndex(points[0].YIndex);
                    Canvas.SetLeft(ellipse, x - ellipse.Width / 2);
                    Canvas.SetTop(ellipse, y - ellipse.Height / 2);

                }
                foreach (var point in points)
                {
                    chartsLine.Polyline.Points.Add(new Point(GetLoactionXByIndex(point.XIndex), GetLoactionYByIndex(point.YIndex)));
                }
            }

        }


        private void DrawAxes()
        {
            //绘制横线
            int hlen = yLabelsList.Count;
            double gridHeight = _height / (hlen - 1);
            for (int i = 0; i < hlen; i++)
            {
                var xAxis = new Line
                {
                    X1 = _leftpadding,
                    Y1 = _height - gridHeight * i,
                    X2 = _width + _leftpadding,
                    Y2 = _height - gridHeight * i,
                    Stroke = gridBrush,
                    StrokeThickness = _axisWidth
                };
                yLabelsList[i].Location = xAxis.Y1;
                var label = new TextBlock
                {
                    Text = yLabelsList[i].Label,
                    FontSize = 14,
                    Foreground = Brushes.Black,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Margin = new Thickness(0, 0, 8, 0)
                };
                Canvas.SetLeft(label, _leftpadding - label.Text.Length * 8 - 5);
                Canvas.SetTop(label, _height - gridHeight * i - 10);
                myCanvas.Children.Add(label);
                myCanvas.Children.Add(xAxis);

            }

            ////绘制纵线
            int vlen = xLabelsList.Count;
            double gridWidth = _width / vlen;

            for (int i = 0; i <= vlen; i++)
            {
                var yAxis = new Line
                {
                    X1 = _leftpadding + gridWidth * i,
                    Y1 = _toppadding - 15,
                    X2 = _leftpadding + gridWidth * i,
                    Y2 = _height,
                    Stroke = gridBrush,
                    StrokeThickness = _axisWidth
                };
                int j = i - 1;
                if (j >= 0)
                {
                    xLabelsList[j].Location = yAxis.X1;
                    //绘制坐标轴标签
                    var label = new TextBlock
                    {
                        Text = xLabelsList[j].Label,
                        FontSize = 12,
                        Foreground = Brushes.Black,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        Margin = new Thickness(0, 8, 0, 0)
                    };
                    Canvas.SetLeft(label, _leftpadding + gridWidth * j + 20);
                    Canvas.SetTop(label, _height - 5);
                    myCanvas.Children.Add(label);
                }

                myCanvas.Children.Add(yAxis);
            }

        }

        
    }
}