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
    public class PetStatusModel : PageModel
    {
        private readonly SqlManager sql;
        private readonly LoginService login;
        private readonly PetService pet;

        public PetStatusModel(SqlManager sql)
        {
            this.sql = sql;
            login = new LoginService(this.sql);
            pet = new PetService(this.sql);
        }
        
        public User CurrentUser { get; private set; } = null;
        public Pet CurrentPet { get; private set; } = null;

        [HttpGet("{id}")]
        public void OnGet(ulong id)
        {
            CurrentUser = login.GetUserFromContext(HttpContext.User);
            // TODO: need to check that the current user owns this pet, and show an error page accordingly
            if (CurrentUser != null)
            {
                CurrentPet = pet.GetPetById(id);
                if (CurrentPet == null)
                {
                    // pet not found, or wrong owner
                    Response.StatusCode = 404;
                    return;
                }
            }
            else
            {
                // http unauthorized
                Response.StatusCode = 404;
                return;
            }
            
        }
    }
}