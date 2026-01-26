using FSK.Sensitivity.Core.Enums;
using FSK.Sensitivity.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.RightsManagement;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FSK.Sensitivity.Main.Controls
{
    /// <summary>
    /// ChartsDCK.xaml 的交互逻辑
    /// </summary>
    public partial class ChartsDCK : UserControl
    {
        public ChartsDCK()
        {
            InitializeComponent();
        }

        
        /// <summary>
        /// 标题中的结果
        /// </summary>
        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register("TitleResult", typeof(string), typeof(ChartsDCK), new PropertyMetadata(string.Empty));
        public string TitleResult
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }
        ///图表数据点集合    
        public List<DCKPointModel> Points
        {
            get { return (List<DCKPointModel>)GetValue(PointsProperty); }
            set { SetValue(PointsProperty, value); }
        }


        public static readonly DependencyProperty PointsProperty =
            DependencyProperty.Register("Points", typeof(List<DCKPointModel>), typeof(ChartsDCK), new PropertyMetadata(new List<DCKPointModel>(), OnPointsChanged));
        private static void OnPointsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ChartsDCK gsfchart = (ChartsDCK)d;
            gsfchart.BindData();
        }

        /// <summary>
        /// 图表中横线、纵线的颜色
        /// </summary>
        public static readonly DependencyProperty ChartLineColorProperty = DependencyProperty.Register("ChartLineColor", typeof(SolidColorBrush), typeof(ChartsDCK), new PropertyMetadata(new SolidColorBrush((Color)ColorConverter.ConvertFromString("#bdbdbd"))));

        public SolidColorBrush ChartLineColor
        {
            get { return (SolidColorBrush)GetValue(ChartLineColorProperty); }
            set
            {
                SetValue(ChartLineColorProperty, value);
            }
        }

        //图表中数据点的颜色
        public static readonly DependencyProperty ChartPointColorProperty = DependencyProperty.Register("ChartPointColor", typeof(SolidColorBrush), typeof(ChartsDCK), new PropertyMetadata(new SolidColorBrush((Color)ColorConverter.ConvertFromString("#1B929D"))));

        public SolidColorBrush ChartPointColor
        {
            get { return (SolidColorBrush)GetValue(ChartPointColorProperty); }
            set
            {
                SetValue(ChartPointColorProperty, value);
            }
        }
        double strokeThickness = 1;

        private double _paddingBottom = 50;//控件内边距
        private double _paddingWidth = 50;//控件下边距
        private double _paddingTop = 10;//控件上边距
        private Point YAxisStartPoint, YAxisEndPoint, XAxisEndPoint;
        private double _width = 1000;//控件总宽度
        private double _height = 800;//控件总高度
        private double _axisWidth = 1;//坐标轴宽度
        private double _tickWidth = 1;//刻度线宽度
        private double _tickHeight = 5;//刻度线高度


        private List<ChartsLabel> xLabelsList = new List<ChartsLabel>() {
            new ChartsLabel() { Label = "5", Location=-1 },
            new ChartsLabel() { Label = "10", Location=-1 },
            new ChartsLabel() { Label = "15", Location=-1 },
            new ChartsLabel() { Label = "20", Location=-1 },
            new ChartsLabel() { Label = "30", Location=-1 },
            new ChartsLabel() { Label = "50", Location=-1 }
        };

        private List<ChartsLabel> yLabelsList = new List<ChartsLabel>() {
            new ChartsLabel() { Label = "0.1", Location=-1 },
            new ChartsLabel() { Label = "0.15", Location=-1 },
            new ChartsLabel() { Label = "0.2", Location=-1 },
            new ChartsLabel() { Label = "0.25", Location=-1 },
            new ChartsLabel() { Label = "0.3", Location=-1 },
            new ChartsLabel() { Label = "0.4", Location=-1 },
            new ChartsLabel() { Label = "0.5", Location=-1 },
            new ChartsLabel() { Label = "0.6", Location=-1 },
            new ChartsLabel() { Label = "0.8", Location=-1 },
            new ChartsLabel() { Label = "1.0", Location=-1 }
        };

        Polyline polyline;

        private Dictionary<DCKValue, string> DCKVAToStr = new Dictionary<DCKValue, string>()
        {
            {DCKValue.VA010,"0.1"},
            {DCKValue.VA015,"0.15"},
            {DCKValue.VA020,"0.2"},
            {DCKValue.VA025,"0.25"},
            {DCKValue.VA030,"0.3"},
            {DCKValue.VA040,"0.4"},
            {DCKValue.VA050,"0.5"},
            {DCKValue.VA060,"0.6"},
            {DCKValue.VA080,"0.8"},
            {DCKValue.VA100,"1.0"}
        };

        private bool _isLoaded = false;
        /// <summary>
        /// 辅助线集合
        /// </summary>
        private List<Line> AuxiliaryLines = new List<Line>();
        private void RenderChart()
        {
            _width = myCanvas.ActualWidth - 2 * _paddingWidth;
            _height = myCanvas.ActualHeight - _paddingBottom - _paddingTop;

            XAxisEndPoint = new Point(_width + _paddingWidth, _height + _paddingTop);
            YAxisStartPoint = new Point(_paddingWidth, _height + _paddingTop);
            YAxisEndPoint = new Point(_paddingWidth, _paddingTop);

            // 清除Canvas内容
            myCanvas.Children.Clear();
            ///绘制坐标轴、刻度线
            polyline = new Polyline
            {
                Stroke = ChartPointColor,
                StrokeThickness = 2
            } ;
            DrawAxes();
            DrawXAxisTicks();
            DrawYAxisTicks();
            
            myCanvas.Children.Add(polyline);

        }

        private void DrawAxes()
        {
            // 绘制X轴
            var xAxis = new Line
            {
                X1 = YAxisStartPoint.X,
                Y1 = YAxisStartPoint.Y,
                X2 = XAxisEndPoint.X,
                Y2 = XAxisEndPoint.Y,
                Stroke = Brushes.Black,
                StrokeThickness = _axisWidth
            };
            myCanvas.Children.Add(xAxis);
            // 创建X轴尾部的三角箭头
            myCanvas.Children.Add(new Polygon
            {
                Points = new PointCollection
                {
                    new Point(XAxisEndPoint.X, XAxisEndPoint.Y-5),
                    new Point(XAxisEndPoint.X, XAxisEndPoint.Y+5),
                    new Point(XAxisEndPoint.X+10, XAxisEndPoint.Y)
                },
                Fill = Brushes.Black
            });


            // 绘制Y轴
            var yAxis = new Line
            {
                X1 = YAxisStartPoint.X,
                Y1 = YAxisStartPoint.Y,
                X2 = YAxisEndPoint.X,
                Y2 = YAxisEndPoint.Y,
                Stroke = Brushes.Black,
                StrokeThickness = _axisWidth
            };
            // 创建Y轴尾部的三角箭头
            myCanvas.Children.Add(new Polygon
            {
                Points = new PointCollection
                {
                    new Point(YAxisEndPoint.X-5, YAxisEndPoint.Y),
                    new Point(YAxisEndPoint.X+5, YAxisEndPoint.Y),
                    new Point(YAxisEndPoint.X, YAxisEndPoint.Y-10)
                },
                Fill = Brushes.Black
            });
            myCanvas.Children.Add(yAxis);
        }
        private void DrawXAxisTicks()
        {
            int numberOfTicks = xLabelsList.Count;
            double tickSpacing = _width / (numberOfTicks + 1);

            for (int i = 1; i <= numberOfTicks; i++)
            {
                double x = i * tickSpacing;

                // 绘制刻度线
                var tick = new Line
                {
                    X1 = x + _paddingWidth,
                    Y1 = YAxisStartPoint.Y,
                    X2 = x + _paddingWidth,
                    Y2 = YAxisStartPoint.Y - _tickHeight,
                    Stroke = Brushes.Black,
                    StrokeThickness = _tickWidth
                };
                xLabelsList[i - 1].Location = tick.X1;
                myCanvas.Children.Add(tick);

                // 添加刻度标签
                var textBlock = new TextBlock
                {
                    Text = xLabelsList[i - 1].Label,
                    Foreground = Brushes.Black,
                    FontSize = 12,

                };
                Canvas.SetLeft(textBlock, x + _paddingWidth - 5);
                Canvas.SetTop(textBlock, YAxisStartPoint.Y + 5);
                myCanvas.Children.Add(textBlock);
            }
        }
        private void DrawYAxisTicks()
        {
            int numberOfTicks = yLabelsList.Count;
            double tickSpacing = _height / (numberOfTicks + 1);

            for (int i = 1; i <= numberOfTicks; i++)
            {
                double y = i * tickSpacing;

                // 绘制刻度线
                var tick = new Line
                {
                    X1 = _paddingWidth,
                    Y1 = YAxisStartPoint.Y - y,
                    X2 = _paddingWidth + _tickHeight,
                    Y2 = YAxisStartPoint.Y - y,
                    Stroke = Brushes.Black,
                    StrokeThickness = _tickWidth
                };
                yLabelsList[i - 1].Location = tick.Y1;
                myCanvas.Children.Add(tick);

                // 添加刻度标签
                var textBlock = new TextBlock
                {
                    Text = yLabelsList[i - 1].Label,
                    Foreground = Brushes.Black,
                    FontSize = 12,
                    HorizontalAlignment = HorizontalAlignment.Right,
                };
                Canvas.SetLeft(textBlock, YAxisStartPoint.X - 30);
                Canvas.SetTop(textBlock, YAxisStartPoint.Y - y - 5);
                myCanvas.Children.Add(textBlock);
            }
        }

        /// <summary>
        /// 绘制x,y轴辅助线
        /// </summary>
        private void DrawAuxiliaryLines()
        {
            ClearAuxiliaryLines();
            foreach (DCKPointModel line in Points)
            {
                if (line == null)
                {
                    return;
                }
                double x = xLabelsList[line.XIndex].Location;
                double y = yLabelsList[line.YIndex].Location;
                //绘制横线
                var xline = new Line
                {
                    X1 = _paddingWidth,
                    Y1 = y,
                    X2 = x,
                    Y2 = y,
                    Stroke = ChartLineColor,
                    StrokeThickness = strokeThickness,
                };
                //绘制竖线
                var yline = new Line
                {
                    X1 = x,
                    Y1 = _height + _paddingTop,
                    X2 = x,
                    Y2 = y,
                    Stroke = ChartLineColor,
                    StrokeThickness = _tickWidth
                };
                AuxiliaryLines.Add(xline);
                AuxiliaryLines.Add(yline);
            }
            foreach (Line line in AuxiliaryLines)
            {
                myCanvas.Children.Add(line);
            }
        }

        private void ClearAuxiliaryLines()
        {
            foreach (Line line in AuxiliaryLines)
            {
                myCanvas.Children.Remove(line);
            }
            AuxiliaryLines.Clear();
        }

        /// <summary>
        /// 绘制折线图
        /// </summary>
        private void DrawPolyLine()
        {
            polyline.Points.Clear();
            foreach (DCKPointModel point in Points)
            {
                if (point == null)
                {
                    return;
                }

                double x = xLabelsList[point.XIndex].Location;
                double y = yLabelsList[point.YIndex].Location;
                polyline.Points.Add(new Point(x, y));
            }

        }


        private void ClearDataLine()
        {
            ClearAuxiliaryLines();
            polyline.Points.Clear();
        }

        private void BindData()
        {
            TitleResult = "";
            if (Points != null && Points.Count > 0)
            {
                var pointquery = Points.Where(p => p.DCKTime == DCKTime.T50);
                if (pointquery.Count() > 0)
                {
                    TitleResult = DCKVAToStr[pointquery.First().DCKValue];
                }

                DrawPolyLine();
                DrawAuxiliaryLines();
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            _isLoaded = true;
            RenderChart();
            BindData();
            
        }

        private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (_isLoaded)
            {
                RenderChart();
                BindData();
            }
        }
    }
}
