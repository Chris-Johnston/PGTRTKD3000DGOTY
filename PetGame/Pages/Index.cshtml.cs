using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PetGame.Core;
using PetGame.Models;
using PetGame.Services;

namespace PetGame.Pages
{
    public class IndexModel : PageModel
    {
        private readonly SqlManager sql;
        private readonly LoginService login;
        private readonly PetService pet;

        /// <summary>
        ///     The current user that is signed in.
        /// </summary>
        public User CurrentUser { get; private set; } = null;

        /// <summary>
        ///     The pets that belong to the current user.
        /// </summary>
        // init as an empty list to avoid null ref exception
        public IEnumerable<PetStatus> Pets { get; set; }
            = new List<PetStatus>();

        public IndexModel(SqlManager sql)
        {
            this.sql = sql;
            login = new LoginService(this.sql);
            pet = new PetService(this.sql);
        }

        public void OnGet()
        {
            CurrentUser = login.GetUserFromContext(HttpContext.User);
            if (CurrentUser != null)
            {
                Pets = pet.GetPetStatusList(CurrentUser.UserId);
            }
            else
            {
                // HTTP Unauthorized
                Response.StatusCode = 401;
            }
        }
    }
}