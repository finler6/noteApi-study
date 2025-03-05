using noteApi;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<NoteDbContext>();

var app = builder.Build();

if (app.Environment.IsDevelopment()) 
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<NoteDbContext>();
    db.Database.Migrate();
}

app.MapGet("/notes", async (NoteDbContext db) => await db.Notes.ToListAsync());

app.MapGet("/notes/{id:int}", async (int id, NoteDbContext db) =>
{
    var note = await db.Notes.FindAsync(id);
    return note != null ? Results.Json(note) : Results.NotFound("Note not found!");
});

app.MapPost("/notes", async (Note note, NoteDbContext db) =>
{
    db.Notes.Add(note);
    await db.SaveChangesAsync();
    return Results.Created($"/notes/{note.Id}", note);
});

app.MapPut("/notes/{id:int}", async (int id, Note updatedNote, NoteDbContext db) =>
{
    var note = await db.Notes.FindAsync(id);
    if (note == null) return Results.NotFound("Task not found");

    note.Title = updatedNote.Title;
    note.Content = updatedNote.Content;
    await db.SaveChangesAsync();
    
    return Results.Ok(note);
});

app.MapDelete("/notes/{id:int}", async (int id, NoteDbContext db) =>
{
    var note = await db.Notes.FindAsync(id);
    if (note == null) return Results.NotFound("Task not found");
    
    db.Notes.Remove(note);
    await db.SaveChangesAsync();
    return Results.Ok("Note deleted");
});

app.Run();

public class Note
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
}