using Contracts.V1;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using Tweeting_book.Data;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddControllers();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Tweeting_book",
        Version = "v1",
        Description = "A Restful API for a tweeting book application",
    });
});

// var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>();
// builder.Services.AddSingleton(jwtSettings);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseAuthentication();

}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();





app.Run();

