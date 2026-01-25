using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer
{
    public class CustomPasswordValidator<TUser> : IPasswordValidator<TUser> where TUser : IdentityUser
    {
        private PasswordHasher<TUser> passwordHasher;

        public CustomPasswordValidator()
        {
            passwordHasher = new PasswordHasher<TUser>();
        }

        public Task<IdentityResult> ValidateAsync(UserManager<TUser> manager, TUser user, string password)
        {
            if (password.Length < 5)
            {
                return Task.FromResult(IdentityResult.Failed(new IdentityError() { Code = "0", Description = "Password must be atleast 5 symbols!" }));
            }
            else
            {
                return Task.FromResult(IdentityResult.Success);
            }
        }
    }
}
