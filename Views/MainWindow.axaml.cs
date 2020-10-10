using Avalonia;
using Avalonia.Controls;
using Avalonia.Diagnostics;
using Avalonia.Markup.Xaml;
using DynamicData.Binding;
using ReactiveUI;
using BarcodeGenerator.Models;
using BarcodeGenerator.ViewModels;
using BarcodeLib;
using System;
using System.Threading.Tasks;
using System.Linq;
using System.Drawing;

namespace BarcodeGenerator.Views
{
    public class MainWindow : Window
    {
        public MainWindow()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
