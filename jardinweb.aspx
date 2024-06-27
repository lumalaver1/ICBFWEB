<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="jardinweb.aspx.cs" Inherits="Jardines_ICBF.jardinweb" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/css/bootstrap.min.css" rel="stylesheet" integrity="sha384-QWTKZyjpPEjISv5WaRU9OFeRpok6YctnYmDr5pNlyT2bRjXh0JMhjY6hW+ALEwIH" crossorigin="anonymous">
    <title>ICBF - JARDINES</title>
</head>
<body>
    <form id="form1" runat="server">
        <div class="container mt-3">
            <asp:Panel ID="PanelRegistro" runat="server" Visible="False">
                <h1><asp:Label ID="lblTitulo" runat="server" Text=""></asp:Label></h1>
                <div class="container">
                    <asp:TextBox ID="txtIdJardin" runat="server" CssClass="form-control" Visible="False"></asp:TextBox>
                    
                    <div class="row mt-2">
                        <asp:Label ID="Label2" runat="server" Text="Nombre"></asp:Label>
                        <asp:TextBox ID="txtNombre" runat="server" CssClass="form-control"></asp:TextBox>
                    </div>
                    <div class="row mt-2">
                        <asp:Label ID="Label3" runat="server" Text="Dirección"></asp:Label>
                        <asp:TextBox ID="txtDireccion" runat="server" CssClass="form-control"></asp:TextBox>
                    </div>
                    <div class="row mt-2">
                        <asp:Label ID="Label4" runat="server" Text="Estado"></asp:Label>
                        <asp:DropDownList ID="ddlEstado" runat="server" CssClass="form-control">
                            <asp:ListItem>Aprobado</asp:ListItem>
                            <asp:ListItem>En Tramite</asp:ListItem>
                            <asp:ListItem>Negado</asp:ListItem>
                        </asp:DropDownList>
                    </div>
                </div>
                <asp:Label ID="lblMensaje" runat="server" Text="" CssClass="text-danger fw-medium"></asp:Label>
                <div class="mt-3">
                    <asp:Button ID="btnRegistrar" runat="server" Text="Registrar" CssClass="btn btn-success" OnClick="btnRegistrar_Click" />
                    <asp:Button ID="btnEditar" runat="server" Text="Editar" CssClass="btn btn-success" OnClick="btnEditar_Click" Visible="False" />
                    <asp:Button ID="btnVolver" runat="server" Text="Volver" CssClass="btn btn-primary" OnClick="btnVolver_Click" />
                </div>
            </asp:Panel>
            <asp:Panel ID="PanelConsulta" runat="server">
                <h1>Lista de Jardines</h1>
                <asp:Button ID="btnNuevo" runat="server" Text="Nuevo" CssClass="btn btn-success mb-2" OnClick="btnNuevo_Click" />
                <asp:GridView ID="gdvJardines" runat="server" CssClass="table table-dark" AutoGenerateColumns="False" OnRowCommand="gdvJardines_RowCommand">
                    <Columns>
                        <asp:BoundField DataField="idJardin" HeaderText="ID JARDIN" />
                        <asp:BoundField DataField="nombre" HeaderText="NOMBRE" />
                        <asp:BoundField DataField="direccion" HeaderText="DIRECCIÓN" />
                        <asp:BoundField DataField="estado" HeaderText="ESTADO" />
                        <asp:TemplateField HeaderText="ACCIONES">
                            <ItemTemplate>
                                <asp:ImageButton ID="imgBtnEditar" runat="server" ImageUrl="~/img/edit.png" Width="24px" CommandName="Editar" />
                                <asp:ImageButton ID="imgBtnEliminar" runat="server" ImageUrl="~/img/delete.png" Width="24px" CommandName="Eliminar" />
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
            </asp:Panel>
        </div>
    </form>
</body>
</html>
