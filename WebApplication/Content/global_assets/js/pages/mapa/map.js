/* ------------------------------------------------------------------------------
 *
 *  # Basic map
 *
 *  Specific JS code additions for maps_google_basic.html page
 *
 * ---------------------------------------------------------------------------- */


// Setup module
// ------------------------------
// $(document).ready
$(document).ready(function () {
    $('.collapse').collapse();
    $('.alert').alert();

    $('.collapse').on('shown.bs.collapse', function () {
        $(this).parent().addClass('active');
    });

    $('.collapse').on('hidden.bs.collapse', function () {
        $(this).parent().removeClass('active');
    });
 
    $(window).resize(function () {
        fn_SettScreenheight(110);
    });

    $(function () {
        $(document).on('click',
            '.alert-close',
            function () {
                $(this).parent().hide();
            });
    });

});

var map = {

    init: function () {
        $('#cargando').fadeIn();

        var _componentSelect2 = function () {
            if (!$().select2) {
                console.warn('Warning - select2.min.js is not loaded.');
                return;
            }

            // Initialize
            $('.select2').select2();
        };

        var _componentFancytree = function () {
            if (!$().fancytree) {
                console.warn('Warning - fancytree_all.min.js is not loaded.');
                return;
            }
 
            $(".tree").fancytree({
                checkbox: true,
                selectMode: 3,
                icon: false,
                source: {
                    url: $("#listar_menu").val() + "/" + $("#per_cod").val(),
                    cache: false,
                    complete: function () {
                        $("#nro_menus").val($.ui.fancytree.getTree(".tree").getSelectedNodes().length);
                        var tree = $.ui.fancytree.getTree(".tree");
                        var d = tree.toDict(true);
                        $("#menus").val(JSON.stringify(d));
                        /*                       console.log('complete');*/
                    }
                },
                strings: {
                    loading: "Cargando . . . "
                },
                select: function (event, data) {
                    var seleccionados = 0;
                    var selKeys = $.map(data.tree.getSelectedNodes(), function (node) {
                        seleccionados += 1;
                        return node.key;
                    });
                    var tree = $.ui.fancytree.getTree(".tree");
                    var d = tree.toDict(true);
                    $("#menus").val(JSON.stringify(d));
                    $("#nro_menus").val(seleccionados);
                    $("#selected_key").val(selKeys);
                    /*     console.log('select');*/
                },
                loadChildren: function () {
                    $("#nro_menus").val($.ui.fancytree.getTree(".tree").getSelectedNodes().length);
                    var tree = $.ui.fancytree.getTree(".tree");
                    var d = tree.toDict(true);
                    $("#menus").val(JSON.stringify(d));
            
                }
            });
        }

        _componentSelect2();
        _componentFancytree();

        fn_reload_map(false);

    }

};


// Initialize module
// ------------------------------

document.addEventListener('DOMContentLoaded', function () {
    map.init();
});
function fn_getScreenheight() {

    // Fixes dual-screen position                         Most browsers      Firefox
    var dualScreenLeft = window.screenLeft != undefined ? window.screenLeft : screen.left;
    var dualScreenTop = window.screenTop != undefined ? window.screenTop : screen.top;

    width = window.innerWidth ? window.innerWidth : document.documentElement.clientWidth ? document.documentElement.clientWidth : screen.width;
    height = window.innerHeight ? window.innerHeight : document.documentElement.clientHeight ? document.documentElement.clientHeight : screen.height;

    return height;

}

function fn_SettScreenheight(param1) {
    // Fixes dual-screen position                         Most browsers      Firefox
    let height = fn_getScreenheight();

    /*    console.log(param1);*/

    let setheight = height - (0 + param1);
    let setheightMenu = height - (0 + param1);
    $('#map_basic').height(setheight);
    $('#three_menu').height(setheightMenu - (250));//390++ 110
}

function load_map(markers, lines, area) {

    // Fixes dual-screen position                         Most browsers      Firefox
    fn_SettScreenheight(110);
 
    initMap(markers, lines, area);
}
async function initMap(markers, lines, area) {

    // Fixes dual-screen position                         Most browsers      Firefox
    fn_SettScreenheight(110);

    // The location of Tunel
    const position = { lat: -32.6058767, lng: -71.2380175 };

    // Request needed libraries.
    const { Map } = await google.maps.importLibrary("maps");
    const { AdvancedMarkerElement } = await google.maps.importLibrary("marker");
    let _zoom = 14;
    let map;
    let expression = $('#map_type').val();

    //======================== MAPA ===========================
    console.log('mapTypeId : ' + expression);
    switch (expression) {
        case 'roadmap':
            // code block
            map = new Map(document.getElementById("map_basic"), {
                zoom: _zoom,
                center: position,
                mapId: "4504f8b37365c3d0",
                mapTypeId: google.maps.MapTypeId.ROADMAP
            });
            map.setTilt(45);

            break;
        case 'satellite':
            // code block
            map = new Map(document.getElementById("map_basic"), {
                zoom: _zoom,
                center: position,
                mapId: "DEMO_MAP_ID",
                mapTypeId: google.maps.MapTypeId.SATELLITE
            });
            map.setTilt(45);

            break;
        case 'hybrid':
            // code block
            map = new Map(document.getElementById("map_basic"), {
                zoom: _zoom,
                center: position,
                mapId: "DEMO_MAP_ID",
                mapTypeId: google.maps.MapTypeId.HYBRID
            });
            map.setTilt(45);

            break;
        case 'terrain':
            // code block
            map = new Map(document.getElementById("map_basic"), {
                zoom: _zoom,
                center: position,
                mapId: "DEMO_MAP_ID",
                mapTypeId: google.maps.MapTypeId.TERRAIN
            });
            map.setTilt(45);

            break;
        default:
        // code block
    }
    // /.====================== MAPA ===========================

    //======================== MARKERS ===========================
    // The marker, positioned at Uluru
    for (const property of markers) {

        const AdvancedMarkerElement = new google.maps.marker.AdvancedMarkerElement({
            map,
            content: buildContent(property),
            position: { lat: parseFloat(property.com_lat_ini), lng: parseFloat(property.com_lon_ini) },
            title: property.com_elemento_nombre,
        });

        AdvancedMarkerElement.addListener("click", () => {
            toggleHighlight(AdvancedMarkerElement, property);
        });
    }
    // /.======================== MARKERS ===========================

    //======================== LINES ===========================
    if (lines != []) {
        /*const ruta = new Array();*/
        var tunel1 = new Array();
        var tunel2 = new Array();

        for (const obj of lines) {

            let tunel = obj.com_id;
            let orden = obj.com_tipo;
            let lat = obj.com_lat;
            let lon = obj.com_lon;

            switch (tunel) {
                case 1:
                    // code block
                    tunel1.push(new google.maps.LatLng(lat, lon));
                    break;
                case 2:
                    // code block
                    tunel2.push(new google.maps.LatLng(lat, lon));
                    break;
                default:
                // code block
            }
        };


        // first, black line
        var flightPath = new google.maps.Polyline({
            path: tunel1,
            strokeColor: '#34495E ',
            strokeOpacity: 1.0,
            strokeWeight: 17,
            geodesic: true,
            map: map
        });

        // second, translucent red line
        var keliasA = new google.maps.Polyline({
            path: tunel1,
            strokeColor: '#EAEDED',
            geodesic: true,
            strokeWeight: 15,
            map: map
        });

        var kelias1 = new google.maps.Polyline({
            path: tunel1,
            geodesic: true,
            strokeColor: "#34495E",
            strokeOpacity: 1.0,
            strokeWeight: 2,
            map: map
        });

        var popup1 = new google.maps.InfoWindow();
        keliasA.addListener('click', function (e) {
            popup1.setContent('Tunel 1');
            popup1.setPosition(e.latLng);
            popup1.open(map);
        });


        var flightPath = new google.maps.Polyline({
            path: tunel2,
            strokeColor: '#34495E ',
            strokeOpacity: 1.0,
            strokeWeight: 17,
            geodesic: true,
            map: map
        });
        var kelias2 = new google.maps.Polyline({
            path: tunel2,
            strokeColor: '#EAEDED',
            geodesic: true,
            strokeWeight: 15,
            map: map
        });

        // second, translucent red line
        var keliasB = new google.maps.Polyline({
            path: tunel2,
            geodesic: true,
            strokeColor: "#34495E",
            strokeOpacity: 1.0,
            strokeWeight: 2,
            map: map
        });
    }
    // /.======================== LINES ===========================

    if (area != []) {
        /*const ruta = new Array();*/
        var area1 = new Array();

        $.each(area, function (index, obj) {

            let corr = obj.corr;
            let descripcion = obj.descripcion;
            let lat = obj.com_lat;
            let lon = obj.com_lon;

            // code block
            area1.push(new google.maps.LatLng(lat, lon));
        });

        var poligono = new google.maps.Polygon({
            path: area1,
            map: map,
            strokeColor: 'rgb(255, 0, 0)',
            fillColor: '#ff000030',
            strokeWeight: 3
        });

        var popup = new google.maps.InfoWindow();
        poligono.addListener('click', function (e) {
            popup.setContent('Area Concesión Túnel El Melón');
            popup.setPosition(e.latLng);
            popup.open(map);
        });



    }
}

function toggleHighlight(markerView, property) {
    if (markerView.content.classList.contains("highlight")) {
        markerView.content.classList.remove("highlight");
        markerView.zIndex = null;
    } else {
        markerView.content.classList.add("highlight");
        markerView.zIndex = 1;
    }
}
function buildContent(property) {
    const content = document.createElement("div");

    let str = '';
    str += '<div class="sidebar-content-detail">';
    str += '<div class="card p-2">';
    str += '<div class="card-header bg-transparent header-elements-inline p-1">';
    str += '<span class="text-uppercase font-size-sm font-weight-semibold"><i class="icon-pushpin mr-2"></i>' + property.com_categoria + '</span>';
    str += '</div>';

    str += '<table class="table table-borderless table-xs border-top-0 my-2">';
    str += '<tbody>';

    str += '<tr>';
    str += '   <td class="text-uppercase font-weight-semibold py-0 px-1">ID:</td>';
    str += '   <td class="text-right py-0 px-1">' + property.com_id_inv + '</td>';
    str += '</tr>';

    str += '<tr>';
    str += '   <td class="text-uppercase font-weight-semibold py-0 px-1">DM:</td>';
    str += '   <td class="text-right py-0 px-1">' + property.com_dm + '</td>';
    str += '</tr>';

    str += '<tr>';
    str += '   <td class="text-uppercase font-weight-semibold py-0 px-1 text-nowrap">Nom Ele:</td>';
    str += '   <td class="py-0 px-1 text-right text-nowrap">' + property.com_elemento_nombre + '</td>';
    str += '</tr>';

    str += '<tr>';
    str += '   <td class="text-uppercase font-weight-semibold py-0 px-1">Ruta:</td>';
    str += '   <td class="text-right py-0 px-1">' + property.com_ruta + '</td>';
    str += '</tr>';
    str += '<tr>';
    str += '   <td class="text-uppercase font-weight-semibold py-0 px-1">Tramo:</td>';
    str += '   <td class="text-right py-0 px-1">' + property.com_tramo_desc + '</td>';
    str += '</tr>';
    str += '<tr>';
    str += '<tr>';
    str += '   <td class="text-uppercase font-weight-semibold py-0 px-1">ID Mop:</td>';
    str += '   <td class="text-right py-0 px-1">' + property.com_id_mop + '</td>';
    str += '</tr>';

    if (property.com_map_ubicacion_flag) {
        str += '<tr>';
        str += '   <td class="text-uppercase font-weight-semibold py-0 px-1">Ubicación:</td>';
        str += '   <td class="text-right py-0 px-1">' + property.com_map_ubicacion_inicio + '</td>';
        str += '</tr>';
    } else {
        str += '<tr>';
        str += '   <td class="text-uppercase font-weight-semibold py-0 px-1">Inicio:</td>';
        str += '   <td class="text-right py-0 px-1">' + property.com_map_ubicacion_inicio + '</td>';
        str += '</tr>';
        str += '<tr>';
        str += '   <td class="text-uppercase font-weight-semibold py-0 px-1">Fin:</td>';
        str += '   <td class="text-right py-0 px-1">' + property.com_map_ubicacion_fin + '</td>';
        str += '</tr>';

    }
    str += '</tbody>';
    str += '</table>';

    str += '<div class="card-body p-0 text-center">';
    str += '   <button type="button" class="btn btn-link p-0" onclick="fn_marker_detail(\'' + property.com_tipo + '\',\'' + property.com_id + '\')"><i class="icon-eye2 mr-2"></i>Ver detalle</button>';
    str += '</div>';

    if (property.com_img_vig) {
        str += '<div class="card-body p-1 text-center">';
        str += '   <img width="200" src="' + property.com_map_img + '" style="border: 1px solid rgba(0,0,0,.125);">';
        str += '</div>';
    }

    str += '</div>';
    str += '</div>';

    let str1 = '';
    str1 += ' <div class="icon">';
    str1 += '   <img src=' + property.com_map_icon + '>';
    str1 += '</div>';

    let str2 = '';
    str2 += ' <div class="details">';
    str2 += str;
    str2 += '</div>';

    //let str3 = '';
    //str3 += '<div class="gm-style-iw-a" style="position: absolute; left: 50px; top: 9px;">';
    //str3 += '   <div class="gm-style-iw-t" style="right: 0px; bottom: 42px;">';
    //str3 += '       <div class="gm-style-iw-tc"></div>';
    //str3 += '   </div>';
    //str3 += '</div>';

    content.classList.add("property");
    content.innerHTML = str1 + str2; 
    return content;
}
function fn_reload_map(isPostback) {
    $('#cargando').fadeIn();

    $('#map_basic').addClass('d-none');
    $('#div_map_alert').removeClass('d-none').addClass('d-none');

    var toggle = $('.custom-template .custom-toggle');
    var className = $('.custom-template').attr('class');
    var list = $('#menus').val();
    let size_icon = $('#size_icon').val();

    if (!isPostback) {
        list = "{\"expanded\":true,\"key\":\"root_1\",\"partsel\":false,\"selected\":false,\"title\":\"root\",\"children\":[]}";
    }

    //Call Ajax Method
    $.ajax({
        url: $("#hddn_getMarkers").val(),
        type: "POST",
        data: JSON.stringify({ jsonData: list, isPostback: isPostback, iIcon: size_icon }),
        dataType: "json",
        contentType: "application/json"
    }).done(function (response) {
 
        if (response.status) {
            $('#map_basic').html('');

            load_map(response.List, response.listLines, response.areaLines);
        }
        else {

            $('#div_map_alert').removeClass('d-none');
            $('#div_map_alert_text').text(response.exception);

            load_map(response.List, response.listLines, response.areaLines);
            fn_SettScreenheight(172);

        }
    }).fail(function (jqXHR, textStatus) {
        console.log("fn_reload_map / Request failed: " + textStatus);
    }).always(function () {

        //!toggle_customSidebar
        if (className == 'custom-template open') {
            $('.custom-template').removeClass('open');
            toggle.removeClass('toggled');
            custom_open = 0;
        }

        $('#map_basic').removeClass('d-none');
        $('#cargando').fadeOut();
    });
    // Fin Call Ajax Method

    return false;
}