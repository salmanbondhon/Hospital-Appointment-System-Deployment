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

namespace HospitalAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);


            builder.Services.AddControllers();


            builder.Services.AddFluentValidationAutoValidation();

            builder.Services.AddValidatorsFromAssemblyContaining<CreateDepartmentDtoValidator>();


            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                    options.UseSqlServer(
                  builder.Configuration.GetConnectionString("DefaultConnection")));


            builder.Services.AddScoped<IDepartmentRepository, DepartmentRepository>();



            builder.Services.AddScoped<IDepartmentRepository, DepartmentRepository>();
            builder.Services.AddScoped<IDepartmentService, DepartmentService>();

            builder.Services.AddScoped<IDoctorRepository, DoctorRepository>();
            builder.Services.AddScoped<IDoctorService, DoctorService>();


            builder.Services.AddScoped<IPatientRepository, PatientRepository>();
            builder.Services.AddScoped<IPatientService, PatientService>();


            builder.Services.AddScoped<IAppointmentRepository, AppointmentRepository>();
            builder.Services.AddScoped<IAppointmentService, AppointmentService>();



            builder.Services.AddAutoMapper(typeof(MappingProfile));

            // Add services to the container.

            //builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();


            

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseMiddleware<ExceptionMiddleware>();

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
