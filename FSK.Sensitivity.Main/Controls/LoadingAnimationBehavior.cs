using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace FSK.Sensitivity.Main.Controls
{
    public static class LoadingAnimationBehavior
    {
        public static bool GetIsLoading(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsLoadingProperty);
        }
        public static void SetIsLoading(DependencyObject obj, bool value)
        {
            obj.SetValue(IsLoadingProperty, value);
        }
        public static readonly DependencyProperty IsLoadingProperty =
            DependencyProperty.RegisterAttached(
                "IsLoading",
                typeof(bool),
                typeof(LoadingAnimationBehavior),
                new PropertyMetadata(false, OnIsLoadingChanged));
        private static void OnIsLoadingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TextBlock textBlock && (bool)e.NewValue)
            {
                StartAnimation(textBlock);
            }
        }
        private static void StartAnimation(TextBlock textBlock)
        {
            var storyboard = new Storyboard
            {
                RepeatBehavior = RepeatBehavior.Forever
            };
            // 创建动画序列：加载中 -> 加载中. -> 加载中.. -> 加载中...
            var keyFrames = new ObjectKeyFrameCollection
            {
                new DiscreteObjectKeyFrame("数据正在加载中", KeyTime.FromTimeSpan(TimeSpan.Zero)),
                new DiscreteObjectKeyFrame("数据正在加载中.", KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(500))),
                new DiscreteObjectKeyFrame("数据正在加载中..", KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(1000))),
                new DiscreteObjectKeyFrame("数据正在加载中...", KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(1500)))
            };
            var animation = new ObjectAnimationUsingKeyFrames
            {
                KeyFrames = keyFrames,
                Duration = new Duration(TimeSpan.FromSeconds(2))
            };
            Storyboard.SetTarget(animation, textBlock);
            Storyboard.SetTargetProperty(animation, new PropertyPath(TextBlock.TextProperty));
            storyboard.Children.Add(animation);
            storyboard.Begin();
        }
    }
}
