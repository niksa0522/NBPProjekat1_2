using Microsoft.AspNetCore.Mvc;
using System.Linq;
using NBP_1_2.Neo4J;
using NBP_1_2.Redis;
using NBP_1_2.Models;
using Neo4jClient;
using ServiceStack.Redis;

namespace NBP_1_2.Controllers
{
    public class FootballTeamController : Controller
    {
        private readonly IGraphClient client;
        private readonly RedisClient redisClient;
        public FootballTeamController(IGraphClient client, RedisClient redisClient)
        {
            this.client = client;
            this.redisClient = redisClient;
        }
        public async Task<IActionResult> Index(string? teamName)
        {
            if (teamName == null)
            {
                IEnumerable<FootballTeam> players = Neo4JLogic.GetFootballTeams(client);
                return View(players);
            }
            else
            {
                IEnumerable<FootballTeam> players = Neo4JLogic.GetTeams(client, teamName);
                return View(players);
            }
        }
        public IActionResult Edit(string? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            FootballTeam list = Neo4J.Neo4JLogic.GetTeam(client, id);
            if (list == null)
            {
                return NotFound();
            }
            return View(list);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string? id, [Bind("id,name,homeTown,stadiumName")] FootballTeam player)
        {

            if (id == null)
            {
                return NotFound();
            }
            ModelState.Remove("players");
            if (ModelState.IsValid)
            {
                bool check = Neo4J.Neo4JLogic.ModifyFootballTeam(client, player.id, player.name, player.homeTown, player.stadiumName);
                if (check)
                {

                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    return NotFound();
                }
            }
            return View(player);

        }
        public async Task<IActionResult> Add()
        {
            return View();

        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add([Bind("id,name,homeTown,stadiumName")] FootballTeam player)
        {
            ModelState.Remove("id");
            ModelState.Remove("players");
            if (ModelState.IsValid)
            {
                Neo4J.Neo4JLogic.AddFootballTeam(client, player.name, player.homeTown, player.stadiumName);
                return RedirectToAction(nameof(Index));

            }
            return View();

        }

        public async Task<IActionResult> Info(string? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            FootballTeam list = Neo4J.Neo4JLogic.GetTeam(client, id);
            if (list == null)
            {
                return NotFound();
            }
            return View(list);
        }
        public async Task<IActionResult> Delete(string? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            Neo4JLogic.DeleteFootballTeam(client, id);
            return RedirectToAction(nameof(Index));
        }
        [HttpPost]
        public async Task<IActionResult> FollowTeam(string? id, [Bind("id,name,homeTown,stadiumName")] FootballTeam player)
        {
            if (id == null)
            {
                return NotFound();

            }
            else
            {
                string username = HttpContext.Session.GetString("username");
                Neo4JLogic.FollowTeam(client, username, id);
                return RedirectToAction("Info", new { id = player.id });
            }
        }
        [HttpPost]
        public async Task<IActionResult> UnfollowTeam(string? id, [Bind("id,name,homeTown,stadiumName")] FootballTeam player)
        {
            if (id == null)
            {
                return NotFound();

            }
            else
            {
                string username = HttpContext.Session.GetString("username");
                Neo4JLogic.UnfollowTeam(client, username, id);
                return RedirectToAction("Info", new { id = player.id });
            }
        }
    }
}
