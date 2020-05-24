using System;
using System.Collections.Generic;
using System.Text;

namespace Decuplr.Serialization.Binary {
    /// <summary>
    /// A interface that should only be attached to a Attribute class, indicating a field to be parsed with the included parser set
    /// </summary>
    public interface ICustomParserAttribute {
        /// <summary>
        /// Supported parser type of this attribute
        /// </summary>
        IReadOnlyDictionary<Type, object> ParserSet { get; }

        /// <summary>
        /// Allow this parser to fall-back to default parser
        /// </summary>
        bool AllowParserFallback { get; }
    }
}
