<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="MainCashup.aspx.cs" Inherits="System.Web.Mvc.ViewPage" %>

<%@ Register TagPrefix="acw" Namespace="Aspose.Cells.GridWeb" Assembly="Aspose.Cells.GridWeb, Version=5.0.1.2000, Culture=neutral, PublicKeyToken=00725b1ceb58d0a9" %>

<%@ Register TagPrefix="acw" Namespace="Aspose.Cells.GridWeb" Assembly="Aspose.Cells.GridWeb" %>



<!DOCTYPE html>



<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
   <script src="//ajax.googleapis.com/ajax/libs/jquery/1.11.1/jquery.min.js"></script>
    

</head>
<body>
    <form id="form1" runat="server">
        <div id="MainContent">
            <%--<asp:Button runat="server" Height="2%" Width="100%" ID="Button1" Text="New CashUp Sheet" OnClick="Button1_OnClick"/>--%>
            
            <asp:Label runat="server" ID="Label1"></asp:Label>
            <asp:ListBox runat="server" Height="10%" Width="100%" ID="ListBox1"/>
            

            <acw:GridWeb ID="GridWeb1" runat="server" Height="90%" Width="100%" OnSaveCommand="GridWeb1_SaveCommand">
            </acw:GridWeb>


        </div>


    </form>
    
    

</body>


<script type="text/javascript">
    $(document).ready(function () {
        // Handler for .ready() called.


        $('#ListBox1').change(function () {

            //get all vars
            var id = $("#ListBox1").val();

         
            //request
            $.ajax({
                url: '/Home/OpenXLSFile',
                data: { "filename": id },
                type: "POST",
                success: function (result) {
                    location.reload(true);          
                    //get the pricing
                },
                error: function () { alert('That did not work please try again'); }
            });

        });

    });

    

</script>




</html>



