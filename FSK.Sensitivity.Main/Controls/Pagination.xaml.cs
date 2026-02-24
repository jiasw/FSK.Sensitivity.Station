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
    /// Pagination.xaml 的交互逻辑
    /// </summary>
    public partial class Pagination : UserControl
    {
       
        public Pagination()
        {
            InitializeComponent();
            
        }
        #region 依赖属性
        /// <summary>
        /// 当前页码
        /// </summary>
        public int CurrentPage
        {
            get { return (int)GetValue(CurrentPageProperty); }
            set { SetValue(CurrentPageProperty, value); }
        }
        public static readonly DependencyProperty CurrentPageProperty =
            DependencyProperty.Register("CurrentPage", typeof(int), typeof(Pagination),
                new PropertyMetadata(1, OnPageChanged));
        /// <summary>
        /// 总页数
        /// </summary>
        public int TotalPages
        {
            get { return (int)GetValue(TotalPagesProperty); }
            set { SetValue(TotalPagesProperty, value); }
        }
        public static readonly DependencyProperty TotalPagesProperty =
            DependencyProperty.Register("TotalPages", typeof(int), typeof(Pagination),
                new PropertyMetadata(0, OnPageChanged));
        /// <summary>
        /// 上一页命令
        /// </summary>
        public object PreviousCommand
        {
            get { return GetValue(PreviousCommandProperty); }
            set { SetValue(PreviousCommandProperty, value); }
        }
        public static readonly DependencyProperty PreviousCommandProperty =
            DependencyProperty.Register("PreviousCommand", typeof(object), typeof(Pagination),
                new PropertyMetadata(null));
        /// <summary>
        /// 下一页命令
        /// </summary>
        public object NextCommand
        {
            get { return GetValue(NextCommandProperty); }
            set { SetValue(NextCommandProperty, value); }
        }
        public static readonly DependencyProperty NextCommandProperty =
            DependencyProperty.Register("NextCommand", typeof(object), typeof(Pagination),
                new PropertyMetadata(null));
        /// <summary>
        /// 是否显示控件
        /// </summary>
        public bool ShowControl
        {
            get { return (bool)GetValue(ShowControlProperty); }
            set { SetValue(ShowControlProperty, value); }
        }
        public static readonly DependencyProperty ShowControlProperty =
            DependencyProperty.Register("ShowControl", typeof(bool), typeof(Pagination),
                new PropertyMetadata(true));
        #endregion
        #region 私有方法
        private static void OnPageChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as Pagination;
            control?.UpdateButtonStates();
        }
        private void UpdateButtonStates()
        {
            
            // 如果总页数小于等于1，不显示控件
            if (TotalPages <= 1)
            {
                Visibility = Visibility.Hidden;
                return;
            }
            else if (ShowControl)
            {
                Visibility = Visibility.Visible;
            }
            else
            {
                Visibility = Visibility.Hidden;
                return;
            }
            // 更新按钮状态
            btnPrevious.IsEnabled = CurrentPage > 1;
            btnNext.IsEnabled = CurrentPage < TotalPages;
        }
        #endregion
    }
}
