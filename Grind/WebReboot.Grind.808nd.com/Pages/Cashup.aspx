<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Cashup.aspx.cs" Inherits="Web.Grind._808nd.com.Pages.Cashup" %>

<%@ Import Namespace="System.Web.Mvc.Html" %>
<%@ Register TagPrefix="acw" Namespace="Aspose.Cells.GridWeb" Assembly="Aspose.Cells.GridWeb, Version=5.0.1.2000, Culture=neutral, PublicKeyToken=00725b1ceb58d0a9" %>

<%@ Register TagPrefix="acw" Namespace="Aspose.Cells.GridWeb" Assembly="Aspose.Cells.GridWeb" %>


<!DOCTYPE html>



<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script src="//ajax.googleapis.com/ajax/libs/jquery/1.11.1/jquery.min.js"></script>
    <link rel="stylesheet" href="//code.jquery.com/ui/1.11.4/themes/smoothness/jquery-ui.css" />
    <script src="//code.jquery.com/ui/1.11.4/jquery-ui.js"></script>


    <link rel="stylesheet" href="~/Content/bootstrap.css" />
    <script src="/Scripts/jquery.hotkeys-0.7.9.js"></script>


</head>
<body>

    <div class="container">
        <div class="navbar navbar-inverse navbar-fixed-top">
            <div class="container">
                <div class="navbar-header">
                    <button type="button" class="navbar-toggle" data-toggle="collapse" data-target=".navbar-collapse">
                        <span class="icon-bar"></span>
                        <span class="icon-bar"></span>
                        <span class="icon-bar"></span>
                    </button>
                    <%= Html.ActionLink("Grind Cashup", "Index", "Home", null, new { @class = "navbar-brand" }) %>
                </div>
                <div class="navbar-collapse collapse">
                    <ul class="nav navbar-nav">
                        <li><a href="<%= ResolveUrl("~/Pages/Cashup.aspx") %>">View cashup sheet</a></li>
                        <li><a href="<%= ResolveUrl("~/Account/Register") %>">Register new user</a></li>
                        <li id="Emailer"><a href="#">Email all</a></li>
                        <li id="EmailerMe"><a href="#">Email just me</a></li>

                    </ul>

                    <%= Html.Partial("_LoginPartial") %>
                </div>
            </div>
        </div>
    </div>
    <div>
        <br />
        <br />
        <br />
    </div>
    <div>
        <form id="form1" runat="server">
            <asp:ScriptManager ID="scm" runat="server" EnablePageMethods="true" />
            <div id="MainContent">
                <%--<asp:Button runat="server" Height="2%" Width="100%" ID="Button1" Text="New CashUp Sheet" OnClick="Button1_OnClick"/>--%>
                <asp:Button OnClick="TestCommand" runat="server" Text="Download Current Sheet"></asp:Button>
                <asp:Label runat="server" ID="Label1"></asp:Label>
                <asp:Label Font-Name="Verdana" Font-Size="18" runat="server" ID="Label2"></asp:Label>
                <asp:Label runat="server" ID="Label3"></asp:Label>

                <div id="tabs">
                    <ul>
                        <li><a href="#tabs-1">Shoreditch</a></li>
                        <li><a href="#tabs-2">Soho</a></li>
                        <li><a href="#tabs-3">London</a></li>
                        <li><a href="#tabs-4">Holborn</a></li>
                        <li><a href="#tabs-5">Stratford</a></li>
                        <li><a href="#tabs-6">Radio</a></li>
                    </ul>
                    <div id="tabs-1">
                        <asp:ListBox CssClass="SheetSelector" runat="server" Height="15%" Width="100%" ID="ListBox1" />
                    </div>
                    <div id="tabs-2">
                           <asp:ListBox CssClass="SheetSelector" runat="server" Height="15%" Width="100%" ID="ListBox2" />
                    </div>
                    <div id="tabs-3">
                           <asp:ListBox CssClass="SheetSelector" runat="server" Height="15%" Width="100%" ID="ListBox3" />
                    </div>
                    <div id="tabs-4">
                           <asp:ListBox CssClass="SheetSelector" runat="server" Height="15%" Width="100%" ID="ListBox4" />
                    </div>
                    <div id="tabs-5">
                           <asp:ListBox CssClass="SheetSelector" runat="server" Height="15%" Width="100%" ID="ListBox5" />
                    </div>
                     <div id="tabs-6">
                           <asp:ListBox CssClass="SheetSelector" runat="server" Height="15%" Width="100%" ID="ListBox6" />
                    </div>

                </div>




                <acw:GridWeb ID="GridWeb1" runat="server" Height="85%" Width="100%" OnSaveCommand="GridWeb1_SaveCommand">
                </acw:GridWeb>

                <asp:Label Visible="False" runat="server" ID="SheetName"></asp:Label>

            </div>



        </form>


    </div>

    <div id="loading" style="display: none; position: fixed; top: 50%; left: 50%; background: url(spinner.gif) no-repeat center #fff; text-align: center; padding: 10px; font: normal 16px Tahoma, Geneva, sans-serif; border: 1px solid #666; margin-left: -50px; margin-top: -50px; z-index: 2; overflow: auto">
        <img src="/Images/ajax-loader-orange-transparent.gif" height="90px" width="90px" />
    </div>
</body>
    
    

<script type="text/javascript">




    /*  $(document).bind('keydown', 'Ctrl+u', function (event) {
          setTimeout(function () {
              $.ajax({
                  url: '/Pages/Cashup.aspx/SaveCurrentWorksheet',
                  data : "test.xls"
                  type: "GET",
              success: function (result) {
                  alert('Saved!');
                  //get the pricing
              },
              error: function () { alert('That did not work please try again'); }
          });
  
      }, 0);
      return false;
      });*/







    $(document).ready(function () {
        
        // Handler for .ready() called.
        $(function() {
                $("#tabs").tabs();

                var currentSetPage = sessionStorage.getItem('currentTab');
                
                $("#tabs").tabs("option", "active", currentSetPage);


    });

        $('.SheetSelector').change(function (e) {

            //get all vars         
          
            var id = this.value;
            /*alert(id);*/
            var currentPage = $("#tabs").tabs('option', 'active');        

            if (currentPage == undefined || currentPage === "") {

                currentPage = 0;
            }
            sessionStorage.setItem('currentTab', currentPage);

            //request
            $.ajax({
                url: '/Home/OpenXLSFile',
                data: { "filename": id},
                type: "POST",
                success: function (result) {
                    location.reload(true);
                    //get the pricing
                },
                error: function () { alert('That did not work please try again'); }
            });

        });

    });




    $('#Emailer').click(function () {
        //get all vars
        var id = '<%= User.Identity.Name%>';


        //request
        $.ajax({
            url: '/Email/EmailToAll',
            data: { "username": id },
            type: "POST",
            success: function () {
                alert('Email sent!!!');

            },
            error: function () { alert('That did not work please try again'); }
        });


    });


    $('#EmailerMe').click(function () {
        //get all vars
        var id = '<%= User.Identity.Name%>';


        //request
        $.ajax({
            url: '/Email/EmailToUser',
            data: { "username": id },
            type: "POST",
            success: function () {
                alert('Email sent!!!');

            },
            error: function () { alert('That did not work please try again'); }
        });


    });



    $(document).ajaxStart(function () {
        $("#loading").show();
    });

    $(document).ajaxStop(function () {
        $("#loading").hide();
    });

</script>




</html>



