using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Baseline;
using Baseline.Conversion;

namespace Oakton.Parsing
{
    public class EnumerableArgument : Argument
    {
        private readonly MemberInfo _member;

        public EnumerableArgument(MemberInfo member, Conversions conversions) : base(member, conversions)
        {
            _member = member;

            _converter = conversions.FindConverter(member.GetMemberType().DeriveElementType());
        }

        public override bool Handle(object input, Queue<string> tokens)
        {
            var elementType = _member.GetMemberType().GetGenericArguments().First();
            var list = typeof (List<>).CloseAndBuildAs<IList>(elementType);

            var wasHandled = false;
            while (tokens.Count > 0 && !tokens.NextIsFlag())
            {
                var value = _converter(tokens.Dequeue());
                list.Add(value);

                wasHandled = true;
            }

            if (wasHandled)
            {
                setValue(input, list);
            }

            return wasHandled;
        }

        public override string ToUsageDescription()
        {
            return "<{0}1 {0}2 {0}3 ...>".ToFormat(_member.Name.ToLower());
        }
    }
}