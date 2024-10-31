namespace Ibdal.Api.Data;

public static class QueryExtensions
{
    public static IFindFluent<T, T> FindNonArchived<T>(this IMongoCollection<T> collection, Expression<Func<T, bool>> filterExpression)
        where T : class
    {
        var filter = Builders<T>.Filter.And(
            Builders<T>.Filter.Eq("IsArchived", false),
            Builders<T>.Filter.Where(filterExpression)
        );
        
        return collection.Find(filter);
    }
    
    public static IFindFluent<T, T> FindNonArchived<T>(
        this IMongoCollection<T> collection,
        IClientSessionHandle session,
        Expression<Func<T, bool>> filterExpression)
        where T : class
    {
        var filter = Builders<T>.Filter.And(
            Builders<T>.Filter.Eq("IsArchived", false),
            Builders<T>.Filter.Where(filterExpression)
        );
        
        return collection.Find(session, filter);
    }
}