using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using Syinpo.Core.Reflection;

namespace Syinpo.Core.IO {
    public class DelimitedLayout<T> {
        public DelimitedLayout()
        {
            InnerFields = new List<(PropertyInfo, int)>();
        }

        public List<(PropertyInfo, int)> InnerFields {
            get; set;
        }


        private MemberExpression GetMemberExpression<TMember>( Expression<Func<T, TMember>> expression ) {
            MemberExpression memberExpression = (MemberExpression)null;
            if( expression.Body.NodeType == ExpressionType.Convert )
                memberExpression = ( (UnaryExpression)expression.Body ).Operand as MemberExpression;
            else if( expression.Body.NodeType == ExpressionType.MemberAccess )
                memberExpression = expression.Body as MemberExpression;
            if( memberExpression == null )
                throw new ArgumentException( "Not a member access", nameof( expression ) );
            return memberExpression;
        }

        public DelimitedLayout<T> WithMember<TMember>( Expression<Func<T, TMember>> expression, int index ) {
            var propertyInfo = (PropertyInfo)this.GetMemberExpression( expression ).Member;
            this.InnerFields.Add( (propertyInfo, index) );
            return this;
        }


        private string innerDelimiter = ",";
        public DelimitedLayout<T> WithDelimiter( string delimiter ) {
            this.Delimiter = delimiter;
            return this;
        }

        private string Delimiter {
            get {
                return this.innerDelimiter;
            }
            set {
                if( string.IsNullOrEmpty( value ) )
                    throw new ArgumentException( "Delimiter cannot be null or empty", nameof( value ) );
                this.innerDelimiter = value;
            }
        }

        internal int HeaderLinesCount {
            get; set;
        }
        public DelimitedLayout<T> HeaderLines( int count ) {
            this.HeaderLinesCount = count;
            return this;
        }


        public T ParseLine( string line ) {
            T obj = (T)ReflectionUtils.CreateInstanceFromType( typeof( T ), null );
            string delimiter = this.Delimiter;
            var data = line.Split( delimiter ).ToArray();
            foreach( var field in this.InnerFields ) {
                var val = ReflectionUtils.StringToTypedValue( data[ field.Item2 ], field.Item1.PropertyType );

                ReflectionUtils.SetProperty( obj, field.Item1.Name, val );
            }
            return obj;
        }

    }
    public class FlatFile<T> where T : new() {
        private readonly DelimitedLayout<T> layout;
        private readonly Stream innerStream;
        private readonly Encoding encoding;

        public FlatFile( DelimitedLayout<T> layout, Stream innerStream, Encoding encoding ) {
            this.layout = layout;
            this.innerStream = innerStream;
            this.encoding = encoding;
        }
        public string ReadLine( StreamReader streamReader ) {
            return streamReader.ReadLine();
        }

        public IEnumerable<T> Read() {
            StreamReader reader = new StreamReader( this.innerStream, this.encoding );
            this.SkipHeaders( reader );
            string line;
            while( ( line = this.ReadLine( reader ) ) != null ) {
                bool flag = false;
                T obj = default( T );
                try {
                    obj = this.layout.ParseLine( line );
                }
                catch( Exception ex ) {
                    flag = true;
                }
                if( !flag )
                    yield return obj;
            }
        }

        private void SkipHeaders( StreamReader reader ) {
            for( int index = 0; index < this.layout.HeaderLinesCount && !reader.EndOfStream; ++index )
                reader.ReadLine();
        }

    }

}
