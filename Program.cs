using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SocialMediaAPI23Okt.Data;
using SocialMediaAPI23Okt.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("Default")));

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
          //ValidIssuer = builder.Configuration["JWT:Issuer"],
          //ValidAudience = builder.Configuration["JWT:Audience"],
          IssuerSigningKey = new SymmetricSecurityKey(signingKey)
      };
  });

builder.Services.AddScoped<ConversationService>();
builder.Services.AddScoped<FriendRequestService>();
builder.Services.AddScoped<InterestService>();
builder.Services.AddScoped<LoginService>();
builder.Services.AddScoped<MessageService>();
builder.Services.AddScoped<PersonalInfoService>();
builder.Services.AddScoped<PostToPublicBoardService>();
builder.Services.AddScoped<RegistrationService>();
builder.Services.AddScoped<UserService>();


builder.Services.AddCors();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseCors(builder => builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
