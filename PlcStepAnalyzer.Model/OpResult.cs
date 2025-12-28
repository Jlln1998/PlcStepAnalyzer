using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlcStepAnalyzer.Model
{
    public class OpResult
    {
        public bool IsSuccess { get; set; }

        public string Message { get; set; } = string.Empty;
    }

    public class OpResult<T>: OpResult
    {
        public T? Value { get; set; } = default;
    }
}
