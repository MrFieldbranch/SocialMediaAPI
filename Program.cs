using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SocialMediaAPI23Okt.Data;
using SocialMediaAPI23Okt.Services;

var builder = WebApplication.CreateBuilder(args);

// Load environment variables
builder.Configuration.AddEnvironmentVariables();

// Debugging: Check if environment variables are loaded
//Console.WriteLine("Reading environment variables...");
//Console.WriteLine($"DefaultConnection (from env): {Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection")}");
//Console.WriteLine($"JWT SigningSecret (from env): {Environment.GetEnvironmentVariable("JWT__SigningSecret")}");

// Add services to the container.

builder.Services.AddControllers();

// Add DbContext configuration for the PostgreSQL connection
builder.Services.AddDbContext<ApplicationDbContext>(options => 
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add JWT Authentication
builder.Services
  .AddAuthentication()
  .AddJwtBearer(options =>
  {
      var signingSecret = builder.Configuration["JWT:SigningSecret"]
      ?? throw new InvalidOperationException("JWT SigningSecret is not configured.");

      var signingKey = Convert.FromBase64String(signingSecret);

      options.TokenValidationParameters = new TokenValidationParameters
      {
          ValidateIssuer = false,
          ValidateAudience = false,
          ValidateLifetime = false,
          ValidateIssuerSigningKey = true,          
          IssuerSigningKey = new SymmetricSecurityKey(signingKey)
      };
  });

// Register services
builder.Services.AddScoped<ConversationService>();
builder.Services.AddScoped<FriendRequestService>();
builder.Services.AddScoped<InterestService>();
builder.Services.AddScoped<LoginService>();
builder.Services.AddScoped<MessageService>();
builder.Services.AddScoped<PersonalInfoService>();
builder.Services.AddScoped<PostToPublicBoardService>();
builder.Services.AddScoped<RegistrationService>();
builder.Services.AddScoped<UserService>();

// Add CORS
builder.Services.AddCors();

var app = builder.Build();

// Configure the HTTP request pipeline.

//app.UseCors(builder => builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
app.UseCors(builder => builder
    .WithOrigins("https://socialmediareactclient.onrender.com", "http://localhost:5173")
    .AllowAnyHeader()
    .AllowAnyMethod());
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();

// Debugging: Check final values used in the application
//Console.WriteLine($"Final DefaultConnection: {builder.Configuration["ConnectionStrings:DefaultConnection"]}");
//Console.WriteLine($"Final JWT SigningSecret: {builder.Configuration["JWT:SigningSecret"]}");
