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
    public class RaceModel : PageModel
    {
        //pet strength, pet endurance,
        //create race entry
        //activity, race and pet tables

        public Pet CurrentPet { get; private set; } = null;
        public User CurrentUser { get; private set; } = null;

        private readonly SqlManager sqlManager;
        private readonly PetService petService;
        private readonly LoginService loginService;

        public RaceModel(SqlManager sql)
        {
            sqlManager = sql;
            petService = new PetService(sql);
            loginService = new LoginService(sql);
        }

        [HttpGet("{id}")]
        public void OnGet(ulong id)
        {
            //null checks performed on previous page
            CurrentUser = loginService.GetUserFromContext(HttpContext.User);
            CurrentPet = petService.GetPetById(id);
        }
    }
}