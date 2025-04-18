using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace razor.Pages
{
    public class EditUserModel : PageModel
    {
        private readonly ILogger<EditUserModel> _logger;
        private readonly UserManager<User> _userManager;
        [BindProperty(SupportsGet = true)]
        public Guid UserId { get; set; }
        [BindProperty]
        public User User { get; set; }
        [BindProperty]
        public InputModel Input { get; set; }

        public EditUserModel(ILogger<EditUserModel> logger, UserManager<User> userManager)
        {
            _logger = logger;
            _userManager = userManager;
        }


        public async Task<IActionResult> OnGet(string userId)
        {
            _logger.LogInformation("editing user of Id : {ID}", userId);
            User = await _userManager.FindByIdAsync(userId.ToString());
            return Page();
        }

        public async Task<IActionResult> OnPost()
        {
            User = await _userManager.FindByIdAsync(UserId.ToString());
            if (User == null)
            {
                return RedirectToPage("Admin/Dashboard");
            }

            if (!String.IsNullOrWhiteSpace(Input.Email)) User.Email = Input.Email;
            if (!String.IsNullOrWhiteSpace(Input.FullName)) User.FullName = Input.FullName;

            _logger.LogInformation(
                "Edited user of Id : {ID} , name: FROM {FullName} TO {NewFullName} , Email: FROM {Email} TO {NewEmail}",
                UserId,
                User?.FullName, Input.FullName,
                User?.Email, Input.Email
            );
            return RedirectToPage("/Admin/Dashboard");
        }


        public class InputModel
        {
            public string Email { get; set; }
            public string FullName { get; set; }
            public string Password { get; set; }
        }
    }
}
