using Auth0.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MoviesAdmin.Models;
using MoviesAdmin.Services;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("MoviesAdminContext") ?? throw new InvalidOperationException("Connection string 'MoviesAdminContext' not found.");

builder.Services.AddDbContext<MoviesAdminContext>(options => options.UseSqlServer(connectionString));

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddScoped<AzureStorageService>();

// Add Auth0 authentication
builder.Services.AddAuth0WebAppAuthentication(options =>
{
    options.Domain = builder.Configuration["Auth0:Domain"];
    options.ClientId = builder.Configuration["Auth0:ClientId"];
    options.ClientSecret = builder.Configuration["Auth0:ClientSecret"];
    options.Scope = "openid profile email";
});

builder.Services.Configure<OpenIdConnectOptions>(Auth0Constants.AuthenticationScheme, options =>
{
    var existingOnTokenValidated = options.Events.OnTokenValidated;
    options.Events.OnTokenValidated = async context =>
    {
        await existingOnTokenValidated(context);

        var principal = context.Principal
            ?? throw new InvalidOperationException("The authenticated user principal was not provided.");
        var userId = principal.GetUserId();
        var email = principal.FindFirstValue(ClaimTypes.Email)
            ?? principal.FindFirstValue("email")
            ?? throw new InvalidOperationException("The authenticated user did not provide an email claim.");
        var name = principal.FindFirstValue("name")
            ?? principal.FindFirstValue(ClaimTypes.Name)
            ?? principal.Identity?.Name
            ?? throw new InvalidOperationException("The authenticated user did not provide a name claim.");

        var dbContext = context.HttpContext.RequestServices.GetRequiredService<MoviesAdminContext>();
        var userSnapshot = await dbContext.UserSnapshot
            .SingleOrDefaultAsync(user => user.UserId == userId, context.HttpContext.RequestAborted);

        if (userSnapshot is null)
        {
            dbContext.UserSnapshot.Add(new UserSnapshot
            {
                UserId = userId,
                Email = email,
                Name = name
            });
        }
        else
        {
            userSnapshot.Email = email;
            userSnapshot.Name = name;
        }

        await dbContext.SaveChangesAsync(context.HttpContext.RequestAborted);
    };

    options.TokenValidationParameters = new TokenValidationParameters
    {
        NameClaimType = "name",
        RoleClaimType = "https://localhost:7180/roles"
    };
});
//end Auth0 authentication

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();
