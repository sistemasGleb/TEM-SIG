var Menu = function () {

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
            },
            loadChildren: function () {
                $("#nro_menus").val($.ui.fancytree.getTree(".tree").getSelectedNodes().length);
                var tree = $.ui.fancytree.getTree(".tree");
                var d = tree.toDict(true);
                $("#menus").val(JSON.stringify(d));
            }
        });


    }

    return {
        init: function () {
            _componentFancytree();
        }
    }
}();

document.addEventListener('DOMContentLoaded', function () {
    Menu.init();
});

function fnc_actulizar_preferencias() {
 
    let swalInit = swal.mixin({
        buttonsStyling: false,
        confirmButtonClass: 'btn btn-primary',
        cancelButtonClass: 'btn btn-light'
    });

    swalInit.fire({
        title: 'Actualizar Configuración',
        html: '¿Está seguro desea actualizar sus parámetros.?.',
        type: 'question',
        showCancelButton: true,
        confirmButtonText: '<i class="icon-checkmark2 mr-1"></i>Aceptar',
        cancelButtonText: '<i class="icon-cross3 mr-1"></i>Cancelar',
        confirmButtonClass: 'btn btn-primary',
        cancelButtonClass: 'btn btn-light',
        buttonsStyling: false
    }).then(function (result) {
        if (result.value) {
            fn_save_menu_preferences();
        }
    });
}

function fn_save_menu_preferences() {

    $('#cargando').fadeIn();
    $('#map_basic').addClass('d-none');
/*    $('#div_map_alert').removeClass('d-none').addClass('d-none');*/

    var toggle = $('.custom-template .custom-toggle');
    var className = $('.custom-template').attr('class');
    var list = $('#menus').val();
    let size_icon = $('#size_icon').val();
    let expression = $('#map_type').val();

    let swalInit = swal.mixin({
        buttonsStyling: false,
        confirmButtonClass: 'btn btn-primary',
        cancelButtonClass: 'btn btn-light'
    });

    //Call Ajax Method
    $.ajax({
        url: $("#hddn_Update_User_Preferences").val(),
        type: "POST",
        data: JSON.stringify({ jsonData: list, isPostback: false, iIcon: size_icon, iMap: expression }),
        dataType: "json",
        contentType: "application/json"
    }).done(function (response) {

        if (response.status) {

            swalInit.fire({
                title: 'Actualizar Parámetros',
                html: response.responseText,
                type: 'success',
                showCancelButton: false,
                confirmButtonText: '<i class="icon-checkmark2 mr-1"></i>Aceptar',
                confirmButtonClass: 'btn btn-primary',
                buttonsStyling: false
            }).then(function (result) {
                if (result.value) {
                    fn_reload_map(true);
                }
            });
        }
        else {
            swalInit.fire({
                title: 'Actualizar Parámetros',
                html: 'ERROR :' + response.responseText,
                type: 'error',
                showCancelButton: false,
                confirmButtonText: '<i class="icon-checkmark2 mr-1"></i>Aceptar',
                confirmButtonClass: 'btn btn-primary',
                buttonsStyling: false
            });
        }

    }).fail(function (jqXHR, textStatus) {
        console.log("fn_save_menu_preferences / Request failed: " + textStatus);
    }).always(function () {

        $('#cargando').fadeOut();
    });
    // Fin Call Ajax Method

    return false;
}