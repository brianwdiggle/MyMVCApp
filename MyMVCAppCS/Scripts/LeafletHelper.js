﻿// Function toInternationalDate ---------------------------------------------------------------------
// Convert date/time string in format DD/MM/YYYY HH:MM:SS to standard javascript format YYYY/MM/DD
const toInternationaDate = function (ukDate) {
    var startTimeLoc = ukDate.indexOf(' ');

    //Override end location if ukDate does not contain a time portion
    if (startTimeLoc == -1) {
        startTimeLoc = ukDate.length;
    }

    ukDateSplit = ukDate.substring(0, startTimeLoc).split('/');

    standardDay = ukDateSplit[0];
    standardMonth = ukDateSplit[1];
    standardYear = ukDateSplit[2];

    return new Date(standardYear, standardMonth, standardDay);
}

//----If GPS data is available associated with the the current page of markers, then display the map----
const OSapiKey = '468YAE3SzsjV8Uu8XPPDQpVVh2mA67vC';

// using  // https://github.com/OrdnanceSurvey/os-transform
//----Define the map centre point by converting an os grid reference into ESPG:27000 coordinates

const map_center = [54.5, -3.09];

// Setup the EPSG:27700 (British National Grid) projection.
const crs = new L.Proj.CRS('EPSG:27700', '+proj=tmerc +lat_0=49 +lon_0=-2 +k=0.9996012717 +x_0=400000 +y_0=-100000 +ellps=airy +towgs84=446.448,-125.157,542.06,0.15,0.247,0.842,-20.489 +units=m +no_defs', {
    resolutions: [896.0, 448.0, 224.0, 112.0, 56.0, 28.0, 14.0, 7.0, 3.5, 1.75],
    origin: [-238375.0, 1376256.0]
});

// Leaflet works natively in WGS84 -a.k.a. EPSG:4326. The following function converts from OS National grid easting and northing to this.
const transformCoords = function (arr) {
    return proj4('EPSG:27700', 'EPSG:4326', arr).reverse();
};


// Define the options associated with the Leaflet map which will be created
const mapOptions = {
    crs: crs,
    minZoom: 0,
    maxZoom: 9,
    center: map_center,
    zoom: 8,   // 8 this is the first 1:25000 zoom level
    maxBounds: [    // as defined by the EPSG:27000 coordinate system
        transformCoords([-238375.0, 0.0]),
        transformCoords([900000.0, 1376256.0])
    ],
    attributionControl: true
};

// Display red or green summit icon depending on whether the hill has been climbed
const climbedsummiticon = L.icon({
    iconUrl: "/Content/images/marker-icon-green-small.png",
    shadowUrl: 'https://cdnjs.cloudflare.com/ajax/libs/leaflet/0.7.7/images/marker-shadow.png',
    iconSize: [15, 25],
    iconAnchor: [7, 25],
    popupAnchor: [1, -20],
    shadowSize: [25, 25]
});

// Display red or green summit icon depending on whether the hill has been climbed
const bluemarkericon = L.icon({
    iconUrl: "/Content/images/marker-icon-blue.png"
});

let climbedmapmarkeroptions = {
    zIndexOffset: 500,
    icon: climbedsummiticon
};

let bluemapmarkeroptions = {
    zIndexOffset: 400,
    icon: bluemarkericon
};

const unclimbedsummiticon = L.icon({
    iconUrl: "/Content/images/marker-icon-red-small.png",
    shadowUrl: 'https://cdnjs.cloudflare.com/ajax/libs/leaflet/0.7.7/images/marker-shadow.png',
    iconSize: [15, 25],
    iconAnchor: [7, 25],
    popupAnchor: [1, -20],
    shadowSize: [25, 25]
});

let unclimbedmapmarkeroptions = {
    zIndexOffset: 1000,
    icon: unclimbedsummiticon
};

// Get all the markers in the map bounds and add to the map
function getAllMarkersInBounds() {
    var newbounds = map.getBounds();

    $.ajax({
        url: document.getElementById("ApplicationRoot").getAttribute("href") + "Marker/_MarkersInMapBounds",
        type: "get",
        data: {
            neLat: newbounds._northEast.lat,
            neLng: newbounds._northEast.lng,
            swLat: newbounds._southWest.lat,
            swLng: newbounds._southWest.lng
        },
        success: function (result) {
            addMarkersToMapAJAX(result.markersinbounds);
        },
        error: function (errorresponse) {
            console.log("failure called ajax function: " + errorresponse);
        }

    });
}



// Given an array of MapMarker objects from the server, add to the leafet map
function addMarkersToMapAJAX(markerstoadd) {

    $.each(markerstoadd, function (index, item) {

        const popupOptions = { className: "markerPopup" }

        marker = new L.marker([item.latitude, item.longtitude], bluemapmarkeroptions)
                .bindPopup(item.popupText, popupOptions)
                .openPopup()
                .addTo(map);
  
  
    });
}

//  Given an array of MapMarker objects from the server, add to the leafet map
function addHillsToMapAJAX(markerstoadd) {

    const popupOptions = { className: "markerPopup" }

    $.each(markerstoadd, function (index, item) {

        if (item.numberOfAscents > 0) {
            marker = new L.marker([item.latitude, item.longtitude], climbedmapmarkeroptions)
                .bindPopup(item.popupText, popupOptions)
                .openPopup()
                .addTo(map);
        } else {
            marker = new L.marker([item.latitude, item.longtitude], unclimbedmapmarkeroptions)
                .bindPopup(item.popupText, popupOptions)
                .openPopup()
                .addTo(map);
        }

  

    });
}

// Get all the hills in the map bounds and add to the map
function getAllHillsInMapBounds() {
    var newbounds = map.getBounds();

    $.ajax({
        url: document.getElementById("ApplicationRoot").getAttribute("href") + "Walks/_HillsInMapBounds",
        type: "get",
        data: {
            neLat: newbounds._northEast.lat,
            neLng: newbounds._northEast.lng,
            swLat: newbounds._southWest.lat,
            swLng: newbounds._southWest.lng
        },
        success: function (result) {
            addHillsToMapAJAX(result.hillsinbounds);
        },
        error: function (errorresponse) {
            console.log("failure called ajax function: " + errorresponse);
        }

    });
}

// Get all the hills in the map bounds which of the specificed class, and add to the map
function getAllHillsInClassInMapBounds(urlpathparam) {
    var newbounds = map.getBounds();

    $.ajax({
        url: document.getElementById("ApplicationRoot").getAttribute("href") + "Walks/_HillsInClassInBounds",
        type: "get",
        data: {
            urlpath: urlpathparam,
            neLat: newbounds._northEast.lat,
            neLng: newbounds._northEast.lng,
            swLat: newbounds._southWest.lat,
            swLng: newbounds._southWest.lng
        },
        success: function (result) {
            addHillsToMapAJAX(result.hillsinbounds);
        },
        error: function (errorresponse) {
            console.log("failure called ajax function: " + errorresponse);
        }

    });
}

function addMarkersToMap() {
    if (markerdata != null) {

        let markeroptions = {
            zIndexOffset: 1000
        };

        //---Add the markers to the map

        for (let i = 0; i < markerdata.length; i++) {
            var thislat = markercoords[i][0];
            var thislong = markercoords[i][1];
            const popText = markerpopups[i];
            const popupOptions = {
                className: "markerPopup"
            }
            marker = new L.marker(transformCoords([thislat, thislong]), climbedmapmarkeroptions)
                .bindPopup(popText, popupOptions)
                .openPopup()
                .addTo(map);
        }

        //----Set the bounds of the map so that all the markers are displayed
        map.fitBounds(
            [
                transformCoords([mineasting, minnorthing]),
                transformCoords([maxeasting, maxnorthing])
            ]
        );

    }
}

