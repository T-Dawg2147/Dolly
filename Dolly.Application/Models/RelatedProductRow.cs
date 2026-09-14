namespace Dolly.Application.Models;

public sealed class RelatedProductRow
{
    public int? LinkId { get; set; }
    public string? Id { get; set; }
    public string? RelatedId { get; set; }
    public string? RelatedDescription { get; set; }
    public string? RelatedSupplier { get; set; }
    public bool Unlink { get; set; }
}