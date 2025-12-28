using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlcStepAnalyzer.Config
{
    public class DataConfig
    {
        public int DefaultStartRow { get; set; } = 1;
        public int DefaultStartCol { get; set; } = 1;
        public int DefaultLineTime { get; set; } = 10;
        public string ThemeColor { get; set; } = "Light";
    }
}
