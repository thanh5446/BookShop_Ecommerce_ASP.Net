using Assignment.DataAccess;
using Assignment.Models.DBO;
using Assignment.Repositories.Abstraction;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Security.Claims;


namespace Assignment.Repositories.Implementation
{
    
    public class UserAuthenticationService : IUserAuthenticationService
    {
        private readonly SignInManager<AppUser> signInManager;
        private readonly UserManager<AppUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly BookShopDbContext _bookShopDbContext;

        public UserAuthenticationService(SignInManager<AppUser> signInManager, UserManager<AppUser> userManager,
            RoleManager<IdentityRole> roleManager, BookShopDbContext bookShopDbContext)
        {
            this.signInManager = signInManager;
            this.userManager = userManager;
            this.roleManager = roleManager;
            _bookShopDbContext = bookShopDbContext;
        }

        public async Task<Status> LoginAsync(LoginModel model)
        {
            var status = new Status();
            var user = await userManager.FindByEmailAsync(model.Email);
            if(user == null)
            {
                status.StatusCode = 0;
                status.Message = "Invalid Email";
                return status;
            }
            if(!await userManager.CheckPasswordAsync(user, model.Password))
            {
                status.StatusCode = 0;
                status.Message = "Invalid password";
                return status;
            }
            var signInResult = await signInManager.PasswordSignInAsync(user, model.Password, false, true);
            if (signInResult.Succeeded)
            {
                var userRoles = await userManager.GetRolesAsync(user);
                var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Email, model.Email)
                };
                foreach(var userRole in userRoles)
                {
                    authClaims.Add(new Claim(ClaimTypes.Role, userRole.ToString()));
                }
                status.StatusCode = 1;
                status.Message = "Login successfully";
                return status;
            }else if (signInResult.IsLockedOut)
            {
                status.StatusCode = 0;
                status.Message = "User Locked out temporarily";
                return status;
            }
            else
            {
                status.StatusCode = 0;
                status.Message = "An error occured while logging in";
                return status;
            }
        }

        public async Task<Status> RegistrationAsync(RegistrationModel model)
        {
            var status = new Status();
            var userExists = await userManager.FindByEmailAsync(model.Email);
            if(userExists != null)
            {
                status.StatusCode = 0;
                status.Message = "This email already exists";
                return status;
            }
            AppUser user = new AppUser
            {
                FullName = model.FullName,
                UserName = model.FullName.Replace(" ", ""),
                Location = model.Address,
                Email = model.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                //ProfilePicture = "abc",
                PhoneNumber = model.PhoneNumber,
                EmailConfirmed = true,
                PhoneNumberConfirmed = true,
                //TwoFactorEnabled = false,
                //LockoutEnabled = false,
                //AccessFailedCount = 0,
                //PasswordHash = model.Password
            };
            try
            {
                //_bookShopDbContext.Users.Add(user);
                //await _bookShopDbContext.SaveChangesAsync();

                var result = await userManager.CreateAsync(user, model.Password);
                await _bookShopDbContext.SaveChangesAsync();
                if (!result.Succeeded)
                {
                    status.StatusCode = 0;

                    
                    foreach (var item in result.Errors)
                    {
                        status.Message += item.Description + " ";
                    }
                   
                    return status;
                }              
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

          
            if(!await roleManager.RoleExistsAsync(model.Role))
            {
                await roleManager.CreateAsync(new IdentityRole(model.Role));
            }

            if(await roleManager.RoleExistsAsync(model.Role))
            {
                await userManager.AddToRoleAsync(user,model.Role);
            }

            status.StatusCode = 1;
            status.Message = "Your account was registered successfully";
            return status;
        }

        public async Task TaskLogoutAsync()
        {
            await signInManager.SignOutAsync();
        }
    }
}
