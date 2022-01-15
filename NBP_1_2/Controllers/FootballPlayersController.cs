using Microsoft.AspNetCore.Mvc;
using System.Linq;
using NBP_1_2.Neo4J;
using NBP_1_2.Redis;
using NBP_1_2.Models;
using Neo4jClient;
using ServiceStack.Redis;

namespace NBP_1_2.Controllers
{
    public class FootballPlayersController : Controller
    {
        private readonly IGraphClient client;
        private readonly RedisClient redisClient;
        public FootballPlayersController(IGraphClient client, RedisClient redisClient)
        {
            this.client = client;
            this.redisClient = redisClient;
        }
        public IActionResult Index(string? teamName, string? playerName)
        {
            if (teamName == null && playerName == null)
            {
                IEnumerable<FootballPlayer> players = Neo4JLogic.GetFootballPlayers(client);

                return View(players);
            }
            if(teamName!=null && playerName != null)
            {
                IEnumerable<FootballPlayer> players = Neo4JLogic.GetPlayersTeamAndName(client,teamName,playerName);

                return View(players);
            }
            if (teamName != null)
            {
                IEnumerable<FootballPlayer> players = Neo4JLogic.GetPlayersTeam(client, teamName);

                return View(players);
            }
            else
            {
                IEnumerable<FootballPlayer> players = Neo4JLogic.GetPlayers(client, playerName);

                return View(players);
            }
        }
        public IActionResult Edit(string? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            FootballPlayer list = Neo4J.Neo4JLogic.GetPlayer(client, id);
            if (list == null)
            {
                return NotFound();
            }
            return View(list);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string? id, [Bind("id,name,birthday,pozition,number,value")] FootballPlayer player)
        {

            if (id == null)
            {
                return NotFound();
            }
            ModelState.Remove("playesIn");
            ModelState.Remove("numberOfPlayers");
            if (ModelState.IsValid)
            {
                bool check = Neo4J.Neo4JLogic.ModifyFootballPlayer(client, player.id, player.name, player.birthday, player.pozition, player.value, player.number);
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
        public async Task<IActionResult> Add([Bind("id,name,birthday,pozition,number,value")] FootballPlayer player)
        {
            ModelState.Remove("id");
            ModelState.Remove("playesIn");
            ModelState.Remove("numberOfPlayers");
            if (ModelState.IsValid)
            {
                Neo4J.Neo4JLogic.AddFootballPlayer(client, player.name, player.birthday, player.pozition, player.value, player.number);
                return RedirectToAction(nameof(Index));
                
            }
            return View();

        }

        [HttpPost]
        public async Task<IActionResult> AddToTeam(string? idTeam, [Bind("id")] FootballPlayer player)
        {

            if (idTeam == null)
            {
                return NotFound();
            }
            bool check = Neo4J.Neo4JLogic.AddFootballPlayerToTeam(client, player.id, idTeam);
            if (check)
            {

                return RedirectToAction(nameof(Index));
            }
            else
            {
                return RedirectToAction("Edit", new {id=player.id});
            }
            

        }
        [HttpPost]
        public async Task<IActionResult> RemoveFromTeam(string? id,[Bind("id")] FootballPlayer player)
        {

            if (id == null)
            {
                return NotFound();
            }
            Neo4J.Neo4JLogic.RemoveFootballPlayerFromAnyTeam(client, player.id);
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Info(string? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            FootballPlayer list = Neo4J.Neo4JLogic.GetPlayer(client, id);
            list.numOfPlayers=RedisLogic.GetNumPlayerHaveInTeam(redisClient, id);
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
            Neo4JLogic.DeleteFootballPlayer(client, id);
            return RedirectToAction(nameof(Index));
        }
    }
}
