using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Blogger.Responses
{
    /* {
          "access_token" : "ya29.AHES6ZQQOug83DDJabhEi2d428WRDm-JOKOjfib8gL1Uc8Y",
          "token_type" : "Bearer",
          "expires_in" : 3600,
          "refresh_token" : "1/xLYECjl0i_nUlT3kAy1-wkhQeTIo4xj_CKvnrcieQD8"
        } */
    public class AccessTokensResponse
    {
        public string access_token {get; set; }
        public string token_type { get; set; }
        public int expires_in { get; set; }
        public string refresh_token { get; set; }
    }
}
