using EasyClustering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace CLustering.Test.TemplateSelector
{
    public class PushPinTemplateSelector : DataTemplateSelector
    {
        public DataTemplate PinTemplate { get; set; }
        public DataTemplate ClusterPinTemplate { get; set; }

        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            var currentItem = item as ItemObjet;
            if (currentItem.item is ItemObjet)
            {
                return PinTemplate;
            }
            else
            {
                return ClusterPinTemplate;
            }
        }
    }
}
