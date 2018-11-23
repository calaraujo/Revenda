using Revenda.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Revenda.Classes
{
    public class DBHelper
    {
        public static Response SaveChanges(RevendaContext db)
        {
            try
            {
                db.SaveChanges();
                return new Response { Succeeded = true, };
            }
            catch (Exception ex)
            {
                var response = new Response { Succeeded = false, };
                if (ex.InnerException != null &&
                    ex.InnerException.InnerException != null &&
                    ex.InnerException.InnerException.Message.Contains("_Index"))
                {
                    response.Message = "Já existe um registro com o mesmo valor";
                }
                else if (ex.InnerException != null &&
                    ex.InnerException.InnerException != null &&
                    ex.InnerException.InnerException.Message.Contains("REFERENCE"))
                {
                    response.Message = "O registro não pode ser excluído porque possui registros relacionados";
                }
                else
                {
                    response.Message = ex.Message;
                }

                return response;
            }
        }
    }
}