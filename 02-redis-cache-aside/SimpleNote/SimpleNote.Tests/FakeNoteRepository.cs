using SimpleNote.Notes;

namespace SimpleNote.Tests;

public class FakeNoteRepository : INoteRepository
{
    public Dictionary<Guid, Note> Notes { get; } = new();

    // Tests assert this stayed at 0 to prove a cache hit never reached the database.
    public int GetByIdCalls { get; private set; }

    public Task<IReadOnlyList<Note>> GetAllAsync()
    {
        var notes = Notes.Values.ToList();
        return Task.FromResult<IReadOnlyList<Note>>(notes);
    }

    public Task<Note?> GetByIdAsync(Guid id)
    {
        // Counted before the lookup: the question is "did anything reach the database",
        // and a lookup that finds nothing still reached it.
        GetByIdCalls++;

        return Notes.TryGetValue(id, out var note)
            ? Task.FromResult<Note?>(note)
            : Task.FromResult<Note?>(null);
    }

    public Task<Note> AddAsync(string content)
    {
        // Id must be set on the note itself, not just used as the key — NoteRepository does the
        // same, and tests round-trip through note.Id after adding.
        var note = new Note { Id = Guid.NewGuid(), Content = content };
        Notes.Add(note.Id, note);
        return Task.FromResult(note);
    }

    public Task<bool> UpdateAsync(Guid id, string content)
    {
        if (Notes.TryGetValue(id, out var note))
        {
            note.Content = content;
            return Task.FromResult(true);
        }

        return Task.FromResult(false);
    }

    public Task<bool> DeleteAsync(Guid id)
    {
        return Task.FromResult(Notes.Remove(id));
    }
}