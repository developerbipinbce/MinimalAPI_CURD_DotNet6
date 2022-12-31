
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MInimalAPI_CURD;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApiContext>(opt => opt.UseInMemoryDatabase("api"));
builder.Services.AddScoped<IArticleService, ArticleService>();
builder.Services.AddSingleton<TokenService>(new TokenService());
builder.Services.AddSingleton<IUserService>(new UserService());

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    var securityScheme = new OpenApiSecurityScheme
    {
        Name = "JWT Authentication",
        Description = "Enter JWT Bearer token * *_only_ * *",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer", // must be lower case
        BearerFormat = "JWT",
        Reference = new OpenApiReference
        {
            Id = JwtBearerDefaults.AuthenticationScheme,
            Type = ReferenceType.SecurityScheme
        }
    };
    c.AddSecurityDefinition(securityScheme.Reference.Id, securityScheme);
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {securityScheme, new string[] { }}
    });
});


builder.Services.AddAuthorization();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(opt =>
{
    opt.TokenValidationParameters = new()
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
    };
});


var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.UseAuthentication();
app.UseAuthorization();


app.MapPost("/login", [AllowAnonymous] async ([FromBodyAttribute] UserRequst userModel, TokenService tokenService,
    IUserService userRepositoryService, HttpResponse response) => {
    var userDto = userRepositoryService.GetUser(userModel);
    if (userDto == null)
    {
        response.StatusCode = 401;
        return;
    }
    var token = tokenService.BuildToken(builder.Configuration["Jwt:Key"], builder.Configuration["Jwt:Issuer"], 
        builder.Configuration["Jwt:Audience"], userDto);
    await response.WriteAsJsonAsync(new { token = token });
    return;
}).Produces(StatusCodes.Status200OK)
.WithName("Login").WithTags("Accounts");


app.MapGet("/AuthorizedResource", (Func<string>)(
[Authorize] () => "Action Succeeded")
).Produces(StatusCodes.Status200OK)
.WithName("Authorized").WithTags("Accounts").RequireAuthorization();


app.MapGet("/articles", async (IArticleService articleService)
    => await articleService.GetArticles())
    .Produces(StatusCodes.Status200OK)
.WithName("GetArticles").WithTags("Article").RequireAuthorization(); 

app.MapGet("/articles/{id}", async (int id, IArticleService articleService)
    => await articleService.GetArticleById(id))
    .WithName("GetArticlesById").WithTags("Article").RequireAuthorization();

app.MapPost("/articles", async (ArticleRequest articleRequest, IArticleService articleService)
    => await articleService.CreateArticle(articleRequest))
    .WithName("SaveArticle").WithTags("Article").RequireAuthorization(); 

app.MapPut("/articles/{id}", async (int id, ArticleRequest articleRequest, IArticleService articleService)
    => await articleService.UpdateArticle(id, articleRequest))
    .WithName("UpdateArticle").WithTags("Article").RequireAuthorization(); 

app.MapDelete("/articles/{id}", async (int id, IArticleService articleService)
    => await articleService.DeleteArticle(id))
    .WithName("DeleteArticle").WithTags("Article").RequireAuthorization();

app.MapGet("/test", () => "add route at end of url as /login");
// for adding multiple url
app.Urls.Add("http://localhost:3000");
app.Urls.Add("http://localhost:4000");

app.Run();
