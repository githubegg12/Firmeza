using Microsoft.AspNetCore.Mvc;
using System;

namespace Firmeza.web.Controllers
{
    public class ErrorHandlingController : Controller
    {
        // This action displays the form to the user.
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        // This action processes the submitted form data.
        [HttpPost]
        public IActionResult ProcessAge(string ageInput)
        {
            try
            {
                // Attempt to convert the user's input to an integer.
                // Using int.Parse to demonstrate the try-catch block as requested.
                int age = int.Parse(ageInput);

                // If successful, display a success message.
                ViewBag.Message = $"Thank you! Your age is: {age}";
            }
            catch (FormatException)
            {
                // This block executes if int.Parse fails because the input is not a valid number.
                // We set a user-friendly error message.
                ViewBag.ErrorMessage = "Invalid input. Please enter a valid whole number for your age.";
            }
            catch (Exception ex)
            {
                // A general catch block for any other unexpected errors.
                ViewBag.ErrorMessage = $"An unexpected error occurred: {ex.Message}";
            }

            // Return the same view to display either the success or error message.
            return View("Index");
        }
    }
}
