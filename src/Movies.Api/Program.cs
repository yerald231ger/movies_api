using Movies.Application;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplication();

var app = builder.Build();

app.MapPostMovie();
app.MapGetMovie();
app.MapPutMovie();
app.MapDeleteMovie();

app.Run();


