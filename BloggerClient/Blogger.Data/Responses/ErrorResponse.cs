using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Blogger.Data.Responses
{
    public class ErrorResponse
    {
        public ServerErrorResponse RawResult { get; set; }

        public Exception Error { get; set; }
    }
}
