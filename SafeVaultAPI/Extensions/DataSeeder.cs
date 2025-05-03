using SafeVaultAPI.Shared; // Make sure this using directive is added
using Microsoft.AspNetCore.Identity;

namespace SafeVaultAPI.Extension
{
    public static class DataSeeder
    {

        // This is using EF for DBContext
        // public static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
        // {
        //     string[] roles = {
        //         Role.RoleName.User1,
        //         Role.RoleName.User2,
        //         Role.RoleName.User3
        //     };

        //     foreach (var role in roles)
        //     {
        //         if (!await roleManager.RoleExistsAsync(role))
        //         {
        //             await roleManager.CreateAsync(new IdentityRole(role));
        //         }
        //     }
        // }


        private static readonly Dictionary<int, string> _roles = new Dictionary<int, string>();

        public static void SeedRoles()
        {
            // Define the roles directly using RoleName constants
            string[] roles = {
                Role.RoleName.User1, // "User"
                Role.RoleName.User2, // "Admin"
                Role.RoleName.User3  // "Client"
            };

            int[] roleValues = {
                Role.RoleValue.Value1, // Role value for "User"
                Role.RoleValue.Value2, // Role value for "Admin"
                Role.RoleValue.Value3  // Role value for "Client"
            };

            // Seed roles into the in-memory dictionary
            for (int i = 0; i < roles.Length; i++)
            {
                if (!_roles.ContainsKey(roleValues[i]))
                {
                    // Add the role if it doesn't exist
                    _roles[roleValues[i]] = roles[i];
                }
            }
        }

        // A method to check if a role exists in the in-memory store (Optional)
        public static bool RoleExists(int role)
        {
            return _roles.ContainsKey(role);
        }

        // A method to get the role by its value (Optional)
        public static string GetRoleByValue(int role)
        {
            return _roles.TryGetValue(role, out var roleName) ? roleName : null;
        }
    }
}
