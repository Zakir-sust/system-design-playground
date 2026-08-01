using Microsoft.EntityFrameworkCore;

namespace SimpleNote.Notes;

/// <summary>
/// Straight database access. Knows nothing about caching — that lives in
/// <see cref="CachedNoteRepository"/>, which wraps this one.
/// </summary>
public sealed class NoteRepository : INoteRepository
{
    private readonly AppDbContext _db;

    public NoteRepository(AppDbContext db) => _db = db;

    public async Task<IReadOnlyList<Note>> GetAllAsync() =>
        await _db.Notes.AsNoTracking().ToListAsync();

    public async Task<Note?> GetByIdAsync(Guid id) =>
        await _db.Notes.AsNoTracking().FirstOrDefaultAsync(n => n.Id == id);

    public async Task<Note> AddAsync(string content)
    {
        var note = new Note { Id = Guid.NewGuid(), Content = content };
        _db.Notes.Add(note);
        await _db.SaveChangesAsync();
        return note;
    }

    public async Task<bool> UpdateAsync(Guid id, string content)
    {
        var note = await _db.Notes.FirstOrDefaultAsync(n => n.Id == id);
        if (note is null) return false;

        note.Content = content;
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var note = await _db.Notes.FirstOrDefaultAsync(n => n.Id == id);
        if (note is null) return false;

        _db.Notes.Remove(note);
        await _db.SaveChangesAsync();
        return true;
    }
}
