namespace 12lab;

using Microsoft.EntityFrameworkCore;


/// <summary>
/// Содержит примеры CRUD-операций для сущности <see cref="Note"/>.
/// </summary>
/// <remarks>
/// CRUD (Create, Read, Update, Delete) это термин по умолчанию для таких запросов
/// (из которых состоит большая часть работы большинства программ).
/// </remarks>
public class Crud
{
    /// <summary>
    /// Создаёт новую заметку и сохраняет её в БД.
    /// </summary>
    /// <returns>
    /// Сущность новой заметки. После сохранения в БД её свойство <see cref="Note.Id"/>
    /// будет содержать реальный ID из СУБД (не 0).
    /// </returns>
    public static async Task<Note> Create(string text, DateTimeOffset createdAt, CancellationToken ct = default)
    {
        await using var db = new DataContext();
        var note = new Note
        {
            Text = text,
            CreatedAt = createdAt
        };
        // В этот момент Id равен 0, так как сущность не сохранена в БД
        db.Notes.Add(note);
        await db.SaveChangesAsync(ct);
        //здесь Id выставлен в корректное значение
        return note;
    }
    
    /// <summary>
    /// Получает список заметок с поиском по частичному совпадению текста.
    /// </summary>
    public static async Task<List<Note>> Read(string search, CancellationToken ct = default)
    {
        await using var db = new DataContext();
        var result = await db.Notes
            .Where(x => EF.Functions.Like(x.Text, $"%{search}%"))
            .ToListAsync(ct);
        return result;
    }
    
    /// <summary>
    /// Ищет конкретную заметку по её ID.
    /// </summary>
    /// <returns>
    /// Сущность найденной заметки или <see langword="null"/> если такой нет.
    /// </returns>
    public static async Task<Note?> Read(int id, CancellationToken ct = default)
    {
        await using var db = new DataContext();
        return await db.Notes.FirstOrDefaultAsync(x => x.Id == id, ct);
    }
    /// <summary>
    /// Обновляет сущность заметки в БД. Меняет данные в той же сущности, не создает новый инстанс.
    /// </summary>
    public static async Task Update(Note note, string text, DateTimeOffset createdAt, CancellationToken ct = default)
    {
        await using var db = new DataContext();
        note.Text = text;
        note.CreatedAt = createdAt;
        db.Notes.Update(note);
        await db.SaveChangesAsync(ct);
    }
    /// <summary>
    /// Удаляет сущность заметки из БД.
    /// </summary>
    public static async Task Delete(Note note, CancellationToken ct = default)
    {
        await using var db = new DataContext();
        db.Notes.Remove(note);
        await db.SaveChangesAsync(ct);
    }
    
}