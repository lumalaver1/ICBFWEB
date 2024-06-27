using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Jardines_ICBF.modelo
{
    public class JardinDAO
    {
        ORMDataContext BD = new ORMDataContext("Data Source=PC-MIGUEL-C\\SQLEXPRESS;Initial Catalog=db_ICBF;Integrated Security=True;Encrypt=True;TrustServerCertificate=True;");
        //ORMDataContext BD = new ORMDataContext();

        public void registrar(Jardines jardin)
        {
            BD.Jardines.InsertOnSubmit(jardin);
            BD.SubmitChanges(); // GUARDAR LOS CAMBIOS
        }

        public Object consultarTodos()
        {
            return from J in BD.Jardines select J;
        }

        public Jardines consultarJardinId(int idJardin)
        {
            return (from J in BD.Jardines
                    where J.idJardin == idJardin
                    select J).FirstOrDefault();
        }

        public bool validarNombre(string nombre)
        {
            Jardines nombreJardin = (from J in BD.Jardines
                                     where J.nombre == nombre
                                     select J).FirstOrDefault();

            if (nombreJardin != null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public void editar(Jardines jardin)
        {
            Jardines jardinEditar = consultarJardinId(jardin.idJardin);
            jardinEditar.nombre = jardin.nombre;
            jardinEditar.direccion = jardin.direccion;
            jardinEditar.estado = jardin.estado;
            BD.SubmitChanges();
        }

        public void eliminar(int idJardin)
        {
            Jardines jardinEliminar = consultarJardinId(idJardin);
            BD.Jardines.DeleteOnSubmit(jardinEliminar);
            BD.SubmitChanges();
        }
    }
}