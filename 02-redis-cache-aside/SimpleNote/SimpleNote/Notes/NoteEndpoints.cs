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
            if (MissingContent(request.Content) is { } problem) return problem;
            var note = await repo.AddAsync(request.Content);
            return Results.Created($"/notes/{note.Id}", note);
        });

        notes.MapPut("/{id:guid}", async (Guid id, UpdateNoteRequest request, INoteRepository repo) =>
        {
            if (MissingContent(request.Content) is { } problem) return problem;

            return await repo.UpdateAsync(id, request.Content)
                ? Results.NoContent()
                : Results.NotFound();
        });

        notes.MapDelete("/{id:guid}", async (Guid id, INoteRepository repo) =>
            await repo.DeleteAsync(id)
                ? Results.NoContent()
                : Results.NotFound());

        return app;
    }

    // Content is non-nullable in C#, but nothing enforces that on the wire: {"content": null} binds
    // happily and only fails at the NOT NULL constraint, which surfaces as a 500. Reject it here so
    // a bad request looks like a bad request.
    private static IResult? MissingContent(string? content) =>
        string.IsNullOrWhiteSpace(content)
            ? Results.ValidationProblem(new Dictionary<string, string[]>
            {
                ["content"] = ["Content is required and cannot be empty."]
            })
            : null;
}
