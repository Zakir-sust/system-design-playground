namespace SimpleNote.Notes;

public record CreateNoteRequest(string Content);

public record UpdateNoteRequest(string Content);

/// <summary>
/// HTTP concerns only: bind the request, call the repository, map the result to a status code.
/// No caching and no EF here — that is what keeps these handlers boring.
/// </summary>
public static class NoteEndpoints
{
    public static IEndpointRouteBuilder MapNoteEndpoints(this IEndpointRouteBuilder app)
    {
        var notes = app.MapGroup("/notes");

        notes.MapGet("/", async (INoteRepository repo) =>
            Results.Ok(await repo.GetAllAsync()));

        notes.MapGet("/{id:guid}", async (Guid id, INoteRepository repo) =>
            await repo.GetByIdAsync(id) is { } note
                ? Results.Ok(note)
                : Results.NotFound());

        notes.MapPost("/", async (CreateNoteRequest request, INoteRepository repo) =>
        {
            var note = await repo.AddAsync(request.Content);
            return Results.Created($"/notes/{note.Id}", note);
        });

        notes.MapPut("/{id:guid}", async (Guid id, UpdateNoteRequest request, INoteRepository repo) =>
            await repo.UpdateAsync(id, request.Content)
                ? Results.NoContent()
                : Results.NotFound());

        notes.MapDelete("/{id:guid}", async (Guid id, INoteRepository repo) =>
            await repo.DeleteAsync(id)
                ? Results.NoContent()
                : Results.NotFound());

        return app;
    }
}
