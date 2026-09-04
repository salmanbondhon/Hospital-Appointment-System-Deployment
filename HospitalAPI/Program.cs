using FluentValidation;
using FluentValidation.AspNetCore;
using HospitalAPI.Data;
using HospitalAPI.Interfaces;
using HospitalAPI.Mappings;
using HospitalAPI.Repositories;
using HospitalAPI.Services;
using HospitalAPI.Validators;
using Microsoft.EntityFrameworkCore;
using HospitalAPI.Middleware;


using HospitalAPI.Configurations;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

using Microsoft.OpenApi.Models;


namespace HospitalAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);


            builder.Services.AddControllers();


           builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular", policy =>
    {
        policy.WithOrigins(
            "http://localhost:4200",
            "https://purple-ocean-0b74bca00.5.azurestaticapps.net"
        )
        .AllowAnyHeader()
        .AllowAnyMethod();
    });
});


            builder.Services.Configure<JwtSettings>(
            builder.Configuration.GetSection("Jwt"));

            var jwtSettings = builder.Configuration
    .GetSection("Jwt")
    .Get<JwtSettings>();




            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            ValidIssuer = jwtSettings!.Issuer,
            ValidAudience = jwtSettings.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtSettings.Key))
        };
    });


            builder.Services.Configure<EmailSettings>(
            builder.Configuration.GetSection("EmailSettings"));

            builder.Services.AddScoped<IEmailService, EmailService>();






            builder.Services.AddAuthorization();


            builder.Services.AddFluentValidationAutoValidation();

            builder.Services.AddValidatorsFromAssemblyContaining<CreateDepartmentDtoValidator>();


            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                    options.UseSqlServer(
                  builder.Configuration.GetConnectionString("DefaultConnection")));


            



            builder.Services.AddScoped<IDepartmentRepository, DepartmentRepository>();
            builder.Services.AddScoped<IDepartmentService, DepartmentService>();

            builder.Services.AddScoped<IDoctorRepository, DoctorRepository>();
            builder.Services.AddScoped<IDoctorService, DoctorService>();


            builder.Services.AddScoped<IPatientRepository, PatientRepository>();
            builder.Services.AddScoped<IPatientService, PatientService>();


            builder.Services.AddScoped<IAppointmentRepository, AppointmentRepository>();
            builder.Services.AddScoped<IAppointmentService, AppointmentService>();

            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<IUserService, UserService>();

            builder.Services.AddScoped<IDoctorLeaveRepository, DoctorLeaveRepository>();
            builder.Services.AddScoped<IDoctorLeaveService, DoctorLeaveService>();

            builder.Services.AddScoped<INotificationRepository, NotificationRepository>();
            builder.Services.AddScoped<INotificationService, NotificationService>();

            builder.Services.AddScoped<IPrescriptionRepository, PrescriptionRepository>();
            builder.Services.AddScoped<IPrescriptionService, PrescriptionService>();

            builder.Services.AddScoped<IMedicalRecordRepository, MedicalRecordRepository>();

            builder.Services.AddScoped<IMedicalRecordService, MedicalRecordService>();

            builder.Services.AddScoped<IDashboardService, DashboardService>();

            builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();
            builder.Services.AddScoped<IPaymentService, PaymentService>();



            builder.Services.AddAutoMapper(typeof(MappingProfile));

            // Add services to the container.

            //builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Hospital API",
                    Version = "v1"
                });

                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Enter: Bearer {your JWT token}"
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
            });




            var app = builder.Build();

            // Configure the HTTP request pipeline.
            app.UseSwagger();
            app.UseSwaggerUI();

            app.UseMiddleware<ExceptionMiddleware>();

            app.UseHttpsRedirection();

            app.UseCors("AllowAngular");

            app.UseAuthentication();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
