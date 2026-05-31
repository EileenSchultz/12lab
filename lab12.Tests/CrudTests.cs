using lab12;

public class CrudTests : IAsyncLifetime
{
    // будем чистить БД, что б тесты не сломались
    public async Task InitializeAsync() //этот метод запускается перед Fact
    {
        await using var db = new DataContext();
        db.Notes.RemoveRange(db.Notes);
        await db.SaveChangesAsync();
    }

    public Task DisposeAsync() // а это после, но он пустой, ибо чистить нечего
    {
        return Task.CompletedTask; // так что вернем пустую задачу
    }

    [Fact]
    public async Task Create_Note()
    {
        // Arrange
        string text = "Тестовая заметка";
        var createdAt = DateTimeOffset.Now;

        // Act 
        var createdNote = await Crud.Create(text, createdAt, CancellationToken.None);

        // Assert 
        Assert.True(createdNote.Id > 0, "ID заметкии 0...");
        Assert.Equal(text, createdNote.Text);

        await using var newContext = new DataContext();
        var exists = newContext.Notes.Any(n => n.Id == createdNote.Id);
        Assert.True(exists, "Заметка не найдена");
    }
    
    [Fact]
    public async Task Read_Search()
    {
        // Arrange
        var now = DateTimeOffset.Now;
            
        await Crud.Create("ironu", now, CancellationToken.None);
        await Crud.Create("i dont no", now, CancellationToken.None);
        await Crud.Create("i dont know", now, CancellationToken.None);

        // Act
        var results = await Crud.Read("no"); 

        // Assert
        Assert.Equal(2, results.Count());
    }
    
    [Fact]
    public async Task Read_ById()
    {
        // Arrange
        var now = DateTimeOffset.Now;
        var noteToFind = await Crud.Create("Izametka", now, CancellationToken.None);

        // Act
        var foundNote = await Crud.Read(noteToFind.Id); 

        // Assert
        Assert.NotNull(foundNote);
        Assert.Equal(noteToFind.Id, foundNote.Id);
        Assert.Equal("Izametka", foundNote.Text);
    }
    
    [Fact]
    public async Task Update_Note()
    {
        // Arrange
        using var context = new DataContext();
        var note = await Crud.Create("директ бай роберт вейд", DateTimeOffset.Now, CancellationToken.None);
            
        string newText = "Коньтьнью";
        var updateTime = DateTimeOffset.Now;
        note.Text = newText;
            
        // Act
        await Crud.Update(note, newText, updateTime);

        // Assert
        var updatedNoteFromDb = context.Notes.Find(note.Id);
        Assert.NotNull(updatedNoteFromDb);
        Assert.Equal(newText, updatedNoteFromDb.Text);
    }
    
    [Fact]
    public async Task Delete_Note()
    {
        // Arrange
        var now = DateTimeOffset.Now;
        var note = await Crud.Create("еще один директ бай роберт вейд", DateTimeOffset.Now, CancellationToken.None);

        // Act
        await Crud.Delete(note, CancellationToken.None); 

        // Assert
        var deletedNote = await Crud.Read(note.Id);
        Assert.Null(deletedNote); 
    }
}
    