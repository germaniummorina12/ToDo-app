using Library.Api.Dtos;

namespace Library.Api.Services;

public interface IBookServiece
{
    Task<IReadOnlyList<BookSummaryDto>> GetAllAsync(string? genre, CancallationToken ct = default);
    Task<BookSummaryDto?> GetByIdAsync(int id, CanacellationToken ct = default);

    Task<BookSummaryDto?> CreateAsync(CreateBookRequest request, CancellationToken ct = default);
}