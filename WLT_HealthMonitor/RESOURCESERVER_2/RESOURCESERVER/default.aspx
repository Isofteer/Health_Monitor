<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="default.aspx.cs" Inherits="RESOURCESERVER.index" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <title>Resource Server </title>
    <style type="text/css">
        .container {
            background-color: #99CCFF;
            border: thick solid #808080;
            padding: 20px;
            margin: 20px;
            border-style:none;

        }
    </style>



    <link href="lib/materialize/css/materialize.min.css" rel="stylesheet" />
    <script src="lib/materialize/js/materialize.min.js"></script>



</head>
<body>
    <div style="display:none">
        <div class="container_">
            <input type="text" id="message" />
            <input type="button" id="sendmessage" value="Send" />
            <input type="hidden" id="displayname" />
            <ul style="display:none" id="discussion"></ul>
        </div>
    </div>


    <style>
        .height-_full {
            height: 100% !important;
        }

        html, body {
            height: 100%;
        }

        .message_box {
            position: absolute;
            left: 40%;
            top: 45%;
        }

        .up-time {
            width: 300px;
        }
    </style>
    <div class="card card-horizontal height-_full">
         
        <div id ="cpus"  class="card">

            <ul id ="cpudata">
               
            </ul>

        </div>

        <div style="float:right;padding:15px;width:350px;" class="card">
            <span>Uptime</span>
            <h3 class="up-time" id="uptime"></h3>
        </div>
        <div class="  message_box ">           
         
            <span>
                <img src="src/preloader.gif" />
            </span>
               <h4 style="padding:px;" class="">  Server Running... </h4>
            <h3 id="host_identifier"> </h3>
        </div>

    </div>

    <script src="Scripts/jquery-1.6.4.min.js"></script>
    <script src="Scripts/jquery.signalR-2.2.3.min.js"></script>
    <!--<script src="signalr/hubs"></script>-->
    <!--<script src="~/signalr/hubs"></script>-->
    <script type="text/javascript" src='<%= ResolveClientUrl("~/signalr/hubs") %>'></script>

    <script type="text/javascript">
        $(function () {
            // Declare a proxy to reference the hub.
            var chat = $.connection.chatHub;
            // Create a function that the hub can call to broadcast messages.




            chat.client.Uptime = function (message) {

                $("#uptime").text(message);


            }



            chat.client.GetClienturl = function (Client) {


                $("#host_identifier").text("connected to :-" + Client);


            }

            chat.client.broadcastMessage = function (name, message) {

                console.log(message);

                var CPUSDATA = $.parseJSON(message);

                var list = JSON.parse(CPUSDATA.CORES);


                console.log(list);

              //$("#cpudata").prepend(CPUSDATA.CORES);

            };
            // Get the user name and store it to prepend to messages.
            $('#displayname').val("name");
            // Set initial focus to message input box.
            $('#message').focus();
            // Start the connection.
            $.connection.hub.start().done(function ()
            {
                console.log("connected");         
                
            });
        });
    </script>
</body>
</html>
