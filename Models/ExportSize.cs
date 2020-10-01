using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace BarcodeGenerator.Models
{
    public enum ExportSize
    {
        [Description("500 x 200")]
        PX500x200,
        [Description("1200 x 400")]
        PX1200x400,
        [Description("Custom Size")]
        Custom,
    }
}
