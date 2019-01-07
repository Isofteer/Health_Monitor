var HUB;
var EmailRequlator = function () {

   



}
var HubsConnection;
            var createChart = function (el) {

                var chart;

                chart = new AmCharts.AmSerialChart();
                chart.marginTop = 0;
                chart.autoMarginOffset = 5;
               
                chart.dataProvider = [],
                    chart.dataDateFormat = "YYYY-MM-DD JJ:NN:SS";

                chart.zoomOutButton = {
                backgroundColor: '#000000',
                    backgroundAlpha: 0.15
                };

                chart.categoryField = "date";

                // AXES
                // category
                var categoryAxis = chart.categoryAxis;
                categoryAxis.parseDates = true; // as our data is date-based, we set parseDates to true
                categoryAxis.minPeriod = "ss"; // our data is daily, so we set minPeriod to DD
                categoryAxis.dashLength = 2;
                categoryAxis.gridAlpha = 0.15;
                categoryAxis.axisColor = "#DADADA";

                // value axis
                var valueAxis = new AmCharts.ValueAxis();
                valueAxis.axisColor = "#FF6600";
                valueAxis.axisThickness = 2;
                valueAxis.gridAlpha = 0;
                chart.addValueAxis(valueAxis);



                // CURSOR
                var chartCursor = new AmCharts.ChartCursor();
                chartCursor.cursorPosition = "mouse";
                chart.addChartCursor(chartCursor);

                // SCROLLBAR
                var chartScrollbar = new AmCharts.ChartScrollbar();
                chart.addChartScrollbar(chartScrollbar);

                // LEGEND
                var legend = new AmCharts.AmLegend();
                legend.marginLeft = 110;
                chart.addLegend(legend);

                // WRITE
                chart.write(el);

                return {el: el, chart: chart };
            }
            var connections = function () {

              
                var _connections;

                var Request = $.ajax({
                        type: "GET",                       
                        url: '/wlt_Connections/getServerConnections',
                        contentType: "application/json; charset=utf-8",
                      
                        async: false,


                    })
                        .done(function (response, textStatus, jqXHR) {

                            _connections = JSON.parse(response);
                            
                        })
                        .fail(function (jqXHR, textStatus, errorThrown) {
                            _connections = [];
                    })




             
                return _connections;
            }


            HubsConnection =  connections();

            $.each(HubsConnection, function (index, ConnObject)
            {                               
                    var connection = $.hubConnection(ConnObject.Connection);

                    var hub = connection.createHubProxy("ChatHub");

                    ConnObject.hub = hub;


                    var broadcastMessage$ = Rx.Observable.fromEvent(hub, 'broadcastMessage',
                        (name, val) => {
                            return {
                                name,
                                val
                            };
                        });
                    broadcastMessage$.throttle(ev => Rx.Observable.interval(10000)).
                    subscribe(function (Message) {
                                               

                        var _json = JSON.parse(Message.val);

                        _json.Connection_ID = ConnObject.conn_ID;


                        var Request = $.ajax({
                            type: "POST",
                            data: JSON.stringify(_json),
                             url: "/wlt_ServerLogs/save",
                             contentType: "application/json; charset=utf-8",
                             processData: true,
                             cache: false,
                         })
                             .done(function (response, textStatus, jqXHR) {
                                 console.log("Saved Successfully ");
                             })
                             .fail(function (jqXHR, textStatus, errorThrown) {
                                 console.log("Success");
                             })


                    });

                    console.log(broadcastMessage$);
                


                    var startConnection = function () {
                        connection.start()
                            .done(function () {
                                var baseurl = connection.baseUrl;

                                // location.protocol + '//' + location.hostname + (location.port ? ':' + location.port : '')
                                var conn_Object = {
                                    Client: location.protocol + window.location.hostname + (location.port ? ':' + location.port : ''),
                                    RefreshRate: ConnObject.RefreshRate
                                };


                                hub.invoke('send', "name", JSON.stringify(conn_Object));

                                console.log('Now connected, connection ID=' + connection.id);

                                M.toast({ html: 'Connection established with :' + baseurl, classes: 'toast-success ' })

                            })
                            .fail(function (data) {
                                var baseurl = connection.baseUrl;

                                console.log('Could not connect', data);
                                M.toast({ html: 'Connection could not be established with :' + baseurl, classes: 'toast-fail ' })
                            });

                    }
                    startConnection();
                  
                
                
               
                    connection.disconnected(function (info) {
                        var baseurl = connection.baseUrl;
                        M.toast({ html: 'Connection lost :' + baseurl, classes: 'toast-fail ' })
                        M.toast({ html: 'Attempting to reconnect to :' + baseurl, classes: 'toast-info ' })
                        setTimeout(function () {
                            startConnection();
                        }, 5000); 
                    });

                    //connection.reconnecting =function (info) {
                    //    var baseurl = connection.baseUrl;
                    //    M.toast({ html: 'Connecting...', classes: 'toast-info' })
                    //};


                    connection.reconnecting ( function (info) {
                        var baseurl = connection.baseUrl;
                        M.toast({ html: 'Reconnecting...', classes: 'toast-info' })
                    });


                    connection.reconnected (function (info) {
                        var baseurl = connection.baseUrl;
                        M.toast({ html: 'Reconnected  successfully to:' + baseurl, classes: 'toast-success ' })
                    });


            })



          
