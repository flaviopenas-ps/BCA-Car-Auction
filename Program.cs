using BCA_Car_Auction.Models.Vehicles;
using BCA_Car_Auction.Services;
using BCA_Car_Auction.Validation;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<ICarFactory, CarFactory>();
builder.Services.AddSingleton<ICarService, CarService>();
builder.Services.AddSingleton<IUserService, UserService>();
builder.Services.AddSingleton<IAuctionService, AuctionService>();

//Exceptions
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

var loggerFactory = LoggerFactory.Create(builder =>
{
    builder.AddConsole(); // or AddSerilog(), AddDebug(), etc.
});

ValidationExtensions.Logger = loggerFactory.CreateLogger("ValidationExtensions");

app.UseExceptionHandler();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
