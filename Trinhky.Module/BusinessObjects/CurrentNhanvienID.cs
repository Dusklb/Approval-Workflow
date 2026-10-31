using DevExpress.Data.Filtering;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmLib;

namespace Trinhky.Module.BusinessObjects
{
    public class CurrentNhanvienID : ICustomFunctionOperatorFormattable
    {
        #region ICustomFunctionOperatorFormattable Members
        // The function's expression to be evaluated on the server.
        string ICustomFunctionOperatorFormattable.Format(Type providerType, params string[] operands)
        {
            return operands[0];
            // This example implements the function for MS Access databases only.
            //            if (providerType == typeof(AccessConnectionProvider))
            //                return string.Format("datepart(\"m\", {0})", operands[0]);
            //            throw new NotSupportedException(string.Concat(
            //                "This provider is not supported: ", providerType.Name));
        }

        public static string NhanvienMail()
        {
            string mail = "";
            Nhansu nv=TCom.GetCurrentNhanvien();
            if (nv != null)
                mail = nv.Email;
            return mail;
        }

        #endregion

        #region ICustomFunctionOperator Members
        // Evaluates the function on the client.
        object ICustomFunctionOperator.Evaluate(params object[] operands)
        {
            //if (operands[0] != null)
            //{
            //    return NhanvienMail();
            //}
            //else
            //    return string.Empty;
            return NhanvienMail();
        }

        string ICustomFunctionOperator.Name
        {
            get { return nameof(NhanvienMail); }
        }

        Type ICustomFunctionOperator.ResultType(params Type[] operands)
        {
            return typeof(string);
        }
        #endregion
    }
}
