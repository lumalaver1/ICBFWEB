using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Jardines_ICBF.modelo
{
    public class DatosBasicosDAO
    {
        ORMDataContext BD = new ORMDataContext("Data Source=PC-MIGUEL-C\\SQLEXPRESS;Initial Catalog=db_ICBF;Integrated Security=True;Encrypt=True;TrustServerCertificate=True;");
        //ORMDataContext BD = new ORMDataContext();

        public void registrar(DatosBasicos datosBasicos)
        {
            BD.DatosBasicos.InsertOnSubmit(datosBasicos);
            BD.SubmitChanges(); // GUARDAR LOS CAMBIOS
        }

        public Object consultarTodos()
        {
            return from D in BD.DatosBasicos select D;
        }

        public DatosBasicos consultarDatosBasicosId(int idDatosBasicos)
        {
            return (from D in BD.DatosBasicos
                    where D.idDatosBasicos == idDatosBasicos
                    select D).FirstOrDefault();
        }

        public void editar(DatosBasicos datosBasicos)
        {
            DatosBasicos datosBasicosEditar = consultarDatosBasicosId(datosBasicos.idDatosBasicos);
            datosBasicosEditar.nombres = datosBasicos.nombres;
            datosBasicosEditar.fechaNacimiento = datosBasicos.fechaNacimiento;
            datosBasicosEditar.celular = datosBasicos.celular;
            datosBasicosEditar.direccion = datosBasicos.direccion;
            BD.SubmitChanges();
        }

        public void eliminar(int idDatosBasicos)
        {
            DatosBasicos datosBasicosEliminar = consultarDatosBasicosId(idDatosBasicos);
            BD.DatosBasicos.DeleteOnSubmit(datosBasicosEliminar);
            BD.SubmitChanges();
        }
    }
}