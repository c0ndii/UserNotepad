using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using UserNotepad.Entities;

namespace UserNotepad.Seeders
{
    public static class DbSeeder
    {
        public static async Task Seed(AppDbContext context, IPasswordHasher<Operator> passwordHasher)
        {
            if (!await context.Users.AnyAsync())
            {
                var users = new List<User>
                {
                    new User { Name = "Maria", Surname = "Skłodowska-Curie", BirthDate = new DateOnly(1867, 11, 7), Sex = SexEnum.Female },
                    new User { Name = "Fryderyk", Surname = "Chopin", BirthDate = new DateOnly(1810, 3, 1), Sex = SexEnum.Male },
                    new User { Name = "Jan", Surname = "Matejko", BirthDate = new DateOnly(1838, 6, 24), Sex = SexEnum.Male },
                    new User { Name = "Lech", Surname = "Wałęsa", BirthDate = new DateOnly(1943, 9, 29), Sex = SexEnum.Male },
                    new User { Name = "Wisława", Surname = "Szymborska", BirthDate = new DateOnly(1923, 7, 2), Sex = SexEnum.Female },
                    new User { Name = "Adam", Surname = "Mickiewicz", BirthDate = new DateOnly(1798, 12, 24), Sex = SexEnum.Male },
                    new User { Name = "Henryk", Surname = "Sienkiewicz", BirthDate = new DateOnly(1846, 5, 5), Sex = SexEnum.Male },
                    new User { Name = "Czesław", Surname = "Miłosz", BirthDate = new DateOnly(1911, 6, 30), Sex = SexEnum.Male },
                    new User { Name = "Ignacy", Surname = "Paderewski", BirthDate = new DateOnly(1860, 11, 6), Sex = SexEnum.Male },
                    new User { Name = "Stanisław", Surname = "Lema", BirthDate = new DateOnly(1921, 9, 12), Sex = SexEnum.Male },
                    new User { Name = "Maria", Surname = "Konopnicka", BirthDate = new DateOnly(1842, 5, 23), Sex = SexEnum.Female },
                    new User { Name = "Jerzy", Surname = "Grotowski", BirthDate = new DateOnly(1933, 8, 11), Sex = SexEnum.Male },
                };

                await context.Users.AddRangeAsync(users);
            }

            if (!await context.Operators.AnyAsync())
            {
                var defaultOperator = new Operator
                {
                    Nickname = "admin",
                    Username = "admin",
                };

                defaultOperator.PasswordHash = passwordHasher.HashPassword(defaultOperator, "admin12345");
                await context.Operators.AddAsync(defaultOperator);
            }

            await context.SaveChangesAsync();
        }
    }
}
