using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Dapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MySqlConnector;
using WebApplicationTemplate.ActionFilter;
using WebApplicationTemplate.AppDB;
using WebApplicationTemplate.Entity;
using WebApplicationTemplate.JWT;

namespace WebApplicationTemplate.Controllers;
[Route("api/[controller]/[action]")]
[ApiController]
[Authorize]
public class UserController : Controller
{
    private readonly IMemoryCache _memoryCache;
    private readonly IConfiguration _config;
    private readonly ILogger<UserController> _logger;
    private AppDB.AppDB Db { get; }
    private readonly IOptionsSnapshot<JWTSettings> jwtsettingOpt;
    private readonly IOptionsSnapshot<DBDataBase> dbDataBaseOpt;
    public UserController(AppDB.AppDB db, IOptionsSnapshot<JWTSettings> jwtsettingOpt, ILogger<UserController> logger, IConfiguration config, IMemoryCache memoryCache, IOptionsSnapshot<DBDataBase> dbDataBaseOpt)
    {
        Db = db;
        this.jwtsettingOpt = jwtsettingOpt;
        _logger = logger;
        _config = config;
        _memoryCache = memoryCache;
        this.dbDataBaseOpt = dbDataBaseOpt;
    }

    [HttpPost]
    [AllowAnonymous]
    [NotTransaction]
    public async Task<ActionResult> UserLogin(string userName, string passWord)
    {
        if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(passWord))
            return BadRequest("Inputdata is Null");
        var userSearchResult =
            await Db.Connection.QueryFirstOrDefaultAsync<User>($"SELECT * FROM user WHERE UserName = @UserName",
                new { UserName = userName });
        if (userSearchResult == null)
            return NotFound("Not this User");
        if (userSearchResult.Password != passWord)
            return BadRequest("Error PassWord");
        var jwt = _memoryCache.GetOrCreate($"ID:{userSearchResult.ID}", (e) =>
        {
            e.AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(7);
            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, userSearchResult.ID.ToString()),
                new(ClaimTypes.Name, userSearchResult.UserName),
                new(ClaimTypes.Role, "user")
            };
            string? key = jwtsettingOpt.Value.SecrectKey;
            DateTime expire = DateTime.Now.AddSeconds(jwtsettingOpt.Value.ExpireSeconds);
            byte[] secBytes = Encoding.UTF8.GetBytes(key);
            var secKey = new SymmetricSecurityKey(secBytes);
            var credentials = new SigningCredentials(secKey, SecurityAlgorithms.HmacSha256Signature);
            var tokenDescriptor =
                new JwtSecurityToken(claims: claims, expires: expire, signingCredentials: credentials);
            string jwt = new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
            _logger.LogDebug($"{userSearchResult.UserName}登陆成功");
            return jwt;
        });
        return Ok(jwt);
    }

    [HttpPost]
    [Authorize(Roles = "admin")]
    [NotRateTransaction]
    public ActionResult AddAdmin()
    {
        var result = User.Claims.ToList();
        result.Add(new(ClaimTypes.Role, "admin"));
        string? key = jwtsettingOpt.Value.SecrectKey;
        DateTime expire = DateTime.Now.AddSeconds(jwtsettingOpt.Value.ExpireSeconds);
        byte[] secBytes = Encoding.UTF8.GetBytes(key);
        var secKey = new SymmetricSecurityKey(secBytes);
        var credentials = new SigningCredentials(secKey, SecurityAlgorithms.HmacSha256Signature);
        var tokenDescriptor = new JwtSecurityToken(claims: result, expires: expire, signingCredentials: credentials);
        string jwt = new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
        _logger.LogDebug($"{result[1].Value}授权管理员成功");
        return Ok(jwt);
    }

    [HttpPost]
    [Authorize(Roles = "admin")]
    public ActionResult ShowJwtMessage()
    {
        string id = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
        string userName = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
        IEnumerable<Claim> roleClaims = User.FindAll(ClaimTypes.Role);
        string roleNames = string.Join(',', roleClaims.Select(x => x.Value));
        return Ok(new { ID = id, UserName = userName, roleNames = roleNames });
    }

    [HttpPost]
    //[Authorize(Roles = "admin")]
    public async Task<ActionResult> CreateTable()
    {
        if (!_config.GetValue("DBSetting:Init", false))
        {
            return BadRequest("数据库初始化已被禁用");
        }
        var result = await Db.Connection.CreateTable<User>(dbDataBaseOpt.Value.UserTable);
        if (!result)
            return BadRequest("failed");
        return Ok(result);
    }
}
