using System.Collections.Generic;
using System.Text;

namespace Decuplr.Serialization.Binary {
    /// <summary>
    /// Represents a class that "formats data" as
    /// </summary>
    /// <typeparam name="T"></typeparam>

    // TODO : Add document to tell everyone that this should inheritted as struct and it wouldn't be boxed

    public interface IFormatter<T> {
        T FormatAs();
    }
}
