using AutoMapper;
using DentalHorizonePRMS;
using DentalHorizonePRMS.Interfaces;
using DentalHorizonePRMS.Profiles;
using DentalHorizonePRMS.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

#region SERVICES
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddRazorPages();

builder.Services.AddOutputCache(options => 
{
    options.DefaultExpirationTimeSpan = TimeSpan.FromSeconds(60);
});

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("").AllowAnyMethod().AllowAnyHeader();
    });
});

builder.Services.AddAutoMapper(mapper => 
{
    mapper.AddProfile<AutoMapperProfiles>();
});

builder.Services.AddTransient<IUserRepository, UserRepository>();
builder.Services.AddTransient<IPatientRepository, PatientRepository>();
builder.Services.AddTransient<IPatientMedicalHistoryRepository, PatientMedicalHistoryRepository>();
#endregion

var app = builder.Build();

#region MIDDLEWARE
app.UseSwagger();
app.UseSwaggerUI();
app.UseOutputCache();
app.UseHttpsRedirection();
app.UseCors();
app.MapControllers();
app.MapRazorPages();
app.UseStaticFiles();

#endregion

app.Run();
