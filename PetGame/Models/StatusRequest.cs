namespace PetGame.Models
{
    /// <summary>
    /// Used to encapsulate the id in order to avoid issues with identical
    /// function signatures in PetController and UserController
    /// </summary>
    public class StatusRequest
    {
        public ulong id { get; set; }
    }
}
