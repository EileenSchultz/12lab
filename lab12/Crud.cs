namespace lab12;

using Microsoft.EntityFrameworkCore;


public class Crud
{
    public static async Task<Note> Create(string text, DateTimeOffset createdAt, int userId, CancellationToken ct = default)
    {
        await using var db = new DataContext();
        var note = new Note
        {
            Text = text,
            CreatedAt = createdAt
            UserId = userId
        };
        db.Notes.Add(note);
        await db.SaveChangesAsync(ct);
        return note;
    }
    
    public static async Task<List<Note>> Read(string search, CancellationToken ct = default)
    {
        await using var db = new DataContext();
        var result = await db.Notes
            .Where(x => EF.Functions.Like(x.Text, $"%{search}%"))
            .ToListAsync(ct);
        return result;
    }
    
    public static async Task<Note?> Read(int id, CancellationToken ct = default)
    {
        await using var db = new DataContext();
        return await db.Notes.FirstOrDefaultAsync(x => x.Id == id, ct);
    }
  
    public static async Task Update(Note note, string text, DateTimeOffset createdAt, CancellationToken ct = default)
    {
        await using var db = new DataContext();
        note.Text = text;
        note.CreatedAt = createdAt;
        db.Notes.Update(note);
        await db.SaveChangesAsync(ct);
    }

    public static async Task Delete(Note note, CancellationToken ct = default)
    {
        await using var db = new DataContext();
        db.Notes.Remove(note);
        await db.SaveChangesAsync(ct);
    }
    
    public static async Task<User> CreateUser(string name, CancellationToken ct = default)
    {
        await using var db = new DataContext();
        var user = new User
        {
            Name = name
        };
        db.Users.Add(user);
        await db.SaveChangesAsync(ct);
        return user;
    }
    
    public static async Task<List<User>> ReadUsers(string search, CancellationToken ct = default)
    {
        await using var db = new DataContext();
        var result = await db.Users
            .Where(x => EF.Functions.Like(x.Name, $"%{search}%"))
            .ToListAsync(ct);
        return result;
    }
    
    public static async Task<User?> ReadUser(int id, CancellationToken ct = default)
    {
        await using var db = new DataContext();
        return await db.Users.FirstOrDefaultAsync(x => x.Id == id, ct);
    }
    
    public static async Task UpdateUser(User user, string name, CancellationToken ct = default)
    {
        await using var db = new DataContext();
        user.Name = name;
        db.Users.Update(user);
        await db.SaveChangesAsync(ct);
    }
    
    public static async Task DeleteUser(User user, CancellationToken ct = default)
    {
        await using var db = new DataContext();
        db.Users.Remove(user);
        await db.SaveChangesAsync(ct);
    }
    
    public static async Task<List<Note>> GetNotesByUser(int userId, CancellationToken ct = default)
    {
        await using var db = new DataContext();
        return await db.Notes
            .Where(x => x.UserId == userId)
            .ToListAsync(ct);
    }
}