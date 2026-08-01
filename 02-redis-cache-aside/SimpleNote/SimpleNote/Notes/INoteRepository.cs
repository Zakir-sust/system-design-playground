namespace SimpleNote.Notes;

public interface INoteRepository
{
    Task<IReadOnlyList<Note>> GetAllAsync();

    Task<Note?> GetByIdAsync(Guid id);

    Task<Note> AddAsync(string content);

    /// <returns><c>false</c> if no note with that id exists.</returns>
    Task<bool> UpdateAsync(Guid id, string content);

    /// <returns><c>false</c> if no note with that id exists.</returns>
    Task<bool> DeleteAsync(Guid id);
}
