using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace PetGame.Pages
{
    public class LoginModel : PageModel
    {
        public string ErrorType { get; set; } = null;

        public void OnGet(string Error="")
        {
            if (string.IsNullOrWhiteSpace(Error))
                ErrorType = null;
            else
            {
                ErrorType = Error;
            }
        }
    }
}