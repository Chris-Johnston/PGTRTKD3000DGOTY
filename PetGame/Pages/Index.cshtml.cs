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
    public class IndexModel : PageModel
    {
        private readonly SqlManager sql;
        private readonly LoginService login;

        public User CurrentUser { get; private set; } = null;

        public List<Pet> SamplePets
        {
            get =>
                new List<Pet>()
                {
                    new Pet()
                    {
                        PetId = 1,
                        Birthday = DateTime.Now,
                        IsDead = false,
                        Name = "The 1st Pet",
                        Endurance = 100,
                        Strength = 100,
                        UserId = 1
                    },
                    new Pet()
                    {
                        PetId = 2,
                        Birthday = DateTime.Now,
                        IsDead = false,
                        Name = "The 2nd Pet",
                        Endurance = 100,
                        Strength = 100,
                        UserId = 1
                    }
                };
        }

        public IndexModel(SqlManager sql)
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