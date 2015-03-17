using EasyClustering;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace CLustering.Test.MapObject
{
    public sealed partial class DefaultControl : UserControl, INotifyPropertyChanged
    {
        private bool isCluster;

        public bool IsCluster
        {
            get { return isCluster; }
            set
            {
                isCluster = value;
                RaisePropertyChanged();
            }
        }


        public ItemObjet ViewModel
        {
            get { return (ItemObjet)GetValue(ViewModelProperty); }
            set
            {
                SetValue(ViewModelProperty, value);
            }
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register("ViewModel", typeof(ItemObjet), typeof(DefaultControl), new PropertyMetadata(null, new PropertyChangedCallback(
                (d, e) =>
                {
                    var sender = d as DefaultControl;
                    if ((e.NewValue as ItemObjet).item is ItemObjet)
                    {
                        sender.IsCluster = false;
                    }
                    else
                    {
                        sender.IsCluster = true;
                    }
                })));



        public DefaultControl()
        {
            this.InitializeComponent();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void RaisePropertyChanged([CallerMemberName]string propName = null)
        {
            var handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propName));
        }

    }
}
