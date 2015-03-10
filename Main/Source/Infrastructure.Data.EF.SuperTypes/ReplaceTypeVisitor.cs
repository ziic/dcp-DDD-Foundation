using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace dcp.DDD.Infrastructure.Data.EF.SuperTypes
{
    /// <summary>
    /// for internal using
    /// </summary>
    public class ReplaceTypeVisitor : ExpressionVisitor
    {
        private readonly Dictionary<Type, Type> _mappings;

        private Dictionary<string, ParameterExpression> _convertedParameters;

        public ReplaceTypeVisitor(Dictionary<Type, Type> mappings)
        {
            _mappings = mappings;
        }

        public Expression<Func<TTo, TDest>> Convert<TFrom, TTo, TDest>(Expression<Func<TFrom, TDest>> expression)
        {
            //var evalExpress = PartialEvaluatingExpressionTreeVisitor.EvaluateIndependentSubtrees(expression);
            //for each parameter in the original expression creates a new parameter with the same name but with changed type 
            _convertedParameters = expression.Parameters
                .ToDictionary(
                    x => x.Name,
                    x => Expression.Parameter(typeof(TTo), x.Name)
                );

            return (Expression<Func<TTo, TDest>>)Visit(expression);
        }

        //handles Properties and Fields accessors 
        protected override Expression VisitMember(MemberExpression node)
        {
            //we want to replace only the nodes of type TFrom
            //so we can handle expressions of the form x=> x.Property.SubProperty
            //in the expression x=> x.Property1 == 6 && x.Property2 == 3
            //this replaces         ^^^^^^^^^^^         ^^^^^^^^^^^            
            Type typeTo;
            if (_mappings.TryGetValue(node.Member.DeclaringType, out typeTo))
            {
                //gets the memberinfo from type TTo that matches the member of type TFrom
                var memeberInfo = typeTo.GetMember(node.Member.Name).First();

                //this will actually call the VisitParameter method in this class
                var newExp = Visit(node.Expression);
                return Expression.MakeMemberAccess(newExp, memeberInfo);
            }

            return base.VisitMember(node);
        }

        // this will be called where ever we have a reference to a parameter in the expression
        // for ex. in the expression x=> x.Property1 == 6 && x.Property2 == 3
        // this will be called twice     ^                   ^
        protected override Expression VisitParameter(ParameterExpression node)
        {
            var newParameter = _convertedParameters[node.Name];
            return newParameter;
        }

        //this will be the first Visit method to be called
        //since we're converting LamdaExpressions
        protected override Expression VisitLambda<T>(Expression<T> node)
        {
            //visit the body of the lambda, this will Traverse the ExpressionTree 
            //and recursively replace parts of the expresion we for witch we have matching Visit methods 
            var newExp = Visit(node.Body);

            //this will create the new expression            
            return Expression.Lambda(newExp, _convertedParameters.Select(x => x.Value));
        }
    }
}