using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using AccountsApi.Database.Infrastructure;
using AccountsApi.Infrastructure;
using AccountsApi.Infrastructure.Database;
using AccountsApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;

namespace AccountsApi.Controllers {
    [ApiController]
    public class AccountsController : ControllerBase {
        private ILogger _logger;
        private IDatabase _database;
        public AccountsController (ILogger logger, IDatabase database) {
            _logger = logger;
            _database = database;
        }

        [HttpGet]
        [Route ("accounts/filter")]
        public ActionResult<AccountFilterResponse> Filter () {
            try {
                var query = RequestParser.ParseFilter (Request.QueryString.Value);
                if (!RequestParser.ValidateFilterQuery (query)) {
                    return BadRequest ();
                }
                var accounts = _database.FilterQuery (query);
                return new AccountFilterResponse (accounts);
            } catch (Exception ex) {
                _logger.Error ("Unknown error", ex);
                throw;
            }

        }

        [HttpGet]
        [Route ("accounts/group")]
        public ActionResult<AccountGroupResponse> Group () {
            try {
                var query = RequestParser.ParseGroup (Request.QueryString.Value);
                if (!RequestParser.ValidateGroupQuery (query)) {
                    return BadRequest ();
                }
                var groups = _database.GroupQuery(query);
                var t = new AccountGroupResponse{
                    Groups = groups.ToArray(),
                };
                return t;
            } catch (Exception ex) {
                _logger.Error ("Unknown error", ex);
                throw;
            }
        }
    }
}