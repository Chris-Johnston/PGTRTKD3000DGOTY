using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace PetGame.Controllers
{
    /// <summary>
    ///     Returns with the corresponding image for a given image ID.
    /// </summary>
    [Route("api/[controller]")]
    public class ImageController : Controller
    {
        // note: because these images are hard-coded and we never expect them to change,
        // it's both easier and faster to have a constant Dictionary of all the pet images,
        // instead of making a new table in the database.

        /// <summary>
        ///     Gets the number of images
        /// </summary>
        /// <returns>The count, boxed into json</returns>
        /// GET /api/Images/count
        [HttpGet("count")]
        public IActionResult GetImageCount()
        {
            // return the number of images boxed in json (w/ anonymous type)
            return Json(new { Count = PetImages.Count });
        }

        /// <summary>
        /// Gets an image by it's Id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public IActionResult GetImage(int id)
        {
            // don't care about validation, if it's invalid
            // then just get the default image
            // this method returns the image path and the MIME type.
            var image = PetImages.GetImageById(id);
            return File(image.Path, image.MIME);
        }
    }
}
