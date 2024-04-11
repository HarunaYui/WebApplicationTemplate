using System.Security.Claims;
using Dapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using MySqlConnector;
using WebApplicationTemplate.ActionFilter;
using WebApplicationTemplate.AppDB;
using WebApplicationTemplate.Entity;
using WebApplicationTemplate.JWT;

namespace WebApplicationTemplate.Controllers;

/// <summary>
/// 用户控制器
/// </summary>
[Route("api/[controller]/[action]")]
[ApiController]
[Authorize]
public class UserController : Controller
{
    private readonly IMemoryCache _memoryCache;
    private readonly IConfiguration _config;
    private readonly ILogger<UserController> _logger;
    private readonly IOptionsSnapshot<JwtSettings> _jwtsettingOpt;
    private readonly IOptionsSnapshot<DBDataBase> _dbDataBaseOpt;
    private readonly MySqlConnection _connection;
    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="jwtsettingOpt"></param>
    /// <param name="logger"></param>
    /// <param name="config"></param>
    /// <param name="memoryCache"></param>
    /// <param name="dbDataBaseOpt"></param>
    /// <param name="cnn"></param>
    public UserController(IOptionsSnapshot<JwtSettings> jwtsettingOpt, ILogger<UserController> logger, IConfiguration config, IMemoryCache memoryCache, IOptionsSnapshot<DBDataBase> dbDataBaseOpt, MySqlConnection cnn)
    {
        _jwtsettingOpt = jwtsettingOpt;
        _logger = logger;
        _config = config;
        _memoryCache = memoryCache;
        _dbDataBaseOpt = dbDataBaseOpt;
        _connection = cnn;
    }

    /// <summary>
    /// 用户登录
    /// </summary>
    /// <param name="userName"></param>
    /// <param name="passWord"></param>
    /// <returns></returns>
    [HttpPost]
    [AllowAnonymous]
    [NotTransaction]
    public async Task<ActionResult<string>> UserLogin(string userName, string passWord)
    {
        if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(passWord))
            return BadRequest("Inputdata is Null");
        var userSearchResult =
            await _connection.QueryFirstOrDefaultAsync<User>($"SELECT * FROM user WHERE UserName = @UserName",
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
            string? key = _jwtsettingOpt.Value.SecrectKey;
            DateTime expire = DateTime.Now.AddSeconds(_jwtsettingOpt.Value.ExpireSeconds);
            string jwt = JwtHelper.JwtCreate(claims,key,expire);
            _logger.LogDebug($"{userSearchResult.UserName}登陆成功");
            return jwt;
        });
        return Ok(jwt);
    }

    /// <summary>
    /// 添加管理员
    /// </summary>
    /// <returns></returns>
    [HttpPost]
    [Authorize(Roles = "admin")]
    [NotRateTransaction]
    public ActionResult AddAdmin()
    {
        var result = User.Claims.ToList();
        result.Add(new(ClaimTypes.Role, "admin"));
        string? key = _jwtsettingOpt.Value.SecrectKey;
        DateTime expire = DateTime.Now.AddSeconds(_jwtsettingOpt.Value.ExpireSeconds);
        string jwt = JwtHelper.JwtCreate(result, key, expire);
        _logger.LogDebug($"{result[1].Value}授权管理员成功");
        return Ok(jwt);
    }

    /// <summary>
    /// 显示jwt
    /// </summary>
    /// <returns></returns>
    [HttpPost]
    [Authorize(Roles = "admin")]
    public ActionResult<Claim> ShowJwtMessage()
    {
        string id = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
        string userName = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
        IEnumerable<Claim> roleClaims = User.FindAll(ClaimTypes.Role);
        string roleNames = string.Join(',', roleClaims.Select(x => x.Value));
        _logger.LogDebug($"{userName}查询了jwt信息");
        return Ok(new { ID = id, UserName = userName, roleNames = roleNames });
    }

    /// <summary>
    /// 初始化数据库
    /// </summary>
    /// <returns></returns>
    [HttpPost]
    [Authorize(Roles = "admin")]
    public async Task<ActionResult> TableInit()
    {
        if (!_config.GetValue("DBSetting:Init", false))
        {
            return BadRequest("数据库初始化已被禁用");
        }
        var result = await _connection.CreateTable<User>(_dbDataBaseOpt.Value.UserTable);
        if (!result)
            return BadRequest("failed");
        string userName = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        _logger.LogDebug($"{userName}创建了数据库");
        return Ok(result);
    }
}
