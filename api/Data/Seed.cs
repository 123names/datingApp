namespace api.Data
{
    public class Seed
    {
        // load seed users
        public static async Task SeedUsers(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager)
        {
            if (await userManager.Users.AnyAsync()) return;

            var userData = await System.IO.File.ReadAllTextAsync("Data/UserSeedData.json");
            var users = JsonSerializer.Deserialize<List<AppUser>>(userData);

            var roles = new List<AppRole>
            {
                new AppRole{Name="Member"},
                new AppRole{Name="Moderator"},
                new AppRole{Name="Admin"}
            };

            foreach (var role in roles)
            {
                await roleManager.CreateAsync(role);
            }

            foreach (var user in users)
            {
                user.UserPhotos.First().IsApproved = true;
                user.UserName = user.UserName.ToLower();
                await userManager.CreateAsync(user, "Pa__w0rd");
                await userManager.AddToRoleAsync(user, "Member");
            }

            var admin = new AppUser
            {
                UserName = "admin"
            };

            await userManager.CreateAsync(admin, "Pa__w0rd");
            await userManager.AddToRolesAsync(admin, new[] { "Admin", "Moderator", "Member" });
        }
    }
}