namespace AccountsApi.Infrastructure
{
    public enum Predicate
    {
        eq,
        neq,
        domain,
        lt,
        gt,
        @any,
        @null,
        starts,
        year,
        now,
        code,
        contains,
    }
}