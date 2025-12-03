using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using LibraryApp.Data;
using LibraryApp.Models;

namespace LibraryApp.Areas.Identity.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly LibraryDbContext _context;
        private readonly ILogger<RegisterModel> _logger;

        public RegisterModel(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, LibraryDbContext context, ILogger<RegisterModel> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
            _logger = logger;
        }

        [BindProperty]
        public InputModel Input { get; set; } = new InputModel();

        public string ReturnUrl { get; set; } = string.Empty;

        public class InputModel
        {
            [Required(ErrorMessage = "Le prénom est requis")]
            [StringLength(50, ErrorMessage = "Le prénom ne peut pas dépasser 50 caractères")]
            [Display(Name = "Prénom")]
            public string FirstName { get; set; } = string.Empty;

            [Required(ErrorMessage = "Le nom est requis")]
            [StringLength(50, ErrorMessage = "Le nom ne peut pas dépasser 50 caractères")]
            [Display(Name = "Nom")]
            public string LastName { get; set; } = string.Empty;

            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; } = string.Empty;

            [Required]
            [StringLength(100, ErrorMessage = "{0} doit avoir au moins {2} caractères.", MinimumLength = 8)]
            [DataType(DataType.Password)]
            [Display(Name = "Mot de passe")]
            public string Password { get; set; } = string.Empty;

            [DataType(DataType.Password)]
            [Compare("Password", ErrorMessage = "Les mots de passe ne correspondent pas.")]
            [Display(Name = "Confirmer le mot de passe")]
            public string ConfirmPassword { get; set; } = string.Empty;
        }

        public void OnGet(string? returnUrl = null)
        {
            ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
        {
            ReturnUrl = returnUrl;
            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                var user = new IdentityUser { UserName = Input.Email, Email = Input.Email };
                var result = await _userManager.CreateAsync(user, Input.Password);
                // Diagnostic: log IdentityResult and DB file path
                try
                {
                    var dbPath = _context.Database.GetDbConnection().DataSource;
                    _logger.LogInformation("Register attempt for {Email}: Succeeded={Succeeded}; DB={DbPath}", Input.Email, result.Succeeded, dbPath);
                    if (!result.Succeeded)
                    {
                        var errs = string.Join(";", result.Errors.Select(e => $"{e.Code}:{e.Description}"));
                        _logger.LogInformation("Identity errors for {Email}: {Errors}", Input.Email, errs);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to log DB path or IdentityResult details for {Email}", Input.Email);
                }

                if (result.Succeeded)
                {
                    try
                    {
                        var exists = await _context.Users.AnyAsync(u => u.Email == Input.Email);
                        _logger.LogInformation("UserManager created user present in DB for {Email}: {Exists}", Input.Email, exists);
                        var found = await _userManager.FindByEmailAsync(Input.Email);
                        _logger.LogInformation("UserManager.FindByEmailAsync returned for {Email}: Id={UserId}", Input.Email, found?.Id ?? "(null)");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Failed to query Users table after CreateAsync for {Email}", Input.Email);
                    }

                    // Create a corresponding Member record with provided data
                    var member = new Member
                    {
                        FirstName = Input.FirstName,
                        LastName = Input.LastName,
                        Email = Input.Email,
                        IdentityUserId = user?.Id
                    };

                    _context.Members.Add(member);
                    await _context.SaveChangesAsync();

                    await _signInManager.SignInAsync(user, isPersistent: false);
                    _logger.LogInformation("New user registered: {Email}", Input.Email);
                    TempData["SuccessMessage"] = "Inscription réussie — vous êtes connecté.";
                    return LocalRedirect(returnUrl ?? Url.Content("~/"));
                }

                foreach (var error in result.Errors)
                {
                    // Add to ModelState for display
                    ModelState.AddModelError(string.Empty, error.Description);
                    // Log each Identity error code + description
                    _logger.LogWarning("Identity create error for {Email}: {Code} - {Description}", Input.Email, error.Code, error.Description);
                }
                try
                {
                    var found2 = await _userManager.FindByEmailAsync(Input.Email);
                    _logger.LogInformation("After failed CreateAsync, FindByEmailAsync for {Email} returned Id={UserId}", Input.Email, found2?.Id ?? "(null)");
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "FindByEmailAsync failed for {Email}", Input.Email);
                }
                // Also expose errors to the UI in TempData for immediate visibility
                try
                {
                    var messages = string.Join(" | ", result.Errors.Select(e => $"{e.Description} ({e.Code})"));
                    if (!string.IsNullOrEmpty(messages)) TempData["ErrorMessage"] = messages;
                }
                catch (Exception ex)
                {
                    _logger.LogDebug(ex, "Failed to build TempData error message for registration");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception while creating user for {Email}", Input.Email);
                TempData["ErrorMessage"] = "Une erreur interne est survenue lors de la création du compte.";
                ModelState.AddModelError(string.Empty, "Une erreur interne est survenue lors de la création du compte.");
            }

            return Page();
        }
    }
}
