using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PetGame.Core;
using PetGame.Models;

namespace PetGame.Pages
{
    public class LeaderboardModel : PageModel
    {
        // initialize to an empty list
        public IEnumerable<LeaderboardEntry> LeaderboardItems { get; set; } = new List<LeaderboardEntry>();

        private readonly SqlManager sqlManager;
        private readonly LeaderboardService leaderboard;

        public LeaderboardModel(SqlManager sql)
        {
            sqlManager = sql;
            leaderboard = new LeaderboardService(sql);
        }

        public void OnGet()
        {
            // update this on all get requests
            LeaderboardItems = leaderboard.GetLeaderboardEntries(0, 10);
        }
    }
}