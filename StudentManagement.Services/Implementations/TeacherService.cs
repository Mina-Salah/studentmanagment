using Microsoft.EntityFrameworkCore;
using StudentManagement.Core.Entities;
using StudentManagement.Core.Interfaces;
using StudentManagement.Services.Interfaces;

public class TeacherService : ITeacherService
{
    private readonly IUnitOfWork _unitOfWork;

    public TeacherService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task CreateTeacherAsync(Teacher teacher, string email, string password)
    {
        // المستخدم تم إنشاؤه بالفعل في AuthService.RegisterAsync
        var user = (await _unitOfWork.Users.GetAllAsync())
            .FirstOrDefault(u => u.Email == email);

        if (user == null)
            throw new Exception("لم يتم العثور على المستخدم.");

        teacher.UserId = user.Id;
        await _unitOfWork.Teachers.AddAsync(teacher);
        await _unitOfWork.CompleteAsync();
    }


    public async Task<IEnumerable<Teacher>> GetAllTeachersAsync()
    {
        return await _unitOfWork.Teachers.GetAllAsync(
            filter: t => !t.IsDeleted,
            include: q => q.Include(t => t.User)
        );
    }

    public async Task<Teacher?> GetTeacherByIdAsync(int id)
    {
        return await _unitOfWork.Teachers.GetByIdAsync(id,
            include: q => q.Include(t => t.User));
    }

    public async Task UpdateTeacherAsync(int id, Teacher updatedTeacher)
    {
        var existingTeacher = await _unitOfWork.Teachers.GetByIdAsync(id);
        if (existingTeacher == null)
            throw new Exception("Teacher not found");

        existingTeacher.Name = updatedTeacher.Name;
        existingTeacher.Address = updatedTeacher.Address;
        existingTeacher.DateOfBirth = updatedTeacher.DateOfBirth;

        _unitOfWork.Teachers.Update(existingTeacher);
        await _unitOfWork.CompleteAsync();
    }

    // ✅ حذف ناعم (مع المستخدم)
    public async Task DeleteTeacherAsync(int id)
    {
        var teacher = await _unitOfWork.Teachers.GetByIdAsync(id);
        if (teacher == null)
            throw new Exception("Teacher not found");

        teacher.IsDeleted = true;
        _unitOfWork.Teachers.Update(teacher);

        if (teacher.UserId.HasValue)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(teacher.UserId.Value);
            if (user != null)
            {
                user.IsDeleted = true;
                _unitOfWork.Users.Update(user);
            }
        }

        await _unitOfWork.CompleteAsync();
    }

    public async Task<IEnumerable<Teacher>> GetDeletedTeachersAsync()
    {
        return await _unitOfWork.Teachers.GetAllAsync(
            filter: t => t.IsDeleted,
            include: q => q.Include(t => t.User),
            includeSoftDeleted: true
        );
    }

    // ✅ استرجاع المدرس والمستخدم
    public async Task RestoreTeacherAsync(int id)
    {
        var teacher = await _unitOfWork.Teachers.GetByIdIncludingDeletedAsync(id);
        if (teacher == null)
            throw new Exception("Teacher not found");

        teacher.IsDeleted = false;
        _unitOfWork.Teachers.Update(teacher);

        if (teacher.UserId.HasValue)
        {
            var user = await _unitOfWork.Users.GetByIdIncludingDeletedAsync(teacher.UserId.Value);
            if (user != null)
            {
                user.IsDeleted = false;
                _unitOfWork.Users.Update(user);
            }
        }

        await _unitOfWork.CompleteAsync();
    }

    // ✅ حذف نهائي من المدرسين والمستخدمين
    public async Task DeleteTeacherPermanentlyAsync(int id)
    {
        var teacher = await _unitOfWork.Teachers.GetByIdIncludingDeletedAsync(id);
        if (teacher == null)
            throw new Exception("Teacher not found");

        _unitOfWork.Teachers.Delete(teacher);

        if (teacher.UserId.HasValue)
        {
            var user = await _unitOfWork.Users.GetByIdIncludingDeletedAsync(teacher.UserId.Value);
            if (user != null)
            {
                _unitOfWork.Users.Delete(user);
            }
        }

        await _unitOfWork.CompleteAsync();
    }
}
