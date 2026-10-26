namespace UabMeto.Core.Content
{
    /// <summary>
    /// Status validasi sebuah materi.
    /// <see cref="Unvalidated"/> khusus untuk konten contoh/placeholder yang belum
    /// diperiksa penutur lokal dan tidak boleh diterbitkan.
    /// </summary>
    public enum ValidationStatus
    {
        Unvalidated = 0,
        Draft = 1,
        InReview = 2,
        Validated = 3,
        Published = 4
    }
}
