using PositiveNewsPlatform.Application.Articles.Create;
using PositiveNewsPlatform.Application.Articles.GetById;
using PositiveNewsPlatform.Application.Articles.GetLatest;
using PositiveNewsPlatform.Application.Articles.Update;
using PositiveNewsPlatform.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Infrastructure (SQL + Mongo + Redis)
builder.Services.AddInfrastructure(builder.Configuration);

// Handlers (Application)
builder.Services.AddScoped<CreateArticleHandler>();
builder.Services.AddScoped<UpdateArticleHandler>();
builder.Services.AddScoped<GetArticleByIdHandler>();
builder.Services.AddScoped<GetLatestArticlesHandler>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();
app.Run();