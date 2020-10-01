using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace BarcodeGenerator.Models
{
    public enum MagicBarcodeCommands
    {
        [Description("Shutdown")]
        Shutdown,
        [Description("Calibrate")]
        Calibrate,
        [Description("Pause")]
        Pause,
        [Description("Dimension")]
        Dimension,
        [Description("Capture")]
        Capture,
        [Description("Save")]
        Save,
        [Description("Force Save")]
        ForceSave,
        [Description("Print")]
        Print,
        [Description("Tare Scale")]
        TareScale,
    }
}
