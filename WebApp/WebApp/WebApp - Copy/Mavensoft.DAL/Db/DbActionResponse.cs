using System.Collections;

namespace Mavensoft.DAL.Db
{
    public class DbActionResponse
    {
        public string EntityName { set; get; }
        public int EntityId { set; get; }
        public char ActionType { set; get; }
        public bool isSuccess { set; get; }
        public string SuccessMessage { set; get; }
        public string ErrorMessage { set; get; }
        public IList Data { set; get; }
    }
}
