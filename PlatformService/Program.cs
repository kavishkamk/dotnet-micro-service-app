using Microsoft.EntityFrameworkCore;
using PlatformService.AsycnDataServices;
using PlatformService.Data;
using PlatformService.SyncDataService.Grpc;
using SyncDataService.Http;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

if(builder.Environment.IsProduction()) {
    Console.WriteLine("--> Using SQL Server DB");
    builder.Services.AddDbContext<AppDbContext>(opt => 
        opt.UseSqlServer(builder.Configuration.GetConnectionString("PlatformDbConnection")));
} else {
    Console.WriteLine("--> Using InMem DB");
    builder.Services.AddDbContext<AppDbContext>(opt => opt.UseInMemoryDatabase("InMem"));
}

builder.Services.AddScoped<IPlatformRepo, PlatformRepo>();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddHttpClient<ICommandDataClient, HttpCommandDataClient>();
builder.Services.AddSingleton<IMessageBusClient, MessageBusClient>();
builder.Services.AddGrpc();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MapGrpcService<GrpcPlatformService>();

app.MapGet("/protos/platforms.proto", async context => {
    await context.Response.WriteAsync(File.ReadAllText("Protos/platforms.proto"));
});

PrepDB.PrepPopulation(app, app.Environment.IsProduction());

app.Run();
