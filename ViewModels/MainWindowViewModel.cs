using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.Linq;
using BarcodeGenerator.Models;
using ReactiveUI;
using BarcodeLib;
using System.IO;
using System.Reactive.Linq;
using System.Runtime.CompilerServices;
using System.Reactive;

namespace BarcodeGenerator.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public delegate void ReRenderBarcode();
        public event ReRenderBarcode RenderBarcode;

        public delegate void ExportBarcodeDelegate();
        public event ExportBarcodeDelegate ExportBarcode;

        public MainWindowViewModel()
        {
            this.WhenAnyValue(x => x.SelectedBarcodeType, selected => selected == BarcodeType.Normal).ToProperty(this, nameof(NormalBarcodeSelected), out _normalBarcodeSelected);
            this.WhenAnyValue(x => x.SelectedBarcodeType, selected => selected == BarcodeType.MagicCommand).ToProperty(this, nameof(MagicCommandSelected), out _magicCommandSelected);
            this.WhenAnyValue(x => x.SelectedBarcodeType, selected => selected == BarcodeType.MagicCheckbox).ToProperty(this, nameof(MagicCheckboxSelected), out _magicCheckboxSelected);
            this.WhenAnyValue(x => x.SelectedBarcodeType, selected => selected == BarcodeType.MagicTextEntry).ToProperty(this, nameof(MagicEntrySelected), out _magicEntrySelected);
            this.WhenAnyValue(x => x.SelectedBarcodeType, selected => selected == BarcodeType.MagicClearEntry).ToProperty(this, nameof(MagicClearSelected), out _magicClearSelected);

            this.WhenAnyValue(x => x.SelectedExportSize, selected => selected == ExportSize.Custom).ToProperty(this, nameof(CustomSizeSelected), out _customSizeSelected);
            this.WhenAnyValue(x => x.RenderingException, exception => !String.IsNullOrWhiteSpace(exception)).ToProperty(this, nameof(RenderFailed), out _renderFailed);

            this.WhenAnyValue(
                x => x.SelectedBarcodeType,
                x => x.NormalBarcodeContent,
                x => x.MagicIsCustomCommand,
                x => x.CustomMagicCommand,
                x => x.MagicCheckboxID,
                x => x.MagicEntryID,
                x => x.MagicEntryText,
                (type, normalContent, isCustom, customCommand, checkID, entryID, entryText) =>
                {
                    switch (type)
                    {
                        case BarcodeType.Normal:
                            return !String.IsNullOrWhiteSpace(normalContent);
                        case BarcodeType.MagicCommand:
                            return !isCustom || !String.IsNullOrWhiteSpace(customCommand);
                        case BarcodeType.MagicCheckbox:
                            return !String.IsNullOrWhiteSpace(checkID);
                        case BarcodeType.MagicTextEntry:
                            return !String.IsNullOrWhiteSpace(entryID) && !String.IsNullOrWhiteSpace(entryText);
                        case BarcodeType.MagicClearEntry:
                            return !String.IsNullOrWhiteSpace(entryID);
                        default:
                            return false;
                    }
                })
                .ToProperty(this, nameof(DataFilled), out _dataFilled);

            this.WhenAnyValue(x => x.NormalBarcodeSelected, x => x.NormalBarcodeContent).Where(x => x.Item1)
                .Subscribe(_ => RenderBarcode?.Invoke());
            this.WhenAnyValue(x => x.MagicCommandSelected, x => x.MagicIsCustomCommand, x => x.SelectedMagicCommand, x => x.CustomMagicCommand).Where(x => x.Item1)
                .Subscribe(_ => RenderBarcode?.Invoke());
            this.WhenAnyValue(x => x.MagicCheckboxSelected, x => x.MagicCheckboxID).Where(x => x.Item1)
                .Subscribe(_ => RenderBarcode?.Invoke());
            this.WhenAnyValue(x => x.MagicEntrySelected, x => x.MagicEntryID, x => x.MagicEntryText).Where(x => x.Item1)
                .Subscribe(_ => RenderBarcode?.Invoke());
            this.WhenAnyValue(x => x.DataFilled)
                .Subscribe(_ => RenderBarcode?.Invoke());
            this.WhenAnyValue(x => x.SelectedExportSize, x => x.CustomWidth, x => x.CustomHeight)
                .Subscribe(_ => RenderBarcode?.Invoke());
        }

        private ObservableAsPropertyHelper<bool> _normalBarcodeSelected;
        public bool NormalBarcodeSelected => _normalBarcodeSelected.Value;

        private ObservableAsPropertyHelper<bool> _magicCommandSelected;
        public bool MagicCommandSelected => _magicCommandSelected.Value;

        private ObservableAsPropertyHelper<bool> _magicCheckboxSelected;
        public bool MagicCheckboxSelected => _magicCheckboxSelected.Value;

        private ObservableAsPropertyHelper<bool> _magicEntrySelected;
        public bool MagicEntrySelected => _magicEntrySelected.Value;

        private ObservableAsPropertyHelper<bool> _magicClearSelected;
        public bool MagicClearSelected => _magicClearSelected.Value;

        private ObservableAsPropertyHelper<bool> _customSizeSelected;
        public bool CustomSizeSelected => _customSizeSelected.Value;

        private ObservableAsPropertyHelper<bool> _dataFilled;
        public bool DataFilled => _dataFilled.Value;

        private ObservableAsPropertyHelper<bool> _renderFailed;
        public bool RenderFailed => _renderFailed.Value;

        public IEnumerable<BarcodeType> BarcodeTypes
        {
            get
            {
                return Enum.GetValues(typeof(BarcodeType)).Cast<BarcodeType>();
            }
        }

        private BarcodeType _selectedBarcodeType = BarcodeType.Normal;
        public BarcodeType SelectedBarcodeType
        {
            get => _selectedBarcodeType;
            set => this.RaiseAndSetIfChanged(ref _selectedBarcodeType, value);
        }

        public IEnumerable<MagicBarcodeCommands> MagicCommands
        {
            get
            {
                return Enum.GetValues(typeof(MagicBarcodeCommands)).Cast<MagicBarcodeCommands>();
            }
        }

        private MagicBarcodeCommands _selectedMagicCommand = MagicBarcodeCommands.Shutdown;
        public MagicBarcodeCommands SelectedMagicCommand
        {
            get => _selectedMagicCommand;
            set => this.RaiseAndSetIfChanged(ref _selectedMagicCommand, value);
        }

        public IEnumerable<ExportSize> ExportSizes
        {
            get
            {
                return Enum.GetValues(typeof(ExportSize)).Cast<ExportSize>();
            }
        }

        private ExportSize _selectedExportSize = ExportSize.PX500x200;
        public ExportSize SelectedExportSize
        {
            get => _selectedExportSize;
            set => this.RaiseAndSetIfChanged(ref _selectedExportSize, value);
        }

        private int _customWidth = 1000;
        [Range(100, int.MaxValue, ErrorMessage = "Width must be greater than {1}")]
        public int CustomWidth
        {
            get => _customWidth;
            set => this.RaiseAndSetIfChanged(ref _customWidth, value);
        }

        private int _customHeight = 250;
        [Range(150, int.MaxValue, ErrorMessage = "Height must be greater than {1}")]
        public int CustomHeight
        {
            get => _customHeight;
            set => this.RaiseAndSetIfChanged(ref _customHeight, value);
        }

        private string _normalBarcodeContent = "";
        public string NormalBarcodeContent
        {
            get => _normalBarcodeContent;
            set => this.RaiseAndSetIfChanged(ref _normalBarcodeContent, value);
        }

        private string _customMagicCommand = "";
        public string CustomMagicCommand
        {
            get => _customMagicCommand;
            set => this.RaiseAndSetIfChanged(ref _customMagicCommand, value);
        }

        private string _magicCheckboxID = "";
        public string MagicCheckboxID
        {
            get => _magicCheckboxID;
            set => this.RaiseAndSetIfChanged(ref _magicCheckboxID, value);
        }

        private string _magicEntryID = "";
        public string MagicEntryID
        {
            get => _magicEntryID;
            set => this.RaiseAndSetIfChanged(ref _magicEntryID, value);
        }

        private string _magicEntryText = "";
        public string MagicEntryText
        {
            get => _magicEntryText;
            set => this.RaiseAndSetIfChanged(ref _magicEntryText, value);
        }

        private bool _magicIsCustomCommand = false;
        public bool MagicIsCustomCommand
        {
            get => _magicIsCustomCommand;
            set => this.RaiseAndSetIfChanged(ref _magicIsCustomCommand, value);
        }

        private Image _image = null;
        public Image Image
        {
            get => _image;
            set => this.RaiseAndSetIfChanged(ref _image, value);
        }

        private string _renderingException = null;
        public string RenderingException
        {
            get => _renderingException;
            set => this.RaiseAndSetIfChanged(ref _renderingException, value);
        }

        private void OnExport()
        {
            ExportBarcode?.Invoke();
        }
    }
}
