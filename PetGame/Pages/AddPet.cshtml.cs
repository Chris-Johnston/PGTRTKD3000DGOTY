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
    public class AddPetModel : PageModel
    {
        private readonly SqlManager sql;
        private readonly LoginService login;

        public User CurrentUser { get; private set; } = null;

        public AddPetModel(SqlManager sql)
        {
            this.sql = sql;
            login = new LoginService(this.sql);
        }

        public void OnGet()
        {
            CurrentUser = login.GetUserFromContext(HttpContext.User);
        }
    }
}