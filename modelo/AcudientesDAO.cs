using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Jardines_ICBF.modelo
{
    public class AcudientesDAO
    {
        ORMDataContext BD = new ORMDataContext("Data Source=PC-MIGUEL-C\\SQLEXPRESS;Initial Catalog=db_ICBF;Integrated Security=True;Encrypt=True;TrustServerCertificate=True;");
        //ORMDataContext BD = new ORMDataContext();

        public void registrar(Acudientes acudientes)
        {
            BD.Acudientes.InsertOnSubmit(acudientes);
            BD.SubmitChanges(); // GUARDAR LOS CAMBIOS
        }

        public Object consultarTodos()
        {
            return from A in BD.Acudientes select A;
        }

        public Acudientes consultarAcudienteId(int idAcudiente)
        {
            return (from A in BD.Acudientes
                    where A.idAcudiente == idAcudiente
                    select A).FirstOrDefault();
        }

        public void editar(Acudientes acudientes)
        {
            Acudientes acudienteEditar = consultarAcudienteId(acudientes.idAcudiente);
            acudienteEditar.cedula = acudientes.cedula;
            acudienteEditar.telefono = acudientes.telefono;
            acudienteEditar.correo = acudientes.correo;
            acudienteEditar.idUsuario = acudientes.idUsuario;
            BD.SubmitChanges();
        }

        public void eliminar(int idAcudiente)
        {
            Acudientes acudienteEliminar = consultarAcudienteId(idAcudiente);
            BD.Acudientes.DeleteOnSubmit(acudienteEliminar);
            BD.SubmitChanges();
        }
    }
}