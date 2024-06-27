using Jardines_ICBF.modelo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Jardines_ICBF
{
    public partial class jardinweb : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                cargarDatos();
            }
        }

        public void cargarDatos()
        {
            JardinDAO jardinDAO = new JardinDAO();
            gdvJardines.DataSource = jardinDAO.consultarTodos();
            gdvJardines.DataBind();
        }

        public void visibilidadPaneles(bool panelRegistro, bool panelConsulta)
        {
            PanelRegistro.Visible = panelRegistro;
            PanelConsulta.Visible = panelConsulta;
        }

        public void visibilidadBotones(bool btn_Registrar, bool btn_Editar)
        {
            btnRegistrar.Visible = btn_Registrar;
            btnEditar.Visible = btn_Editar;
        }

        public void limpiarCampos()
        {
            txtIdJardin.Text = string.Empty;
            txtNombre.Text = string.Empty;
            txtDireccion.Text = string.Empty;
            ddlEstado.SelectedIndex = 0;
        }

        protected void gdvJardines_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            GridViewRow fila = (GridViewRow)((Control)e.CommandSource).NamingContainer;
            int indice = fila.RowIndex;

            if (e.CommandName == "Eliminar")
            {
                JardinDAO jardinDAO = new JardinDAO();
                jardinDAO.eliminar(int.Parse(gdvJardines.Rows[indice].Cells[0].Text));
                cargarDatos();
            }
            else if (e.CommandName == "Editar")
            {
                txtIdJardin.ReadOnly = true;
                visibilidadPaneles(true, false);
                visibilidadBotones(false, true);
                lblTitulo.Text = "Editar Jardín";
                lblMensaje.Text = "";

                txtIdJardin.Text = gdvJardines.Rows[indice].Cells[0].Text;
                txtNombre.Text = gdvJardines.Rows[indice].Cells[1].Text;
                txtDireccion.Text = gdvJardines.Rows[indice].Cells[2].Text;
                ddlEstado.Text = gdvJardines.Rows[indice].Cells[3].Text;
            }
        }

        protected void btnNuevo_Click(object sender, EventArgs e)
        {
            visibilidadPaneles(true, false);
            visibilidadBotones(true, false);
            limpiarCampos();
            lblTitulo.Text = "Registrar Jardín";
            lblMensaje.Text = "";
        }

        protected void btnRegistrar_Click(object sender, EventArgs e)
        {
            JardinDAO jardinDAO = new JardinDAO();
            Jardines jardin = new Jardines();
            jardin.nombre = txtNombre.Text;
            jardin.direccion = txtDireccion.Text;
            jardin.estado = ddlEstado.Text;

            if (jardinDAO.validarNombre(jardin.nombre))
            {
                jardinDAO.registrar(jardin);
                cargarDatos();
                visibilidadPaneles(false, true);
                limpiarCampos();
            }
            else
            {
                lblMensaje.Text = "El nombre del Jardín ya existe";
            }
        }

        protected void btnEditar_Click(object sender, EventArgs e)
        {
            JardinDAO jardinDAO = new JardinDAO();
            Jardines jardin = new Jardines();
            jardin.idJardin = int.Parse(txtIdJardin.Text);
            jardin.nombre = txtNombre.Text;
            jardin.direccion = txtDireccion.Text;
            jardin.estado = ddlEstado.Text;

            jardinDAO.editar(jardin);
            cargarDatos();
            visibilidadPaneles(false, true);
            limpiarCampos();
        }

        protected void btnVolver_Click(object sender, EventArgs e)
        {
            visibilidadPaneles(false, true);
            //limpiarCampos();
        }
    }
}