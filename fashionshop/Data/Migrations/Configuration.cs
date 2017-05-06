namespace Data.Migrations
{
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using Model.Models;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<Data.FashionShopDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(Data.FashionShopDbContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            //
            //    context.People.AddOrUpdate(
            //      p => p.FullName,
            //      new Person { FullName = "Andrew Peters" },
            //      new Person { FullName = "Brice Lambson" },
            //      new Person { FullName = "Rowan Miller" }
            //    );
            //

            // Update-database báo lỗi: Object reference not set to an instance of an object.
            // Lưu ý: Khi đặt mật khẩu phải lớn hơn 5 chữ số
            CreateUser(context);
        }

        private void CreateUser(FashionShopDbContext context)
        {
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new FashionShopDbContext()));
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(new FashionShopDbContext()));
            var user = new ApplicationUser()
            {
                UserName = "admin",
                Email = "lymaodt@gmail.com",
                EmailConfirmed = true,
                BirthDay = DateTime.Now,
                FullName = "lymao"
            };
            userManager.Create(user, "123456");
            if (!roleManager.Roles.Any())
            {
                roleManager.Create(new IdentityRole { Name = "Admin" });
                //roleManager.Create(new IdentityRole { Name = "User" });
            };

            var adminUser = userManager.FindByEmail("lymaodt@gmail.com");
            userManager.AddToRole(adminUser.Id, "Admin");
        }
    }
}
