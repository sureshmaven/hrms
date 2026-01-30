using System;

namespace Mavensoft.Common
{
    public class ExceptionNew : Exception
    {
        public string ErrorInfo { get; set; }
        public string ErrorMessage { get; set; }

        public ExceptionNew(string entity, Transaction_Touch_Type TransactionType, Exception ex)
        {
            string operation = TransactionType == Transaction_Touch_Type.I ? "insert" : (TransactionType == Transaction_Touch_Type.U ? "update" : (TransactionType == Transaction_Touch_Type.D ? "delete" : "select"));

            ErrorInfo = entity + " " + operation + " failed.";
            ErrorMessage = ex.Message + " >> " + ex.StackTrace;
        }

        public override string ToString()
        {
            return ErrorInfo + "#"
                + ErrorMessage;
        }
    }
}
