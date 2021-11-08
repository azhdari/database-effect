namespace LanguageExt.Effects.Database;

using Microsoft.EntityFrameworkCore;
using System;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

public static class StoredProcExtensions
{
    public static StoredProcQuery WithParam(this StoredProcQuery query,
                                            string name,
                                            Option<object> value,
                                            ParameterDirection direction = ParameterDirection.Input,
                                            DbType? dbType = null,
                                            int size = 0)
    {
        var cmd       = query.Context.Database.GetDbConnection().CreateCommand();
        var parameter = CreateParameter(cmd, name, value, direction, dbType, size);

        return query with { Parameters = query.Parameters.Add(parameter) };
    }

    public static StoredProcQuery WithParams<TEntity>(this StoredProcQuery query,
                                                      TEntity entity,
                                                      Expression<Func<TEntity, object>> expression)
        where TEntity : class, new()
    {
        var cmd   = query.Context.Database.GetDbConnection().CreateCommand();
        var value = expression.Compile().Invoke(entity);

        Option<PropertyInfo> GetProperty(string name)
            =>
            #pragma warning disable CS8622
            entity.GetType().GetProperty(name).Apply(Optional<PropertyInfo>);
            #pragma warning restore CS8622

        Option<object> GetPropertyValue(PropertyInfo prop)
            =>
            #pragma warning disable CS8622
            prop.GetValue(entity).Apply(Optional<object>);
            #pragma warning restore CS8622

        Option<DbParameter> AddUnary(UnaryExpression unary) {
            if (unary.Operand is MethodCallExpression methodExpression) {
                return CreateParameter(cmd, methodExpression.Method.Name, value);
            }
            else {
                return Option<DbParameter>.None;
            }
        }

        Option<Arr<DbParameter>> AddRest()
            =>  expression.Body
                          .Type
                          .GetProperties()
                          .Select(property => new
                          {
                              Property = property,
                              Value = GetProperty(property.Name).Bind(GetPropertyValue)
                          })
                          .Select((x) => x.Value.Map(v => CreateParameter(cmd, x.Property.Name, v)))
                          .Somes()
                          .Apply(toArray);

        var parameters = expression.Body switch
        {
            UnaryExpression unary => AddUnary(unary).Apply(o => o.Map(x => Array(x))),
            MemberExpression member => CreateParameter(cmd, member.Member.Name, value).Apply(x => Array(x)),
            _ => AddRest(),
        };

        return parameters.Match(
            Some: l => query with { Parameters = query.Parameters.AddRange(l) },
            None: () => query
        );
    }

    private static DbParameter CreateParameter(DbCommand cmd,
                                               string name,
                                               Option<object> value,
                                               ParameterDirection direction = ParameterDirection.Input,
                                               DbType? dbType = null,
                                               int size = 0)
    {
        DbParameter parameter = cmd.CreateParameter();
        parameter.ParameterName = name;
        parameter.Direction = direction;
        parameter.Size = size;

        if (direction == ParameterDirection.Input || direction == ParameterDirection.InputOutput)
        {
            parameter.Value = value.IfNone(DBNull.Value);
        }

        if (dbType.HasValue)
        {
            parameter.DbType = dbType.Value;
        }

        return parameter;
    }
}
