using lab12;

public class CrudTests : IAsyncLifetime
{
    // будем чистить БД, что б тесты не сломались
    public async Task InitializeAsync() //этот метод запускается перед Fact
    {
        await using var db = new DataContext();
        await db.Database.EnsureCreatedAsync(); // теперь если таблицы нет, то создаем
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
        var user = await Crud.CreateUser("ура первый юзер", CancellationToken.None);
        string text = "Тестовая заметка";
        var createdAt = DateTimeOffset.Now;

        // Act 
        var createdNote = await Crud.Create(text, createdAt, CancellationToken.None);

        // Assert 
        Assert.True(createdNote.Id > 0, "ID заметкии 0...");
        Assert.Equal(text, createdNote.Text);
        Assert.Equal(user.Id, createdNote.UserId);

        await using var newContext = new DataContext();
        var exists = newContext.Notes.Any(n => n.Id == createdNote.Id);
        Assert.True(exists, "Заметка не найдена");
    }
    
    [Fact]
    public async Task Read_Search()
    {
        // Arrange
        var user = await Crud.CreateUser("SearchUser", CancellationToken.None);
        var now = DateTimeOffset.Now;
            
        await Crud.Create("ironu", now, user.Id, ancellationToken.None);
        await Crud.Create("i dont no", now, user.Id, CancellationToken.None);
        await Crud.Create("i dont know", now, user.Id, CancellationToken.None);

        // Act
        var results = await Crud.Read("no"); 

        // Assert
        Assert.Equal(2, results.Count());
    }
    
    [Fact]
    public async Task Read_ById()
    {
        // Arrange
        var user = await Crud.CreateUser("FUser", CancellationToken.None);
        var now = DateTimeOffset.Now;
        var noteToFind = await Crud.Create("Izametka", now, user.Id, CancellationToken.None);

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
        var user = await Crud.CreateUser("Апдейтинг", CancellationToken.None);
        var note = await Crud.Create("директ бай роберт вейд", DateTimeOffset.Now, user.Id, CancellationToken.None);
            
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
        var user = await Crud.CreateUser("Daily quest", CancellationToken.None);
        var now = DateTimeOffset.Now;
        var note = await Crud.Create("еще один директ бай роберт вейд", DateTimeOffset.Now, user.Id, CancellationToken.None);

        // Act
        await Crud.Delete(note, CancellationToken.None); 

        // Assert
        var deletedNote = await Crud.Read(note.Id);
        Assert.Null(deletedNote); 
    }
    
    [Fact]
    public async Task GetNotes_ByUser()
    {
        // Arrange
        var user = await Crud.CreateUser("Ты", CancellationToken.None);
        var now = DateTimeOffset.Now;
        
        await Crud.Create("Твоя заметка 1", now, user.Id, CancellationToken.None);
        await Crud.Create("Твоя заметка 2", now, user.Id, CancellationToken.None);
        
        var otherUser = await Crud.CreateUser("Сын маминой подруги", CancellationToken.None);
        await Crud.Create("А это не твоя заметка", now, otherUser.Id, CancellationToken.None);

        // Act
        var userNotes = await Crud.GetNotesByUser(user.Id); 

        // Assert
        Assert.Equal(2, userNotes.Count);
        Assert.All(userNotes, n => Assert.Equal(user.Id, n.UserId));
    }
    
    [Fact]
    public async Task Create_User()
    {
        // Arrange
        string name = "тЕстИРоВщик";

        // Act 
        var createdUser = await Crud.CreateUser(name, CancellationToken.None);

        // Assert 
        Assert.True(createdUser.Id > 0, "ID пользователя равен 0...");
        Assert.Equal(name, createdUser.Name);

        await using var newContext = new DataContext();
        var exists = newContext.Users.Any(u => u.Id == createdUser.Id);
        Assert.True(exists, "Пользователь не найден");
    }
    
    [Fact]
    public async Task ReadUser_ById()
    {
        // Arrange
        var userToFind = await Crud.CreateUser("Поиск1212", CancellationToken.None);

        // Act
        var foundUser = await Crud.ReadUser(userToFind.Id); 

        // Assert
        Assert.NotNull(foundUser);
        Assert.Equal(userToFind.Id, foundUser.Id);
        Assert.Equal("Поиск1212", foundUser.Name);
    }
    
    [Fact]
    public async Task ReadUsers_Search()
    {
        // Arrange
        await Crud.CreateUser("Кроули", CancellationToken.None);
        await Crud.CreateUser("Чеширский кот", CancellationToken.None);
        await Crud.CreateUser("Радужный кот", CancellationToken.None);

        // Act
        var results = await Crud.ReadUsers("кот"); 

        // Assert
        Assert.Equal(2, results.Count());
    }
    
    [Fact]
    public async Task Update_User()
    {
        // Arrange
        using var context = new DataContext();
        var user = await Crud.CreateUser("Старое иМЯ", CancellationToken.None);
            
        string newName = "Новое иМЯ";
            
        // Act
        await Crud.UpdateUser(user, newName);

        // Assert
        var updatedUserFromDb = context.Users.Find(user.Id);
        Assert.NotNull(updatedUserFromDb);
        Assert.Equal(newName, updatedUserFromDb.Name);
    }
    
    [Fact]
    public async Task Delete_User()
    {
        // Arrange
        var user = await Crud.CreateUser("Подвержен удалению", CancellationToken.None);

        // Act
        await Crud.DeleteUser(user, CancellationToken.None); 

        // Assert
        var deletedUser = await Crud.ReadUser(user.Id);
        Assert.Null(deletedUser); 
    }
}
    