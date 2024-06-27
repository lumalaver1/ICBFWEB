using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Jardines_ICBF.modelo
{
    public class UsuariosDAO
    {
        ORMDataContext BD = new ORMDataContext("Data Source=PC-MIGUEL-C\\SQLEXPRESS;Initial Catalog=db_ICBF;Integrated Security=True;Encrypt=True;TrustServerCertificate=True;");
        //ORMDataContext BD = new ORMDataContext();

        public void registrar(Usuarios usuarios)
        {
            BD.Usuarios.InsertOnSubmit(usuarios);
            BD.SubmitChanges(); // GUARDAR LOS CAMBIOS
        }

        public Object consultarTodos()
        {
            return from U in BD.Usuarios select U;
        }

        public Usuarios consultarUsuarioId(int idUsuario)
        {
            return (from U in BD.Usuarios
                    where U.idUsuario == idUsuario
                    select U).FirstOrDefault();
        }

        public void editar(Usuarios usuarios)
        {
            Usuarios usuarioEditar = consultarUsuarioId(usuarios.idUsuario);
            usuarioEditar.rol = usuarios.rol;
            usuarioEditar.nombreUsuario = usuarios.nombreUsuario;
            usuarioEditar.clave = usuarios.clave;
            usuarioEditar.idDatosBasicos = usuarios.idDatosBasicos;
            BD.SubmitChanges();
        }

        public void eliminar(int idUsuario)
        {
            Usuarios usuarioEliminar = consultarUsuarioId(idUsuario);
            BD.Usuarios.DeleteOnSubmit(usuarioEliminar);
            BD.SubmitChanges();
        }
    }
}