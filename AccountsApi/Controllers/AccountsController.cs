using System.Collections.Generic;
using System.Collections.Specialized;
using Microsoft.AspNetCore.Mvc;

namespace AccountsApi.Controllers
{
    [ApiController]
    public class AccountsController: ControllerBase
    {
        [HttpGet]
        [Route("accounts/filter")]
        public ActionResult<IEnumerable<string>> Filter([FromQuery] NameValueCollection query)
        {
            var dict = Microsoft.AspNetCore.WebUtilities.QueryHelpers.ParseQuery(Request.QueryString.Value);
            return dict.Keys;
        }
    }
}