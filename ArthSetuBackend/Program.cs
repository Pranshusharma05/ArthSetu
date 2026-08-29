using ArthSetuBackend.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ArthSetuBackend.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddScoped<RecommendationService>();
builder.Services.AddScoped<SourceSyncService>();
builder.Services.AddHttpClient<IGovernmentConnector, NsfdcConnectorService>();
builder.Services.AddHttpClient<IGovernmentConnector, NbcfdcConnectorService>();
builder.Services.AddHttpClient<IGovernmentConnector, NstfdcConnectorService>();
builder.Services.AddHttpClient<IGovernmentConnector, NdfdcConnectorService>();
builder.Services.AddHttpClient<IGovernmentConnector, JanSamarthConnectorService>();
builder.Services.AddHttpClient<IGovernmentConnector, NspConnectorService>();
builder.Services.AddHttpClient<IGovernmentConnector, VidyalaxmiConnectorService>();
builder.Services.AddHttpClient<IGovernmentConnector, MudraConnectorService>();
builder.Services.AddHttpClient<IGovernmentConnector, PmegpConnectorService>();
builder.Services.AddHttpClient<IGovernmentConnector, PmVishwakarmaConnectorService>();
builder.Services.AddHttpClient<IGovernmentConnector, PmfmeConnectorService>();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
    });
});

// Configure EF Core to use SQL Server
// Placeholder connection string since SQL Server is Pending Infrastructure in this environment
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=ArthSetuDb;Trusted_Connection=True;MultipleActiveResultSets=true"));

var app = builder.Build();
  
  
  
    
  
      
  
    

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<ApplicationDbContext>();
    DbInitializer.Initialize(context);
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthorization();
app.MapControllers();

app.Run();


























