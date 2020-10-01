using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using DynamicData.Binding;
using ReactiveUI;
using BarcodeGenerator.Models;
using BarcodeGenerator.ViewModels;
using BarcodeLib;
using System;
using System.Threading.Tasks;
using System.Linq;

namespace BarcodeGenerator.Views
{
    public class MainWindow : Window
    {
        private MainWindowViewModel _viewModel;
        private Barcode _barcode;
        private const string MagicPrefix = "\u0014\u0012";

        public MainWindow()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
            _barcode = new Barcode()
            {
                IncludeLabel = false,
            };

            _viewModel = new MainWindowViewModel();
            _viewModel.RenderBarcode += RenderBarcode;
            _viewModel.ExportBarcode += OnExport;
            this.DataContext = _viewModel;
        }

        private void RenderBarcode()
        {
            _viewModel.RenderingException = null;

            if (_viewModel.DataFilled)
            {
                string content = BuildBarcodeContent();
                (int width, int height) = GetBarcodeSize();

                try
                {
                    _viewModel.Image = _barcode.Encode(TYPE.CODE128, content, width, height);
                }
                catch(Exception e)
                {
                    _viewModel.RenderingException = e.Message;
                }
            }
        }

        private (int width, int height) GetBarcodeSize()
        {
            switch (_viewModel.SelectedExportSize)
            {
                case ExportSize.PX1200x400:
                    return (1200, 400);
                case ExportSize.PX500x200:
                    return (500, 200);
                case ExportSize.Custom:
                    return (_viewModel.CustomWidth, _viewModel.CustomHeight);
                default:
                    return (500, 200);
            }
        }

        private string BuildBarcodeContent()
        {
            switch (_viewModel.SelectedBarcodeType)
            {
                case BarcodeType.Normal:
                    return _viewModel.NormalBarcodeContent;

                case BarcodeType.MagicCommand:
                    if (_viewModel.MagicIsCustomCommand)
                    {
                        return MagicPrefix + _viewModel.CustomMagicCommand;
                    }
                    else
                    {
                        return MagicPrefix + _viewModel.SelectedMagicCommand.ToString();
                    }

                case BarcodeType.MagicCheckbox:
                    return MagicPrefix + "ToggleCheckbox" + _viewModel.MagicCheckboxID;

                case BarcodeType.MagicTextEntry:
                    return MagicPrefix + "FillEntry" + _viewModel.MagicEntryID + MagicPrefix + _viewModel.MagicEntryText;

                default:
                    return "";
            }
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private async void OnExport()
        {
            var dialog = new SaveFileDialog();
            dialog.Filters.Add(new FileDialogFilter() { Name = "PNG", Extensions = { "png" } });
            dialog.Filters.Add(new FileDialogFilter() { Name = "JPEG", Extensions = { "jpg" } });
            var result = await dialog.ShowAsync(this);
            if (result != null)
            {
                _viewModel.Image.Save(result);
            }
        }
    }
}
