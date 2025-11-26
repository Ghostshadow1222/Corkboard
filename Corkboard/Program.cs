using Corkboard.Data;
using Corkboard.Models;
using Corkboard.Hubs;
using Corkboard.Data.Services;
using Corkboard.Authorization.Requirements;
using Corkboard.Authorization.Handlers;
using Corkboard.Authorization.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<UserAccount>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddSignalR();

// Register domain services (scoped to match DbContext)
builder.Services.AddScoped<IServerService, ServerService>();
builder.Services.AddScoped<IChannelService, ChannelService>();
builder.Services.AddScoped<IMessageService, MessageService>();
builder.Services.AddScoped<IInviteService, InviteService>();

// Register HTTP context accessor for authorization handlers
builder.Services.AddHttpContextAccessor();

// Register authorization helpers
builder.Services.AddScoped<RouteDataHelper>();

// Register authorization handlers
builder.Services.AddScoped<IAuthorizationHandler, ServerMemberHandler>();
builder.Services.AddScoped<IAuthorizationHandler, ServerModeratorHandler>();

// Configure authorization policies
builder.Services.AddAuthorization(options =>
{
	options.AddPolicy("ServerMember", policy =>
		policy.Requirements.Add(new ServerMemberRequirement()));
	
	options.AddPolicy("ServerModerator", policy =>
		policy.Requirements.Add(new ServerModeratorRequirement()));
});

builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
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

app.MapRazorPages()
   .WithStaticAssets();

app.MapHub<ChatHub>("/chatHub");

app.Run();
