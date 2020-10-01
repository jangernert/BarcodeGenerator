using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace BarcodeGenerator.Models
{
    public enum BarcodeType
    {
        [Description("Normal Barcode")]
        Normal,
        [Description("Magic Barcode Command")]
        MagicCommand,
        [Description("Magic Barcode to toggle Checkbox")]
        MagicCheckbox,
        [Description("Magic Barcode to fill Entry")]
        MagicTextEntry,
    }
}
