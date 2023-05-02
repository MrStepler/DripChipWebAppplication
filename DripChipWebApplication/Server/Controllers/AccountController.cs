using DripChipWebApplication.Server.Models.ResponseModels.Account;
using DripChipWebApplication.Server.Services.ServiceInterfaces;
using DripChipWebApplication.Server.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;

namespace DripChipWebApplication.Server.Controllers
{
    [Authorize]
    [ApiController]
    public class AccountController : Controller
    {
        IAccountService accountService;
        IAnimalsService animalsService;
        public AccountController(IAccountService service, IAnimalsService animalsService)
        {
            this.accountService = service;
            this.animalsService = animalsService;
        }
        [Route("accounts/{id}")]
        [HttpGet]
        public ActionResult<AccountDTO> GetAccountById(int? id) //Ready
        {

            if (id <= 0 || id == null)
            {
                return BadRequest();
            }
            if (accountService.GetAccount((int)id) == null)
            {
                return StatusCode(404);
            }
            return Ok(accountService.GetAccount((int)id));
        }
        [Route("accounts/search")]
        [HttpGet] //Ready
        public ActionResult<AccountDTO[]> SearchAccount([FromQuery] string? firstName, [FromQuery] string? lastName, [FromQuery] string? email, [FromQuery] int? from = 0, [FromQuery] int? size = 10)
        {
            if (from < 0)
            {
                return StatusCode(400);
            }
            if (from == null)
            {
                from = 0;
            }
            if (size == null)
            {
                size = 10;
            }
            if (size <= 0)
            {
                return StatusCode(400);
            }
            return Ok(accountService.SearchAccounts(firstName, lastName, email, (int)from, (int)size));
        }
        [Route("registration")]
        [HttpPost]
        public ActionResult<AccountDTO> RegisterAccount(AccountRegistrationDTO account) //Ready
        {
            if (HttpContext.User.Identity.Name != "guest")
            {
                return StatusCode(403);
            }
            if (!IsValideData(account))
            {
                return StatusCode(400);
            }
            if (accountService.GetAccountByEmail(account.email) != null)
            {
                return StatusCode(409);
            }
            return Created("", accountService.AddAccount(account));
        }

        [Route("accounts/{accountId}")]
        [HttpPut]
        public ActionResult<AccountDTO> EditAccount(int accountId, [FromBody] AccountRegistrationDTO account)
        {
            if (HttpContext.User.Identity.Name == "guest")
            {
                return StatusCode(401);
            }
            if (accountService.GetAccount((int)accountId) == null)
            {
                return StatusCode(403);
            }
            if (accountService.GetAccount((int)accountId).Email != HttpContext.User.Identity.Name)
            {
                return StatusCode(403);
            }
            
            if (accountService.GetAccountByEmail(account.email) != null && accountService.GetAccount((int)accountId).Email != account.email) //????????????????
            {
                return StatusCode(409);
            }
            if (!IsValideData(account))
            {
                return StatusCode(400);
            }
            return Ok(accountService.EditAccount(accountId, account));
        }

        [Route("accounts/{id}")]
        [HttpDelete] //READY
        public IActionResult DeleteAccount(int? id) 
        {
            if (HttpContext.User.Identity.Name == "guest")
            {
                return StatusCode(401);
            }
            if (id == null || id <= 0)
            {
                return StatusCode(400);
            }
            if (animalsService.SearchAnimal(null,null,chipperId:id, null, null, null,0,10) != null) 
            {
                return StatusCode(400);
            }
            if (accountService.GetAccount((int)id) == null) 
            {
                return StatusCode(403);
            }
            if (accountService.GetAccount((int)id).Email != HttpContext.User.Identity.Name)
            {
                return StatusCode(403);
            }
            
            accountService.DeleteAccount((int)id);
            return Ok();
        }
        private bool IsValideData(AccountRegistrationDTO account)
        {
            const string emailPattern = @"^(?("")(""[^""]+?""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
                @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9]{2,17}))$";
            if (string.IsNullOrWhiteSpace(account.firstName) || string.IsNullOrEmpty(account.firstName))
            {
                return false;
            }
            if (string.IsNullOrWhiteSpace(account.lastName) || string.IsNullOrEmpty(account.lastName))
            {
                return false;
            }
            if (string.IsNullOrWhiteSpace(account.password) || string.IsNullOrEmpty(account.password))
            {
                return false;
            }
            if (string.IsNullOrWhiteSpace(account.email) || string.IsNullOrEmpty(account.email) || !Regex.IsMatch(account.email, emailPattern, RegexOptions.IgnoreCase))
            {
                return false;
            }
            return true;
        }
    }
}