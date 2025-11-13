using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using UserNotepad.Entities;
using UserNotepad.Models;

namespace UserNotepad.Services
{
    public interface IUserService
    {
        public Task<PageDto<UserDto>> GetAllUsers(PageInput pageRequest, CancellationToken cancellationToken);
        public Task<UserDto?> GetUser(Guid id, CancellationToken cancellationToken);
        public Task<UserDto> AddUser(UserInput user, CancellationToken cancellationToken);
        public Task<UserDto?> UpdateUser(Guid id, UserInput user, CancellationToken cancellationToken);
        public Task<Guid?> RemoveUser(Guid id, CancellationToken cancellationToken);
        public Task<byte[]> GetReport(DateTime generationDateTime, CancellationToken cancellationToken);
    }

    public class UserService : IUserService
    {
        private readonly AppDbContext _context;

        public UserService(AppDbContext context)
        {
            this._context = context;
        }

        public async Task<PageDto<UserDto>> GetAllUsers(PageInput pageRequest, CancellationToken cancellationToken)
        {
            var query = _context.Users
                .Include(x => x.Attributes)
                .AsQueryable();

            var totalCount = await query.CountAsync();

            var users = await query
                .AsNoTracking()
                .OrderBy(x => x.CreatedAt)
                .Skip((pageRequest.Page - 1) * pageRequest.PageSize)
                .Take(pageRequest.PageSize)
                .Select(x => MapUserFromDb(x))
                .ToListAsync(cancellationToken);

            return new PageDto<UserDto>
            {
                Items = users,
                TotalCount = totalCount,
                Page = pageRequest.Page,
                PageSize = pageRequest.PageSize
            };
        }

        public async Task<UserDto?> GetUser(Guid id, CancellationToken cancellationToken)
        {
            var user = await _context.Users
                .Include(x => x.Attributes)
                .AsNoTracking()
                .SingleOrDefaultAsync(x => x.ID == id, cancellationToken);

            if (user is null)
                return null;
            return MapUserFromDb(user);
        }

        public async Task<UserDto> AddUser(UserInput user, CancellationToken cancellationToken)
        {
            var userToDb = new User
            {
                Name = user.Name,
                Surname = user.Surname,
                BirthDate = user.BirthDate,
                Sex = user.Sex,
            };

            if (user.Attributes is not null)
                userToDb.Attributes = MapAttributesFromInput(user.Attributes);

            await _context.Users.AddAsync(userToDb, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            return MapUserFromDb(userToDb);
        }

        public async Task<UserDto?> UpdateUser(Guid id, UserInput user, CancellationToken cancellationToken)
        {
            var dbUser = await _context.Users.SingleOrDefaultAsync(x => x.ID == id, cancellationToken);
            if (dbUser is null)
                return null;

            dbUser.Name = user.Name;
            dbUser.Surname = user.Surname;
            dbUser.BirthDate = user.BirthDate;
            dbUser.Sex = user.Sex;

            var attributes = await _context.Attributes
                .Where(x => x.UserID == id)
                .ToListAsync(cancellationToken);

            var toRemove = attributes
                .Where(x => !user.Attributes
                    .Any(y => y.Key == x.Key))
                .ToList();
            var toAdd = new List<UserAttribute>();

            foreach (var attribute in user.Attributes)
            {
                var existingAttribute = attributes.SingleOrDefault(x => x.Key == attribute.Key);
                if (existingAttribute is not null)
                {
                    existingAttribute.Value = attribute.Value;
                    existingAttribute.ValueType = attribute.ValueType;
                }
                else
                {
                    toAdd.Add(new UserAttribute
                    {
                        Key = attribute.Key,
                        Value = attribute.Value,
                        ValueType = attribute.ValueType,
                        UserID = dbUser.ID,
                        User = dbUser
                    });
                }
            }

            if (toAdd.Count > 0)
                await _context.Attributes.AddRangeAsync(toAdd, cancellationToken);

            if (toRemove.Count > 0)
            {
                _context.Attributes.RemoveRange(toRemove);
            }

            await _context.SaveChangesAsync(cancellationToken);
            return MapUserFromDb(dbUser);
        }

        public async Task<Guid?> RemoveUser(Guid id, CancellationToken cancellationToken)
        {
            var user = await _context.Users.SingleOrDefaultAsync(x => x.ID == id, cancellationToken);

            if (user is null) return null;

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return user.ID;
        }

        public async Task<byte[]> GetReport(DateTime generationDateTime, CancellationToken cancellationToken)
        {
            var users = await _context.Users
                .Include(x => x.Attributes)
                .OrderBy(x => x.CreatedAt)
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            var report = Document.Create(doc =>
            {
                doc.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(20);
                    page.Header()
                        .Text($"Users report, \ngenerated: {generationDateTime:dd.MM.yyyy HH:mm:ss} UTC")
                        .FontSize(16)
                        .Bold();

                    page.DefaultTextStyle(x => x.FontSize(10));
                    page.Content().Padding(10).Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.ConstantColumn(30);
                            columns.ConstantColumn(30);
                            columns.RelativeColumn(1.5f); 
                            columns.RelativeColumn(1.5f);
                            columns.RelativeColumn(0.8f);
                            columns.RelativeColumn(1);   
                            columns.RelativeColumn(0.8f); 
                            columns.RelativeColumn(4);  
                        });

                        table.Header(header =>
                        {
                            header.Cell().Element(CellHeaderStyle).Text("#").AlignStart();
                            header.Cell().Element(CellHeaderStyle).Text("Title").AlignStart();
                            header.Cell().Element(CellHeaderStyle).Text("Name").AlignStart();
                            header.Cell().Element(CellHeaderStyle).Text("Surname").AlignStart();
                            header.Cell().Element(CellHeaderStyle).Text("Age").AlignStart();
                            header.Cell().Element(CellHeaderStyle).Text("Birth Date").AlignStart();
                            header.Cell().Element(CellHeaderStyle).Text("Sex").AlignStart();
                            header.Cell().Element(CellHeaderStyle).Text("Attributes").AlignStart();

                            static IContainer CellHeaderStyle(IContainer container)
                            {
                                return container
                                    .Background(Colors.Grey.Lighten3)
                                    .Padding(4)
                                    .Border(1)
                                    .BorderColor(Colors.Grey.Lighten2)
                                    .DefaultTextStyle(x => x.SemiBold());
                            }
                        });

                        int i = 1;
                        foreach (var user in users)
                        {
                            table.Cell().Element(CellBodyStyle).Text(i++.ToString());

                            var title = user.Sex switch
                            {
                                SexEnum.Male => "Mr.",
                                SexEnum.Female => "Ms.",
                                _ => "Mx."
                            };
                            table.Cell().Element(CellBodyStyle).Text(title);

                            table.Cell().Element(CellBodyStyle).Text(user.Name);
                            table.Cell().Element(CellBodyStyle).Text(user.Surname);

                            var today = DateOnly.FromDateTime(DateTime.UtcNow);
                            var age = today.Year - user.BirthDate.Year - (today < user.BirthDate.AddYears(today.Year - user.BirthDate.Year) ? 1 : 0);

                            table.Cell().Element(CellBodyStyle).Text(age.ToString());

                            table.Cell().Element(CellBodyStyle).Text(user.BirthDate.ToString("dd.MM.yyyy"));
                            table.Cell().Element(CellBodyStyle).Text(user.Sex.ToString());

                            var attrSummary = user.Attributes.Any()
                                ? string.Join(", ", user.Attributes.Select(a =>
                                {
                                    string formattedValue = a.ValueType switch
                                    {
                                        AttributeTypeEnum.@Date => DateOnly.TryParse(a.Value, out var dt)
                                                                      ? dt.ToString("dd.MM.yyyy")
                                                                      : a.Value,
                                        AttributeTypeEnum.@bool => bool.TryParse(a.Value, out var b) ? b.ToString() : a.Value,
                                        AttributeTypeEnum.@double => double.TryParse(a.Value, out var d) ? d.ToString() : a.Value,
                                        AttributeTypeEnum.@int => int.TryParse(a.Value, out var i) ? i.ToString() : a.Value,
                                        _ => a.Value
                                    };

                                    return $"{a.Key}: {formattedValue}";
                                }))
                                : "-";

                            table.Cell().Element(CellBodyStyle).Text(attrSummary);

                            static IContainer CellBodyStyle(IContainer container)
                            {
                                return container
                                    .Padding(4)
                                    .BorderBottom(1)
                                    .BorderColor(Colors.Grey.Lighten3);
                            }
                        }
                    });

                    page.Footer()
                        .AlignRight()
                        .Text(footer =>
                        {
                            footer.Span("Page ");
                            footer.CurrentPageNumber();
                            footer.Span(@"/");
                            footer.TotalPages();
                        });
                });
            });

            return report.GeneratePdf();
        }

        private IEnumerable<UserAttribute> MapAttributesFromInput(IEnumerable<UserAttributeInput> attributes)
        {
            return attributes
                .Select(x => new UserAttribute
                {
                    Key = x.Key,
                    Value = x.Value,
                    ValueType = x.ValueType,
                }).ToList();
        }

        private static UserDto MapUserFromDb(User user)
        {
            return new UserDto
            {
                ID = user.ID,
                Name = user.Name,
                Surname = user.Surname,
                BirthDate = user.BirthDate,
                Sex = user.Sex,
                Attributes = user.Attributes.Select(x => new UserAttributeDto
                {
                    Key = x.Key,
                    Value = x.Value,
                    ValueType = x.ValueType
                }).ToList()
            };
        }
    }
}
