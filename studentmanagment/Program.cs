using Microsoft.EntityFrameworkCore;
using StudentManagement.Core.Interfaces;
using StudentManagement.Core.Mapping;
using StudentManagement.Data.Contextt;
using StudentManagement.Data.Repositories;
using StudentManagement.Data.UnitOfWork;
using StudentManagement.Services.Interfaces;
using StudentManagement.Services.Implementations;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ManagDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped<ISubjectService, SubjectService>();
builder.Services.AddScoped<IStudentService, StudentService>();

builder.Services.AddAutoMapper(typeof(MappingProfile));

builder.Services.AddControllersWithViews();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
