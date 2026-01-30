using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    using System;
   
        public interface IDbEntity<TIdType> where TIdType : IEquatable<TIdType>
        {
            /// <summary>
            /// Gets or sets the identifier.
            /// </summary>
            /// <value>
            /// The identifier.
            /// </value>
            TIdType Id { get; set; }
        }
    
}
