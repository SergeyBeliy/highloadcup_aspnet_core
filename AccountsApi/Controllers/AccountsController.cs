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
using Newtonsoft.Json;

namespace AccountsApi.Controllers
{
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private ILogger _logger;
        private IDatabase _database;
        public AccountsController(ILogger logger, IDatabase database)
        {
            _logger = logger;
            _database = database;
        }

        [HttpGet]
        [Route("accounts/filter")]
        public ActionResult<AccountFilterResponse> Filter()
        {
            try
            {
                var query = RequestParser.ParseFilter(Request.QueryString.Value);
                if (!RequestParser.ValidateFilterQuery(query))
                {
                    return BadRequest();
                }
                var accounts = _database.FilterQuery(query);
                return new AccountFilterResponse(accounts, query);
            }
            catch (Exception ex)
            {
                _logger.Error("Unknown error", ex);
                throw;
            }

        }

        [HttpGet]
        [Route("accounts/group")]
        public ActionResult<AccountGroupResponse> Group()
        {
            try
            {
                var query = RequestParser.ParseGroup(Request.QueryString.Value);
                if (!RequestParser.ValidateGroupQuery(query))
                {
                    return BadRequest();
                }
                var groups = _database.GroupQuery(query);
                var t = new AccountGroupResponse
                {
                    Groups = groups.ToArray(),
                };
                return t;
            }
            catch (Exception ex)
            {
                _logger.Error("Unknown error", ex);
                throw;
            }
        }

        [HttpGet]
        [Route("accounts/{id}/recommend")]
        public ActionResult<AccountRecommendResponse> Recommended(long id)
        {
            try
            {
                var query = RequestParser.ParseRecommend(Request.QueryString.Value);
                query.AccountId = id;
                if (!RequestParser.ValidateRecommendQuery(query))
                {
                    return BadRequest();
                }
                var recommendations = _database.RecommendQuery(query);
                if (recommendations == null)
                {
                    return NotFound();
                }
                return new AccountRecommendResponse(recommendations);
            }
            catch (Exception ex)
            {
                _logger.Error("Unknown error", ex);
                throw;
            }

        }

        [HttpGet]
        [Route("accounts/{id}/suggest")]
        public ActionResult<AccountSuggestResponse> Suggest(long id)
        {
            try
            {
                var query = RequestParser.ParseSuggest(Request.QueryString.Value);
                query.AccountId = id;
                if (!RequestParser.ValidateSuggestQuery(query))
                {
                    return BadRequest();
                }
                var suggestions = _database.SuggestQuery(query);
                if (suggestions == null)
                {
                    return NotFound();
                }
                return new AccountSuggestResponse(suggestions);
            }
            catch (Exception ex)
            {
                _logger.Error("Unknown error", ex);
                throw;
            }

        }

        [HttpPost]
        [Route("accounts/new")]
        public async Task<ActionResult> New([FromBody] Account account)
        {
            try
            {
                await _database.Put(account);
                Response.StatusCode = 201;
                return Content("{}");
            }
            catch (Exception ex)
            {
                _logger.Error("Exception in New action", ex);
                return BadRequest();
            }
        }

        [HttpPost]
        [Route("accounts/{id}")]
        public async Task<ActionResult> Update(long id, [FromBody] Account account)
        {
            try
            {
                var success = await _database.Update(account);
                if (success)
                {
                    return NotFound();
                }
                Response.StatusCode = 202;
                return Content("{}");
            }
            catch (Exception ex)
            {
                _logger.Error("Exception in Update action", ex);
                return BadRequest();
            }
        }

        [HttpPost]
        [Route("accounts/likes")]

        public async Task<ActionResult> Likes([FromBody] LikesUpdateModel model)
        {
            try
            {
                await _database.UpdateLikes(model.Likes);
                Response.StatusCode = 202;
                return Content("{}");
            }
            catch (Exception ex)
            {
                _logger.Error("Exception in Update action", ex);
                return BadRequest();
            }
        }

    }
}