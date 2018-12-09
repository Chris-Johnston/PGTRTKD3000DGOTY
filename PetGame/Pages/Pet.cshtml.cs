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
        private readonly ActivityService activity;

        public PetStatusModel(SqlManager sql)
        {
            this.sql = sql;
            login = new LoginService(this.sql);
            pet = new PetService(this.sql);
            activity = new ActivityService(this.sql);
        }

        public User CurrentUser { get; private set; } = null;
        public PetStatus CurrentPetStatus { get; private set; } = null;

        [HttpGet("{id}")]
        public void OnGet(ulong id)
        {
            CurrentUser = login.GetUserFromContext(HttpContext.User);
            // TODO: need to check that the current user owns this pet, and show an error page accordingly
            if (CurrentUser != null)
            {
                CurrentPetStatus = pet.GetPetStatusById(id);
                if (CurrentPetStatus == null)
                {
                    // pet not found, or wrong owner
                    Response.StatusCode = 404;
                    return;
                }
                // make a userlogin activity
                activity.MakeActivityForPet(CurrentPetStatus.Pet.PetId, ActivityType.UserLogin);
            }
            else
            {
                // http unauthorized
                Response.StatusCode = 401;
                return;
            }
        }
    }
}
