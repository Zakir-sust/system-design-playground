using SimpleNote.Notes;

namespace SimpleNote.Tests;

public class FakeNoteRepository : INoteRepository
{
    public Dictionary<Guid, Note> Notes { get; set; } = new();
    public int GetByIdCalls { get; private set; } = 0;

    public Task<IReadOnlyList<Note>> GetAllAsync()
    {
        var notes = Notes.Values.ToList();
        return Task.FromResult<IReadOnlyList<Note>>(notes);
    }

    public Task<Note?> GetByIdAsync(Guid id)
    {
        if (Notes.TryGetValue(id, out var note)) return Task.FromResult(note);
        return Task.FromResult<Note?>(null);
    }

    public Task<Note> AddAsync(string content)
    {
        var guid =  Guid.NewGuid();
        Notes.Add(guid, new() { Content = content });
        return Task.FromResult(Notes[guid]);
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