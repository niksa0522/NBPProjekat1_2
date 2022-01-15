using Microsoft.AspNetCore.Mvc;
using NBP_1_2.Models;
using System.Diagnostics;
using Neo4j;
using ServiceStack.Redis;
using NBP_1_2.Redis;


namespace NBP_1_2.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly RedisClient _redisClient;

        public HomeController(ILogger<HomeController> logger, RedisClient redisClient)
        {
            _logger = logger;
            _redisClient = redisClient;
        }

        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("username") == null)
            {
                return Redirect("/Login/Index");
            }
            HomePageInfoModel model = new HomePageInfoModel();
            model.username= HttpContext.Session.GetString("username");
            model.UserType= HttpContext.Session.GetString("userType");
            model.onlinePlayers = RedisLogic.CurrentOnlineUsers(_redisClient);
            return View(model);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        public IActionResult Logout()
        {
            if (HttpContext.Session.GetString("username") != null)
            {
                RedisLogic.UserLogout(_redisClient, HttpContext.Session.GetString("username"));

            }

            HttpContext.Session.Clear();

            return Redirect("/Login/Index");
        }
        public void Logoff()
        {
            if (HttpContext.Session.GetString("username") != null)
            {
                RedisLogic.UserLogout(_redisClient, HttpContext.Session.GetString("username"));
                HttpContext.Session.Clear();
            }
        }
    }
}
