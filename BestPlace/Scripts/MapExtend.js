var myOption;
var map;
var myLocation;
var infoWindow;

function Init() {
    myOptions = {
        zoom: 14,
        mapTypeId: google.maps.MapTypeId.ROADMAP
    };

    map = new google.maps.Map(document.getElementById('map_canvas'),
            myOptions);
}

function initialize() {
//    alert("init");
//    i = 5;
//    var myOptions = {
//        zoom: 15,
//        mapTypeId: google.maps.MapTypeId.ROADMAP
//    };
//    map = new google.maps.Map(document.getElementById('map_canvas'),
//            myOptions);

//    // Try HTML5 geolocation
//    if (navigator.geolocation) {
//        navigator.geolocation.getCurrentPosition(function (position) {
//            myLocation = new google.maps.LatLng(position.coords.latitude,
//                                             position.coords.longitude);

//            map.setCenter(myLocation);

//            //bonus
//            var marker = new google.maps.Marker({
//                position: myLocation,
//                title: "Hello World!"
//            });
//            marker.setMap(map);

//            google.maps.event.addListener(marker, 'click', function () {
//                var infowindow = new google.maps.InfoWindow({
//                    content: "your place"
//                });
//                infowindow.open(map, marker);
//            });

//        }, function () {
//            //handleNoGeolocation(true);
//        });
//    } else {
//        // Browser doesn't support Geolocation
//        //handleNoGeolocation(false);
//    }

////    // Create the DIV to hold the control and call the HomeControl() constructor
////    // passing in this DIV.
//    var homeControlDiv = document.createElement('DIV');
//    var homeControl = new HomeControl(homeControlDiv, map);

//    homeControlDiv.index = 1;
//    map.controls[google.maps.ControlPosition.RIGHT_BOTTOM].push(homeControlDiv);

}

function handleNoGeolocation(errorFlag) {
    if (errorFlag) {
        var content = 'Error: The Geolocation service failed.';
    } else {
        var content = 'Error: Your browser doesn\'t support geolocation.';
    }

    var options = {
        map: map,
        position: new google.maps.LatLng(60, 105),
        content: content
    };

    var infowindow = new google.maps.InfoWindow(options);
    map.setCenter(options.position);
}

google.maps.event.addDomListener(window, 'load', initialize);


/**
* The HomeControl adds a control to the map that simply
* returns the user to Chicago. This constructor takes
* the control DIV as an argument.
*/

function HomeControl(controlDiv, map) {

    // Set CSS styles for the DIV containing the control
    // Setting padding to 5 px will offset the control
    // from the edge of the map.
    controlDiv.style.padding = '5px';

    // Set CSS for the control border.
    var controlUI = document.createElement('DIV');
    controlUI.style.backgroundColor = 'white';
    controlUI.style.borderStyle = 'solid';
    controlUI.style.borderWidth = '2px';
    controlUI.style.cursor = 'pointer';
    controlUI.style.textAlign = 'center';
    controlUI.title = 'Click to set the map to Home';
    controlDiv.appendChild(controlUI);

    // Set CSS for the control interior.
    var controlText = document.createElement('DIV');
    controlText.style.fontFamily = 'Arial,sans-serif';
    controlText.style.fontSize = '12px';
    controlText.style.paddingLeft = '4px';
    controlText.style.paddingRight = '4px';
    controlText.innerHTML = 'Home';
    controlUI.appendChild(controlText);

//    // Setup the click event listeners: simply set the map to Chicago.
//    if (myLocation){
//        google.maps.event.addDomListener(controlUI, 'click', function () {
//            map.setCenter(myLocation)
//        });
    //    }
    // Setup the click event listeners: simply set the map to Chicago
    google.maps.event.addDomListener(controlUI, 'click', function () {
        if (navigator.geolocation) {
            navigator.geolocation.getCurrentPosition(function (position) {
                myLocation = new google.maps.LatLng(position.coords.latitude, position.coords.longitude);
                map.setCenter(myLocation);
            });
        }
    });
}

function selectPlace() {
    google.maps.event.clearListeners(map, "click");
    google.maps.event.addListenerOnce(map, 'click', function (event) {
        addMarker(event.latLng);
    });
}

function addMarker(location) {
    var marker = new google.maps.Marker({
        position: location,
        draggable: true,
        animation: google.maps.Animation.DROP,
        map: map
    });

    google.maps.event.addListener(marker, 'click', function () {

        var divContent = document.createElement("div");

        var content =
            "<form name='AddPlace' action='#'>" +
                "<input type='hidden' name='lon' value=" + marker.getPosition().lng() + " />" +
                "<input type='hidden' name='lat' value=" + marker.getPosition().lat() + " />" +
                "<textarea id='textComment' name='Content' rows='2' cols='51'" +
                    "style='width:200px; height:50px;' ></textarea>" +
                "<input class='IMetroButton' type='button' name='submit' value='Add'" +
                    "style='vertical-align:top; height:36px; margin:0px; border:none' />" +
            "</form>";

        $(divContent).append(content);

        $("input", divContent).click(function () {

            $.ajax(
            {
                type: "POST",
                url: "/Place/Add",
                data: "lon=" + $(this).parent().get(0).lon.value + "&lat=" + $(this).parent().get(0).lat.value + "&des=" + $(this).parent().get(0).Content.value,
                dataType: "html",
                success: function (result) {
                    infoWindow.close();
                },
                error: function (error) {
                    alter(error);
                }
            });
        });

        if (infoWindow)
            infoWindow.close();
        infoWindow = new google.maps.InfoWindow({
            content: divContent //marker.mid.toString() + ": " + marker.getPosition().lat() + "|" + marker.getPosition().lng()
        });
        infoWindow.open(map, marker);

        //setTimeout(function () { infoWindow.close(); }, 5000);

        //        $(infoWindow).mouseout(function () {
        //            this.close();
        //        });

        //        google.maps.event.addListener(infoWindow, 'click', function () {
        //            //infoWindow.close();
        //            alert("close");
        //        });

    });
}

function addPlace() {
    $.ajax({ url: "http://localhost:5265/ServicebyWCF.svc/add",
        type: "POST",
        contentType: "application/json",
        data: "{\"Lng\":1, \"Lat\":2,\"Description\":\"Place is added\"}",
        success: function (data) {
            alert("Place is added by Ajax with tag:" + data.Description);
        }
    });
}

function ShowPlace(e) {
    var lat=$(e).parent().get(0).lat.value;
    var lon=$(e).parent().get(0).lon.value;
    var des=$(e).parent().get(0).des.value;

    var myPlace = new google.maps.LatLng(lat, lon);

    map.setCenter(myPlace);

    var marker = new google.maps.Marker({
        position: myPlace,
        title: des
    });
    marker.setMap(map);

    infoWindow = new google.maps.InfoWindow({
        content: des
    });
    infoWindow.open(map, marker);

    google.maps.event.addListener(marker, 'click', function () {
        if (infoWindow)
            infoWindow.close();

        infoWindow = new google.maps.InfoWindow({
            content: des
        });
        infowindow.open(map, marker);
    });
}

function AddMaker(lat, lon, des) {
    var myPlace = new google.maps.LatLng(lat, lon);

    var marker = new google.maps.Marker({
        position: myPlace,
        title: des
    });
    marker.setMap(map);

    google.maps.event.addListener(marker, 'click', function () {
        var infowindow = new google.maps.InfoWindow({
            content: des
        });
        infowindow.open(map, marker);
    });
}

function RegisterInitialize() {
    var myOptions = {
        zoom: 15,
        mapTypeId: google.maps.MapTypeId.ROADMAP
    };
    map = new google.maps.Map(document.getElementById('map_canvas'),
            myOptions);

    // Try HTML5 geolocation
    if (navigator.geolocation) {
        navigator.geolocation.getCurrentPosition(function (position) {
            myLocation = new google.maps.LatLng(position.coords.latitude,
                                             position.coords.longitude);

            //map.setCenter(myLocation);

            //bonus
            var marker = new google.maps.Marker({
                position: myLocation,
                draggable: true,
                animation: google.maps.Animation.DROP,
                title: "Select your place"
            });
            marker.setMap(map);

            google.maps.event.addListener(marker, 'click', function () {
                var infowindow = new google.maps.InfoWindow({
                    content: "Select your place where you want to notify."
                });
                infowindow.open(map, marker);
            });

            google.maps.event.addListener(marker, 'dragend', function () {
                //alert("moved");
                $("#Latitude").val(marker.getPosition().lat());
                $("#Longitude").val(marker.getPosition().lng());
            });

        }, function () {
            handleNoGeolocation(true);
        });
    } else {
        // Browser doesn't support Geolocation
        handleNoGeolocation(false);
    }

    // Create the DIV to hold the control and call the HomeControl() constructor
    // passing in this DIV.
    var homeControlDiv = document.createElement('DIV');
    var homeControl = new HomeControl(homeControlDiv, map);

    homeControlDiv.index = 1;
    map.controls[google.maps.ControlPosition.RIGHT_BOTTOM].push(homeControlDiv);
}

function SetPlace(lat, lon, des) {

    var myLocation = new google.maps.LatLng(lat, lon);
    map.setCenter(myLocation);

    //bonus
    var marker = new google.maps.Marker({
        position: myLocation,
        title: des
    });
    marker.setMap(map);

    google.maps.event.addListener(marker, 'click', function () {
        var infowindow = new google.maps.InfoWindow({
            content: des
        });
        infowindow.open(map, marker);
    });
}

function AddSupport(placeId, url) {

    alert("Bạn đã đăng ký hỗ trợ địa điểm trên, vậy hãy nhanh tay.");
    $("#place-" + placeId).hide();

    $.ajax(
    {
        type: "Get",
        url: "/Place/Support",
        data: "placeId=" + placeId,
        dataType: "html",
        success: function (result) {
        },
        error: function (error) {
            alter(error);
        }
    });

    window.open(url);

}