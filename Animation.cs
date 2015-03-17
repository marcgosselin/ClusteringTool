using System;
using System.Collections.Generic;
using System.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;

namespace EasyClustering
{
    public static class Animation
    {
        public static void AnimateY(UIElement pin, double fromY, double toY, int duration, EasingFunctionBase easingFunction)
        {
            pin.RenderTransform = new TranslateTransform();

            var sb = new Storyboard();
            var animation = new DoubleAnimation()
            {
                From = fromY,
                To = toY,
                Duration = new TimeSpan(0, 0, 0, 0, duration),
                EasingFunction = easingFunction
            };

            Storyboard.SetTargetProperty(animation, "(UIElement.RenderTransform).(TranslateTransform.Y)");
            Storyboard.SetTarget(animation, pin);

            sb.Children.Add(animation);
            sb.Begin();
        }

        public static void Grow(UIElement pin)
        {
            pin.RenderTransformOrigin = new Windows.Foundation.Point(0.5,0.5);
            pin.RenderTransform = new ScaleTransform();

            Storyboard story = new Storyboard();
            var dbX = new DoubleAnimation() 
            {
                To=1,
                From=0,
                Duration=new Duration(new TimeSpan(0,0,1)),
                EasingFunction = new ElasticEase()
                {
                     EasingMode= EasingMode.EaseOut
                }
            };
            Storyboard.SetTargetProperty(dbX,"(UIElement.RenderTransform).(CompositeTransform.ScaleX)");
            story.Children.Add(dbX);
            Storyboard.SetTarget(dbX, pin);

            var dbY = new DoubleAnimation()
            {
                To = 1,
                From = 0,
                Duration = new Duration(new TimeSpan(0, 0, 1)),
                EasingFunction = new ElasticEase()
                {
                    EasingMode = EasingMode.EaseOut
                }
            };
            Storyboard.SetTargetProperty(dbY, "(UIElement.RenderTransform).(CompositeTransform.ScaleY)");
            story.Children.Add(dbY);
            Storyboard.SetTarget(dbY, pin);

            story.Begin();
        }

        public static void Drop(UIElement pin, double? height, int? duration)
        {
            height = (height.HasValue && height.Value > 0) ? height : 150;
            duration = (duration.HasValue && duration.Value > 0) ? duration : 150;

            //var anchor = MapLayer.GetPositionAnchor(pin);
            var from = 0 + height.Value;

            AnimateY(pin, -from, -0, duration.Value, new QuadraticEase()
            {
                EasingMode = Windows.UI.Xaml.Media.Animation.EasingMode.EaseIn
            });
        }

        public static void Bounce(UIElement pin, double? height, int? duration)
        {
            height = (height.HasValue && height.Value > 0) ? height : 150;
            duration = (duration.HasValue && duration.Value > 0) ? duration : 1000;

            //var anchor = MapLayer.GetPositionAnchor(pin);
            var from = 0 + height.Value;

            AnimateY(pin, -from, -0, duration.Value, new BounceEase()
            {
                Bounces = 2,
                EasingMode = Windows.UI.Xaml.Media.Animation.EasingMode.EaseOut,
                Bounciness = 2
            });
        }
    }
}
