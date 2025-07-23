using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StreamingApplication.Dtos
{
    public class ResultDto<T>
    {
        public bool? IsSuccess { get; init; }
        public string? Error { get; init; }
        public T? Data { get; init; }
        public string? ErrorCode { get; init; }
    }
}
