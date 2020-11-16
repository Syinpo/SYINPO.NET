using System;
using System.Collections.Generic;

namespace Syinpo.Core {
    public class SysException : Exception {
        public int Code { get; set; } = 500;

        public IList<SysFailure> ErrorData { get; set; }

        public bool HasErrorData
        {
            get { return ErrorData != null && ErrorData.Count > 0; }
        }

        public SysException() {}

        public SysException( string message )
            : base( message ) {
        }

        public SysException( string message, Exception innerException )
            : base( message, innerException ) {
        }

        public SysException( int code, string message  )
            : base( message ) {
            Code = code;
        }

        public SysException( int code, string message, Exception innerException )
            : base( message, innerException ) {
            Code = code;
        }

        public SysException( string message, IList<SysFailure> errorData )
            : base( message )
        {
            ErrorData = errorData;
        }
    }

    public class SysFailure {
        private SysFailure() {
        }

        /// <summary>Creates a new validation failure.</summary>
        public SysFailure( string propertyName, string errorMessage )
            : this( propertyName, errorMessage, (object)null ) {
        }

        /// <summary>Creates a new ValidationFailure.</summary>
        public SysFailure( string propertyName, string errorMessage, object attemptedValue ) {
            this.PropertyName = propertyName;
            this.ErrorMessage = errorMessage;
            this.AttemptedValue = attemptedValue;
        }

        /// <summary>The name of the property.</summary>
        public string PropertyName {
            get; set;
        }

        /// <summary>The error message</summary>
        public string ErrorMessage {
            get; set;
        }

        /// <summary>The property value that caused the failure.</summary>
        public object AttemptedValue {
            get; set;
        }

        /// <summary>Creates a textual representation of the failure.</summary>
        public override string ToString() {
            return this.ErrorMessage;
        }
    }
}
