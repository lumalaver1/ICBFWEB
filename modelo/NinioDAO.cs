using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Jardines_ICBF.modelo
{
    public class NinioDAO
    {
        ORMDataContext BD = new ORMDataContext("Data Source=PC-MIGUEL-C\\SQLEXPRESS;Initial Catalog=db_ICBF;Integrated Security=True;Encrypt=True;TrustServerCertificate=True;");
        //ORMDataContext BD = new ORMDataContext();

        public void registrar(Ninos ninos)
        {
            BD.Ninos.InsertOnSubmit(ninos);
            BD.SubmitChanges(); // GUARDAR LOS CAMBIOS
        }

        public Object consultarTodo()
        {
            return from N in BD.Ninos select N;
        }

        public Ninos consultarNinioId(int idNinio)
        {
            return (from N in BD.Ninos
                    where N.idNino == idNinio
                    select N).FirstOrDefault();
        }

        public void editar(Ninos ninos)
        {
            Ninos ninoEditar = consultarNinioId(ninos.idNino);
            ninoEditar.NUIP = ninos.NUIP;
            ninoEditar.tipoSangre = ninos.tipoSangre;
            ninoEditar.ciudadNacimiento = ninos.ciudadNacimiento;
            ninoEditar.peso = ninos.peso;
            ninoEditar.estatura = ninos.estatura;
            ninoEditar.idJardin = ninos.idJardin;
            ninoEditar.idAcudiente = ninos.idAcudiente;
            ninoEditar.idDatosBasicos = ninos.idDatosBasicos;
            ninoEditar.idEps = ninos.idEps;
            BD.SubmitChanges();
        }

        public void eliminar(int idNinio)
        {
            Ninos ninoEliminar = consultarNinioId(idNinio);
            BD.Ninos.DeleteOnSubmit(ninoEliminar);
            BD.SubmitChanges();
        }
    }
}