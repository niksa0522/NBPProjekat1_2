using Microsoft.AspNetCore.Mvc;
using System.Linq;
using NBP_1_2.Neo4J;
using NBP_1_2.Redis;
using NBP_1_2.Models;
using Neo4jClient;
using ServiceStack.Redis;

namespace NBP_1_2.Controllers
{
    public class LoginController : Controller
    {

        private readonly IGraphClient client;
        private readonly RedisClient redisClient;
        public LoginController(IGraphClient client,RedisClient redisClient)
        {
            this.client = client;
            this.redisClient = redisClient;
        }
        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("username") != null)
            {
                return Redirect("/Home/Index");
            }
            return View();
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(Player player)
        {
            Player p = NBP_1_2.Neo4J.Neo4JLogic.Login(client, player.username, player.password);
            if (p == null)
            {
                player.LoginErrorMessage = "Bad username or password";
                return View("Index",player);
            }
            else
            {
                HttpContext.Session.SetString("username", p.username);
                HttpContext.Session.SetString("userType", p.userType);
                RedisLogic.UserLogin(redisClient, player.username);
                return Redirect("/Home/Index");
            }

        }
        [HttpPost("Register")]
        public async Task<IActionResult> Register(Player player)
        {
            Player p = NBP_1_2.Neo4J.Neo4JLogic.Register(client, player.username, player.password);
            if (p == null)
            {
                player.LoginErrorMessage = "Bad username or password";
                return View("Index", player);
            }
            else
            {
                RedisLogic.AddUserToLeaderboard(redisClient, p.username);
                RedisLogic.UserLogin(redisClient, player.username);
                HttpContext.Session.SetString("username", p.username);
                HttpContext.Session.SetString("userType", p.userType);
                return RedirectToPage("/Home/Index");
            }
        }
    }
}
