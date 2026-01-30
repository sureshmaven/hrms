using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    /// <summary>
    /// Options for Database include
    /// DropCreateDatabaseIfModelChanges
    /// DropCreateDatabaseAlways
    /// CreateDatabaseIfNotExists
    /// MigrateDatabaseToLatestVersion
    /// </summary>
    public partial class ContextBaseDbInitializer : DropCreateDatabaseIfModelChanges<ContextBase>
    {
        #region "CTors"
        /// <summary>
        /// Initializes a new instance of the <see cref="ContextBaseDbInitializer"/> class.
        /// </summary>
        public ContextBaseDbInitializer()
        {
        }

        #endregion "CTors"

        /// <summary>
        /// Seeds the specified o context.
        /// </summary>
        /// <param name="oContext">The o context.</param>
        protected override void Seed(ContextBase oContext)
        {
            base.Seed(oContext);
        }
    }
}
