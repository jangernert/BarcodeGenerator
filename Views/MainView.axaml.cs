using Avalonia;
using Avalonia.Controls;
using Avalonia.Diagnostics;
using Avalonia.Markup.Xaml;
using Avalonia.Controls.ApplicationLifetimes;
using BarcodeGenerator.Models;
using BarcodeGenerator.ViewModels;
using BarcodeLib;
using System;
using System.Threading.Tasks;
using System.Linq;
using System.Drawing;

namespace BarcodeGenerator.Views
{
    public class MainView : UserControl
    {
        private MainWindowViewModel _viewModel;
        private Barcode _barcode;
        private const string MagicPrefix = "\u0014\u0012";

        public MainView()
        {
            AvaloniaXamlLoader.Load(this);
            
            _barcode = new Barcode()
            {
                IncludeLabel = false,
                BackColor = Color.Transparent,
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
                    return MagicPrefix + "ToggleCheckbox" + MagicPrefix + _viewModel.MagicCheckboxID;

                case BarcodeType.MagicTextEntry:
                    return MagicPrefix + "FillEntry" + MagicPrefix + _viewModel.MagicEntryID + MagicPrefix + _viewModel.MagicEntryText;

                default:
                    return "";
            }
        }

        private string BuildFileName()
        {
            switch (_viewModel.SelectedBarcodeType)
            {
                case BarcodeType.Normal:
                    return _viewModel.NormalBarcodeContent;

                case BarcodeType.MagicCommand:
                    if (_viewModel.MagicIsCustomCommand)
                    {
                        return $"magic_{_viewModel.CustomMagicCommand}";
                    }
                    else
                    {
                        return $"magic_{_viewModel.SelectedMagicCommand}";
                    }

                case BarcodeType.MagicCheckbox:
                    return $"magic_ToggleCheckbox_{_viewModel.MagicCheckboxID}";

                case BarcodeType.MagicTextEntry:
                    return $"magic_FillEntry_{_viewModel.MagicEntryID}_{_viewModel.MagicEntryText}";

                default:
                    return "unknown";
            }
        }

        private async void OnExport()
        {
            if (Application.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktopLifetime)
            {
                var dialog = new SaveFileDialog();
                dialog.InitialFileName = $"barcode_{BuildFileName()}";
                dialog.Filters.Add(new FileDialogFilter() { Name = "PNG", Extensions = { "png" } });
                dialog.Filters.Add(new FileDialogFilter() { Name = "JPEG", Extensions = { "jpg" } });
                var result = await dialog.ShowAsync(desktopLifetime.MainWindow);
                if (result != null)
                {
                    _viewModel.Image.Save(result);
                }
            }
            else
            {
                // NOT SUPPORTED
            }
        }
    }
}